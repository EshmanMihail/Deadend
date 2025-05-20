using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.VisualScripting;
using System.ComponentModel;

public class HideInCabinetScript : NetworkBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;

    private SoundManager soundManager;
    private Character characterMovement;
    private FlashLightBarController flashLightBarController;

    private GameObject cabinet;
    private bool inside = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        soundManager = GetComponent<SoundManager>();
        characterMovement = GetComponent<Character>();
        flashLightBarController = GetComponent<FlashLightBarController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;
        if (cabinet != null && Input.GetKeyDown(KeyCode.E))
        {
            if (isLocalPlayer)
            {
                UIManager.Instance.HideTextHint();
            }

            if (inside)
            {
                inside = false;
                CmdGetOutFromCabinet();
            }
            else
            {
                inside = true;
                CmdHideInCabinet();
            }
        }
    }

    [Command]
    private void CmdHideInCabinet()
    {
        RpcChangeState(false, false, false, false, true);
    }

    [Command]
    private void CmdGetOutFromCabinet()
    {
        RpcChangeState(true, true, true, true, false);
    }

    [ClientRpc]
    private void RpcChangeState(params bool[] bools)
    {
        soundManager.CmdPlayAudioClip(3, 1);

        rb.simulated = bools[0];
        boxCollider.enabled = bools[1];
        spriteRenderer.enabled = bools[2];

        characterMovement.IsPlayerCanMove(bools[3]);
        flashLightBarController.OnTerminal(bools[4]);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Cabinet"))
        {
            if (isLocalPlayer)
            {
                UIManager.Instance.ShowTextHint(transform.position, "ֽאזלטעו ֵ");
            }
            cabinet = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Cabinet"))
        {
            if (!inside) cabinet = null;
            if (isLocalPlayer)
            {
                UIManager.Instance.HideTextHint();
            }
        }
    }
}
