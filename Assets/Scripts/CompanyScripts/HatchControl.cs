using UnityEngine;
using Mirror;

public class HatchControl : NetworkBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Collider2D collider;
    private AudioSource audioSource;

    [SerializeField] private AudioClip openClip;
    [SerializeField] private AudioClip closeClip;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource не найден на объекте люка!");
        }
    }

    [Command(requiresAuthority = false)]
    public void OpenHatch()
    {
        spriteRenderer.enabled = false;
        collider.enabled = false;

        RpcUpdateHatchState(false);

        RpcPlaySound(1);

        StartCoroutine(AutoCloseHatch());
    }

    [ClientRpc]
    private void RpcUpdateHatchState(bool state)
    {
        spriteRenderer.enabled = state;
        collider.enabled = state;
    }

    [ClientRpc]
    private void RpcPlaySound(int index)
    {
        if (audioSource != null)
        {
            if (index == 1)
            {
                audioSource.PlayOneShot(openClip);
            }
            else
            {
                audioSource.PlayOneShot(closeClip);
            }
        }
    }

    [Server]
    private System.Collections.IEnumerator AutoCloseHatch()
    {
        yield return new WaitForSeconds(6);
        CloseHatch();
    }

    [Server]
    public void CloseHatch()
    {
        spriteRenderer.enabled = true;
        collider.enabled = true;

        RpcUpdateHatchState(true);

        RpcPlaySound(2);
    }
}