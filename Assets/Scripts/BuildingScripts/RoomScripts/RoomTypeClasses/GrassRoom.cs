using Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build;
using Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_spawn;
using UnityEngine;

namespace Assets.Scripts.BuildingScripts.RoomScripts
{
    public class GrassRoom : Room
    {
        public GrassRoom(Vector2 entryPoint, RoomType roomType, RoomWallsInfo wallsInfo, RoomBiom roomBiom,
            IRoomStructure structureGenerator, IRoomObjectPlacer roomObjectPlacer)
            : base(entryPoint, roomType, wallsInfo, roomBiom, structureGenerator, roomObjectPlacer) { }

        public override void GenerateRoomStructure()
        {
            if (wallsInfo.countOfWallsDown + wallsInfo.countOfWallsUp > 5)
            {
                structureGenerator.SetChancesForStructures(50, 100);
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
