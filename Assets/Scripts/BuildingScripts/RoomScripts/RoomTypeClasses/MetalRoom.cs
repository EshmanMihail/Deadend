using Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build;
using Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_spawn;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.BuildingScripts.RoomScripts
{
    public class MetalRoom : Room
    {
        public MetalRoom(Vector2 entryPoint, RoomType roomType, RoomWallsInfo wallsInfo, RoomBiom roomBiom,
            IRoomStructure structureGenerator, IRoomObjectPlacer roomObjectPlacer)
            : base(entryPoint, roomType, wallsInfo, roomBiom, structureGenerator, roomObjectPlacer) { }

        public override void GenerateRoomStructure()
        {
            if (wallsInfo.countOfWallsDown + wallsInfo.countOfWallsUp > 5)
            {
                structureGenerator.SetChancesForStructures(100, 100);
                structureGenerator.Generate(this);
                positionsToSpawnObjects = structureGenerator.GetPlacesToSetObjects();

                //for (int i = 0; i < list.Count; i++)
                //{
                //    tileSetter.SetTile(tiles[9], (int)list[i].x, (int)list[i].y, ObjectsLayers.FrontObjects);
                //}
            }
        }

        public override void SpawnRoomObjects()
        {
            roomObjectPlacer.SetRoomObjects(this);
        }
    }
}
