using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    [SerializeField] private string nameOfItem;

    [SerializeField] private byte medkitUseCount = 0;

    private GameObject player;

    private bool isTeleportingStarted = false;
    private bool isShowEmptyMedkitTextStart = false;

    private Text infoText;
    private float timerToteleport = 0;


    void Start()
    {
        infoText = GameObject.Find("CharacterSetUp/InfoText").GetComponent<Text>();

        if (infoText == null)
        {
            Debug.LogError("InfoText не найден. Убедитесь, что объект существует в сцене и имеет правильное имя.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        timerToteleport -= Time.deltaTime;
        if (isTeleportingStarted)
        {
            infoText.text = ((int)timerToteleport).ToString();
        }
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }

    public void NonPlayer()
    {
        player = null;
    }

    public void SetItemMethod()
    {
        if (nameOfItem == "medkit") MedkitHeal();

        if (nameOfItem == "teleport") TeleportToTheShip();
    }

    private void MedkitHeal()
    {
        if (medkitUseCount >= 4)
        {
            GetComponent<SpriteRenderer>().color = new Color(30, 0, 103);
            infoText.gameObject.SetActive(true);
            infoText.text = "Midkit is empty";
            if (!isShowEmptyMedkitTextStart) StartCoroutine(MedkitIsEmptyShow());
            return;
        }

        //player.GetComponent<HealthBar>().Heal(25);
        medkitUseCount++;

        if (medkitUseCount == 4)
        {
            GetComponent<SpriteRenderer>().color = new Color(30, 0, 103);
        }
    }


    private void TeleportToTheShip()
    {
        if (!isTeleportingStarted)
        {
            StartCoroutine(TeleportWaitTime());
            isTeleportingStarted = true;
            timerToteleport = 3;
            infoText.gameObject.SetActive(true);
        }
    }

    private IEnumerator TeleportWaitTime()
    {
        yield return new WaitForSeconds(3);
        infoText.gameObject.SetActive(false);
        isTeleportingStarted = false;
        if (player != null) player.transform.position = new Vector3(-32.3f, 46.29f, 0);
    }

    private IEnumerator MedkitIsEmptyShow()
    {
        yield return new WaitForSeconds(3);
        if (!isTeleportingStarted) infoText.gameObject.SetActive(false);
        isShowEmptyMedkitTextStart = false;
    }

    public string GetItemName()
    {
        return nameOfItem;
    }
}
