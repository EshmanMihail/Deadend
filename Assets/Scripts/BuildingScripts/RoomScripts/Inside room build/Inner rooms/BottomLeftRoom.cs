using Assets.Scripts.BuildingScripts.BuildingTypes;
using Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build.Inner_rooms.InnerRoomStructs;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build.Inner_rooms
{
    public class BottomLeftRoom : InnerRoom
    {
        public BottomLeftRoom(Room room, System.Random rand, Tile[] roomTiles, List<Vector2> ocupiedPlaces)
            : base(room, rand, roomTiles, ocupiedPlaces)
        {
            startX = (int)room.GetLeftUpperAngle().x;
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

            CollectFloorWalls(startX + 1, startX + innerWalls.countOfWallsRight - 1, startY);
        }

        private void DetermineRoomSize()
        {
            int roomLength = room.wallsInfo.countOfWallsRight + room.wallsInfo.countOfWallsLeft;
            int roomWidth = room.wallsInfo.countOfWallsDown + room.wallsInfo.countOfWallsUp;

            int countOfWallsRight = rand.Next(roomLength / 2 - 1, roomLength / 2 + 3);
            int countOfWallsUp = rand.Next(3, roomWidth / 2 + 1);

            innerWalls = new RoomWallsInfo
            {
                countOfWallsRight = countOfWallsRight,
                countOfWallsLeft = 0,
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

            for (int x = startX + 1; x < startX + innerWalls.countOfWallsRight; x++)
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
            if (!IsOnLadderPosition(startX + innerWalls.countOfWallsRight, startY + innerWalls.countOfWallsUp))
            {
                room.tileSetter.SetTile(roomTiles[14], startX + innerWalls.countOfWallsRight, startY + innerWalls.countOfWallsUp, ObjectsLayers.Walls);
                room.tileSetter.RotateTile(startX + innerWalls.countOfWallsRight, startY + innerWalls.countOfWallsUp, 90);
                ocupiedPlaces.Add(new Vector2(startX + innerWalls.countOfWallsRight, startY + innerWalls.countOfWallsUp));
            }
        }

        private void SetTilesByWidth()
        {
            room.tileSetter.SetTile(roomTiles[10], startX + innerWalls.countOfWallsRight, startY, ObjectsLayers.Walls);
            room.tileSetter.RotateTile(startX + innerWalls.countOfWallsRight, startY, 90);

            for (int y = startY + 1; y < startY + innerWalls.countOfWallsUp; y++) 
            {
                if (!IsOnLadderPosition(startX + innerWalls.countOfWallsRight, y))
                {
                    room.tileSetter.SetTile(roomTiles[11], startX + innerWalls.countOfWallsRight, y, ObjectsLayers.Walls);
                    room.tileSetter.RotateTile(startX + innerWalls.countOfWallsRight, y, 90);
                    ocupiedPlaces.Add(new Vector2(startX + innerWalls.countOfWallsRight, y));
                }
            }

            Vector2 ladderPositionFromEntry = new Vector2(startX + innerWalls.countOfWallsRight, startY);
            if (BuildingData.ladder.Contains(ladderPositionFromEntry))
            {
                room.tileSetter.RemoveWall(new Vector3Int(startX + innerWalls.countOfWallsRight, startY + 1));
            }
        }

        private void AddInnerOcupatePlaces()
        {
            for (int x = startX + 1; x <= startX + innerWalls.countOfWallsRight; x++)
            {
                for (int y = startY; y <= startY + innerWalls.countOfWallsUp; y++)
                {
                    ocupiedPlaces.Add(new Vector2(x, y));
                }
            }
        }

        private void MakeEntranceToRoom()
        {
            int doorX = startX + innerWalls.countOfWallsRight;
            int doorY = startY + 1;

            room.tileSetter.RemoveWall(new Vector3Int(doorX, doorY, 10));
            if (rand.Next(0, 101) > 50) BuildingData.door.Add((new Vector2(doorX, doorY), room.roomBiom));
        }
    }
}
