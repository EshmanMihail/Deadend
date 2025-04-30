using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    private float defaultPitch;

    [SerializeField] private AudioSource audioSource = null;
    [SerializeField] private AudioSource footStepsAudio = null;

    [SerializeField] private AudioClip[] stepSounds;
    [SerializeField] private AudioClip[] stepSoundsOnSand;
    [SerializeField] public AudioClip jumpHit;

    [HideInInspector]public bool isPlayerOnLadder;

    [SerializeField] private float minPitch = 0f;
    [SerializeField] private float maxPitch = 1f;
    [SerializeField] private float maxSpeed = 200f;

    private int currentStepIndex = 0;
    private bool isPlayerOnMetalWall = false;

    private float stepRunInterval = 0.3f;
    private float stepTimer = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        defaultPitch = audioSource.pitch;
    }

    public void PlayStepsAudio(float move, float stepInterval)
    {
        if (Mathf.Abs(move) > 0)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0)
            {
                PlayOneStepAudio();
                stepTimer = stepInterval;
            }
        }
        //else
        //{
        //    stepTimer = 0;
        //}
    }

    private void PlayOneStepAudio()
    {
        if (isPlayerOnMetalWall)
        {
            if (stepSounds.Length == 0) return;

            footStepsAudio.clip = stepSounds[currentStepIndex];
            footStepsAudio.Play();

            currentStepIndex = (currentStepIndex + 1) % stepSounds.Length;
        }
        else
        {
            if (stepSounds.Length == 0) return;

            footStepsAudio.clip = stepSoundsOnSand[currentStepIndex];
            footStepsAudio.Play();

            currentStepIndex = (currentStepIndex + 1) % stepSounds.Length;
        }
    }

    public void PlayJumpHit(bool randomPitch = false)
    {
        //footStepsAudio.pitch = randomPitch ? Random.Range(minPitch, maxPitch) : defaultPitch;
        footStepsAudio.PlayOneShot(jumpHit);
    }

    public void PlaySound(AudioClip clip, bool randomPitch = false)
    {
        audioSource.pitch = randomPitch ? Random.Range(minPitch, maxPitch) : defaultPitch;
        audioSource.PlayOneShot(clip);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MetalWalls"))
        {
            isPlayerOnMetalWall = true;

        }
        else if (collision.gameObject.CompareTag("Sand"))
        {
            isPlayerOnMetalWall = false;
        }
    }
}
