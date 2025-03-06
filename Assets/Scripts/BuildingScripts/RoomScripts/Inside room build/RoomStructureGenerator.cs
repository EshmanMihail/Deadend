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

        List<Vector2> placesToSpawnObjects;

        public RoomStructureGenerator(System.Random random)
        {
            rand = random;
            insideRoomWalls = new List<Vector2>();
            placesToSpawnObjects = new List<Vector2>();
        }

        public void Generate(Room room)
        {
            if (room.wallsInfo.countOfWallsDown + room.wallsInfo.countOfWallsUp > 5)
            {
                if (rand.Next(0, 100) < chanceToCreateInnerRooms)
                {
                    InnerRoomsCreator innerRoomsCreator = new InnerRoomsCreator(room, rand);
                    insideRoomWalls = innerRoomsCreator.CreateInnerRooms();
                    placesToSpawnObjects = innerRoomsCreator.GetFloorWalls();
                }

                if (rand.Next(0, 100) < chanceToCreateWallsPlatforms || insideRoomWalls.Count == 0)
                {
                    WallPlatformsCreator wallPlatformsCreator = new WallPlatformsCreator(room, rand, insideRoomWalls);
                    List<Vector2> wallFromPlatfroms = wallPlatformsCreator.CreatePlatfroms();

                    for (int i = 0; i < wallFromPlatfroms.Count; i++)
                    {
                        placesToSpawnObjects.Add(wallFromPlatfroms[i]);
                    }
                }

                Debug.Log(placesToSpawnObjects.Count);

                //for (int i = 0; i < placesToSpawnObjects.Count; i++)
                //{
                //    placesToSpawnObjects.Add(placesToSpawnObjects[i]);
                //    room.tileSetter.SetTile(room.GetTiles()[9], (int)placesToSpawnObjects[i].x, (int)placesToSpawnObjects[i].y, ObjectsLayers.FrontObjects);
                //}
            }
        }

        public void SetChancesForStructures(int chanceToCreateInnerRooms, int chanceToCreateWallsPlatforms)
        {
            this.chanceToCreateInnerRooms = chanceToCreateInnerRooms;
            this.chanceToCreateWallsPlatforms = chanceToCreateWallsPlatforms;
        }
    }
}
