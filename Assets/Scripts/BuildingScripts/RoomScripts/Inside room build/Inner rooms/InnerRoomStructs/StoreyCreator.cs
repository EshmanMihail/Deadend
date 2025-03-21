using Assets.Scripts.BuildingScripts.BuildingTypes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build.Inner_rooms.InnerRoomStructs
{
    public class StoreyCreator
    {
        private InnerRoom innerRoom;
        private Tile[] tiles;
        protected System.Random rand;

        private Vector2 startPosition;
        private RoomWallsInfo roomWallsInfo;

        private int storeyHeight = 4;
        private int storeyRoomWidth = 6;

        private List<(Vector2, bool)> floorPosition;

        private bool isHaveRooms = false;

        public StoreyCreator(InnerRoom innerRoom, System.Random rand)
        {
            this.innerRoom = innerRoom;
            this.rand = rand;

            floorPosition = new List<(Vector2, bool)>();
        }

        public void CreateStoreies()
        {
            wbolwevdnwvdenl();

            int leftX = (int)startPosition.x - roomWallsInfo.countOfWallsLeft;
            int rightX = (int)startPosition.x + roomWallsInfo.countOfWallsRight;

            leftX += 2;
            rightX -= 2;

            int startY = (int)startPosition.y - roomWallsInfo.countOfWallsDown + 4;
            int roomCeiling = (int)startPosition.y + roomWallsInfo.countOfWallsUp;

            int counterToRightWall = 0; // щётчик для установления стены комныты.

            for (int y = startY; y < roomCeiling; y += storeyHeight)
            {
                for (int x = leftX; x <= rightX; x++)
                {
                    if (rand.Next(0, 100) < 100 && counterToRightWall == storeyRoomWidth)
                    {
                        counterToRightWall = 0;
                        isHaveRooms = true;
                        erblkkerbjnewvny(x, y);
                    }
                    else
                    {
                        if (x == rightX) floorPosition.Add((new Vector2(x, y - storeyHeight + 1), true));
                        else floorPosition.Add((new Vector2(x, y - storeyHeight + 1), false));

                        if (!BuildingData.ladder.Contains(new Vector2(x, y)))
                            innerRoom.room.tileSetter.SetTile(tiles[11], x, y, ObjectsLayers.Walls);
                    }
                    counterToRightWall++;
                }
                SetLadders(leftX - 1, rightX + 1, y);

                if (!isHaveRooms) LampsSpawner.SpawnTwoLamps(leftX, rightX, y - storeyHeight, y - 1, innerRoom.room);
            }
        }

        private void wbolwevdnwvdenl()
        {
            tiles = innerRoom.room.GetTiles();
            roomWallsInfo = innerRoom.GetInnerRoomWallsInfo();
            startPosition = innerRoom.GetStartPosition();
        }

        private void erblkkerbjnewvny(int x, int startY)
        {
            innerRoom.room.tileSetter.SetTile(tiles[13], x, startY, ObjectsLayers.Walls);
            innerRoom.room.tileSetter.RotateTile(x, startY, 90);

            for (int y = startY - 1; y > startY - storeyHeight + 1; y--)
            {
                innerRoom.room.tileSetter.SetTile(tiles[11], x, y, ObjectsLayers.Walls);
                innerRoom.room.tileSetter.RotateTile(x, y, 90);
            }

            BuildingData.door.Add((new Vector2(x, startY - storeyHeight), innerRoom.room.roomBiom));

            LampsSpawner.SpawnLampInCenter(x - storeyRoomWidth, x, startY - storeyHeight, startY, innerRoom.room);
        }

        private void SetLadders(int leftX, int rightX, int startY)
        {
            //left ladder
            if (!BuildingData.ladder.Contains(new Vector2(leftX, startY)))
            {
                innerRoom.room.tileSetter.SetTile(tiles[16], leftX, startY, ObjectsLayers.Ladder);
                for (int y = startY - 1; y > startY - storeyHeight + 1; y--)
                {
                    innerRoom.room.tileSetter.SetTile(tiles[17], leftX, y, ObjectsLayers.Ladder);
                }
                innerRoom.room.tileSetter.SetTile(tiles[18], leftX, startY - storeyHeight + 1, ObjectsLayers.Ladder);
            }

            //right ladder
            if (!BuildingData.ladder.Contains(new Vector2(rightX, startY)))
            {
                innerRoom.room.tileSetter.SetTile(tiles[16], rightX, startY, ObjectsLayers.Ladder);
                for (int y = startY - 1; y > startY - storeyHeight + 1; y--)
                {
                    innerRoom.room.tileSetter.SetTile(tiles[17], rightX, y, ObjectsLayers.Ladder);
                }
                innerRoom.room.tileSetter.SetTile(tiles[18], rightX, startY - storeyHeight + 1, ObjectsLayers.Ladder);
            }
        }

        public List<(Vector2, bool)> GetFlooyPositions()
        {
            return floorPosition;
        }
    }
}
