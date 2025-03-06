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
        private List<Vector2> floorWalls;

        private int chanceToSpawnInnerRoom = 80;
        private int reduceChance = 20;

        public InnerRoomsCreator(Room room, System.Random random)
        {
            this.room = room;
            this.rand = random;

            roomTiles = room.GetTiles();

            ocupiedPlaces = new List<Vector2>();
            floorWalls = new List<Vector2>();
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

                    List<Vector2> floorWallFromTopLeftRoom = topLeftRoom.GetFLoorWalls();
                    SetFloorWalls(floorWallFromTopLeftRoom);
                    break;

                case InnerRoomType.TopRight:
                    InnerRoom topRightRoom = new TopRightRoom(room, rand, roomTiles, ocupiedPlaces);
                    topRightRoom.CraeteRoom();

                    List<Vector2> floorWallFromTopRightRoom = topRightRoom.GetFLoorWalls();
                    SetFloorWalls(floorWallFromTopRightRoom);
                    break;

                case InnerRoomType.BottomLeft:
                    InnerRoom bottomLeftRoom = new BottomLeftRoom(room, rand, roomTiles, ocupiedPlaces);
                    bottomLeftRoom.CraeteRoom();

                    List<Vector2> floorWallFromBottomLeftRoom = bottomLeftRoom.GetFLoorWalls();
                    SetFloorWalls(floorWallFromBottomLeftRoom);
                    break;

                case InnerRoomType.BottomRight:
                    InnerRoom bottomRightRoom = new BottomRightRoom(room, rand, roomTiles, ocupiedPlaces);
                    bottomRightRoom.CraeteRoom();

                    List<Vector2> floorWallFromBottomRightRoom = bottomRightRoom.GetFLoorWalls();
                    SetFloorWalls(floorWallFromBottomRightRoom);
                    break;
            }
        }

        private void SetFloorWalls(List<Vector2> floorWallsFromRoom)
        {
            for (int i = 0; i < floorWallsFromRoom.Count; i++)
            {
                floorWalls.Add(floorWallsFromRoom[i]);
            }
        }

        public List<Vector2> GetFloorWalls() { return floorWalls; }
    }
}
