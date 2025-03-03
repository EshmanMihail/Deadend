using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.BuildingScripts.RoomScripts
{
    public class GrassRoom : Room
    {
        public GrassRoom(Vector2 entryPoint, RoomType roomType, RoomWallsInfo wallsInfo, RoomBiom roomBiom)
            : base(entryPoint, roomType, wallsInfo, roomBiom) { }

        public override void GenerateRoomStructure()
        {
            structureGenerator.SetChancesForStructures(10, 100);
            structureGenerator.Generate(this);
        }

        public override void SpawnRoomObjects()
        {

        }
    }
}
