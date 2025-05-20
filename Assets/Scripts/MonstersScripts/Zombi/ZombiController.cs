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
    private List<Vector2> nodes;
    private int numberOfMaskPlayer;
    private LayerMask doorLayer;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float climbingSpeed = 3f;
    [SerializeField] private float interactionDistance = 1f;
    [SerializeField] private float openingDoorTime = 3f;
    [SerializeField] private float standingTime = 5f;
    [SerializeField] private AudioClip chaseClip;

    private Vector2 targetPosition;
    private List<Vector2> path;
    private int currentIndex = 0;
    private bool isClimbing = false;
    private float defaultGravityScale;
    private BuildingDoorScript doorController;
    private int speedParam = Animator.StringToHash("Speed");

    [SyncVar(hook = nameof(OnFlipChanged))]
    private bool isFlipped;

    private bool isRouming = true;
    private bool isStanding = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        nodes = BuildingData.node;
        numberOfMaskPlayer = LayerMask.GetMask("Player");
        doorLayer = LayerMask.GetMask("Door");
        defaultGravityScale = rb.gravityScale;

        SetNewNodePosition();
        path = Pathfinding.FindPath(transform.position, targetPosition);
    }

    void Update()
    {
        animator.SetFloat(speedParam, moveSpeed);
    }

    private void FixedUpdate()
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
            path = Pathfinding.FindPath(transform.position, targetPosition);
            Debug.Log(path.Count);
            Debug.Log("ind = " + currentIndex);
        }

        Vector2 currentTarget = path[currentIndex];

        Vector2 direction = (currentTarget - (Vector2)transform.position).normalized;

        FlipSprite(direction.x);

        rb.velocity = direction * moveSpeed;

        if (isClimbing)
        {
            //if (Vector2.Distance(transform.position, currentTarget) < 0.1f)
            //{
            //    currentIndex++;
            //}
            if ((Vector2)transform.position == currentTarget)
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
    }

    private void SetNewNodePosition()
    {
        int randIndex = Random.Range(0, BuildingData.node.Count);
        targetPosition = BuildingData.node[randIndex];
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ladder"))
        {
            isClimbing = true;
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
    }
}
