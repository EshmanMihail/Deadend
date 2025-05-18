using Assets.Scripts.BuildingScripts.BuildingTypes;
using System.Collections;
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
    [SerializeField] private float standingTime = 5f;
    [SerializeField] private AudioClip chaseClip;

    private bool isSeePlayer = false;
    private bool isChasePlayer = false;
    private bool isStanding = true;
    private bool isRouming = false;
    private bool isTimerToStandStart = false;
    private bool isTimeToGetNewPosition = false;

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
        
    }

    void FixedUpdate()
    {
        
    }
}
