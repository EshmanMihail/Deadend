using UnityEngine;

public class LadderClimbing : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource = null;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioClip[] stepsOnLadder;

    public float climbingSpeed = 5f;
    public float climbingStepsInterval = 0.5f;

    private float move;

    private Rigidbody2D rb;
    private bool isClimbing = false;
    private float defaultGravityScale;
    private float stepTimer = 0f;
    private int currentStepIndex = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravityScale = rb.gravityScale;
    }

    private void Update()
    {
        HandleClimbing();
    }

    private void FixedUpdate()
    {
        
    }

    private void HandleClimbing()
    {
        if (isClimbing)
        {
            move = Input.GetAxisRaw("Vertical");

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Mathf.Abs(move) > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, move * climbingSpeed);
                if (Mathf.Approximately(move, 0f))
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0f);
                }

                PlayStepsAudio(move, climbingStepsInterval);
            }
        }
    }

    public void PlayStepsAudio(float move, float stepInterval)
    {
        if (Mathf.Abs(move) > 0)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0)
            {
                PlayOneStepAudio();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0;
        }
    }

    private void PlayOneStepAudio()
    {
        if (stepsOnLadder.Length == 0) return;

        audioSource.clip = stepsOnLadder[currentStepIndex];
        audioSource.Play();

        currentStepIndex = (currentStepIndex + 1) % stepsOnLadder.Length;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ladder"))
        {
            isClimbing = true;
            SoundManager.Instance.isPlayerOnLadder = true;
            rb.gravityScale = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ladder"))
        {
            isClimbing = false;
            rb.gravityScale = defaultGravityScale;
            SoundManager.Instance.isPlayerOnLadder = false;
        }
    }
}