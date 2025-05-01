using Mirror;
using UnityEngine;

public class LadderClimbing : NetworkBehaviour
{
    [SerializeField] private AudioSource audioSource = null;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioClip[] stepsOnLadder;

    public float climbingSpeed = 5f;
    public float climbingStepsInterval = 0.5f;

    private float move;
    private Rigidbody2D rb;
    private NetworkIdentity networkIdentity;
    private bool isClimbing = false;
    private float defaultGravityScale;
    private float stepTimer = 0f;

    [SyncVar]
    private int currentStepIndex = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        networkIdentity = GetComponent<NetworkIdentity>();
        defaultGravityScale = rb.gravityScale;
    }

    private void Update()
    {
        if (networkIdentity != null && !networkIdentity.isLocalPlayer)
        {
            return;
        }

        HandleClimbing();
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
                CmdPlayStepSound();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0;
        }
    }

    [Command]
    private void CmdPlayStepSound()
    {
        RpcPlayStepSound();
    }

    [ClientRpc]
    private void RpcPlayStepSound()
    {
        PlayOneStepAudio();
    }

    private void PlayOneStepAudio()
    {
        if (stepsOnLadder.Length == 0) return;

        audioSource.clip = stepsOnLadder[currentStepIndex];
        audioSource.Play();

        if (isServer)
        {
            currentStepIndex = (currentStepIndex + 1) % stepsOnLadder.Length;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ladder"))
        {
            isClimbing = true;
            //SoundManager.Instance.isPlayerOnLadder = true;
            rb.gravityScale = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ladder"))
        {
            isClimbing = false;
            rb.gravityScale = defaultGravityScale;
            //SoundManager.Instance.isPlayerOnLadder = false;
        }
    }
}