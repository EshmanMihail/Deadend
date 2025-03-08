using Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build.Inner_rooms.InnerRoomStructs;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build.Inner_rooms
{
    public class TopRightRoom : InnerRoom
    {
        public TopRightRoom(Room room, System.Random rand, Tile[] roomTiles, List<Vector2> ocupiedPlaces)
            : base(room, rand, roomTiles, ocupiedPlaces)
        {
            startX = (int)room.GetRightBottomAngle().x;
            startY = (int)room.GetLeftUpperAngle().y;
        }

        public override void CraeteRoom()
        {
            DetermineRoomSize();

            sizeCorrector = new RoomSizeCorrector(this, ocupiedPlaces);
            sizeCorrector.CorrectSize(ref innerWalls);

            if (!IsInnerRoomCanExist()) return;

            SetRoomTiles();
            AddInnerOcupatePlaces();

            MakeLadderPath();

            SpawnLamps();

            CollectFloorWalls(startX - 1, startX - innerWalls.countOfWallsLeft + 1, startY - innerWalls.countOfWallsDown);
        }

        private void DetermineRoomSize()
        {
            int roomLength = room.wallsInfo.countOfWallsRight + room.wallsInfo.countOfWallsLeft;
            int roomWidth = room.wallsInfo.countOfWallsDown + room.wallsInfo.countOfWallsUp;

            int countOfWallsLeft = rand.Next(roomLength / 2 + 2, roomLength / 2 + 3);
            int countOfWallsDown = rand.Next(3, roomWidth / 2 + 1);

            innerWalls = new RoomWallsInfo
            {
                countOfWallsRight = 0,
                countOfWallsLeft = countOfWallsLeft,
                countOfWallsUp = 0,
                countOfWallsDown = countOfWallsDown
            };
        }

        private void SetRoomTiles()
        {
            SetTilesByLength();

            SetTilesByWidth();
        }

        private void SetTilesByLength()
        {
            room.tileSetter.SetTile(roomTiles[10], startX, startY - innerWalls.countOfWallsDown, ObjectsLayers.Walls);
            room.tileSetter.RotateTile(startX, startY - innerWalls.countOfWallsDown, 180);

            for (int x = startX - 1; x > startX - innerWalls.countOfWallsLeft; x--)
            {
                if (!IsOnLadderPosition(x, startY - innerWalls.countOfWallsDown))
                {
                    room.tileSetter.SetTile(roomTiles[11], x, startY - innerWalls.countOfWallsDown, ObjectsLayers.Walls);
                    ocupiedPlaces.Add(new Vector2(x, startY - innerWalls.countOfWallsDown));
                }
            }

            SetAngleTile();
        }

        private void SetAngleTile()
        {
            if (!IsOnLadderPosition(startX - innerWalls.countOfWallsLeft, startY - innerWalls.countOfWallsDown))
            {
                room.tileSetter.SetTile(roomTiles[14], startX - innerWalls.countOfWallsLeft, startY - innerWalls.countOfWallsDown, ObjectsLayers.Walls);
                room.tileSetter.RotateTile(startX - innerWalls.countOfWallsLeft, startY - innerWalls.countOfWallsDown, 270);
                ocupiedPlaces.Add(new Vector2(startX - innerWalls.countOfWallsLeft, startY - innerWalls.countOfWallsDown));
            }
        }

        private void SetTilesByWidth()
        {
            if (!IsOnLadderPosition(startX - innerWalls.countOfWallsLeft, startY))
            {
                room.tileSetter.SetTile(roomTiles[10], startX - innerWalls.countOfWallsLeft, startY, ObjectsLayers.Walls);
                room.tileSetter.RotateTile(startX - innerWalls.countOfWallsLeft, startY, 270);
            }

            for (int y = startY - 1; y > startY - innerWalls.countOfWallsDown; y--)
            {
                if (!IsOnLadderPosition(startX - innerWalls.countOfWallsLeft, y))
                {
                    room.tileSetter.SetTile(roomTiles[11], startX - innerWalls.countOfWallsLeft, y, ObjectsLayers.Walls);
                    room.tileSetter.RotateTile(startX - innerWalls.countOfWallsLeft, y, 90);
                    ocupiedPlaces.Add(new Vector2(startX - innerWalls.countOfWallsLeft, y));
                }
            }
        }

        private void AddInnerOcupatePlaces()
        {
            for (int x = startX - 1; x >= startX - innerWalls.countOfWallsLeft; x--)
            {
                for (int y = startY; y >= startY - innerWalls.countOfWallsDown; y--)
                {
                    ocupiedPlaces.Add(new Vector2(x, y));
                }
            }
        }

        private void MakeLadderPath()
        {
            Vector2 leftWall = new Vector2(startX - innerWalls.countOfWallsLeft, startY);
            Vector2 rightWall = new Vector2(startX, startY);
            int innerRoomFloorY = startY - innerWalls.countOfWallsDown;

            LadderPathBuilder.MakePathToRoom(leftWall, rightWall, innerRoomFloorY, room, rand);
        }
    }
}
