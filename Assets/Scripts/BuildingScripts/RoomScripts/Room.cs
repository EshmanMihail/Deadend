using Assets.Scripts.BuildingScripts.RoomScripts;
using Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.BuildingScripts
{
    public abstract class Room
    {
        public Vector2 entryPoint;

        public RoomWallsInfo wallsInfo;

        public RoomType roomType;
        public RoomBiom roomBiom;

        protected RoomStructureGenerator structureGenerator;

        protected TilesSetter tileSetter;

        protected Tile[] tiles;
        protected GameObject[] gameObjects;

        protected List<Vector2> positionOfWalls;
        protected List<Vector2> freePlacesToSpawn;
        protected List<Vector2> nodes;

        public Room(Vector2 entryPoint, RoomType roomType, RoomWallsInfo wallsInfo, RoomBiom roomBiom) 
        {
            this.entryPoint = entryPoint;
            this.roomType = roomType;
            this.wallsInfo = wallsInfo;
            this.roomBiom = roomBiom;
        }

        public abstract void GenerateRoomStructure();

        public abstract void SpawnRoomObjects();

        public void SetGameObjects(GameObject[] gameObjects)
        {
            this.gameObjects = gameObjects;
        }

        public void SetTilesAndTileSetter(Tile[] tiles, TilesSetter tilesSetter)
        {
            this.tiles = tiles;
            this.tileSetter = tilesSetter;
        }

        public void SetStructureGenerator(RoomStructureGenerator structureGenerator)
        {
            this.structureGenerator = structureGenerator;
        }

        public Tile[] GetTiles() 
        { 
            return this.tiles; 
        }

        public Vector2 GetLeftUpperAngle()
        {
            return new Vector2((int)entryPoint.x - wallsInfo.countOfWallsLeft, (int)entryPoint.y + wallsInfo.countOfWallsUp);
        }

        public Vector2 GetRightBottomAngle()
        {
            return new Vector2((int)entryPoint.x + wallsInfo.countOfWallsRight, (int)entryPoint.y - wallsInfo.countOfWallsDown);
        }
    }
}
