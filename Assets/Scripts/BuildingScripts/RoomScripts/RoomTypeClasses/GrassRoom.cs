using Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.BuildingScripts.RoomScripts
{
    public class GrassRoom : Room
    {
        public GrassRoom(Vector2 entryPoint, RoomType roomType, RoomWallsInfo wallsInfo, RoomBiom roomBiom, IRoomStructure structureGenerator)
            : base(entryPoint, roomType, wallsInfo, roomBiom, structureGenerator) { }

        public override void GenerateRoomStructure()
        {
            if (wallsInfo.countOfWallsDown + wallsInfo.countOfWallsUp > 5)
            {
                structureGenerator.SetChancesForStructures(10, 100);
                structureGenerator.Generate(this);
                positionsToSpawnObjects = structureGenerator.GetPlacesToSetObjects();
            }
        }

        public override void SpawnRoomObjects()
        {

        }
    }
}
