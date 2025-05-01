using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SoundManager : NetworkBehaviour
{
    public static SoundManager Instance;
    private float defaultPitch;

    [SerializeField] private AudioSource audioSource = null;
    [SerializeField] private AudioSource footStepsAudio = null;

    [SerializeField] private AudioClip[] stepSounds;
    [SerializeField] private AudioClip[] stepSoundsOnSand;
    [SerializeField] public AudioClip jumpHit;

    [HideInInspector] public bool isPlayerOnLadder;

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
        if (!isLocalPlayer) return; // ”бедимс€, что это локальный игрок

        if (Mathf.Abs(move) > 0)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0)
            {
                CmdPlayOneStepAudio();
                stepTimer = stepInterval;
            }
        }
    }

    [Command] // Ётот метод вызываетс€ клиентом, но выполн€етс€ на сервере
    private void CmdPlayOneStepAudio()
    {
        RpcPlayOneStepAudio();
    }

    [ClientRpc] // Ётот метод выполн€етс€ на всех клиентах
    private void RpcPlayOneStepAudio()
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
            if (stepSoundsOnSand.Length == 0) return;

            footStepsAudio.clip = stepSoundsOnSand[currentStepIndex];
            footStepsAudio.Play();

            currentStepIndex = (currentStepIndex + 1) % stepSoundsOnSand.Length;
        }
    }

    public void PlayJumpHit(bool randomPitch = false)
    {
        if (!isLocalPlayer) return; // ”бедимс€, что это локальный игрок

        CmdPlayJumpHit(randomPitch);
    }

    [Command] // Ётот метод вызываетс€ клиентом, но выполн€етс€ на сервере
    private void CmdPlayJumpHit(bool randomPitch)
    {
        RpcPlayJumpHit(randomPitch);
    }

    [ClientRpc] // Ётот метод выполн€етс€ на всех клиентах
    private void RpcPlayJumpHit(bool randomPitch)
    {
        //footStepsAudio.pitch = randomPitch ? Random.Range(minPitch, maxPitch) : defaultPitch;
        footStepsAudio.PlayOneShot(jumpHit);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isLocalPlayer) return; // ѕровер€ем, что это локальный игрок

        if (collision.gameObject.CompareTag("MetalWalls"))
        {
            CmdSetPlayerOnMetalWall(true);
        }
        else if (collision.gameObject.CompareTag("Sand"))
        {
            CmdSetPlayerOnMetalWall(false);
        }
    }

    [Command] // —инхронизаци€ состо€ни€ через сервер
    private void CmdSetPlayerOnMetalWall(bool isOnMetalWall)
    {
        isPlayerOnMetalWall = isOnMetalWall;
        RpcSetPlayerOnMetalWall(isOnMetalWall);
    }

    [ClientRpc] // ѕередаем состо€ние всем клиентам
    private void RpcSetPlayerOnMetalWall(bool isOnMetalWall)
    {
        isPlayerOnMetalWall = isOnMetalWall;
    }
}