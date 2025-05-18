using Mirror;
using UnityEngine;

public class BumpingOutsideScript : NetworkBehaviour
{
    [SerializeField] private AudioClip[] bumpings;
    [SerializeField] private Vector2 randomTimerRange = new Vector2(5f, 15f);

    private AudioSource audioSource;
    private float nextPlayTime;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (isServer)
        {
            SetNextPlayTime();
        }
    }

    void Update()
    {
        if (!isServer) return;

        if (Time.time >= nextPlayTime)
        {
            PlayRandomSound();
            SetNextPlayTime();
        }
    }

    private void SetNextPlayTime()
    {
        float randomDelay = Random.Range(randomTimerRange.x, randomTimerRange.y);
        nextPlayTime = Time.time + randomDelay;
    }

    private void PlayRandomSound()
    {
        int randomIndex = Random.Range(0, bumpings.Length);
        RpcPlaySound(randomIndex);
    }

    [ClientRpc]
    private void RpcPlaySound(int index)
    {
        if (index >= 0 && index < bumpings.Length)
        {
            audioSource.clip = bumpings[index];
            audioSource.Play();
        }
    }
}