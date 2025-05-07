using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : NetworkBehaviour
{
    private Image[] slots;
    private Image[] slotsImages;
    private Sprite emptySlotSprite;

    [SerializeField] private PlayerOnGroundChecker groundedChecker;

    private List<GameObject> inventory = new List<GameObject>();
    private List<bool> isInventorySlotFool = new List<bool>();

    private GameObject selectedItem;
    private int numberOfSelectedItem = 0;

    void Start()
    {
        slots = UIManager.Instance.slots;
        slotsImages = UIManager.Instance.slotsImages;
        emptySlotSprite = UIManager.Instance.emptySlotSprite;

        for (int i = 0; i < 4; i++)
        {
            isInventorySlotFool.Add(false);
        }
        for (int i = 0; i < 4; i++)
        {
            inventory.Add(null);
        }
        ChangeColor();
    }

    void Update()
    {
        if (!isLocalPlayer) return;
        SelectSlot();
        SelectSlotWithMouseWheel();
    }

    private void SelectSlotWithMouseWheel()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            // Прокрутка колесика мыши вверх
            numberOfSelectedItem--;
            if (numberOfSelectedItem < 0)
                numberOfSelectedItem = inventory.Count - 1;
            ChangeColor();
        }
        else if (scroll < 0f)
        {
            // Прокрутка колесика мыши вниз
            numberOfSelectedItem++;
            if (numberOfSelectedItem >= inventory.Count)
                numberOfSelectedItem = 0;
            ChangeColor();
        }
    }

    private void SelectSlot()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (isInventorySlotFool[0])
            {
                selectedItem = inventory[0];
            }
            numberOfSelectedItem = 0;
            ChangeColor();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (isInventorySlotFool[1])
            {
                selectedItem = inventory[1];
            }
            numberOfSelectedItem = 1;
            ChangeColor();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (isInventorySlotFool[2])
            {
                selectedItem = inventory[2];
            }
            numberOfSelectedItem = 2;
            ChangeColor();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (isInventorySlotFool[3])
            {
                selectedItem = inventory[3];
            }
            numberOfSelectedItem = 3;
            ChangeColor();
        }
    }

    private void ChangeColor()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (i == numberOfSelectedItem) slots[i].color = Color.gray;
            else slots[i].color = Color.blue;
        }
    }

    #region Add loot
    public void AddLootToInventory(GameObject loot)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (!isInventorySlotFool[i])
            {
                inventory[i] = loot;
                isInventorySlotFool[i] = true;

                CmdMoveLoot(loot, new Vector3(0, 150, 10));

                slotsImages[i].sprite = loot.GetComponent<SpriteRenderer>().sprite;
                break;
            }
        }
    }

    [Command]
    private void CmdMoveLoot(GameObject loot, Vector3 newPosition)
    {
        loot.transform.position = newPosition;
        RpcMoveLoot(loot, newPosition);
    }

    [ClientRpc]
    private void RpcMoveLoot(GameObject loot, Vector3 newPosition)
    {
        loot.transform.position = newPosition;
    }
    #endregion

    #region Drop loot
    public void DropSelectedLoot()
    {
        bool bGrounded = groundedChecker.isPlayerOnGround;
        if (isInventorySlotFool[numberOfSelectedItem] && bGrounded)
        {
            Vector3 vec = new Vector3(0, 0.26f, 0);
            if (inventory[numberOfSelectedItem].GetComponent<InventoryItem>() != null)
            {
                inventory[numberOfSelectedItem].GetComponent<InventoryItem>().NonPlayer();
                vec = new Vector3(0, 0, 0);
            }

            //inventory[numberOfSelectedItem].transform.position = transform.position - vec;
            CmdMoveLoot(inventory[numberOfSelectedItem], transform.position - vec);
            inventory[numberOfSelectedItem].GetComponent<LootProperty>().CmdPlayDropAudioClip();

            inventory[numberOfSelectedItem] = null;
            isInventorySlotFool[numberOfSelectedItem] = false;
            slotsImages[numberOfSelectedItem].sprite = emptySlotSprite;
        }
    }

    public void DropAllLoot()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] != null)
            {
                Vector3 vec = new Vector3(0, 0.26f, 0);
                inventory[i].transform.position = transform.position - vec;
                inventory[i] = null;
                isInventorySlotFool[i] = false;
                slotsImages[i].sprite = emptySlotSprite;
            }
        }
    }
    #endregion
}
