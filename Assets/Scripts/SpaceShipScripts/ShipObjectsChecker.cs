using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipObjectsChecker : MonoBehaviour
{
    public static ShipObjectsChecker Instance;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int GetLootNumberInShip()
    {
        Collider2D[] colliders = Physics2D.OverlapAreaAll(gameObject.GetComponent<BoxCollider2D>().bounds.min,
            gameObject.GetComponent<BoxCollider2D>().bounds.max);

        int lootCountInShip = 0;

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].CompareTag("Loot"))
            {
                lootCountInShip++;
            }
        }
        return lootCountInShip;
    }

    public int GetLootCommonCostInShip()
    {
        Collider2D[] colliders = Physics2D.OverlapAreaAll(gameObject.GetComponent<BoxCollider2D>().bounds.min,
            gameObject.GetComponent<BoxCollider2D>().bounds.max);

        int lootCommonCost = 0;

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].CompareTag("Loot"))
            {
                if (colliders[i].GetComponent<LootProperty>() != null)
                {
                    lootCommonCost += colliders[i].GetComponent<LootProperty>().currentCost;
                }
            }
        }
        return lootCommonCost;
    }

    public void SetLootIdInList()
    {
        MissionSettings.lootInShip.Clear();

        Collider2D[] colliders = Physics2D.OverlapAreaAll(gameObject.GetComponent<BoxCollider2D>().bounds.min,
            gameObject.GetComponent<BoxCollider2D>().bounds.max);

        int countOfCollectedLoot = 0;
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].CompareTag("Loot"))
            {
                LootProperty lootProperty = colliders[i].GetComponent<LootProperty>();
                if (lootProperty != null)
                {
                    if (!lootProperty.isCollected)
                    {
                        lootProperty.isCollected = true;
                        countOfCollectedLoot++;
                    }

                    int lootId = lootProperty.Id;
                    int lootCost = lootProperty.currentCost;
                    MissionSettings.lootInShip.Add((lootId, colliders[i].transform.position, lootCost, true));
                }
            }
        }
        MissionSettings.countOfCollectedLoot = countOfCollectedLoot;
    }
}
