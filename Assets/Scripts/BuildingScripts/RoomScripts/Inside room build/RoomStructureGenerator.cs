using Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build.Platforms;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build
{
    public class RoomStructureGenerator : IRoomStructure
    {
        protected System.Random rand;

        private int chanceToCreateInnerRooms = 100;
        private int chanceToCreateWallsPlatforms = 50;

        List<Vector2> insideRoomWalls;

        public RoomStructureGenerator(System.Random random)
        {
            rand = random;
            insideRoomWalls = new List<Vector2>();
        }

        public void Generate(Room room)
        {
            if (room.wallsInfo.countOfWallsDown + room.wallsInfo.countOfWallsUp > 5)
            {
                if (rand.Next(0, 100) < chanceToCreateInnerRooms)
                {
                    InnerRoomsCreator innerRoomsCreator = new InnerRoomsCreator(room, rand);
                    insideRoomWalls = innerRoomsCreator.CreateInnerRooms();
                }

                if (rand.Next(0, 100) < chanceToCreateInnerRooms || insideRoomWalls.Count == 0)
                {
                    WallPlatformsCreator wallPlatformsCreator = new WallPlatformsCreator(room, rand, insideRoomWalls);
                    wallPlatformsCreator.CreatePlatfroms();
                }
            }
        }

        public void SetChancesForStructures(int chanceToCreateInnerRooms, int chanceToCreateWallsPlatforms)
        {
            this.chanceToCreateInnerRooms = chanceToCreateInnerRooms;
            this.chanceToCreateWallsPlatforms = chanceToCreateWallsPlatforms;
        }
    }
}
