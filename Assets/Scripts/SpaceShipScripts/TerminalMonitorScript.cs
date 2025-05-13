using Mirror;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TerminalMonitorScript : NetworkBehaviour
{
    [SyncVar] private GameObject currentPlayer;
    [SyncVar] private bool isActivated = false;

    [SerializeField] private InputField inputField;
    [SerializeField] private GameObject[] textObjects;
    [SerializeField] private Text errorText;
    [SerializeField] private Text acceptedText;
    [SerializeField] private string[] namesOfMissions;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] keysSounds;
    [SerializeField] private GameObject placeInShip;
    [SerializeField] private Text lootCountInShipText;
    [SerializeField] private Text lootCommonCostInShipText;

    private int currentSceneIndex = 1;

    void Start()
    {
        inputField.onValueChanged.AddListener(PlayKeySound);
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    private void Update()
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
                    CmdTryActivateTextObject(0);
                }

                if (textObjects[0] != null && Input.GetKeyDown(KeyCode.Return))
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

    private void RunTheCommand(string commandText)
    {
        NetworkIdentity playerIdentity = currentPlayer.GetComponent<NetworkIdentity>();

        if (commandText == "Commands" || commandText == "commands" || commandText == "Com" || commandText == "com" || commandText == "back")
        {
            CmdTryActivateTextObject(1);
        }
        else if (commandText == "Missions" || commandText == "missions" || commandText == "mis" || commandText == "Mis")
        {
            if (currentSceneIndex == 1)
            {
                CmdTryActivateTextObject(2);
            }
        }
        else if (commandText == "ShipLoot" || commandText == "shiploot")
        {
            CmdTryActivateTextObject(4);
            lootCountInShipText.text = "Предметов в корабле: " + ShipObjectsChecker.Instance.GetLootNumberInShip();
            lootCommonCostInShipText.text = "Общая стоимость: " + ShipObjectsChecker.Instance.GetLootCommonCostInShip();
        }
        else if (commandText == "Experementation" || commandText == "exp")
        {
            if (currentSceneIndex == 1)
            {
                CmdSyncAcceptedMission("Experementation", 2);
                ShowAcceptedMessage(playerIdentity, "Задание принято!");
            }
            else
            {
                CmdShowErrorMessage(playerIdentity, "Вы уже на задании!");
            }
        }
        else if (commandText == "Rend")
        {
            if (currentSceneIndex == 1)
            {
                CmdSyncAcceptedMission("Rend", 3);
                ShowAcceptedMessage(playerIdentity, "Задание принято!");
            }
            else
            {
                CmdShowErrorMessage(playerIdentity, "Вы уже на задании!");
            }
        }
        else if (commandText == "Titan")
        {
            if (currentSceneIndex == 1)
            {
                CmdSyncAcceptedMission("Titan", 4);
                ShowAcceptedMessage(playerIdentity, "Задание принято!");
            }
            else
            {
                CmdShowErrorMessage(playerIdentity, "Вы уже на задании!");
            }
        }
        else if (commandText == "Company")
        {
            if (currentSceneIndex == 1)
            {
                CmdSyncAcceptedMission("Company", 5);
                ShowAcceptedMessage(playerIdentity, "Задание принято!");
            }
            else
            {
                CmdShowErrorMessage(playerIdentity, "Вы на задании!");
            }
        }
        else
        {
            CmdShowErrorMessage(playerIdentity, "Неизвестная команда!");
        }

        inputField.ActivateInputField();
        inputField.Select();
    }

    private void PlayKeySound(string text)
    {
        if (keysSounds.Length > 0)
        {
            AudioClip randomClip = keysSounds[Random.Range(0, keysSounds.Length)];
            audioSource.PlayOneShot(randomClip);
        }
    }

    #region Sync choosen mission with server
    [Command(requiresAuthority = false)]
    private void CmdSyncAcceptedMission(string name, int index)
    {
        LevelSettings.SetLevelSettings(name);
        MissionSettings.NameOfScene = name;
        MissionSettings.sceneIndex = index;

        RpcSyncAcceptedMission(name, index);
    }

    [ClientRpc]
    private void RpcSyncAcceptedMission(string name, int index)
    {
        LevelSettings.SetLevelSettings(name);
        MissionSettings.NameOfScene = name;
        MissionSettings.sceneIndex = index;
    }
    #endregion

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
    private void CmdTryActivateTextObject(int objectIndex)
    {
        if (currentPlayer != null)
        {
            NetworkIdentity playerIdentity = currentPlayer.GetComponent<NetworkIdentity>();
            if (playerIdentity != null && playerIdentity.connectionToClient != null)
            {
                TargetActivateTextObject(playerIdentity.connectionToClient, objectIndex);
            }
            else
            {
                Debug.LogWarning("Не удалось получить подключение при активации объекта.");
            }
        }
    }

    [TargetRpc]
    private void TargetActivateTextObject(NetworkConnection target, int objectIndex)
    {
        if (objectIndex == 0)
        {
            textObjects[0].SetActive(true);
            textObjects[1].SetActive(true);

            for (int i = 2; i < textObjects.Length; i++)
            {
                textObjects[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 1; i < textObjects.Length; i++)
            {
                if (i == objectIndex) textObjects[i].SetActive(true);
                else textObjects[i].SetActive(false);
            }
        }

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
        isActivated = false;
        if (textObjects[0].activeSelf)
        {
            textObjects[0].SetActive(false);
        }

        currentPlayer.gameObject.GetComponent<Character>().IsPlayerCanMove(true);
        currentPlayer.gameObject.GetComponent<FlashLightBarController>().OnTerminal(false);

        currentPlayer = null;
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