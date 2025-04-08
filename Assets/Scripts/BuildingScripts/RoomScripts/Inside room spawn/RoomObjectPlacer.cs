using Assets.Scripts.BuildingScripts.BuildingTypes;
using Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_spawn
{
    public abstract class RoomObjectPlacer
    {
        protected System.Random rand;

        protected GameObject[] objects;
        protected List<PositionProperty> positions = new();

        public RoomObjectPlacer(System.Random rand)
        {
            this.rand = rand;
        }

        public virtual void SetRoomObjects(Room room)
        {
            PlaceObjets();
        }

        protected void SetPositionsAndObjectsOfRoom(Room room)
        {
            positions = room.GetPositionsToPlaceObjects();
            objects = room.GetObjects();
        }

        private void PlaceObjets()
        {
            for (int i = 0; i < positions.Count; i++)
            {
                GameObject obj = GetRandObject();

                if (rand.Next(0, 100) < obj.GetComponent<ObjectProperty>().chanceToSpawn)
                {
                    Vector2 position = new Vector2(positions[i].X, positions[i].Y);

                    int objectWidth = obj.GetComponent<ObjectProperty>().Width;
                    int objectHeight = obj.GetComponent<ObjectProperty>().Height;
                    if (objectWidth <= positions[i].widthRight && objectHeight <= positions[i].height && !BuildingData.ladder.Contains(position))
                    {
                        if (obj.tag == "LootSofa") BuildingData.lootSofas.Add(new Vector2(positions[i].X, positions[i].Y));
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
