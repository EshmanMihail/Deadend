using Mirror;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class SavedLootSpawner : NetworkBehaviour
{
    [SerializeField] private List<GameObject> lootDbPrefabs = new();

    private Dictionary<int, GameObject> lootDb = new();

    void Start()
    {
        InitializeLootDb();
        SpawnSavedLoot();
    }

    private void InitializeLootDb()
    {
        lootDb.Clear();
        for (int i = 0; i < lootDbPrefabs.Count; i++)
        {
            int lootId = lootDbPrefabs[i].GetComponent<LootProperty>().Id;
            if (!lootDb.ContainsKey(lootId))
            {
                lootDb.Add(lootId, lootDbPrefabs[i]);
            }
            else
            {
                Debug.LogWarning($"Дубликат ключа {lootId} обнаружен в lootDbEntries и будет проигнорирован.");
            }
        }
    }

    private void SpawnSavedLoot()
    {
        List<GameObject> spawnedLoot = new();

        for (int i = 0; i < MissionSettings.lootInShip.Count; i++)
        {
            int lootId = MissionSettings.lootInShip[i].Item1;

            if (lootDb.TryGetValue(lootId, out GameObject prefab))
            {
                GameObject loot = Instantiate(prefab, MissionSettings.lootInShip[i].Item2, Quaternion.identity);
                spawnedLoot.Add(loot);
                NetworkServer.Spawn(loot);
            }
            else
            {
                Debug.LogWarning($"Лут с ID {lootId} отсутствует в базе lootDb.");
            }
        }
        for (int i = 0; i < spawnedLoot.Count; i++)
        {
            spawnedLoot[i].GetComponent<LootProperty>().ChangeCost(MissionSettings.lootInShip[i].Item3);
        }
    }
}