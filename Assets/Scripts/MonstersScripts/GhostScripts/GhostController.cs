using Assets.Scripts.BuildingScripts.BuildingTypes;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private List<Vector2> nodes;
    private int numberOfMaskPlayer;

    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private float damageDistance = 1.0f;
    [SerializeField] private float standingTime = 5f;
    [SerializeField] private AudioClip chaseClip;

    private bool isSeePlayer = false;
    private bool isChasePlayer = false;
    private bool isStanding = true;
    private bool isRouming = false;
    private bool isTimerToStandStart = false;
    private bool isTimeToGetNewPosition = false;
    private bool isFirstTimeSeePlayer = true;

    private float timerStand = 0;
    private Vector2 targetPosition;
    private GameObject targetPlayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        nodes = BuildingData.node;
        numberOfMaskPlayer = LayerMask.GetMask("Player");
    }

    void Update()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.localPosition, 10, numberOfMaskPlayer);

        if (playerCollider != null)
        {
            targetPlayer = playerCollider.gameObject;
            isChasePlayer = true;
            isTimeToGetNewPosition = false;
            isRouming = false;
        }
        else
        {
            isChasePlayer = false;
            if (!isRouming && !isStanding && !isTimeToGetNewPosition && !isTimerToStandStart)
            {
                isStanding = true;
                isFirstTimeSeePlayer = true;
            }
        }

        if (isTimerToStandStart)
        {
            timerStand += Time.deltaTime;
            if (timerStand >= standingTime)
            {
                isTimeToGetNewPosition = true;
                isTimerToStandStart = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (isChasePlayer)
        {
            if (isFirstTimeSeePlayer)
            {
                audioSource.PlayOneShot(chaseClip);
                isFirstTimeSeePlayer = false;
            }

            if (targetPlayer.GetComponent<SpriteRenderer>().enabled)
            {
                Debug.Log("chsing");
                ChasePlayer();
            }
            else
            {
                isChasePlayer = false;
                isStanding = true;
                isFirstTimeSeePlayer = true;
            }
        }
        else
        {
            if (isStanding)
            {
                timerStand = 0;
                isTimerToStandStart = true;
                isStanding = false;
            }
            else if (isTimeToGetNewPosition)
            {
                GetRandomPosition();
                isTimeToGetNewPosition = false;
                isRouming = true;
            }
            else if (isRouming)
            {
                MoveToTargetPosition();
            }
        }
    }

    private void ChasePlayer()
    {
        if (targetPlayer != null)
        {
            Vector2 currentPosition = rb.position;
            Vector2 direction = ((Vector2)targetPlayer.transform.position - currentPosition).normalized;
            Vector2 newPosition = currentPosition + direction * moveSpeed * Time.fixedDeltaTime;

            rb.MovePosition(newPosition);

            float distanceToPlayer = Vector2.Distance(currentPosition, targetPlayer.transform.position);

            if (distanceToPlayer < damageDistance)
            {
                targetPlayer.GetComponent<HealthBar>().CmdTakeDamage(damageAmount);
            }
        }
    }

    private void GetRandomPosition()
    {
        int numberOfNewPosition = Random.Range(0, nodes.Count);
        targetPosition = new Vector2(nodes[numberOfNewPosition].x, nodes[numberOfNewPosition].y);
    }

    private void MoveToTargetPosition()
    {
        Vector2 currentPosition = rb.position;
        Vector2 direction = (targetPosition - currentPosition).normalized;
        Vector2 newPosition = currentPosition + direction * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(newPosition);

        if (Vector2.Distance(currentPosition, targetPosition) < 0.1f)
        {
            isRouming = false;
            isStanding = true;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 10f);
    }
}
