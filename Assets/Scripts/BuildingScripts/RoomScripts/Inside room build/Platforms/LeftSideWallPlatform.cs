using Assets.Scripts.BuildingScripts.BuildingTypes;
using Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build.Inner_rooms;
using Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build.Inner_rooms.InnerRoomStructs;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build.Platforms
{
    public class LeftSideWallPlatform
    {
        private Room room;
        System.Random rand;
        private List<Vector2> occupiedPlaces;
        private List<Vector2> platformWall = new List<Vector2>();
        private List<Vector2> platformsPositions = new List<Vector2>();

        private int chanceToCreateWallPlatform = 70;
        private int chanceToMakePlatform = 20;

        public LeftSideWallPlatform(Room room, System.Random rand, List<Vector2> occupiedPlaces)
        {
            this.room = room;
            this.rand = rand;
            this.occupiedPlaces = occupiedPlaces;
        }

        public List<Vector2> CreatePlatformsOnLeftSide()
        {
            int startY = (int)room.entryPoint.y - room.wallsInfo.countOfWallsDown + 3;
            int startX = (int)room.entryPoint.x - room.wallsInfo.countOfWallsLeft;

            int roomCeilingY = (int)room.entryPoint.y + room.wallsInfo.countOfWallsUp;

            CreatePlatfroms(startX, startY, roomCeilingY);

            return platformWall;
        }

        private void CreatePlatfroms(int startX, int startY, int roomCeilingY)
        {
            int roomLength = room.wallsInfo.countOfWallsLeft + room.wallsInfo.countOfWallsRight;

            for (int y = startY; y < roomCeilingY - 2; y += 4)
            {
                if (rand.Next(0, 100) < chanceToCreateWallPlatform) CreatePlatform(startX, y, roomLength);
            }
        }

        private void CreatePlatform(int startX, int y, int roomLength)
        {
            bool isHaveLadderPath = false;
            int platformLength = rand.Next(1, roomLength / 2 + 1);
            CorrectLeftPlatformLength(ref platformLength, startX, y, ref isHaveLadderPath);

            if (!IsPlatformCanExists(startX, y, platformLength)) return;

            SetTilesOfLeftPlatform(startX, y, platformLength, ref isHaveLadderPath);

            int lampsY = y + 2;
            if (IsCanSpawnLamps(startX, lampsY, platformLength))
            {
                if (platformLength > 6) SpawnLamps(startX, y, platformLength, lampsY);
                else SpawnOneLamp(startX, y, platformLength, lampsY);
            }

            if (!isHaveLadderPath)
                MakePathToPlatform(new Vector2(startX + platformLength + 1, y));

            if (platformLength > 5) NodeSpawner.SpawnNode((startX + 2), y);

            if (platformLength > 4)
                platformsPositions = WallToPlatformChanger.MakeWallToPlatform(room, startX + 1, startX + platformLength, y, rand, chanceToMakePlatform);

            CorrectListOfWallsPositions();

            if (platformLength > 3 && room.roomBiom == RoomBiom.grass)
                SpawnGrassOnFloor(startX + 1, y + 1, platformLength);
        }

        private void CorrectLeftPlatformLength(ref int platformLength, int startX, int startY, ref bool isHaveLadderPath)
        {
            bool flag = false;
            for (int x = startX; x < startX + platformLength; x++)
            {
                if (occupiedPlaces.Contains(new Vector2(x + 1, startY)) || occupiedPlaces.Contains(new Vector2(x + 1, startY + 1)))
                {
                    platformLength = x + 1 - startX;
                    platformLength -= 2;
                    flag = true;
                }
                if (BuildingData.ladder.Contains(new Vector2(x + 1, startY)))
                {
                    platformLength = x - startX;
                    isHaveLadderPath = true;
                    flag = true;
                }

                if (flag) break;
            }
            if (occupiedPlaces.Contains(new Vector2(startX + platformLength + 1, startY + 1)))
            {
                platformLength--;
            }
        }

        private bool IsPlatformCanExists(int startX, int y, int platformLength)
        {
            if (platformLength < 1) return false;
            if (occupiedPlaces.Contains(new Vector2(startX + 1, y))) return false;
            if (occupiedPlaces.Contains(new Vector2(startX + 1, y + 1))) return false;

            return true;
        }

        private void SetTilesOfLeftPlatform(int startX, int startY, int platformLength, ref bool isHaveLadderPath)
        {
            Tile[] tile = room.GetTiles();

            room.tileSetter.SetTile(tile[10], startX, startY, ObjectsLayers.Walls);

            for (int x = startX + 1; x < startX + platformLength; x++)
            {
                if (!BuildingData.ladder.Contains(new Vector2(x, startY)))
                {
                    room.tileSetter.SetTile(tile[11], x, startY, ObjectsLayers.Walls);
                    platformWall.Add(new Vector2(x, startY));
                }
                else isHaveLadderPath = true;
            }
            if (!BuildingData.ladder.Contains(new Vector2(startX + platformLength, startY)))
            {
                room.tileSetter.SetTile(tile[12], startX + platformLength, startY, ObjectsLayers.Walls);
                platformWall.Add(new Vector2(startX + platformLength, startY));
            }
            else isHaveLadderPath = true;
        }

        private bool IsCanSpawnLamps(int startX, int y, int plaformLength)
        {
            if (y > (int)room.entryPoint.y + room.wallsInfo.countOfWallsUp) return false;

            for (int x = startX + 1; x <= startX + plaformLength; x++)
            {
                if (occupiedPlaces.Contains(new Vector2(x, y))) return false;
            }
            return true;
        }

        private void SpawnOneLamp(int startX, int startY, int platformLength, int lampY)
        {
            Tile lampTile = room.GetTiles()[9];
            int platfromCenxterX = (startX + startX + platformLength) / 2;

            room.tileSetter.SetTile(lampTile, platfromCenxterX, lampY, ObjectsLayers.BackgroundWalls);
            BuildingData.lamp.Add((new Vector2(platfromCenxterX, lampY), room.roomBiom));
        }

        private void SpawnLamps(int startX, int startY, int platformLength, int lampY)
        {
            Tile lampTile = room.GetTiles()[9];

            Vector2 leftEnd = new Vector2(startX + 1, startY);
            Vector2 rightEnd = new Vector2(startX + platformLength, startY);

            int platfromCenxterX = ((int)leftEnd.x + (int)rightEnd.x) / 2;

            int x1 = (platfromCenxterX + (int)rightEnd.x) / 2;
            room.tileSetter.SetTile(lampTile, x1, lampY, ObjectsLayers.BackgroundWalls);
            BuildingData.lamp.Add((new Vector2(x1, lampY), room.roomBiom));

            int x2 = ((int)leftEnd.x + platfromCenxterX) / 2;
            room.tileSetter.SetTile(lampTile, x2, lampY, ObjectsLayers.BackgroundWalls);
            BuildingData.lamp.Add((new Vector2(x2, lampY), room.roomBiom));
        }

        private void MakePathToPlatform(Vector2 beginPosition)
        {
            Tile[] tile = room.GetTiles();

            int y = (int)beginPosition.y;
            int x = (int)beginPosition.x;

            room.tileSetter.SetTile(tile[16], x, y, ObjectsLayers.Ladder);
            y--;

            int roomFloorY = (int)room.entryPoint.y - room.wallsInfo.countOfWallsDown;
            int upperTheFloorY = roomFloorY++;
            bool isMeetPlatform = false;

            while (upperTheFloorY < y)
            {
                if (platformWall.Contains(new Vector2(x, y)) || occupiedPlaces.Contains(new Vector2(x, y)))
                {
                    room.tileSetter.SetTile(tile[18], x, y, ObjectsLayers.Ladder);
                    isMeetPlatform = true;
                    break;
                }

                room.tileSetter.SetTile(tile[17], x, y, ObjectsLayers.Ladder);
                y--;
            }

            if (!isMeetPlatform) room.tileSetter.SetTile(tile[18], x, roomFloorY, ObjectsLayers.Ladder);
        }

        private void SpawnGrassOnFloor(int startX, int y, int platformLength)
        {
            Tile[] tile = room.GetTiles();
            Tile[] grass = { tile[20], tile[21], tile[22] };

            for (int x = startX; x < startX + platformLength; x++)
            {
                if (!platformsPositions.Contains(new Vector2(x, y - 1)))
                {
                    int randIndex = rand.Next(0, grass.Length);
                    room.tileSetter.SetTile(grass[randIndex], x, y, ObjectsLayers.FrontObjects);
                }
            }
        }

        private void CorrectListOfWallsPositions()
        {
            platformWall.RemoveAll(position => platformsPositions.Contains(position));
        }
    }
}
