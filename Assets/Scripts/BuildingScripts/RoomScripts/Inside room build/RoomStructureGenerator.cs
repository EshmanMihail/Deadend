using Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build.Platforms;
using Assets.Scripts.NodeScripts;
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

        List<PositionProperty> placesToSpawnObjects;

        public RoomStructureGenerator(System.Random random)
        {
            rand = random;
            insideRoomWalls = new List<Vector2>();
            placesToSpawnObjects = new List<PositionProperty>();
        }

        public void Generate(Room room)
        {
            if (rand.Next(0, 100) < chanceToCreateInnerRooms)
            {
                CreateInnerRooms(room);
            }

            if (rand.Next(0, 100) < chanceToCreateWallsPlatforms || insideRoomWalls.Count == 0)
            {
                CreateWallPlatforms(room);
            }

            SpawnNodesInBottomPartOfRoom(room);
        }

        public void SetChancesForStructures(int chanceToCreateInnerRooms, int chanceToCreateWallsPlatforms)
        {
            this.chanceToCreateInnerRooms = chanceToCreateInnerRooms;
            this.chanceToCreateWallsPlatforms = chanceToCreateWallsPlatforms;
        }

        private void CreateInnerRooms(Room room)
        {
            InnerRoomsCreator innerRoomsCreator = new InnerRoomsCreator(room, rand);
            insideRoomWalls = innerRoomsCreator.CreateInnerRooms();

            List<PositionProperty> innerRoomPositionsToSpawn = innerRoomsCreator.GetFloorWalls();
            AddPositionForSpawnObjects(innerRoomPositionsToSpawn);
        }

        private void CreateWallPlatforms(Room room)
        {
            WallPlatformsCreator wallPlatformsCreator = new WallPlatformsCreator(room, rand, insideRoomWalls);

            List<PositionProperty> wallFromPlatfroms = wallPlatformsCreator.CreatePlatfroms();
            AddPositionForSpawnObjects(wallFromPlatfroms);
        }

        private void SpawnNodesInBottomPartOfRoom(Room room)
        {
            int leftX = (int)room.entryPoint.x - room.wallsInfo.countOfWallsLeft;
            int rightX = (int)room.entryPoint.x + room.wallsInfo.countOfWallsRight;
            int floorY = (int)room.entryPoint.y - room.wallsInfo.countOfWallsDown;

            NodePositionSpawner.SpawnNodes(leftX + 1, rightX - 1, floorY + 1);
        }

        private void AddPositionForSpawnObjects(List<PositionProperty> newPositions)
        {
            for (int i = 0; i < newPositions.Count; i++)
            {
                placesToSpawnObjects.Add(newPositions[i]);
            }
        }

        public List<PositionProperty> GetPlacesToSetObjects()
        {
            return placesToSpawnObjects;
        }

        public List<Vector2> GetOccuiedPlaces()
        {
            return insideRoomWalls;
        }
    }
}
