using Assets.Scripts.CharactersStates;
using Mirror;
using UnityEngine;

public class Character : NetworkBehaviour
{
    private StateMachine movementSM;
    public StandingState standing;
    public JumpingState jumping;

    [SerializeField] private Animator animator;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private PlayerOnGroundChecker playerOnGroundChecker;
    public float characterNormaWalkSpeed = 5f;
    public float characterRunningSpeed = 12f;

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
    private Vector3 previousPosition;
    private float highestPosition;

    private bool canMove = true;
    private bool bGrounded = false;

    [SyncVar(hook = nameof(OnFlipChanged))]
    private bool isFlipped;

    private int groundSoundIndex = 0;

    private HealthBar healthBar;

    #region Movement methods

    public void MoveRightAndLeft(float move, float speedNow, float stepsInterval)
    {
        if (!isLocalPlayer) return;

        FlipSprite(move);

        if (playerOnGroundChecker.isPlayerOnGround)
        {
            soundManager.PlayStepAudio(move, stepsInterval, groundSoundIndex);
        }

        rb.velocity = new Vector2(move * speedNow, rb.velocity.y);
    }

    public void Jump(int animParam)
    {
        if (!isLocalPlayer) return;

        bGrounded = playerOnGroundChecker.isPlayerOnGround;

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

        // Урон от падения
        HandleFallDamage();
    }

    private void HandleFallDamage()
    {
        if (!bGrounded)
        {
            if (transform.position.y < previousPosition.y && firstTime)
            {
                firstTime = false;
                isFalling = true;
                highestPosition = transform.position.y;
            }
            previousPosition = transform.position;
        }

        if (bGrounded && isFalling)
        {
            float fallHeight = Mathf.Abs(highestPosition - transform.position.y);
            float volume = Mathf.Clamp(fallHeight / 10f, 0.1f, 1f);

            // Вызываем звук с динамическим уровнем громкости

            soundManager.CmdPlayAudioClip(0, volume);
            if ((int)(Mathf.Abs(highestPosition - transform.position.y)) > maxDifferenceHeightToTakeDamage)
            {
                CmdTakeFallDamage((int)(Mathf.Abs(highestPosition - transform.position.y)) * 2);
            }
            isFalling = false;
            firstTime = true;
        }
    }

    [Command]
    private void CmdTakeFallDamage(int damage)
    {
        healthBar = GetComponent<HealthBar>();
        soundManager.CmdPlayAudioClip(2, damage);
        healthBar.CmdTakeDamage(damage);
    }

    public void IsPlayerCanMove(bool isCanMove)
    {
        if (!isLocalPlayer) return;
        canMove = isCanMove;
    }

    public void TriggerMoveAnimation(int param, float speed)
    {
        animator.SetFloat(param, speed);
    }
    #endregion

    #region Flip Sprite
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
        if (rbSprite != null)
        {
            rbSprite.flipX = newValue;
        }
    }
    #endregion

    #region MonoBehaviour Callbacks
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rbSprite = GetComponent<SpriteRenderer>();

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MetalWalls") || collision.gameObject.CompareTag("OneWayPlatform"))
        {
            groundSoundIndex = 0;
        }
        if (collision.gameObject.CompareTag("Sand"))
        {
            groundSoundIndex = 1;
        }
    }
    #endregion
}