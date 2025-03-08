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
        public BottomRightRoom(Room room, System.Random rand, Tile[] roomTiles, List<Vector2> ocupiedPlaces)
            : base(room, rand, roomTiles, ocupiedPlaces)
        {
            startX = (int)room.GetRightBottomAngle().x;
            startY = (int)room.GetRightBottomAngle().y;
        }

        public override void CraeteRoom()
        {
            DetermineRoomSize();

            sizeCorrector = new RoomSizeCorrector(this, ocupiedPlaces);
            sizeCorrector.CorrectSize(ref innerWalls);

            if (!IsInnerRoomCanExist()) return;

            SetRoomTiles();
            AddInnerOcupatePlaces();

            MakeEntranceToRoom();

            if (rand.Next(0, 101) > 30) SpawnLamps();

            CollectFloorWalls(startX - 1, startX - innerWalls.countOfWallsLeft + 1, startY);
        }

        private void DetermineRoomSize()
        {
            int roomLength = room.wallsInfo.countOfWallsRight + room.wallsInfo.countOfWallsLeft;
            int roomWidth = room.wallsInfo.countOfWallsDown + room.wallsInfo.countOfWallsUp;

            int countOfWallsLeft = rand.Next(roomLength / 2 - 1, roomLength / 2 + 3);
            int countOfWallsUp = rand.Next(3, roomWidth / 2 + 1);

            innerWalls = new RoomWallsInfo
            {
                countOfWallsRight = 0,
                countOfWallsLeft = countOfWallsLeft,
                countOfWallsUp = countOfWallsUp,
                countOfWallsDown = 0
            };
        }

        private void SetRoomTiles()
        {
            SetTilesByLength();

            SetTilesByWidth();
        }

        private void SetTilesByLength()
        {
            room.tileSetter.SetTile(roomTiles[10], startX, startY + innerWalls.countOfWallsUp, ObjectsLayers.Walls);
            room.tileSetter.RotateTile(startX, startY + innerWalls.countOfWallsUp, 180);

            for (int x = startX - 1; x > startX - innerWalls.countOfWallsLeft; x--)
            {
                if (!IsOnLadderPosition(x, startY + innerWalls.countOfWallsUp))
                {
                    room.tileSetter.SetTile(roomTiles[11], x, startY + innerWalls.countOfWallsUp, ObjectsLayers.Walls);
                    ocupiedPlaces.Add(new Vector2(x, startY + innerWalls.countOfWallsUp));
                }
            }

            SetAngleTile();
        }

        private void SetAngleTile()
        {
            if (!IsOnLadderPosition(startX - innerWalls.countOfWallsLeft, startY + innerWalls.countOfWallsUp))
            {
                room.tileSetter.SetTile(roomTiles[14], startX - innerWalls.countOfWallsLeft, startY + innerWalls.countOfWallsUp, ObjectsLayers.Walls);
                room.tileSetter.RotateTile(startX - innerWalls.countOfWallsLeft, startY + innerWalls.countOfWallsUp, 180);
                ocupiedPlaces.Add(new Vector2(startX - innerWalls.countOfWallsLeft, startY + innerWalls.countOfWallsUp));
            }
        }

        private void SetTilesByWidth()
        {
            room.tileSetter.SetTile(roomTiles[10], startX - innerWalls.countOfWallsLeft, startY, ObjectsLayers.Walls);
            room.tileSetter.RotateTile(startX - innerWalls.countOfWallsLeft, startY, 90);
            for (int y = startY + 1; y < startY + innerWalls.countOfWallsUp; y++)
            {
                if (!IsOnLadderPosition(startX - innerWalls.countOfWallsLeft, y))
                {
                    room.tileSetter.SetTile(roomTiles[11], startX - innerWalls.countOfWallsLeft, y, ObjectsLayers.Walls);
                    room.tileSetter.RotateTile(startX - innerWalls.countOfWallsLeft, y, 90);
                    ocupiedPlaces.Add(new Vector2(startX - innerWalls.countOfWallsLeft, y));
                }
            }

            Vector2 ladderPositionFromEntry = new Vector2(startX - innerWalls.countOfWallsLeft, startY);
            if (BuildingData.ladder.Contains(ladderPositionFromEntry))
            {
                room.tileSetter.RemoveWall(new Vector3Int(startX - innerWalls.countOfWallsLeft, startY + 1));
            }
        }


        private void AddInnerOcupatePlaces()
        {
            for (int x = startX - 1; x >= startX - innerWalls.countOfWallsLeft; x--)
            {
                for (int y = startY; y <= startY + innerWalls.countOfWallsUp; y++)
                {
                    ocupiedPlaces.Add(new Vector2(x, y));
                }
            }
        }

        private void MakeEntranceToRoom()
        {
            int doorX = startX - innerWalls.countOfWallsLeft;
            int doorY = startY + 1;

            room.tileSetter.RemoveWall(new Vector3Int(doorX, doorY, 10));
            if (rand.Next(0, 101) > 50) BuildingData.door.Add((new Vector2(doorX, doorY), room.roomBiom));
        }
    }
}
