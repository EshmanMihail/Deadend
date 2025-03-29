using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.BuildingScripts.BuildingTypes
{
    public class FullGrassedBuildign : Building
    {
        public override Vector2 GeneratePathToNextRoom(Vector2 entryPoint, RoomType NextRoomType, Room room, System.Random rand)
        {
            return Vector2.zero;
        }

        public override void GenerateRoomsSize(RoomType roomType, System.Random rand, ref int countOfWallsUp, ref int countOfWallsDown, ref int countOfWallsLeft, ref int countOfWallsRight)
        {
            
        }
    }
}
