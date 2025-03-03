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

        private int chanceToSpawnInnerRoom = 80;
        private int reduceChance = 20;

        public InnerRoomsCreator(Room room, System.Random random)
        {
            this.room = room;
            this.rand = random;

            roomTiles = room.GetTiles();

            ocupiedPlaces = new List<Vector2>();
        }

        public List<Vector2> CreateInnerRooms()
        {
            List<int> innerRoomTypesValues = new List<int>() { 0, 1, 2, 3 };

            while (chanceToSpawnInnerRoom > 0 && innerRoomTypesValues.Count > 0)
            {
                int randomIndex = rand.Next(0, innerRoomTypesValues.Count);
                InnerRoomType innerRoomType = (InnerRoomType)innerRoomTypesValues[randomIndex];

                if (rand.Next(0, 100) < chanceToSpawnInnerRoom)
                    InnerRoomManager(innerRoomType);

                innerRoomTypesValues.RemoveAt(randomIndex);
                chanceToSpawnInnerRoom -= reduceChance;
            }

            return ocupiedPlaces;
        }

        private void InnerRoomManager(InnerRoomType innerRoomType)
        {
            switch (innerRoomType)
            {
                case InnerRoomType.TopLeft:
                    InnerRoom topLeftRoom = new TopLeftRoom(room, rand, roomTiles, ocupiedPlaces);
                    topLeftRoom.CraeteRoom();
                    break;

                case InnerRoomType.TopRight:
                    InnerRoom topRightRoom = new TopRightRoom(room, rand, roomTiles, ocupiedPlaces);
                    topRightRoom.CraeteRoom();
                    break;

                case InnerRoomType.BottomLeft:
                    InnerRoom bottomLeftRoom = new BottomLeftRoom(room, rand, roomTiles, ocupiedPlaces);
                    bottomLeftRoom.CraeteRoom();
                    break;

                case InnerRoomType.BottomRight:
                    InnerRoom bottomRightRoom = new BottomRightRoom(room, rand, roomTiles, ocupiedPlaces);
                    bottomRightRoom.CraeteRoom();
                    break;
            }
        }
    }
}
