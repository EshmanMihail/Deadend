using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build.Inner_rooms
{
    public class TopLeftRoom : InnerRoom
    {
        public TopLeftRoom(Room room, System.Random rand, Tile[] roomTiles, List<Vector2> ocupiedPlaces, List<Vector2> laderPlaces)
            : base(room, rand, roomTiles, ocupiedPlaces, laderPlaces)
        { }

        public override void CraeteRoom()
        {
            int roomLength = room.wallsInfo.countOfWallsRight + room.wallsInfo.countOfWallsLeft;
            int roomWidth = room.wallsInfo.countOfWallsDown + room.wallsInfo.countOfWallsUp;

            int countOfWallsRight = rand.Next(3, roomLength / 2 + 3);
            int countOfWallsDown = rand.Next(3, roomWidth / 2 + 3);

            CorrectLeftRoomSize(ref countOfWallsRight, ref countOfWallsDown);

            if (!IsInnerRoomCanExist(0, countOfWallsRight, countOfWallsDown, 0)) return;

            SetTilesToTopLeftRoom(countOfWallsRight, countOfWallsDown);
        }

        private void CorrectLeftRoomSize(ref int countOfWallsRight, ref int countOfWallsDown)
        {
            //correct length
            int startX = (int)room.GetLeftUpperAngle().x;
            int startY = (int)room.GetLeftUpperAngle().y;

            int minPossibleLengthRight = countOfWallsRight;

            for (int y = startY - 1; y >= startY - countOfWallsDown; y--)
            {
                for (int x = startX; x <= startX + countOfWallsRight; x++)
                {
                    Vector2 position = new Vector2(x, y);

                    if (ocupiedPlaces.Contains(position))
                    {
                        if (minPossibleLengthRight > x)
                        {
                            minPossibleLengthRight = x;
                        }
                    }
                }
            }
            countOfWallsRight = minPossibleLengthRight;

            //correct width
            int minPossibleWidthDown = countOfWallsDown;

            for (int x = startX; x <= startX + countOfWallsRight; x++)
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

        private void SetTilesToTopLeftRoom(int countOfWallsRight, int countOfWallsDown)
        {
            int startX = (int)room.GetLeftUpperAngle().x;
            int startY = (int)room.GetLeftUpperAngle().y;

            //set by length
            room.tileSetter.SetTile(roomTiles[10], startX, startY - countOfWallsDown, ObjectsLayers.Walls);
            for (int x = startX + 1; x < startX + countOfWallsRight; x++)
            {
                room.tileSetter.SetTile(roomTiles[11], x, startY - countOfWallsDown, ObjectsLayers.Walls);
            }
            room.tileSetter.SetTile(roomTiles[14], startX + countOfWallsRight, startY - countOfWallsDown, ObjectsLayers.Walls);

            //set by width
            room.tileSetter.SetTile(roomTiles[10], startX + countOfWallsRight, startY, ObjectsLayers.Walls);
            room.tileSetter.RotateTile(startX + countOfWallsRight, startY, 270);
            for (int y = startY - 1; y > startY - countOfWallsDown; y--)
            {
                room.tileSetter.SetTile(roomTiles[11], startX + countOfWallsRight, y, ObjectsLayers.Walls);
                room.tileSetter.RotateTile(startX + countOfWallsRight, y, 90);
            }
        }

    }
}
