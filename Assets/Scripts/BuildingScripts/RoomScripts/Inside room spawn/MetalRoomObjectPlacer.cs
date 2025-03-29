using Assets.Scripts.BuildingScripts.BuildingTypes;
using Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_spawn
{
    public class MetalRoomObjectPlacer : IRoomObjectPlacer
    {
        System.Random rand;

        private GameObject[] objects;
        private List<PositionProperty> positions = new List<PositionProperty>();

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

                if (true || rand.Next(0, 100) < obj.GetComponent<ObjectProperty>().chanceToSpawn)
                {
                    if (obj.tag == "LootSofa") BuildingData.lootSofas.Add(new Vector2(positions[i].X, positions[i].Y));

                    Vector2 position = new Vector2(positions[i].X, positions[i].Y);

                    int objectWidth = obj.GetComponent<ObjectProperty>().Width;
                    int objectHeight = obj.GetComponent<ObjectProperty>().Height;
                    if (objectWidth <= positions[i].widthRight && objectHeight <= positions[i].height && !BuildingData.ladder.Contains(position))
                    {
                        UnityEngine.Object.Instantiate(obj, position, Quaternion.identity);
                        i += objectWidth - 1;
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
