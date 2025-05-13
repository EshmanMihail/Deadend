using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MissionDayController : NetworkBehaviour
{
    [SerializeField] private Light2D globalLight;
    [SerializeField] private float targetIntensity = 0f;
    [SerializeField] private float transitionTime = 10f;

    [SerializeField] private GameObject dayBackground;
    [SerializeField] private GameObject nightBackground;

    private float initialIntensity;
    private float elapsedTime;

    private void Start()
    {
        initialIntensity = globalLight.intensity;
        transitionTime *= 60;
        elapsedTime = 0f;
        if (nightBackground != null) nightBackground.SetActive(false);

        SyncGlobalIntensityTime();
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= transitionTime)
        {
            globalLight.intensity = targetIntensity;
            ChangeBackground();
        }
        else
        {
            float t = elapsedTime / transitionTime;
            globalLight.intensity = Mathf.Lerp(initialIntensity, targetIntensity, t);
        }
    }

    private void ChangeBackground()
    {
        if (dayBackground != null && nightBackground != null)
        {
            dayBackground.SetActive(false);
            nightBackground.SetActive(true);
        }
    }

    public float GetIntensity()
    {
        return globalLight.intensity;
    }

    [Server]
    private void SyncGlobalIntensityTime()
    {
        CmdSendServerGlobalIntensity(globalLight.intensity);
    }

    [Command(requiresAuthority = false)]
    private void CmdSendServerGlobalIntensity(float intensity)
    {
        RpcSendServerGlobalIntensity(intensity);
    }

    [ClientRpc]
    private void RpcSendServerGlobalIntensity(float intensity)
    {
        globalLight.intensity = intensity;
    }
}
