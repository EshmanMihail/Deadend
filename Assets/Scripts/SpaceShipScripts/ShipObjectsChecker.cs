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

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].CompareTag("Loot"))
            {
                if (colliders[i].GetComponent<LootProperty>() != null)
                {
                    int lootId = colliders[i].GetComponent<LootProperty>().Id;
                    int lootCost = colliders[i].GetComponent<LootProperty>().currentCost;
                    MissionSettings.lootInShip.Add((lootId, colliders[i].transform.position, lootCost));
                }
            }
        }
    }
}
