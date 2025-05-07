using Mirror;
using System;
using UnityEngine;

public class LootProperty : NetworkBehaviour
{
    [SerializeField] private AudioClip dropSound;
    public string name = "";
    public int weight = 10;

    public int minCost = 5;
    public int maxCost = 50;

    [HideInInspector] public int currentCost = 0;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        System.Random rand = new System.Random(Guid.NewGuid().GetHashCode());
        currentCost = rand.Next(minCost, maxCost);
    }

    [Command(requiresAuthority = false)]
    public void CmdPlayDropAudioClip()
    {
        RpcPlayAudioClip();
    }

    [ClientRpc]
    private void RpcPlayAudioClip()
    {
        PlayAudioClip();
    }

    private void PlayAudioClip()
    {
        if (audioSource != null)
        {
            audioSource.clip = dropSound;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudipSource is not alive!");
        }
    }
}
