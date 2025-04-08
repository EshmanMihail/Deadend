using Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build;
using Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_spawn;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.BuildingScripts.RoomScripts
{
    public class FrozenRoom : Room
    {
        public FrozenRoom(Vector2 entryPoint, RoomType roomType, RoomWallsInfo wallsInfo, RoomBiom roomBiom,
            IRoomStructure structureGenerator, RoomObjectPlacer roomObjectPlacer)
            : base(entryPoint, roomType, wallsInfo, roomBiom, structureGenerator, roomObjectPlacer) { }

        public override void GenerateRoomStructure()
        {
            if (wallsInfo.countOfWallsDown + wallsInfo.countOfWallsUp > 5)
            {
                structureGenerator.SetChancesForStructures(70, 70);
                structureGenerator.Generate(this);
                positionsToSpawnObjects = structureGenerator.GetPlacesToSetObjects();
            }
        }

        public override void SpawnRoomObjects()
        {
            roomObjectPlacer.SetRoomObjects(this);
        }
    }
}