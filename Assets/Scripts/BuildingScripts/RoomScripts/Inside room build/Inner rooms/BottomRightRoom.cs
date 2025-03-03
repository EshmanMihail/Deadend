using Assets.Scripts.BuildingScripts.BuildingTypes;
using Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build.Inner_rooms.InnerRoomStructs;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build.Inner_rooms
{
    public class BottomRightRoom : InnerRoom
    {
        private int startX = 0;
        private int startY = 0;

        private int countOfWallsLeft = 1;
        private int countOfWallsUp = 1;

        private bool isHaveLadderEntrance = false;
        private int isCollisionWithRoom = 0;

        public BottomRightRoom(Room room, System.Random rand, Tile[] roomTiles, List<Vector2> ocupiedPlaces)
            : base(room, rand, roomTiles, ocupiedPlaces)
        {
            startX = (int)room.GetRightBottomAngle().x;
            startY = (int)room.GetRightBottomAngle().y;
        }

        public override void CraeteRoom()
        {
            int roomLength = room.wallsInfo.countOfWallsRight + room.wallsInfo.countOfWallsLeft;
            int roomWidth = room.wallsInfo.countOfWallsDown + room.wallsInfo.countOfWallsUp;

            countOfWallsLeft = rand.Next(roomLength / 2 - 1, roomLength / 2 + 3);
            countOfWallsUp = rand.Next(3, roomWidth / 2 + 1);

            CorrectRightRoomSize();

            if (!IsInnerRoomCanExist(countOfWallsLeft, 0, 0, countOfWallsUp)) return;

            SetRoomTiles();
            AddInnerOcupatePlaces();

            MakeEntranceToRoom();

            if (rand.Next(0, 101) > 30) SpawnLamps();

            FreePositionsCollector.CollectFreePositions(startX - countOfWallsLeft, startX, startY + 1);
        }

        private void CorrectRightRoomSize()
        {
            //correct length
            int minPossibleLengthLeft = countOfWallsLeft;

            for (int y = startY + 1; y <= startY + countOfWallsUp; y++)
            {
                for (int x = startX; x >= startX - countOfWallsLeft; x--)
                {
                    Vector2 position = new Vector2(x, y);

                    if (ocupiedPlaces.Contains(position))
                    {
                        if (minPossibleLengthLeft > startX - x)
                        {
                            minPossibleLengthLeft = startX - x;
                            isCollisionWithRoom++;
                        }
                    }
                }
            }
            countOfWallsLeft = minPossibleLengthLeft;

            //correct width
            int minPossibleWidthUp = countOfWallsUp;

            for (int x = startX; x > startX - countOfWallsLeft; x--)
            {
                for (int y = startY + 1; y <= startY + countOfWallsUp; y++)
                {
                    Vector2 position = new Vector2(x, y);

                    if (ocupiedPlaces.Contains(position))
                    {
                        if (minPossibleWidthUp < startY + y)
                        {
                            minPossibleWidthUp = startY + y;
                        }
                    }
                }
            }
            countOfWallsUp = minPossibleWidthUp;
        }


        private void SetRoomTiles()
        {
            SetTilesByLength();

            SetTilesByWidth();
        }

        private void SetTilesByLength()
        {
            room.tileSetter.SetTile(roomTiles[10], startX, startY + countOfWallsUp, ObjectsLayers.Walls);
            room.tileSetter.RotateTile(startX, startY + countOfWallsUp, 180);
            AddTileToMapData(startX, startY + countOfWallsUp, roomTiles[10], ObjectsLayers.Walls);

            for (int x = startX - 1; x > startX - countOfWallsLeft; x--)
            {
                if (!IsOnLadderPosition(x, startY + countOfWallsUp))
                {
                    room.tileSetter.SetTile(roomTiles[11], x, startY + countOfWallsUp, ObjectsLayers.Walls);
                    AddTileToMapData(x, startY + countOfWallsUp, roomTiles[10], ObjectsLayers.Walls);

                    ocupiedPlaces.Add(new Vector2(x, startY + countOfWallsUp));
                }
                else isHaveLadderEntrance = true;
            }

            SetAngleTile();
        }

        private void SetAngleTile()
        {
            if (!IsOnLadderPosition(startX - countOfWallsLeft, startY + countOfWallsUp))
            {
                room.tileSetter.SetTile(roomTiles[14], startX - countOfWallsLeft, startY + countOfWallsUp, ObjectsLayers.Walls);
                room.tileSetter.RotateTile(startX - countOfWallsLeft, startY + countOfWallsUp, 180);
                AddTileToMapData(startX - countOfWallsLeft, startY + countOfWallsUp, roomTiles[14], ObjectsLayers.Walls);

                ocupiedPlaces.Add(new Vector2(startX - countOfWallsLeft, startY + countOfWallsUp));
            }
            else isHaveLadderEntrance = true;
        }

        private void SetTilesByWidth()
        {
            room.tileSetter.SetTile(roomTiles[10], startX - countOfWallsLeft, startY, ObjectsLayers.Walls);
            room.tileSetter.RotateTile(startX - countOfWallsLeft, startY, 90);
            AddTileToMapData(startX - countOfWallsLeft, startY, roomTiles[10], ObjectsLayers.Walls);

            for (int y = startY + 1; y < startY + countOfWallsUp; y++)
            {
                if (!IsOnLadderPosition(startX - countOfWallsLeft, y))
                {
                    room.tileSetter.SetTile(roomTiles[11], startX - countOfWallsLeft, y, ObjectsLayers.Walls);
                    room.tileSetter.RotateTile(startX - countOfWallsLeft, y, 90);

                    AddTileToMapData(startX - countOfWallsLeft, y, roomTiles[11], ObjectsLayers.Walls);

                    ocupiedPlaces.Add(new Vector2(startX - countOfWallsLeft, y));
                }
            }

            Vector2 ladderPositionFromEntry = new Vector2(startX - countOfWallsLeft, startY);
            if (BuildingData.ladder.Contains(ladderPositionFromEntry))
            {
                room.tileSetter.RemoveWall(new Vector3Int(startX - countOfWallsLeft, startY + 1));
            }
        }


        private void AddInnerOcupatePlaces()
        {
            for (int x = startX - 1; x >= startX - countOfWallsLeft; x--)
            {
                for (int y = startY; y <= startY + countOfWallsUp; y++)
                {
                    ocupiedPlaces.Add(new Vector2(x, y));
                }
            }
        }

        private void SpawnLamps()
        {
            Vector2 leftWall = new Vector2(startX, startY);
            Vector2 rightWall = new Vector2(startX - countOfWallsLeft, startY);
            int floorY = startY;
            int ceilingY = startY + countOfWallsUp;

            LampsSpawner.SpawnLamps(leftWall, rightWall, floorY, ceilingY, room);
        }

        private void MakeEntranceToRoom()
        {
            int doorX = startX - countOfWallsLeft;
            int doorY = startY + 1;

            room.tileSetter.RemoveWall(new Vector3Int(doorX, doorY, 10));
            if (rand.Next(0, 101) > 50) BuildingData.door.Add(new Vector2(doorX, doorY));

            if (!isHaveLadderEntrance && rand.Next(0, 101) > 30)
            {
                Vector2 leftWall = new Vector2(startX - countOfWallsLeft, startY);
                Vector2 rightWall = new Vector2(startX, startY);
                int innerRoomCeiling = startY + countOfWallsUp;

                LadderPathBuilder.MakePathToRoom(leftWall, rightWall, innerRoomCeiling, room, rand);
            }
        }
    }
}
