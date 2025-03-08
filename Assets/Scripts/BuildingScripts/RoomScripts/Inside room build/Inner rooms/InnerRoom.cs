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

        protected List<Vector2> floorWalls;
        protected List<Vector2> platforms;

        public InnerRoom(Room room, System.Random rand, Tile[] roomTiles, List<Vector2> ocupiedPlaces)
        {
            this.room = room;
            this.rand = rand;
            this.roomTiles = roomTiles;
            this.ocupiedPlaces = ocupiedPlaces;

            floorWalls = new List<Vector2>();
            platforms = new List<Vector2>();
        }

        public abstract void CraeteRoom();

        public Vector2 GetStartPosition()
        {
            return new Vector2(startX, startY);
        }

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

        protected void SpawnLamps()
        {
            Vector2 leftWall = new Vector2(startX - innerWalls.countOfWallsLeft, startY);
            Vector2 rightWall = new Vector2(startX + innerWalls.countOfWallsRight, startY);
            int floorY = startY - innerWalls.countOfWallsDown;
            int ceilingY = startY + innerWalls.countOfWallsUp;

            LampsSpawner.SpawnLamps(leftWall, rightWall, floorY, ceilingY, room);
        }

        protected void CollectFloorWalls(int leftX, int rightX, int y)
        {
            for (int x = leftX; x < rightX; x++)
            {
                if (!IsOnLadderPosition(x, y + 1) && !platforms.Contains(new Vector2(x, y)))
                {
                    floorWalls.Add(new Vector2Int(x, y));
                }
            }
        }

        protected void SetPlatformToList(int x, int y)
        {
            platforms.Add(new Vector2Int(x, y));
        }

        public List<Vector2> GetFLoorWalls()
        {
            return floorWalls;
        }
    }
}
