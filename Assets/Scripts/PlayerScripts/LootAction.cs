using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class LootAction : NetworkBehaviour
{
    private List<GameObject> lootInRange = new List<GameObject>();

    private InventoryController inventoryController;

    void Start()
    {
        inventoryController = GetComponent<InventoryController>();
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (lootInRange.Count > 0) 
            {
                GameObject lootToPick = lootInRange[0];
                if (lootToPick.GetComponent<InventoryItem>() != null)
                {
                    lootToPick.GetComponent<InventoryItem>().SetPlayer(gameObject);
                }

                inventoryController.AddLootToInventory(lootToPick);

                lootInRange.Remove(lootToPick);
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            inventoryController.DropSelectedLoot();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Loot"))
        {
            if (!lootInRange.Contains(collision.gameObject))
            {
                lootInRange.Add(collision.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Loot"))
        {
            if (lootInRange.Contains(collision.gameObject))
            {
                lootInRange.Remove(collision.gameObject);
            }
        }
    }
}