using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Mirror;
using Assets.Scripts.BuildingScripts.BuildingTypes;

public class FadeImageScript : NetworkBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 3f;
    [SerializeField] private Text shipleftText;
    [SerializeField] private AudioClip startShipSound;
    [SerializeField] private GameObject movingBackgroundObject;
    private MovingBackground movingBackground;

    private AudioSource audioSource;
    private float textDuration = 10f;

    private bool isStartToEnd = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (movingBackgroundObject != null)
        {
            movingBackground = movingBackgroundObject.GetComponent<MovingBackground>();
        }
    }

    public void FadeOutAndLoadNextScene(float duration, int number)
    {
        if (duration >= 0) textDuration = duration;
        if (!isStartToEnd)
        {
            isStartToEnd = true;
            StartCoroutine(CountdownAndFadeOut());

            if (movingBackground != null)
            {
                movingBackground.StartSpeedingX();
            }
        }
    }

    private IEnumerator CountdownAndFadeOut()
    {
        audioSource.clip = startShipSound;
        audioSource.Play();

        shipleftText.gameObject.SetActive(true);

        float timer = textDuration;
        while (timer > 0f)
        {
            shipleftText.text = "Времени до начала полёта осталось: " + ((int)timer).ToString();
            timer -= Time.deltaTime;
            yield return null;
        }

        StartCoroutine(FadeOutAndLoadScene());
    }

    private IEnumerator FadeOutAndLoadScene()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = timer / fadeDuration;
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        ShipObjectsChecker.Instance.SetLootIdInList();

        StartFlight();
    }

    [Server]
    public void StartFlight()
    {
        BuildingData.ClearBuildingDataMap();
        if (!string.IsNullOrEmpty(MissionSettings.NameOfScene))
        {
            ServerChangeScene(MissionSettings.NameOfScene);
        }
        else
        {
            Debug.LogError("Имя сцены не указано.");
        }
    }

    [Server]
    private void ServerChangeScene(string sceneName)
    {
        if (NetworkManager.singleton != null)
        {
            NetworkManager.singleton.ServerChangeScene(sceneName);
        }
        else
        {
            Debug.LogError("NetworkManager не найден!");
        }
    }
}