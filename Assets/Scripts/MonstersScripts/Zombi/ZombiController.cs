using Assets.Scripts.BuildingScripts.BuildingTypes;
using Assets.Scripts.MonstersScripts;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiController : NetworkBehaviour
{
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private MonstersSoundManager monstersSoundManager;

    private List<Vector2> nodes;
    private int playerLayer;
    private LayerMask doorLayer;
    private LayerMask wallLayer;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float climbingSpeed = 3f;
    [SerializeField] private float interactionDistance = 1f;
    [SerializeField] private float openingDoorTime = 3f;
    [SerializeField] private float maxSecondsJustStand = 5f;
    [SerializeField] private AudioClip chaseClip;
    [SerializeField] private LayerMask lmWalls;
    [SerializeField] private LayerMask lmPlatform;

    private Vector2 targetPosition;
    private List<Vector2> path;
    private int currentIndex = 0;
    private bool isClimbing = false;
    private bool isOnTheGround = true;
    private int groundSoundIndex = 0;
    private float defaultGravityScale;
    private BuildingDoorScript doorController;
    private int speedParam = Animator.StringToHash("Speed");

    [SyncVar(hook = nameof(OnFlipChanged))]
    private bool isFlipped;

    private GameObject player;

    private bool isPlayerVisibal = false;
    private bool isMovingToLastPos = false;
    private bool isRouming = true;
    private bool isStanding = false;
    private bool isPathSetToLastPlayerPosition = false;

    private Vector2 lastSeenPositionOfPlayer;
    private float timerToStand;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        monstersSoundManager = GetComponent<MonstersSoundManager>();

        nodes = BuildingData.node;
        playerLayer = LayerMask.GetMask("Player");
        doorLayer = LayerMask.GetMask("Door");
        wallLayer = LayerMask.GetMask("Wall");
        defaultGravityScale = rb.gravityScale;

        SetNewNodePosition();
        path = Pathfinding.FindPath(transform.position, targetPosition);
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (timerToStand > 0)
        {
            timerToStand -= Time.fixedDeltaTime;
        }

        Vector2? playerPosition = DetectPlayerPosition();
        if (playerPosition != null)
        {
            isPlayerVisibal = true;
            isRouming = false;
            isStanding = false;
            timerToStand = 0;
        }
        else
        {
            isPlayerVisibal = false;
        }

        if (isPlayerVisibal)
        {
            ChasePlayer();
        }
        //else if (!isPlayerVisibal && isMovingToLastPos)
        //{
        //    MoveToTheLastPositionOfPlayer();
        //}
        else
        {
            JustStand();
        }
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }

    public Vector2? DetectPlayerPosition()
    {
        float rayLength = 10f;

        Vector2[] directions = new Vector2[]
        {
            Vector2.right,
            Vector2.left,
            Vector2.up
        };

        foreach (Vector2 direction in directions)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, rayLength, playerLayer | wallLayer);

            if (hit.collider != null)
            {
                if ((1 << hit.collider.gameObject.layer & wallLayer) != 0)
                {
                    //Debug.DrawLine(transform.position, hit.point, Color.yellow, 1f);
                    continue;
                }

                if ((1 << hit.collider.gameObject.layer & playerLayer) != 0 && !hit.collider.gameObject.GetComponent<HealthBar>().isCharacterDead)
                {
                    //Debug.DrawLine(transform.position, hit.point, Color.green, 1f);

                    player = hit.collider.gameObject;
                    isMovingToLastPos = true;
                    return hit.collider.transform.position;
                }
            }
            else
            {
                //Debug.DrawRay(transform.position, direction * rayLength, Color.red, 1f);
            }
        }

        return null;
    }

    private void ChasePlayer()
    {
        if (player != null && !player.gameObject.GetComponent<HealthBar>().isCharacterDead)
        {
            isMovingToLastPos = false;
            isPlayerVisibal = false;
        }

        lastSeenPositionOfPlayer = player.transform.position;
        Move(player.transform.position);
    }

    private async void MoveToTheLastPositionOfPlayer()
    {
        if (isPathSetToLastPlayerPosition && currentIndex >= path.Count)
        {
            timerToStand = maxSecondsJustStand;
            isMovingToLastPos = false;
            isPathSetToLastPlayerPosition = false;
            player = null;
        }
        else
        {
            if (!isPathSetToLastPlayerPosition)
            {
                currentIndex = 0;
                path = await Pathfinding.FindPathAsync(transform.position, lastSeenPositionOfPlayer);
                isPathSetToLastPlayerPosition = true;
            }
            else
            {
                Move(path[currentIndex]);
            }
        }
    }

    private void JustStand()
    {
        if (timerToStand > 0)
        {
            if (isOnTheGround)
            {
                rb.velocity = new Vector2(0f, rb.velocity.y);
            }
        }
        else
        {
            Rouming();
        }
    }

    #region Rouming
    private async void Rouming()
    {
        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("Path is empty. Cannot move to target.");
            return;
        }

        if (currentIndex >= path.Count)
        {
            currentIndex = 0;
            SetNewNodePosition();
            path = await Pathfinding.FindPathAsync(transform.position, targetPosition);
        }

        Move(path[currentIndex]);
    }

    private void SetNewNodePosition()
    {
        int randIndex = Random.Range(0, BuildingData.node.Count);
        targetPosition = BuildingData.node[randIndex];
    }

    #endregion

    private void Move(Vector2 targetPoint)
    {
        Vector2 currentTarget = targetPoint;

        Vector2 direction = (currentTarget - (Vector2)transform.position).normalized;

        FlipSprite(direction.x);
        PlayStepsAudio();

        rb.velocity = direction * moveSpeed;

        if (isClimbing)
        {
            if (Vector2.Distance(transform.position, currentTarget) < 0.07f)
            {
                currentIndex++;
            }
        }
        else
        {
            if (Vector2.Distance(transform.position, currentTarget) < 0.4f)
            {
                currentIndex++;
            }
        }
        animator.SetFloat(speedParam, Mathf.Abs(rb.velocity.x));
    }

    #region Flip Sprite
    public void FlipSprite(float move)
    {
        if (move < 0 && !isFlipped)
        {
            CmdFlipSprite(true);
        }
        else if (move > 0 && isFlipped)
        {
            CmdFlipSprite(false);
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdFlipSprite(bool flipState)
    {
        isFlipped = flipState;
    }

    private void OnFlipChanged(bool oldValue, bool newValue)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = newValue;
        }
    }
    #endregion

    private IEnumerator OpeningDoor()
    {
        float currectMoveSpeed = moveSpeed;
        moveSpeed = 0;
        yield return new WaitForSeconds(openingDoorTime);
        if (doorController != null)
        {
            doorController.ChangeDoorState();
            doorController = null;
        }
        moveSpeed = 5f;
    }

    #region Steps Sound
    private void PlayStepsAudio()
    {
        OnTheGroundChecker();

       if (isOnTheGround)
       {
            monstersSoundManager.PlayStepAudio(moveSpeed, 0.6f, groundSoundIndex); 
       }
    }

    private void OnTheGroundChecker()
    {
        Vector2 v2GroundedBoxCheckPosition = (Vector2)transform.position + new Vector2(0, -0.01f);
        Vector2 v2GroundedBoxCheckScale = (Vector2)transform.localScale + new Vector2(-0.02f, 0);

        bool grounded = Physics2D.OverlapBox(v2GroundedBoxCheckPosition, v2GroundedBoxCheckScale, 0, lmWalls);
        if (!grounded)
        {
            grounded = Physics2D.OverlapBox(v2GroundedBoxCheckPosition, v2GroundedBoxCheckScale, 0, lmPlatform);
        }
        isOnTheGround = grounded;
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ladder"))
        {
            isClimbing = true;
            groundSoundIndex = 1;
            rb.gravityScale = 0;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Door"))
        {
            doorController = collision.gameObject.GetComponent<BuildingDoorScript>();
            if (doorController != null)
            {
                if (!doorController.isDoorOpen())
                {
                    StartCoroutine(OpeningDoor());
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ladder"))
        {
            isClimbing = false;
            groundSoundIndex = 0;
            rb.gravityScale = defaultGravityScale;
        }
    }

    private void OnDrawGizmos()
    {
        if (path == null || path.Count == 0)
            return;

        Gizmos.color = Color.green;

        for (int i = 0; i < path.Count - 1; i++)
        {
            Gizmos.DrawLine(path[i], path[i + 1]);
        }

        Gizmos.color = Color.red;
        foreach (var point in path)
        {
            Gizmos.DrawSphere(point, 0.1f);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(lastSeenPositionOfPlayer, 0.1f);

        Gizmos.color = isOnTheGround ? Color.green : Color.red;

        Vector2 v2GroundedBoxCheckPosition = (Vector2)transform.position + new Vector2(0, -0.01f);
        Vector2 v2GroundedBoxCheckScale = (Vector2)transform.localScale + new Vector2(-0.02f, 0);
        Gizmos.DrawWireCube(v2GroundedBoxCheckPosition, v2GroundedBoxCheckScale);
    }
}
