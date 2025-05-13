using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelSettings
{
    public static int roomCount = 40;
    public static int LootObejctsCount = 66;
    public static int chanceToMakeGrassedBuilding = 10;
    public static void SetLevelSettings(string levelName)
    {
        switch (levelName)
        {
            case "Experementation":
                roomCount = Random.Range(25, 41);
                LootObejctsCount = Random.Range(40, 60);
                chanceToMakeGrassedBuilding = 1;
                break;

            case "Rend":
                roomCount = Random.Range(40, 70);
                LootObejctsCount = Random.Range(80, 140);
                chanceToMakeGrassedBuilding = 80;
                break;

            case "Titan":
                roomCount = Random.Range(80, 120);
                LootObejctsCount = Random.Range(150, 200);
                break;
        }
    }
}
