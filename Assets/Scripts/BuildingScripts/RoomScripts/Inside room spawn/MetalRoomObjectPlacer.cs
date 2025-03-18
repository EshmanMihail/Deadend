using Assets.Scripts.BuildingScripts.BuildingTypes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_spawn
{
    public class MetalRoomObjectPlacer : IRoomObjectPlacer
    {
        System.Random rand;
        private int chanceToSpawnObject = 50;

        private GameObject[] objects;
        private List<Vector2> positions = new List<Vector2>();

        public MetalRoomObjectPlacer(System.Random rand)
        {
            this.rand = rand;
        }

        public void SetRoomObjects(Room room)
        {
            positions = room.GetPositionsToPlaceObjects();
            objects = room.GetObjects();

            for (int i = 0;  i < positions.Count; i++)
            {
                if (rand.Next(0, 100) < chanceToSpawnObject)
                {
                    GameObject obj = GetRandObject();

                    if (obj.tag == "LootSofa") BuildingData.lootSofas.Add(positions[i]);

                    if (obj.GetComponent<ObjectProperty>().Width > 1 && !BuildingData.ladder.Contains(positions[i + 1]))
                    {

                    }
                }
            }
        }

        private GameObject GetRandObject()
        {
            int randIndex = rand.Next(0, objects.Length);

            return objects[randIndex];
        }
    }
}
