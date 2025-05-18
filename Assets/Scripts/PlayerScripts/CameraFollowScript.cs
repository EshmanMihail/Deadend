using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CameraFollowScript : NetworkBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float movingSpeed = 5f;

    private Camera mainCamera;

    private List<HealthBar> otherPlayers;
    private bool isLocalPlayerDead = false;
    private int playerIndex = -1;

    void Start()
    {
        if (!isLocalPlayer) return;

        mainCamera = Camera.main;

        if (mainCamera != null && targetTransform != null)
        {
            mainCamera.transform.position = new Vector3(
                targetTransform.position.x,
                targetTransform.position.y,
                targetTransform.position.z - 10
            );
            mainCamera.transform.SetParent(null);
        }
    }

    void Update()
    {
        if (!isLocalPlayer || targetTransform == null || mainCamera == null) return;

        if (isLocalPlayerDead && Input.GetKeyDown(KeyCode.Mouse0))
        {
            SwitchToNextPlayer();
        }

        Vector3 target = new Vector3(
            targetTransform.position.x,
            targetTransform.position.y,
            targetTransform.position.z - 10
        );

        Vector3 pos = Vector3.Lerp(mainCamera.transform.position, target, movingSpeed * Time.deltaTime);

        mainCamera.transform.position = pos;
    }

    public void OnPlayerDied(HealthBar deadPlayer)
    {
        isLocalPlayerDead = true;

        otherPlayers = LevelManager.Instance.GetPlayers();
        otherPlayers.RemoveAll(player => player.isCharacterDead);

        if (deadPlayer.transform == targetTransform)
        {
            SwitchToNextPlayer();
        }
    }

    private void SwitchToNextPlayer()
    {
        if (otherPlayers == null || otherPlayers.Count == 0)
        {
            Debug.LogWarning("Нет других игроков для переключения камеры.");
            return;
        }

        if (playerIndex < 0)
        {
            playerIndex = SetPlayerIndex();
        }
        else
        {
            playerIndex++;
            if (playerIndex == otherPlayers.Count) playerIndex = 0;

            if (otherPlayers[playerIndex].isCharacterDead || otherPlayers[playerIndex].transform == targetTransform)
            {
                playerIndex++;
                if (playerIndex == otherPlayers.Count) playerIndex = 0;
            }
        }

        targetTransform = otherPlayers[playerIndex].transform;
    }

    private int SetPlayerIndex()
    {
        for (int i = 0; i < otherPlayers.Count; i++)
        {
            if (!otherPlayers[i].isCharacterDead && otherPlayers[i].transform != targetTransform)
            {
                return i;
            }
        }
        return -1;
    }
}