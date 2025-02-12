using System;
using UnityEngine;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace Assets.Scripts.BuildingScripts.BuildingTypes
{
    public class NormalBuilding : Building
    {
        public override void GenerateRoomsSize(RoomType roomType, System.Random rand,
            ref int countOfWallsUp, ref int countOfWallsDown,
            ref int countOfWallsLeft, ref int countOfWallsRight)
        {
            countOfWallsUp = rand.Next(RoomSizes.MinCountWallsUpLength, RoomSizes.MaxCountWallsUpLength);
            if (roomType == RoomType.Bottom) countOfWallsUp = 0;

            countOfWallsDown = rand.Next(RoomSizes.MinCountWallsDownLength, RoomSizes.MaxCountWallsDownLength);
            if (roomType == RoomType.Upper) countOfWallsDown = 0;

            countOfWallsLeft = rand.Next(RoomSizes.MinCountWallsLeftLength, RoomSizes.MaxCountWallsLeftLength);
            if (roomType == RoomType.Right) countOfWallsLeft = 0;

            countOfWallsRight = rand.Next(RoomSizes.MinCountWallsRightLength, RoomSizes.MaxCountWallsRightLength);
            if (roomType == RoomType.Left) countOfWallsRight = 0;

            if (roomType == RoomType.Bottom) countOfWallsDown = rand.Next(RoomSizes.MinCountWallsDownInBottomRoom, RoomSizes.MaxCountWallsDownInBottomRoom);

            if (roomType == RoomType.Upper) countOfWallsUp = rand.Next(RoomSizes.MinCountWallsUpInUpperRoom, RoomSizes.MaxCountWallsUpInUpperRoom);

            if (roomType == RoomType.Left || roomType == RoomType.Right) countOfWallsDown = 1;
        }

        public override Vector2 GeneratePathToNextRoom(Vector2 entryPoint, RoomType NextRoomType, Room room, System.Random rand)
        {
            int pathPointX = 0, pathPointY = 0;
            if (NextRoomType == RoomType.Upper)
            {
                pathPointX = rand.Next((int)entryPoint.x - room.countOfWallsLeft + 1, (int)entryPoint.x + room.countOfWallsRight - 1);
                pathPointY = (int)entryPoint.y + room.countOfWallsUp;
            }
            if (NextRoomType == RoomType.Bottom)
            {
                pathPointX = rand.Next((int)entryPoint.x - room.countOfWallsLeft + 1, (int)entryPoint.x + room.countOfWallsRight - 1);
                pathPointY = (int)entryPoint.y - room.countOfWallsDown;
            }
            if (NextRoomType == RoomType.Right)
            {
                pathPointX = (int)entryPoint.x + room.countOfWallsRight;
                pathPointY = (int)entryPoint.y - room.countOfWallsDown + 1;
            }
            if (NextRoomType == RoomType.Left)
            {
                pathPointX = (int)entryPoint.x - room.countOfWallsLeft;
                pathPointY = (int)entryPoint.y - room.countOfWallsDown + 1;
            }

            return new Vector2 (pathPointX, pathPointY);
        }
    }
}
