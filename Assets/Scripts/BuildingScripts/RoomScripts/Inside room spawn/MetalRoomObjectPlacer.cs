using Assets.Scripts.BuildingScripts.BuildingTypes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_spawn
{
    public class MetalRoomObjectPlacer : IRoomObjectPlacer
    {
        System.Random rand;

        private GameObject[] objects;
        private List<(Vector2, bool)> positions = new List<(Vector2, bool)>();

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
                GameObject obj = GetRandObject();

                if (rand.Next(0, 100) < obj.GetComponent<ObjectProperty>().chanceToSpawn)
                {
                    if (obj.tag == "LootSofa") BuildingData.lootSofas.Add(positions[i].Item1);

                    if (obj.GetComponent<ObjectProperty>().Width > 1
                        && !positions[i].Item2 && !BuildingData.ladder.Contains(positions[i].Item1))
                    {
                        Vector2 spawnPosition = positions[i].Item1;
                        UnityEngine.Object.Instantiate(obj, spawnPosition, Quaternion.identity);
                        i++;
                    }
                    else
                    {
                        Vector2 spawnPosition = positions[i].Item1;
                        UnityEngine.Object.Instantiate(obj, spawnPosition, Quaternion.identity);
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
