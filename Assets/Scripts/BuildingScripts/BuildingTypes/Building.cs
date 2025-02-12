using System;
using UnityEngine;

namespace Assets.Scripts.BuildingScripts.BuildingTypes
{
    public abstract class Building
    {
        public BuildingTypeStructure Structure { get; set; }

        public abstract Vector2 GeneratePathToNextRoom(Vector2 entryPoint, RoomType NextRoomType, Room room, System.Random rand);

        public abstract void GenerateRoomsSize(RoomType roomType, System.Random rand,
             ref int countOfWallsUp, ref int countOfWallsDown,
             ref int countOfWallsLeft, ref int countOfWallsRight);
    }
}
