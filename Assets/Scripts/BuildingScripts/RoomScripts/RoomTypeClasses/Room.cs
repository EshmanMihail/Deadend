using Assets.Scripts.BuildingScripts.BuildingTypes;
using Assets.Scripts.BuildingScripts.RoomScripts;
using Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build;
using Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_spawn;
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

        protected IRoomStructure structureGenerator;
        protected RoomObjectPlacer roomObjectPlacer;

        public TilesSetter tileSetter;
        protected Tile[] tiles;

        protected GameObject[] gameObjects;
        protected List<PositionProperty> positionsToSpawnObjects;

        public Room(Vector2 entryPoint, RoomType roomType, RoomWallsInfo wallsInfo, RoomBiom roomBiom,
            IRoomStructure structureGenerator, RoomObjectPlacer roomObjectPlacer) 
        {
            this.entryPoint = entryPoint;
            this.roomType = roomType;
            this.wallsInfo = wallsInfo;
            this.roomBiom = roomBiom;
            this.structureGenerator = structureGenerator;
            this.roomObjectPlacer = roomObjectPlacer;

            positionsToSpawnObjects = new List<PositionProperty>();
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

        public void SetStructureGenerator(IRoomStructure structureGenerator)
        {
            this.structureGenerator = structureGenerator;
        }

        public void SetPositionForLoot()
        {
            for (int i = 0; i < positionsToSpawnObjects.Count; i++)
            {
                Vector2 position = new Vector2(positionsToSpawnObjects[i].X, positionsToSpawnObjects[i].Y);
                BuildingData.loot.Add(position);
            }

            int leftX = (int)entryPoint.x - wallsInfo.countOfWallsLeft;
            int rightX = (int)entryPoint.x + wallsInfo.countOfWallsRight;
            int floorY = (int)entryPoint.y - wallsInfo.countOfWallsDown + 1;
            for (int x = leftX + 2; x < rightX - 1; x++)
            {
                BuildingData.loot.Add(new Vector2(x, floorY));
            }
        }

        public Tile[] GetTiles() 
        { 
            return this.tiles; 
        }

        public GameObject[] GetObjects()
        {
            return gameObjects;
        }

        public List<PositionProperty> GetPositionsToPlaceObjects()
        {
            return positionsToSpawnObjects;
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
