﻿using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.BuildingScripts.RoomScripts
{
    public class MetalRoom : Room
    {
        public MetalRoom(Vector2 entryPoint, RoomType roomType,
            int countOfWallsUp, int countOfWallsDown, int countOfWallsLeft, int countOfWallsRight, RoomBiom roomBiom)
            : base(entryPoint, roomType, countOfWallsUp, countOfWallsDown, countOfWallsLeft, countOfWallsRight, roomBiom) { }

        public override void GenerateRoomStructure()
        {
            throw new NotImplementedException();
        }
    }
}
