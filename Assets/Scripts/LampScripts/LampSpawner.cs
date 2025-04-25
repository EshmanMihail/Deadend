using Assets.Scripts.BuildingScripts.BuildingTypes;
using Assets.Scripts.BuildingScripts;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LampSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject metalLampPrefab;
    [SerializeField] private GameObject frozenLampPrefab;
    [SerializeField] private GameObject grassLampPrefab;

    public void SpawnLamps()
    {
        if (!NetworkServer.active)
        {
            Debug.LogError("SpawnLamps может быть вызван только на сервере!");
            return;
        }

        Quaternion rotation = Quaternion.Euler(0, 0, 180);

        foreach (var lamp in BuildingData.lamp)
        {
            Vector2 position = new Vector2(lamp.Item1.x + 0.5f, lamp.Item1.y + 0.5f);
            GameObject lampInstance = null;

            switch (lamp.Item2)
            {
                case RoomBiom.metal:
                    lampInstance = Instantiate(metalLampPrefab, position, rotation);
                    break;
                case RoomBiom.frozen:
                    lampInstance = Instantiate(frozenLampPrefab, position, rotation);
                    break;
                case RoomBiom.grass:
                    lampInstance = Instantiate(grassLampPrefab, position, rotation);
                    break;
                default:
                    Debug.LogWarning($"Неизвестный биом: {lamp.Item2}");
                    continue;
            }

            if (lampInstance != null)
            {
                NetworkServer.Spawn(lampInstance);
            }
        }
    }
}