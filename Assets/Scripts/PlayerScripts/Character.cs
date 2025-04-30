using Assets.Scripts.CharactersStates;
using Mirror;
using System.Collections;
using UnityEngine;

public class Character : NetworkBehaviour
{
    private StateMachine movementSM;
    public StandingState standing;
    public JumpingState jumping;

    [SerializeField] private Animator animator;
    [SerializeField] public float characterNormaWalkSpeed = 5f;
    [SerializeField] public float characterRunningSpeed = 12f;

    [SerializeField] public LayerMask lmWalls;
    [SerializeField] public LayerMask lmPlatform;
    [SerializeField] private float fJumpVelocity = 30;

    [SerializeField] private float fJumpPressedRememberTime = 0.2f;
    [SerializeField] private float fGroundedRememberTime = 0.25f;
    [SerializeField][Range(0, 1)] private float fCutJumpHeight = 0.5f;

    [SerializeField] private int maxDifferenceHeightToTakeDamage = 15;

    [HideInInspector] public bool doubleTapD = false;
    [HideInInspector] public bool doubleTapA = false;
    [HideInInspector] public float doubleTapTimeThreshold = 0.3f;
    [HideInInspector] public float doubleTapTimer = 0f;
    [HideInInspector] public float walkInterval = 0.5f;
    [HideInInspector] public float runInterval = 0.3f;

    private Rigidbody2D rb;
    private SpriteRenderer rbSprite;

    private float fJumpPressedRemember = 0;
    private float fGroundedRemember = 0;

    private bool firstTime = true;
    private bool isFalling = false;
    private Vector3 previoursPosition;
    private float hieghestPosition;

    private bool canMove = true;
    private bool bGrounded = false;

    [SyncVar(hook = nameof(OnFlipChanged))]
    private bool isFlipped;

    public void MoveRightAndLeft(float move, float speedNow, float stepsInterval)
    {
        FlipSprite(move);

        SoundManager.Instance.PlayStepsAudio(move, stepsInterval);

        rb.velocity = new Vector2(move * speedNow, rb.velocity.y);
    }

    public void Jump(int animParam)
    {
        Vector2 v2GroundedBoxCheckPosition = (Vector2)transform.position + new Vector2(0, -0.01f);
        Vector2 v2GroundedBoxCheckScale = (Vector2)transform.localScale + new Vector2(-0.02f, 0);
        bGrounded = Physics2D.OverlapBox(v2GroundedBoxCheckPosition, v2GroundedBoxCheckScale, 0, lmWalls);
        if (!bGrounded) bGrounded = Physics2D.OverlapBox(v2GroundedBoxCheckPosition, v2GroundedBoxCheckScale, 0, lmPlatform);

        fGroundedRemember -= Time.deltaTime;
        if (bGrounded)
        {
            fGroundedRemember = fGroundedRememberTime;
        }

        fJumpPressedRemember -= Time.deltaTime;
        if (Input.GetButtonDown("Jump"))
        {
            fJumpPressedRemember = fJumpPressedRememberTime;
        }

        if (Input.GetButtonUp("Jump"))
        {
            if (rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * fCutJumpHeight);
            }
        }

        if ((fJumpPressedRemember > 0) && (fGroundedRemember > 0))
        {
            fJumpPressedRemember = 0;
            fGroundedRemember = 0;
            rb.velocity = new Vector2(rb.velocity.x, fJumpVelocity);
        }

        animator.SetFloat(animParam, rb.velocity.y);

        #region Урон от падения
        if (!bGrounded)
        {
            if (transform.position.y < previoursPosition.y && firstTime)
            {
                firstTime = false;
                isFalling = true;
                hieghestPosition = transform.position.y;
            }
            previoursPosition = transform.position;
        }
        if (bGrounded && isFalling)
        {
            SoundManager.Instance.PlayJumpHit();
            if ((int)(Mathf.Abs(hieghestPosition - transform.position.y)) > maxDifferenceHeightToTakeDamage)
            {
                //healthBar.TakeDamage((int)(Mathf.Abs(hieghestPosition - transform.position.y)) * 2);
            }
            isFalling = false;
            firstTime = true;
        }
        #endregion
    }

    public void TriggerMoveAnimation(int param, float speed)
    {
        animator.SetFloat(param, speed);
    }

    #region cmd flip sprite
    public void FlipSprite(float move)
    {
        if (!isLocalPlayer) return;

        if (move < 0 && !isFlipped)
        {
            CmdFlipSprite(true);
        }
        else if (move > 0 && isFlipped)
        {
            CmdFlipSprite(false);
        }
    }

    [Command]
    private void CmdFlipSprite(bool flipState)
    {
        isFlipped = flipState;
    }

    private void OnFlipChanged(bool oldValue, bool newValue)
    {
        rbSprite.flipX = newValue;
    }
    #endregion

    #region MonoBehaviour Callbacks
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rbSprite = rb.GetComponent<SpriteRenderer>();

        movementSM = new StateMachine();
        standing = new StandingState(this, movementSM);
        jumping = new JumpingState(this, movementSM);

        movementSM.Initialize(standing);
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (canMove)
        {
            movementSM.CurrentState.HandleInput();

            movementSM.CurrentState.LogicUpdate();
        }
    }

    private void FixedUpdate()
    {
        movementSM.CurrentState.PhysicsUpdate();
    }
    #endregion
}
