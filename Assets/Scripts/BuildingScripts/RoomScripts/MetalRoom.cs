using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.BuildingScripts.RoomScripts
{
    public class MetalRoom : Room
    {
        public MetalRoom(Vector2 entryPoint, RoomType roomType, RoomWallsInfo wallsInfo, RoomBiom roomBiom)
            : base(entryPoint, roomType, wallsInfo, roomBiom) { }

        public override void GenerateRoomStructure()
        {
            structureGenerator.Generate(this);
        }

        public override void SpawnRoomObjects()
        {
            
        }
    }
}
