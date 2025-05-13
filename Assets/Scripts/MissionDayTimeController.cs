using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MissionDayTimeController : NetworkBehaviour
{
    [SerializeField] private Text timeText;
    [SerializeField] private float levelDurationInMinutes = 10f;
    private float levelTimer;

    [SerializeField] private GameObject fadeImage; 
    private FadeImageScript screenFader;

    private bool endGameStarted = false;

    private int startHour = 8;
    private int endHour = 24;

    void Start()
    {
        levelTimer = levelDurationInMinutes * 60f;

        screenFader = fadeImage.GetComponent<FadeImageScript>();

        SyncGameTime();
    }

    void Update()
    {
        if (levelTimer > 0f)
        {
            levelTimer -= Time.deltaTime;
            UpdateTimeText();
        }
        else if (!endGameStarted)
        {
            EndGame();
        }
    }

    void UpdateTimeText()
    {
        float timeProgress = 1f - (levelTimer / (levelDurationInMinutes * 60f));

        float currentHour = Mathf.Lerp(startHour, endHour, timeProgress);
        int hour = Mathf.FloorToInt(currentHour);
        int minute = Mathf.FloorToInt((currentHour - hour) * 60);

        string period = hour >= 12 && hour < 24 ? "PM" : "AM";

        int displayHour = hour > 12 ? hour - 12 : hour;
        if (displayHour == 0) displayHour = 12;
        timeText.text = string.Format("{0:00}:{1:00} {2}", displayHour, minute, period);
    }

    void EndGame()
    {
        endGameStarted = true;

        fadeImage.SetActive(true);

        screenFader.FadeOutAndLoadNextScene(60f, 1);
    }

    [Server]
    private void SyncGameTime()
    {
        CmdSendServerTime(levelTimer);
    }

    [Command(requiresAuthority = false)]
    private void CmdSendServerTime(float serverTime)
    {
        RpcSendServerTime(serverTime);
    }

    [ClientRpc]
    private void RpcSendServerTime(float serverTime)
    {
        levelTimer = serverTime;
    }
}