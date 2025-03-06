using Assets.Scripts.BuildingScripts.BuildingTypes;
using Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build.Inner_rooms.InnerRoomStructs;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build.Inner_rooms
{
    public class TopLeftRoom : InnerRoom
    {
        private int startX = 0;
        private int startY = 0;

        private int countOfWallsRight = 1;
        private int countOfWallsDown = 1;

        public TopLeftRoom(Room room, System.Random rand, Tile[] roomTiles, List<Vector2> ocupiedPlaces)
            : base(room, rand, roomTiles, ocupiedPlaces)
        {
            startX = (int)room.GetLeftUpperAngle().x;
            startY = (int)room.GetLeftUpperAngle().y;
        }

        public override void CraeteRoom()
        {
            int roomLength = room.wallsInfo.countOfWallsRight + room.wallsInfo.countOfWallsLeft;
            int roomWidth = room.wallsInfo.countOfWallsDown + room.wallsInfo.countOfWallsUp;

            countOfWallsRight = rand.Next(roomLength / 2 - 1, roomLength / 2 + 3);
            countOfWallsDown = rand.Next(3, roomWidth / 2 + 1);

            CorrectLeftRoomSize();

            if (!IsInnerRoomCanExist(0, countOfWallsRight, countOfWallsDown, 0)) return;

            SetTilesToTopLeftRoom();
            AddInnerOcupatePlaces();

            MakeLadderPath();

            SpawnLamps();

            int centerX = (startX + countOfWallsRight + startX) / 2;
            int NodePositionY = startY - countOfWallsDown + 1;
            NodeSpawner.SpawnNode(centerX, NodePositionY);

            //FreePositionsCollector.CollectFreePositions(startX, startX + countOfWallsRight, startY - countOfWallsDown + 1);
            CollectFloorWalls(startX + 1, startX + countOfWallsRight - 1, startY -  countOfWallsDown);
        }

        private void CorrectLeftRoomSize()
        {
            //correct length
            int minPossibleLengthRight = countOfWallsRight;

            for (int y = startY - 1; y >= startY - countOfWallsDown; y--)
            {
                for (int x = startX; x <= startX + countOfWallsRight; x++)
                {
                    Vector2 position = new Vector2(x, y);

                    if (ocupiedPlaces.Contains(position))
                    {
                        if (minPossibleLengthRight > x - startX)
                        {
                            minPossibleLengthRight = x - startX;
                            break;
                        }
                    }
                }
            }
            countOfWallsRight = minPossibleLengthRight;

            //correct width
            int minPossibleWidthDown = countOfWallsDown;

            for (int x = startX; x < startX + countOfWallsRight; x++)
            {
                for (int y = startY - 1; y >= startY - countOfWallsDown; y--)
                {
                    Vector2 position = new Vector2(x, y);

                    if (ocupiedPlaces.Contains(position))
                    {
                        if (minPossibleWidthDown < startY - y)
                        {
                            minPossibleWidthDown = startY - y;
                            break;
                        }
                    }
                }
            }
            countOfWallsDown = minPossibleWidthDown;
        }

        private void SetTilesToTopLeftRoom()
        {
            SetTilesByLength();

            SetTilesByWidth();
        }

        private void SetTilesByLength()
        {
            if (!IsOnLadderPosition(startX, startY - countOfWallsDown)) 
            { 
                room.tileSetter.SetTile(roomTiles[10], startX, startY - countOfWallsDown, ObjectsLayers.Walls);
                AddTileToMapData(startX, startY - countOfWallsDown, roomTiles[10], ObjectsLayers.Walls);
            }

            for (int x = startX + 1; x < startX + countOfWallsRight; x++)
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
            if (!IsOnLadderPosition(startX + countOfWallsRight, startY - countOfWallsDown))
            {
                room.tileSetter.SetTile(roomTiles[14], startX + countOfWallsRight, startY - countOfWallsDown, ObjectsLayers.Walls);
                AddTileToMapData(startX + countOfWallsRight, startY - countOfWallsDown, roomTiles[14], ObjectsLayers.Walls);

                ocupiedPlaces.Add(new Vector2(startX + countOfWallsRight, startY - countOfWallsDown));
            }
        }

        private void SetTilesByWidth()
        {
            if (!IsOnLadderPosition(startX + countOfWallsRight, startY))
            {
                room.tileSetter.SetTile(roomTiles[10], startX + countOfWallsRight, startY, ObjectsLayers.Walls);
                room.tileSetter.RotateTile(startX + countOfWallsRight, startY, 270);

                AddTileToMapData(startX + countOfWallsRight, startY, roomTiles[10], ObjectsLayers.Walls);
            }

            for (int y = startY - 1; y > startY - countOfWallsDown; y--)
            {
                if (!IsOnLadderPosition(startX + countOfWallsRight, y))
                {
                    room.tileSetter.SetTile(roomTiles[11], startX + countOfWallsRight, y, ObjectsLayers.Walls);
                    room.tileSetter.RotateTile(startX + countOfWallsRight, y, 90);

                    AddTileToMapData(startX + countOfWallsRight, y, roomTiles[11], ObjectsLayers.Walls);

                    ocupiedPlaces.Add(new Vector2(startX + countOfWallsRight, y));
                }  
            }
        }

        private void AddInnerOcupatePlaces()
        {
            for (int x = startX + 1;  x <= startX + countOfWallsRight; x++)
            {
                for (int y = startY; y >= startY - countOfWallsDown; y--)
                {
                    ocupiedPlaces.Add(new Vector2(x, y));
                }
            }
        }

        private void MakeLadderPath()
        {
            Vector2 leftWall = new Vector2(startX, startY);
            Vector2 rightWall = new Vector2(startX + countOfWallsRight, startY);
            int innerRoomFloorY = startY - countOfWallsDown;

            LadderPathBuilder.MakePathToRoom(leftWall, rightWall, innerRoomFloorY, room, rand);
        }

        private void SpawnLamps()
        {
            Vector2 leftWall = new Vector2(startX, startY);
            Vector2 rightWall = new Vector2(startX + countOfWallsRight, startY);
            int floorY = startY - countOfWallsDown;
            int ceilingY = startY;

            LampsSpawner.SpawnLamps(leftWall, rightWall, floorY, ceilingY, room);
        }
    }
}
