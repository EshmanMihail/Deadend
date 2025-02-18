using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.BuildingScripts
{
    public abstract class Room
    {
        public Vector2 entryPoint;

        public int countOfWallsUp;
        public int countOfWallsDown;
        public int countOfWallsRight;
        public int countOfWallsLeft;

        public RoomType roomType;
        public RoomBiom roomBiom;

        protected TilesSetter tileSetter;


        protected Tile[] tiles;
        protected GameObject[] gameObjects;

        public Room(Vector2 entryPoint, RoomType roomType,
            int countOfWallsUp, int countOfWallsDown,
            int countOfWallsLeft, int countOfWallsRight, RoomBiom roomBiom) 
        {
            this.entryPoint = entryPoint;
            this.roomType = roomType;
            this.countOfWallsUp = countOfWallsUp;
            this.countOfWallsDown = countOfWallsDown;
            this.countOfWallsLeft = countOfWallsLeft;
            this.countOfWallsRight = countOfWallsRight;
            this.roomBiom = roomBiom;
        }

        public abstract void GenerateRoomStructure();

        public abstract void SpawnRoomObjects();

        public void GetGameObjects(GameObject[] gameObjects)
        {
            this.gameObjects = gameObjects;
        }

        public void GetTilesAndTileSetter(Tile[] tiles, TilesSetter tilesSetter)
        {
            this.tiles = tiles;
            this.tileSetter = tilesSetter;
        }

        public Tile[] GetTiles() 
        { 
            return this.tiles; 
        }

        public Vector2 GetLeftUpperAngle()
        {
            return new Vector2((int)entryPoint.x - countOfWallsLeft, (int)entryPoint.y + countOfWallsUp);
        }

        public Vector2 GetRightBottomAngle()
        {
            return new Vector2((int)entryPoint.x + countOfWallsRight, (int)entryPoint.y - countOfWallsDown);
        }
    }
}
