using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowResaultOfTheMission : MonoBehaviour
{
    [SerializeField] private GameObject resaultOfTheMission;
    [SerializeField] private Text lootCollected;
    [SerializeField] private Text numberOfDeath;
    [SerializeField] private Text minusMoney;
    [SerializeField] private Text totalMark;

    void Start()
    {
        if (!MissionSettings.isComingFromMenu)
        {
            lootCollected.text = "Собрано вещей: " + MissionSettings.countOfCollectedLoot.ToString() + '/' + LevelSettings.LootObejctsCount.ToString();

            numberOfDeath.text = "Игроков умерло: " + MissionSettings.countOfDeath.ToString();

            int minusPersents = 20 * MissionSettings.countOfDeath;
            minusMoney.text = "Убыток: -" + minusPersents.ToString() + '%';
            MinusPlayersMoneyForDeath(minusPersents);

            totalMark.text = "Итоговая оценка: " + SetTotalMark();

            StartCoroutine(ShowResaltObjectWithText());
        }
        else
        {
            MissionSettings.isComingFromMenu = false;
        } 
    }

    private void MinusPlayersMoneyForDeath(int minusPersents)
    {
        QuotaSettings.money -= QuotaSettings.money * minusPersents;
        if (QuotaSettings.money < 0) QuotaSettings.money = 0;
    }

    private string SetTotalMark()
    {
        string resalt = "F";

        if (MissionSettings.countOfCollectedLoot == LevelSettings.LootObejctsCount)
        {
            resalt = "A";
        }
        else if (MissionSettings.countOfDeath == 0 && MissionSettings.countOfCollectedLoot * 0.75 >= LevelSettings.LootObejctsCount)
        {
            resalt = "A";
        }
        else if (MissionSettings.countOfDeath < 2 && MissionSettings.countOfCollectedLoot * 0.6 >= LevelSettings.LootObejctsCount)
        {
            resalt = "B";
        }
        else if (MissionSettings.countOfDeath >= 2 || MissionSettings.countOfCollectedLoot * 0.4 >= LevelSettings.LootObejctsCount)
        {
            resalt = "C";
        }
        return resalt;
    }

    private IEnumerator ShowResaltObjectWithText()
    {
        resaultOfTheMission.SetActive(true);
        yield return new WaitForSeconds(8);
        resaultOfTheMission.SetActive(false);
    }
}
