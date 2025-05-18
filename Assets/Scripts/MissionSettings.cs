using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MissionSettings
{
    public static bool isComingFromMenu = true;

    public static int sceneIndex = 1;
    public static string NameOfScene = "Space";

    public static List<(int, Vector3, int, bool)> lootInShip = new();
    public static int countOfCollectedLoot = 0;
    public static int countOfDeath = 0;
}
