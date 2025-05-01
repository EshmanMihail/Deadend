using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerOnGroundChecker : NetworkBehaviour
{
    public static PlayerOnGroundChecker Instance;

    [SyncVar][HideInInspector] public bool isPlayerOnGround;

    [SerializeField] private LayerMask lmWalls;
    [SerializeField] private LayerMask lmPlatform;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void FixedUpdate()
    {
        if (!isServer) return;

        Vector2 v2GroundedBoxCheckPosition = (Vector2)transform.position + new Vector2(0, -0.01f);
        Vector2 v2GroundedBoxCheckScale = (Vector2)transform.localScale + new Vector2(-0.02f, 0);

        bool grounded = Physics2D.OverlapBox(v2GroundedBoxCheckPosition, v2GroundedBoxCheckScale, 0, lmWalls);
        if (!grounded)
        {
            grounded = Physics2D.OverlapBox(v2GroundedBoxCheckPosition, v2GroundedBoxCheckScale, 0, lmPlatform);
        }
        isPlayerOnGround = grounded;
    }
}