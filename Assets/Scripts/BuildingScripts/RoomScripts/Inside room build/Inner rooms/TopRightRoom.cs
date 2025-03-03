using Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build.Inner_rooms.InnerRoomStructs;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build.Inner_rooms
{
    public class TopRightRoom : InnerRoom
    {
        private int startX = 0;
        private int startY = 0;

        private int countOfWallsLeft = 1;
        private int countOfWallsDown = 1;

        public TopRightRoom(Room room, System.Random rand, Tile[] roomTiles, List<Vector2> ocupiedPlaces)
            : base(room, rand, roomTiles, ocupiedPlaces)
        {
            startX = (int)room.GetRightBottomAngle().x;
            startY = (int)room.GetLeftUpperAngle().y;
        }

        public override void CraeteRoom()
        {
            int roomLength = room.wallsInfo.countOfWallsRight + room.wallsInfo.countOfWallsLeft;
            int roomWidth = room.wallsInfo.countOfWallsDown + room.wallsInfo.countOfWallsUp;

            countOfWallsLeft = rand.Next(roomLength / 2 + 2, roomLength / 2 + 3);
            countOfWallsDown = rand.Next(3, roomWidth / 2 + 1);

            CorrectRightRoomSize();

            if (!IsInnerRoomCanExist(countOfWallsLeft, 0, countOfWallsDown, 0)) return;

            SetTilesToTopRightRoom();
            AddInnerOcupatePlaces();

            MakeLadderPath();

            SpawnLamps();
        }

        private void CorrectRightRoomSize()
        {
            //correct length
            int minPossibleLengthLeft = countOfWallsLeft;

            for (int y = startY - 1; y >= startY - countOfWallsDown; y--)
            {
                for (int x = startX; x >= startX - countOfWallsLeft; x--)
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
            countOfWallsLeft = minPossibleLengthLeft;

            //correct width
            int minPossibleWidthDown = countOfWallsDown;

            for (int x = startX; x > startX - countOfWallsLeft; x--)
            {
                for (int y = startY - 1; y >= startY - countOfWallsDown; y--)
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
            countOfWallsDown = minPossibleWidthDown;
        }

        private void SetTilesToTopRightRoom()
        {
            SetTilesByLength();

            SetTilesByWidth();
        }

        private void SetTilesByLength()
        {
            room.tileSetter.SetTile(roomTiles[10], startX, startY - countOfWallsDown, ObjectsLayers.Walls);
            room.tileSetter.RotateTile(startX, startY - countOfWallsDown, 180);
            AddTileToMapData(startX, startY - countOfWallsDown, roomTiles[10], ObjectsLayers.Walls);

            for (int x = startX - 1; x > startX - countOfWallsLeft; x--)
            {
                if (!IsOnLadderPosition(x, startY - countOfWallsDown))
                {
                    room.tileSetter.SetTile(roomTiles[11], x, startY - countOfWallsDown, ObjectsLayers.Walls);
                    AddTileToMapData(x, startY - countOfWallsDown, roomTiles[10], ObjectsLayers.Walls);

                    ocupiedPlaces.Add(new Vector2(x, startY - countOfWallsDown));
                }
            }

            SetAngleTile();
        }

        private void SetAngleTile()
        {
            if (!IsOnLadderPosition(startX - countOfWallsLeft, startY - countOfWallsDown))
            {
                room.tileSetter.SetTile(roomTiles[14], startX - countOfWallsLeft, startY - countOfWallsDown, ObjectsLayers.Walls);
                room.tileSetter.RotateTile(startX - countOfWallsLeft, startY - countOfWallsDown, 270);
                AddTileToMapData(startX - countOfWallsLeft, startY - countOfWallsDown, roomTiles[14], ObjectsLayers.Walls);

                ocupiedPlaces.Add(new Vector2(startX - countOfWallsLeft, startY - countOfWallsDown));
            }
        }

        private void SetTilesByWidth()
        {
            room.tileSetter.SetTile(roomTiles[10], startX - countOfWallsLeft, startY, ObjectsLayers.Walls);
            room.tileSetter.RotateTile(startX - countOfWallsLeft, startY, 270);
            AddTileToMapData(startX - countOfWallsLeft, startY, roomTiles[10], ObjectsLayers.Walls);

            for (int y = startY - 1; y > startY - countOfWallsDown; y--)
            {
                if (!IsOnLadderPosition(startX - countOfWallsLeft, y))
                {
                    room.tileSetter.SetTile(roomTiles[11], startX - countOfWallsLeft, y, ObjectsLayers.Walls);
                    room.tileSetter.RotateTile(startX - countOfWallsLeft, y, 90);

                    AddTileToMapData(startX - countOfWallsLeft, y, roomTiles[11], ObjectsLayers.Walls);

                    ocupiedPlaces.Add(new Vector2(startX - countOfWallsLeft, y));
                }
            }
        }

        private void AddInnerOcupatePlaces()
        {
            for (int x = startX - 1; x >= startX - countOfWallsLeft; x--)
            {
                for (int y = startY; y >= startY - countOfWallsDown; y--)
                {
                    ocupiedPlaces.Add(new Vector2(x, y));
                }
            }
        }

        private void MakeLadderPath()
        {
            Vector2 leftWall = new Vector2(startX - countOfWallsLeft, startY);
            Vector2 rightWall = new Vector2(startX, startY);
            int innerRoomFloorY = startY - countOfWallsDown;

            LadderPathBuilder.MakePathToRoom(leftWall, rightWall, innerRoomFloorY, room, rand);
        }

        private void SpawnLamps()
        {
            Vector2 leftWall = new Vector2(startX - countOfWallsLeft, startY);
            Vector2 rightWall = new Vector2(startX, startY);
            int floorY = startY - countOfWallsDown;
            int ceilingY = startY;

            LampsSpawner.SpawnLamps(leftWall, rightWall, floorY, ceilingY, room);
        }
    }
}
