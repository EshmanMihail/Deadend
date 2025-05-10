using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.BuildingScripts.BuildingTypes
{
    public static class BuildingData
    {
        public static List<Vector2> node = new();

        public static List<Vector2> ladder = new();

        public static List<(Vector2, RoomBiom)> lamp = new();

        public static List<(Vector2, RoomBiom)> door = new();

        public static List<Vector2> loot = new();

        public static List<Vector2> lootSofas = new();

        public static void ClearBuildingDataMap()
        {
            node.Clear();
            ladder.Clear();
            door.Clear();
            loot.Clear();
            lootSofas.Clear();
            lamp.Clear();
        }

    }
}
