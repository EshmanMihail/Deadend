using System;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build.Inner_rooms.InnerRoomStructs
{
    public class RoomSizeCorrector
    {
        private InnerRoom room;
        private List<Vector2> ocupiedPlaces;

        private int startX = 0;
        private int startY = 0;

        public RoomSizeCorrector(InnerRoom room, List<Vector2> ocupiedPlaces)
        {
            this.room = room;
            this.ocupiedPlaces = ocupiedPlaces;
        }

        public void CorrectSize(ref RoomWallsInfo wallInfo)
        {
            int countWallsRight = wallInfo.countOfWallsRight;
            int countWallsLeft = wallInfo.countOfWallsLeft;
            int countWallsUp = wallInfo.countOfWallsUp;
            int countWallsDown = wallInfo.countOfWallsDown;

            startX = (int)room.GetStartPosition().x;
            startY = (int)room.GetStartPosition().y;

            if (countWallsRight != 0) CorrectRight(ref countWallsRight, countWallsUp, countWallsDown);
            if (countWallsLeft != 0) CorrectLeft(ref countWallsLeft, countWallsUp, countWallsDown);
            if (countWallsDown != 0) CorrectDown(ref countWallsDown, countWallsLeft, countWallsRight);
            if (countWallsUp != 0) CorrectUp(ref countWallsUp, countWallsLeft, countWallsRight);

            RoomWallsInfo correctedRoomSize = new RoomWallsInfo
            {
                countOfWallsRight = countWallsRight,
                countOfWallsLeft = countWallsLeft,
                countOfWallsUp = countWallsUp,
                countOfWallsDown = countWallsDown
                
            };
            wallInfo = correctedRoomSize;
        }

        private void CorrectRight(ref int countWallsRight, int countWallsUp, int countWallsDown)
        {
            int minPossibleLengthRight = countWallsRight;

            for (int y = startY + countWallsUp; y >= startY - countWallsDown; y--)
            {
                for (int x = startX; x <= startX + countWallsRight; x++)
                {
                    Vector2 position = new Vector2(x, y);

                    if (ocupiedPlaces.Contains(position))
                    {
                        if (minPossibleLengthRight > x - startX)
                        {
                            minPossibleLengthRight = x - startX;
                        }
                    }
                }
            }
            countWallsRight = minPossibleLengthRight;
            if (startX + countWallsRight == (int)room.room.entryPoint.x) countWallsRight--;
        }

        private void CorrectLeft(ref int countWallsLeft, int countWallsUp, int countWallsDown)
        {
            int minPossibleLengthLeft = countWallsLeft;

            for (int y = startY + countWallsUp; y >= startY - countWallsDown; y--)
            {
                for (int x = startX; x >= startX - countWallsLeft; x--)
                {
                    Vector2 position = new Vector2(x, y);

                    if (ocupiedPlaces.Contains(position))
                    {
                        if (minPossibleLengthLeft > startX - x)
                        {
                            minPossibleLengthLeft = startX - x;
                        }
                    }
                }
            }
            countWallsLeft = minPossibleLengthLeft;
            if (startX - countWallsLeft == (int)room.room.entryPoint.x) countWallsLeft--;
        }

        private void CorrectDown(ref int countWallsDown, int countWallsLeft, int countWallsRight)
        {
            int minPossibleWidthDown = countWallsDown;

            for (int x = startX - countWallsLeft; x <= startX + countWallsRight; x++)
            {
                for (int y = startY - 1; y >= startY - countWallsDown; y--)
                {
                    Vector2 position = new Vector2(x, y);

                    if (ocupiedPlaces.Contains(position))
                    {
                        if (minPossibleWidthDown < startY - y)
                        {
                            minPossibleWidthDown = startY - y;
                        }
                    }
                }
            }
            countWallsDown = minPossibleWidthDown;
        }

        private void CorrectUp(ref int countWallsUp, int countWallsLeft, int countWallsRight)
        {
            int minPossibleWidthUp = countWallsUp;

            for (int x = startX - countWallsLeft; x <= startX + countWallsRight; x++)
            {
                for (int y = startY + 1; y <= startY + countWallsUp; y++)
                {
                    Vector2 position = new Vector2(x, y);

                    if (ocupiedPlaces.Contains(position))
                    {
                        if (minPossibleWidthUp < y - startY)
                        {
                            minPossibleWidthUp = y - startY;
                        }
                    }
                }
            }
            countWallsUp = minPossibleWidthUp;
        }
    }
}
