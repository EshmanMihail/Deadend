using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartLeverScript : NetworkBehaviour
{
    [SerializeField] AudioClip leverSound;
    private AudioSource audioSource;
    private SpriteRenderer sr;

    private GameObject startLeverUI;
    private Image startLeverFillingBar;
    private Text startLeverErrorText;
    private Image fadeImage;

    private GameObject player;
    private bool isUsed = false;

    [SerializeField] private Sprite startedSprite;
    [SerializeField] private float maxFillTime = 2f;
    private float fillTimer = 0f;
    private bool isFilling = false;

    private int currentSceneIndex = 1;

    void Start()
    {
        startLeverUI = UIManager.Instance.startLeverUI;
        startLeverFillingBar = UIManager.Instance.startLeverFillingBar;
        startLeverErrorText = UIManager.Instance.startLeverErrorText;
        fadeImage = UIManager.Instance.fadeImage;

        audioSource = GetComponent<AudioSource>();
        sr = GetComponent<SpriteRenderer>();

        MissionSettings.sceneIndex = 1;
        MissionSettings.NameOfScene = "SpaceShipScene";

        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    void Update()
    {
        if (player != null && Input.GetKeyDown(KeyCode.E))
        {
            isFilling = true;
            fillTimer = 0f;
        }
        if (player != null && Input.GetKeyUp(KeyCode.E))
        {
            isFilling = false;
            fillTimer = 0f;
            startLeverFillingBar.fillAmount = 0f;
        }

        if (isFilling)
        {
            fillTimer += Time.deltaTime;
            startLeverFillingBar.fillAmount = fillTimer / maxFillTime;

            if (fillTimer >= maxFillTime)
            {
                if (isUsed)
                {
                    ShowError("Задание выбрано!");
                }
                else if (currentSceneIndex == 1 && MissionSettings.sceneIndex == 1)
                {
                    ShowError("Необходимо выбрать задание!");
                }
                else
                {
                    CmdStartFlight();
                }
            }
        }
    }

    #region Error message
    public void ShowError(string errorMessage)
    {
        if (startLeverErrorText != null)
        {
            StartCoroutine(ShowErrorForTwoSeconds(errorMessage));
        }
    }
    private IEnumerator ShowErrorForTwoSeconds(string errorMessage)
    {
        startLeverErrorText.text = errorMessage;
        startLeverErrorText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        startLeverErrorText.gameObject.SetActive(false);
    }
    #endregion

    [Command(requiresAuthority = false)]
    private void CmdStartFlight()
    {
        RpcStartFlight();
    }

    [ClientRpc]
    private void RpcStartFlight()
    {
        isUsed = true;
        audioSource.clip = leverSound;
        audioSource.Play();
        sr.sprite = startedSprite;

        fadeImage.GetComponent<FadeImageScript>().FadeOutAndLoadNextScene(10f, 111);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            player = collision.gameObject;
            startLeverUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            player = null;
            if (startLeverUI != null)
            {
                startLeverUI.SetActive(false);
            }
        }
    }
}
