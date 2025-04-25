using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class TerminalMonitorScript : NetworkBehaviour
{
    [SyncVar] private GameObject currentPlayer;

    [SerializeField] private InputField inputField;

    [SerializeField] private GameObject[] textObjects;

    [SerializeField] private Text errorText;
    [SerializeField] private Text acceptedText;

    [SerializeField] private string[] namesOfMissions;

    private void Update()
    {
        if (currentPlayer != null && Input.GetKeyDown(KeyCode.E))
        {
            CmdTryActivateTextObject(0);
        }

        if (currentPlayer != null && textObjects[0] != null && Input.GetKeyDown(KeyCode.Return))
        {
            string enteredText = inputField.text.Trim();
            RunTheCommand(enteredText);
            inputField.text = "";
        }

        if (currentPlayer != null && Input.GetKeyDown(KeyCode.Escape))
        {
            CmdDeactivateTerminal();
        }
    }

    private void RunTheCommand(string commandText)
    {
        if (commandText == "Commands" || commandText == "commands" || commandText == "Com" || commandText == "com")
        {
            CmdTryActivateTextObject(1);
        }
        if (commandText == "Missions" || commandText == "missions" || commandText == "mis" || commandText == "Mis")
        {
            CmdTryActivateTextObject(2);
        }
        if (commandText == "Experementation" || commandText == "exp")
        {
            CmdRunTheCommand("Experementation");
        }
        if (commandText == "Rend")
        {
            CmdRunTheCommand("Rend");
        }
        if (commandText == "Titan")
        {
            CmdRunTheCommand("Titan");
        }
        else
        {

        }
    }

    #region Run next scene
    [Command(requiresAuthority = false)]
    private void CmdRunTheCommand(string commandText)
    {
        if (commandText == "Experementation" || commandText == "Exp")
        {
            ServerChangeScene("Experementation");
        }
        else if (commandText == "Rend")
        {
            ServerChangeScene("Rend");
        }
        else if (commandText == "Titan")
        {
            ServerChangeScene("TestScene");
        }
        else
        {
            if (currentPlayer != null)
            {
                NetworkIdentity playerIdentity = currentPlayer.GetComponent<NetworkIdentity>();
                if (playerIdentity != null && playerIdentity.connectionToClient != null)
                {
                    TargetShowErrorMessage(playerIdentity.connectionToClient, "Неизвестная команда.");
                }
            }
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

    [TargetRpc]
    private void TargetShowErrorMessage(NetworkConnection target, string message)
    {
        errorText.text = message;
        errorText.gameObject.SetActive(true);
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
                Debug.LogWarning("Не удалось получить соединение для активации объекта.");
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
                Debug.LogWarning("Не удалось получить соединение для деактивации терминала.");
            }
        }
        currentPlayer = null;
    }

    [TargetRpc]
    private void TargetDeactivateTerminal(NetworkConnection target)
    {
        if (textObjects[0].activeSelf)
        {
            textObjects[0].SetActive(false);
        }
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && currentPlayer == null)
        {
            currentPlayer = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject == currentPlayer)
        {
            currentPlayer = null;
            NetworkIdentity playerIdentity = collision.gameObject.GetComponent<NetworkIdentity>();
            if (playerIdentity != null && playerIdentity.connectionToClient != null)
            {
                TargetDeactivateTerminal(playerIdentity.connectionToClient);
            }
        }
    }
}
