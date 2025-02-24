using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build.Inner_rooms
{
    public class TopRightRoom : InnerRoom
    {
        public TopRightRoom(Room room, System.Random rand, Tile[] roomTiles, List<Vector2> ocupiedPlaces, List<Vector2> laderPlaces)
            : base(room, rand, roomTiles, ocupiedPlaces, laderPlaces)
        { }

        public override void CraeteRoom()
        {
            int roomLength = room.wallsInfo.countOfWallsRight + room.wallsInfo.countOfWallsLeft;
            int roomWidth = room.wallsInfo.countOfWallsDown + room.wallsInfo.countOfWallsUp;

            int countOfWallsLeft = rand.Next(3, roomLength / 2 + 3);
            int countOfWallsDown = rand.Next(3, roomWidth / 2 + 3);


            CorrectRightRoomSize(ref countOfWallsLeft, ref countOfWallsDown);

            if (!IsInnerRoomCanExist(countOfWallsLeft, 0, countOfWallsDown, 0)) return;
        }

        private void CorrectRightRoomSize(ref int countOfWallsLeft, ref int countOfWallsDown)
        {
            //correct length
            int startX = (int)room.GetRightBottomAngle().x;
            int startY = (int)room.GetLeftUpperAngle().y;

            int minPossibleLengthLeft = countOfWallsLeft;

            for (int y = startY - 1; y >= startY - countOfWallsDown; y--)
            {
                for (int x = startX; x >= startX - countOfWallsLeft; x--)
                {
                    Vector2 position = new Vector2(x, y);

                    if (ocupiedPlaces.Contains(position))
                    {
                        if (minPossibleLengthLeft > x)
                        {
                            minPossibleLengthLeft = x;
                        }
                    }
                }
            }
            countOfWallsLeft = minPossibleLengthLeft;

            //correct width
            int minPossibleWidthDown = countOfWallsDown;

            for (int x = startX; x >= startX - countOfWallsLeft; x--)
            {
                for (int y = startY - 1; y >= startY - countOfWallsDown; y--)
                {
                    Vector2 position = new Vector2(x, y);

                    if (ocupiedPlaces.Contains(position))
                    {
                        if (minPossibleWidthDown > y)
                        {
                            minPossibleWidthDown = y;
                        }
                    }
                }
            }
            countOfWallsDown = minPossibleWidthDown;
        }

        private void SetTilesToTopRightRoom(int countOfWallsLeft, int countOfWallsDown)
        {

        }
    }
}
