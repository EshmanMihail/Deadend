using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : NetworkBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource stepsAudioSource;

    [SerializeField] private AudioClip[] stepsOnMetal;
    [SerializeField] private AudioClip[] stepsOnGround;
    [SerializeField] private AudioClip[] audioClips;

    [SyncVar]
    private int currentStepIndex = 0;
 
    private List<List<AudioClip>> steps;
    private float stepTimer = 0.0f;
    private float defaultVolume = 1.0f;

    private void Awake()
    {
        steps = new List<List<AudioClip>>();
        AddStepSoundsOnList();
    }

    void Start()
    {
        defaultVolume = audioSource.volume;
    }

    private void AddStepSoundsOnList()
    {
        steps.Add(new List<AudioClip>());
        for (int i = 0; i < stepsOnMetal.Length; i++)
        {
            steps[0].Add(stepsOnMetal[i]);
        }

        steps.Add(new List<AudioClip>());
        for (int i = 0; i < stepsOnGround.Length; i++)
        {
            steps[1].Add(stepsOnGround[i]);
        }
    }

    #region Play one audio
    [Command]
    public void CmdPlayAudioClip(int audioIndex, float volume)
    {
        RpcPlayAudioClip(audioIndex, volume);
    }

    [ClientRpc]
    private void RpcPlayAudioClip(int audioIndex, float volume)
    {
        PlayAudioClip(audioIndex, volume);
    }

    private void PlayAudioClip(int audioIndex, float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
            audioSource.clip = audioClips[audioIndex];
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudipSource is not alive!");
        }
    }
    #endregion

    #region Play steps
    public void PlayStepAudio(float move, float stepInterval, int stepsSoundIndex)
    {
        if (Mathf.Abs(move) > 0)
        {
            if (stepTimer <= 0)
            {
                CmdPlayStepSound(stepsSoundIndex);
                stepTimer = stepInterval;
            }
            else
            {
                stepTimer -= Time.deltaTime;
            }
        }
    }

    [Command]
    private void CmdPlayStepSound(int stepsSoundIndex)
    {
        RpcPlayStepSound(stepsSoundIndex);
    }

    [ClientRpc]
    private void RpcPlayStepSound(int stepsSoundIndex)
    {
        PlayOneStepAudio(stepsSoundIndex);
    }

    private void PlayOneStepAudio(int stepsSoundIndex)
    {
        if (steps[stepsSoundIndex].Count == 0) return;

        stepsAudioSource.clip = steps[stepsSoundIndex][currentStepIndex];
        stepsAudioSource.Play();

        if (isServer)
        {
            currentStepIndex = (currentStepIndex + 1) % steps[stepsSoundIndex].Count;
        }
    }
    #endregion
}
