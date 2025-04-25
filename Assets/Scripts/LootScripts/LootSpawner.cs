using Assets.Scripts.BuildingScripts.BuildingTypes;
using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] loot;

    private System.Random rand;
    private int lootCount;
    private int lootForSofas;
    private int lootForFloor;

    public void Spawn(System.Random rand)
    {
        this.rand = rand;
        lootCount = LevelSettings.LootObejctsCount;

        lootForSofas = Mathf.FloorToInt(lootCount * 0.8f);
        lootForFloor = lootCount - lootForSofas;

        SpawnObjectsInLootSofa();

        if (lootForFloor > 0)
        {
            SpawnLootOnTheFloor();
        }
    }

    private void SpawnObjectsInLootSofa()
    {
        Vector2[] sofaPositions = new Vector2[]
        {
            new Vector2(0.52f, 0.127f),
            new Vector2(0.52f, 0.696f),
            new Vector2(0.52f, 1.27f)
        };

        List<Vector2> sofasToProcess = new List<Vector2>(BuildingData.lootSofas);

        while (sofasToProcess.Count > 0 && lootForSofas > 0)
        {
            int randomSofaIndex = rand.Next(sofasToProcess.Count);
            Vector2 currentSofaPosition = sofasToProcess[randomSofaIndex];

            for (int j = 0; j < sofaPositions.Length; j++)
            {
                if (lootForSofas <= 0)
                    return;

                int randomLootIndex = rand.Next(loot.Length);

                Vector2 spawnPosition = currentSofaPosition + sofaPositions[j];
                GameObject spawnedObject = Instantiate(loot[randomLootIndex], spawnPosition, Quaternion.identity);
                NetworkServer.Spawn(spawnedObject);

                lootForSofas--;
            }

            sofasToProcess.RemoveAt(randomSofaIndex);
        }
    }

    private void SpawnLootOnTheFloor()
    {
        List<Vector2> lootPositions = new List<Vector2>(BuildingData.loot);

        while (lootForFloor > 0 && lootPositions.Count > 0)
        {
            int randomLootPosIndex = rand.Next(lootPositions.Count);
            Vector2 currentLootPosition = lootPositions[randomLootPosIndex];

            int randomLootIndex = rand.Next(loot.Length);

            GameObject spawnedObject = Instantiate(loot[randomLootIndex], currentLootPosition, Quaternion.identity);
            NetworkServer.Spawn(spawnedObject);

            lootPositions.RemoveAt(randomLootPosIndex);
            lootForFloor--;
        }
    }
}