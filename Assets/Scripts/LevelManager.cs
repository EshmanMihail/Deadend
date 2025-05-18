using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : NetworkBehaviour
{
    public static LevelManager Instance;

    private List<HealthBar> players = new List<HealthBar>();
    private Image fadeImage;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    //[Server]
    public void RegisterPlayer(HealthBar player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
        }
    }

    [Server]
    public void UnregisterPlayer(HealthBar player)
    {
        if (players.Contains(player))
        {
            players.Remove(player);
        }
    }

    [Server]
    public void CheckAllPlayersDead()
    {
        foreach (var player in players)
        {
            if (!player.isCharacterDead)
            {
                return;
            }
        }

        EndLevel();
    }

    [Server]
    public void SetCountOfDeadPlayers()
    {
        int countOfDeadPlayers = 0;
        foreach (var player in players)
        {
            if (player.isCharacterDead)
            {
                countOfDeadPlayers++;
            }
        }
        MissionSettings.countOfDeath = countOfDeadPlayers;
        RpcSyncCountOfDeath(countOfDeadPlayers);
    }

    [ClientRpc]
    private void RpcSyncCountOfDeath(int countOfDeadPlayers)
    {
        MissionSettings.countOfDeath = countOfDeadPlayers;
    }

    [Server]
    private void EndLevel()
    {
        Debug.Log("Все игроки мертвы. Уровень завершён.");
        RpcEndLevel();
    }

    [ClientRpc]
    private void RpcEndLevel()
    {
        fadeImage = UIManager.Instance.fadeImage;
        fadeImage.GetComponent<FadeImageScript>().FadeOutAndLoadNextScene(10f, 111);
    }

    public List<HealthBar> GetPlayers()
    {
        return players;
    }
}