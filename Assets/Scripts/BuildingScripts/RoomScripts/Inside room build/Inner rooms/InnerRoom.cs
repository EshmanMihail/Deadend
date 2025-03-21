using Assets.Scripts.BuildingScripts.BuildingTypes;
using Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build.Inner_rooms.InnerRoomStructs;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build.Inner_rooms
{
    public abstract class InnerRoom
    {
        public Room room;
        protected System.Random rand;
        protected Tile[] roomTiles;

        protected int startX = 0;
        protected int startY = 0;
        protected RoomWallsInfo innerWalls;

        protected List<Vector2> ocupiedPlaces;
        protected RoomSizeCorrector sizeCorrector;

        protected List<(Vector2, bool)> floorWalls;
        protected List<Vector2> platforms;

        public InnerRoom(Room room, System.Random rand, Tile[] roomTiles, List<Vector2> ocupiedPlaces)
        {
            this.room = room;
            this.rand = rand;
            this.roomTiles = roomTiles;
            this.ocupiedPlaces = ocupiedPlaces;

            floorWalls = new List<(Vector2, bool)>();
            platforms = new List<Vector2>();
        }

        public abstract void CraeteRoom();

        protected bool IsInnerRoomCanExist()
        {
            if ((innerWalls.countOfWallsLeft + innerWalls.countOfWallsRight) * (innerWalls.countOfWallsDown + innerWalls.countOfWallsUp) < 4)
                return false;

            if (innerWalls.countOfWallsLeft + innerWalls.countOfWallsRight < 3) return false;

            if (innerWalls.countOfWallsDown + innerWalls.countOfWallsUp < 3) return false;

            return true;
        }

        protected bool IsOnLadderPosition(int x, int y)
        {
            Vector2 position = new Vector2(x, y);
            return BuildingData.ladder.Contains(position);
        }

        protected void CreateStoreies()
        {
            StoreyCreator storeyCreator = new StoreyCreator(this, rand);
            storeyCreator.CreateStoreies();

            List<(Vector2, bool)> pos = storeyCreator.GetFlooyPositions();
            for (int i = 0; i < pos.Count; i++)
            {
                floorWalls.Add(pos[i]);
            }
        }

        protected void SpawnLamps()
        {
            int leftX = startX - innerWalls.countOfWallsLeft;
            int rightX = startX + innerWalls.countOfWallsRight;
            int floorY = startY - innerWalls.countOfWallsDown;
            int ceilingY = startY + innerWalls.countOfWallsUp;

            if (rightX -  leftX > 6)
               LampsSpawner.SpawnTwoLamps(leftX, rightX, floorY, ceilingY, room);
            else
                LampsSpawner.SpawnLampInCenter(leftX, rightX, floorY, ceilingY, room);
        }

        protected void CollectFloorWalls()
        {
            int leftX = startX - innerWalls.countOfWallsLeft + 1;
            int rightX = startX + innerWalls.countOfWallsRight;
            int floorY = startY - innerWalls.countOfWallsDown;

            for (int x = leftX; x < rightX - 1; x++)
            {
                if (!IsOnLadderPosition(x, floorY + 1) && !platforms.Contains(new Vector2(x, floorY)))
                {
                    floorWalls.Add((new Vector2Int(x, floorY + 1), false));
                }
            }

            floorWalls.Add((new Vector2Int(rightX - 1, floorY + 1), true));
        }

        public RoomWallsInfo GetInnerRoomWallsInfo()
        {
            return innerWalls;
        }

        public Vector2 GetStartPosition()
        {
            return new Vector2(startX, startY);
        }

        public List<(Vector2, bool)> GetFLoorWalls()
        {
            return floorWalls;
        }
    }
}
