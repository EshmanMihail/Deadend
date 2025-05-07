using Mirror;
using System;
using UnityEngine;

public class BuildingDoorScript : NetworkBehaviour
{
    [SerializeField] private AudioClip openDoorSound;
    [SerializeField] private AudioClip closeDoorSound;
    [SerializeField] private AudioClip unlookDoorSound;
    [SerializeField] private Sprite openDoorSprite;
    [SerializeField] private Sprite closeDoorSprite;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D doorCollider;
    private BoxCollider2D triggerDoorCollider;
    private AudioSource audioSource;
    private bool isOpen = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        doorCollider = GetComponent<BoxCollider2D>();
        triggerDoorCollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    public void ChangeDoorState()
    {
        CmdToggleDoor();
    }

    [Command(requiresAuthority = false)]
    private void CmdToggleDoor()
    {
        isOpen = !isOpen;
        RpcUpdateDoorState(isOpen);
    }

    [ClientRpc]
    private void RpcUpdateDoorState(bool isOpen)
    {
        audioSource.clip = isOpen ? openDoorSound : closeDoorSound;
        audioSource.Play();

        spriteRenderer.sprite = isOpen ? openDoorSprite : closeDoorSprite;

        if (doorCollider != null)
        {
            doorCollider.enabled = !isOpen;
        }

        if (triggerDoorCollider != null)
        {
            triggerDoorCollider.enabled = !isOpen;
        }
    }
}
