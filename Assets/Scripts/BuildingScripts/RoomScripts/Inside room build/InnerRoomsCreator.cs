using Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build.Inner_rooms;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build
{
    public class InnerRoomsCreator
    {
        private Room room;
        private Tile[] roomTiles;

        private System.Random rand;

        private enum InnerRoomType
        {
            TopLeft = 0, TopRight = 1, BottomLeft = 2, BottomRight = 3
        }

        private List<Vector2> ocupiedPlaces;
        private  List<Vector2> laderPlaces;

        private int chanceToSpawnInnerRoom = 80;
        private int reduceChance = 20;

        public InnerRoomsCreator(Room room, System.Random random)
        {
            this.room = room;
            this.rand = random;

            roomTiles = room.GetTiles();

            ocupiedPlaces = new List<Vector2>();
            laderPlaces = new List<Vector2>();
        }

        public void CreateInnerRooms()
        {
            List<int> innerRoomTypesValues = new List<int>() { 0, 1, 2, 3 };

            while (chanceToSpawnInnerRoom > 0 && innerRoomTypesValues.Count > 0)
            {
                int randomIndex = rand.Next(0, innerRoomTypesValues.Count);
                InnerRoomType innerRoomType = (InnerRoomType)innerRoomTypesValues[randomIndex];

                //if (rand.Next(0, chanceToSpawnInnerRoom) < chanceToSpawnInnerRoom)
                //    InnerRoomManager(innerRoomType);
                InnerRoomManager(innerRoomType);

                innerRoomTypesValues.RemoveAt(randomIndex);
                chanceToSpawnInnerRoom -= reduceChance;
            }
        }

        private void InnerRoomManager(InnerRoomType innerRoomType)
        {
            switch (innerRoomType)
            {
                case InnerRoomType.TopLeft:
                    InnerRoom topLeftRoom = new TopLeftRoom(room, rand, roomTiles, ocupiedPlaces, laderPlaces);
                    topLeftRoom.CraeteRoom();
                    break;

                case InnerRoomType.TopRight:
                    InnerRoom topRightRoom = new TopRightRoom(room, rand, roomTiles, ocupiedPlaces, laderPlaces);
                    topRightRoom.CraeteRoom();
                    break;

                case InnerRoomType.BottomLeft:
                    
                    break;

                case InnerRoomType.BottomRight:
                    
                    break;
            }
        }
    }
}
