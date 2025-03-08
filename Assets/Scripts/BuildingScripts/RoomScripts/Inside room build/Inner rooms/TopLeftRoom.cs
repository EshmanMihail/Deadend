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
        public TopLeftRoom(Room room, System.Random rand, Tile[] roomTiles, List<Vector2> ocupiedPlaces)
            : base(room, rand, roomTiles, ocupiedPlaces)
        {
            startX = (int)room.GetLeftUpperAngle().x;
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

            CollectFloorWalls(startX + 1, startX + innerWalls.countOfWallsRight - 1, startY -  innerWalls.countOfWallsDown);
        }

        private void DetermineRoomSize()
        {
            int roomLength = room.wallsInfo.countOfWallsRight + room.wallsInfo.countOfWallsLeft;
            int roomWidth = room.wallsInfo.countOfWallsDown + room.wallsInfo.countOfWallsUp;

            int countOfWallsRight = rand.Next(roomLength / 2 - 1, roomLength / 2 + 3);
            int countOfWallsDown = rand.Next(3, roomWidth / 2 + 1);

            innerWalls = new RoomWallsInfo
            {
                countOfWallsRight = countOfWallsRight,
                countOfWallsLeft = 0,
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
            if (!IsOnLadderPosition(startX, startY - innerWalls.countOfWallsDown)) 
            { 
                room.tileSetter.SetTile(roomTiles[10], startX, startY - innerWalls.countOfWallsDown, ObjectsLayers.Walls);
            }

            for (int x = startX + 1; x < startX + innerWalls.countOfWallsRight; x++)
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
            if (!IsOnLadderPosition(startX + innerWalls.countOfWallsRight, startY - innerWalls.countOfWallsDown))
            {
                room.tileSetter.SetTile(roomTiles[14], startX + innerWalls.countOfWallsRight, startY - innerWalls.countOfWallsDown, ObjectsLayers.Walls);
                ocupiedPlaces.Add(new Vector2(startX + innerWalls.countOfWallsRight, startY - innerWalls.countOfWallsDown));
            }
        }

        private void SetTilesByWidth()
        {
            if (!IsOnLadderPosition(startX + innerWalls.countOfWallsRight, startY))
            {
                room.tileSetter.SetTile(roomTiles[10], startX + innerWalls.countOfWallsRight, startY, ObjectsLayers.Walls);
                room.tileSetter.RotateTile(startX + innerWalls.countOfWallsRight, startY, 270);
            }

            for (int y = startY - 1; y > startY - innerWalls.countOfWallsDown; y--)
            {
                if (!IsOnLadderPosition(startX + innerWalls.countOfWallsRight, y))
                {
                    room.tileSetter.SetTile(roomTiles[11], startX + innerWalls.countOfWallsRight, y, ObjectsLayers.Walls);
                    room.tileSetter.RotateTile(startX + innerWalls.countOfWallsRight, y, 90);
                    ocupiedPlaces.Add(new Vector2(startX + innerWalls.countOfWallsRight, y));
                }  
            }
        }

        private void AddInnerOcupatePlaces()
        {
            for (int x = startX + 1;  x <= startX + innerWalls.countOfWallsRight; x++)
            {
                for (int y = startY; y >= startY - innerWalls.countOfWallsDown; y--)
                {
                    ocupiedPlaces.Add(new Vector2(x, y));
                }
            }
        }

        private void MakeLadderPath()
        {
            Vector2 leftWall = new Vector2(startX, startY);
            Vector2 rightWall = new Vector2(startX + innerWalls.countOfWallsRight, startY);
            int innerRoomFloorY = startY - innerWalls.countOfWallsDown;

            LadderPathBuilder.MakePathToRoom(leftWall, rightWall, innerRoomFloorY, room, rand);
        }
    }
}
