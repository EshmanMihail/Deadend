using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ComputerScript : NetworkBehaviour
{
    [SerializeField] private GameObject computerObj;
    [SerializeField] private Text moreMoneyText;
    [SerializeField] private InputField inputField;
    [SerializeField] private Text errorText;
    [SerializeField] private Text acceptedText;

    [SerializeField] private GameObject leftGate;
    [SerializeField] private GameObject rightGate;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource audioSource2D;
    [SerializeField] private AudioClip enterInComputer;
    [SerializeField] private AudioClip exitFromComputer;
    [SerializeField] private AudioClip[] keysSounds;
    [SerializeField] private AudioClip[] micSounds;

    [SerializeField] private GameObject placeForLoot;

    private GameObject currentPlayer;
    [SyncVar] private bool isActivated = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        inputField.onValueChanged.AddListener(PlayKeySound);
    }

    void Update()
    {
        if (currentPlayer != null)
        {
            if (currentPlayer.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                if (!isActivated && Input.GetKeyDown(KeyCode.E))
                {
                    UIManager.Instance.HideTextHint();

                    currentPlayer.gameObject.GetComponent<Character>().IsPlayerCanMove(false);
                    currentPlayer.gameObject.GetComponent<FlashLightBarController>().OnTerminal(true);

                    isActivated = true;
                    CmdTryActivateTextObject();
                }

                if (computerObj != null && Input.GetKeyDown(KeyCode.Return))
                {
                    string enteredText = inputField.text.Trim();
                    RunTheCommand(enteredText);
                    inputField.text = "";
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    CmdDeactivateTerminal();
                }
            }
        }
    }

    private void PlayKeySound(string text)
    {
        if (keysSounds.Length > 0)
        {
            AudioClip randomClip = keysSounds[Random.Range(0, keysSounds.Length)];
            audioSource.PlayOneShot(randomClip);
        }
    }

    private void RunTheCommand(string commandText)
    {
        NetworkIdentity playerIdentity = currentPlayer.GetComponent<NetworkIdentity>();

        if (commandText == "Submit" || commandText == "submit" || commandText == "sub")
        {
            ActivateLootFalling();

            OpenGates();
        }
        else
        {
            CmdShowErrorMessage(playerIdentity, "Неизвестная команда!");
        }

        inputField.ActivateInputField();
        inputField.Select();
    }

    private void ActivateLootFalling()
    {
        Collider2D[] colliders = Physics2D.OverlapAreaAll(placeForLoot.gameObject.GetComponent<BoxCollider2D>().bounds.min,
            placeForLoot.gameObject.GetComponent<BoxCollider2D>().bounds.max);

        int lootCommonCost = 0;

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].CompareTag("Loot"))
            {
                if (colliders[i].GetComponent<LootProperty>() != null)
                {
                    lootCommonCost += colliders[i].GetComponent<LootProperty>().currentCost;
                }
                if (colliders[i].GetComponent<FallingItem>() != null)
                {
                    colliders[i].GetComponent<FallingItem>().CmdActivatePhysics();
                }
            }
        }

        SyncMoneyAndSoldedStuff(lootCommonCost);
    }

    [Command(requiresAuthority = false)]
    private void SyncMoneyAndSoldedStuff(int lootCommonCost)
    {
        int randIndexOfMicSound = Random.Range(0, micSounds.Length);
        RpcSyncMoneyAndSoldedStuff(lootCommonCost, randIndexOfMicSound);
    }

    [ClientRpc]
    private void RpcSyncMoneyAndSoldedStuff(int lootCommonCost, int randSoundIndex)
    {
        QuotaSettings.money += lootCommonCost;
        QuotaSettings.soldLootAmount += lootCommonCost;

        if (QuotaSettings.soldLootAmount >= QuotaSettings.quota)
        {
            StartCoroutine(PlayMicSoundWithDelay(randSoundIndex));
        }
    }

    private IEnumerator PlayMicSoundWithDelay(int randSoundIndex)
    {
        yield return new WaitForSeconds(8);
        AudioClip randomClip = micSounds[randSoundIndex];
        audioSource2D.PlayOneShot(randomClip);
    }

    private void OpenGates()
    {
        HatchControl leftGateControl = leftGate.GetComponent<HatchControl>();
        if (leftGateControl != null)
        {
            leftGateControl.OpenHatch();
        }
        HatchControl rightGateControl = rightGate.GetComponent<HatchControl>();
        if (rightGateControl != null)
        {
            rightGateControl.OpenHatch();
        }
    }

    #region Show Accent or Error terminal message
    [Command(requiresAuthority = false)]
    private void ShowAcceptedMessage(NetworkIdentity playerIdentity, string massage)
    {
        if (playerIdentity != null && playerIdentity.connectionToClient != null)
        {
            TargetShowAccentMessage(playerIdentity.connectionToClient, massage);
        }
    }

    [TargetRpc]
    private void TargetShowAccentMessage(NetworkConnection target, string message)
    {
        StartCoroutine(ShowAcceptForTwoSeconds(message));
    }

    private IEnumerator ShowAcceptForTwoSeconds(string acceptMessage)
    {
        acceptedText.text = acceptMessage;
        acceptedText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        acceptedText.gameObject.SetActive(false);
    }


    [Command(requiresAuthority = false)]
    private void CmdShowErrorMessage(NetworkIdentity playerIdentity, string massage)
    {
        if (playerIdentity != null && playerIdentity.connectionToClient != null)
        {
            TargetShowErrorMessage(playerIdentity.connectionToClient, massage);
        }
    }

    [TargetRpc]
    private void TargetShowErrorMessage(NetworkConnection target, string message)
    {
        StartCoroutine(ShowErrorForTwoSeconds(message));
    }

    private IEnumerator ShowErrorForTwoSeconds(string errorMessage)
    {
        errorText.text = errorMessage;
        errorText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        errorText.gameObject.SetActive(false);
    }
    #endregion

    #region activate objet with text
    [Command(requiresAuthority = false)]
    private void CmdTryActivateTextObject()
    {
        if (currentPlayer != null)
        {
            NetworkIdentity playerIdentity = currentPlayer.GetComponent<NetworkIdentity>();
            if (playerIdentity != null && playerIdentity.connectionToClient != null)
            {
                TargetActivateTextObject(playerIdentity.connectionToClient);
            }
            else
            {
                Debug.LogWarning("Не удалось получить подключение при активации объекта.");
            }
        }
    }

    [TargetRpc]
    private void TargetActivateTextObject(NetworkConnection target)
    {
        audioSource.PlayOneShot(enterInComputer);
        computerObj.gameObject.SetActive(true);

        inputField.ActivateInputField();
        inputField.Select();
    }
    #endregion

    #region diactivate terminal
    [Command(requiresAuthority = false)]
    private void CmdDeactivateTerminal()
    {
        if (currentPlayer != null)
        {
            NetworkIdentity playerIdentity = currentPlayer.GetComponent<NetworkIdentity>();
            if (playerIdentity != null && playerIdentity.connectionToClient != null)
            {
                TargetDeactivateTerminal(playerIdentity.connectionToClient);
            }
            else
            {
                Debug.LogWarning("Не удалось получить подключение.");
            }
        }
    }

    [TargetRpc]
    private void TargetDeactivateTerminal(NetworkConnection target)
    {
        audioSource.PlayOneShot(exitFromComputer);
        errorText.gameObject.SetActive(false);
        acceptedText.gameObject.SetActive(false);

        computerObj.gameObject.SetActive(false);

        currentPlayer.gameObject.GetComponent<Character>().IsPlayerCanMove(true);
        currentPlayer.gameObject.GetComponent<FlashLightBarController>().OnTerminal(false);

        currentPlayer = null;
        isActivated = false;
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (currentPlayer == null) currentPlayer = collision.gameObject;

            if (collision.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                UIManager.Instance.ShowTextHint(transform.position, "Нажмите Е");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!isActivated) currentPlayer = null;
            if (collision.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                UIManager.Instance.HideTextHint();
            }
        }
    }
}
