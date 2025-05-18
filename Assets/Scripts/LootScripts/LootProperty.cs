using Mirror;
using System;
using UnityEngine;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

public class LootProperty : NetworkBehaviour
{
    [SerializeField] private AudioClip dropSound;
    public int Id = 0;
    public string name = "";
    public int weight = 10;

    public int minCost = 5;
    public int maxCost = 50;

    [SyncVar] public int currentCost = -1;
    public bool isCollected = false;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SetCostOfItem();
    }

    [Server]
    private void SetCostOfItem()
    {
        if (currentCost == -1) currentCost = UnityEngine.Random.Range(minCost, maxCost + 1);
    }

    [Server]

   public void ChangeCost(int newCost)
   {
        currentCost = newCost;
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
