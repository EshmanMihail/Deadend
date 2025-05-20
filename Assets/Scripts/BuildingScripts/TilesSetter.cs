using Assets.Scripts.BuildingScripts.BuildingTypes;
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace Assets.Scripts.BuildingScripts
{
    public class TilesSetter
    {
        private BuildingGenerator buildingGenerator;
        private NetworkTileSetter networkTileSetter;
        private TileDataBase tileDataBase;

        private Tilemap wallsTilemap;
        private Tilemap backgroundWalls;
        private Tilemap ladderTilemap;
        private Tilemap platformsTilemap;
        private Tilemap frontTiles;
        private Tilemap backwardTiles;

        public TilesSetter(BuildingGenerator buildingGenerator,  Tilemap wallsTilemap, Tilemap backgroundWalls,
            Tilemap ladder, Tilemap platformsTilmap, Tilemap frontTiles, Tilemap backwardTiles, NetworkTileSetter networkTileSetter, GameObject tileDataBase)
        {
            this.buildingGenerator = buildingGenerator;
            this.wallsTilemap = wallsTilemap;
            this.ladderTilemap = ladder;
            this.platformsTilemap = platformsTilmap;
            this.backgroundWalls = backgroundWalls;
            this.frontTiles = frontTiles;
            this.backwardTiles = backwardTiles; ;
            this.networkTileSetter = networkTileSetter;
            this.tileDataBase = tileDataBase.GetComponent<TileDataBase>();
        }

        public void RemoveWall(Vector3Int positionToRemove)
        {
            wallsTilemap.SetTile(positionToRemove, null);
            networkTileSetter.AddTilesToRemove(positionToRemove, ObjectsLayers.Walls);
        }

        private void RemoveTile(Vector3Int positionToRemove, Tilemap tilemap, int tilemapInt)
        {
            tilemap.SetTile(positionToRemove, null);
            networkTileSetter.AddTilesToRemove(positionToRemove, tilemapInt);
        }

        public void SetTile(Tile tile, int x, int y, int layer)
        {
            Vector3Int tilePosition = new Vector3Int(x, y, 10);
            if (layer == ObjectsLayers.Walls)
            {
                RemoveTile(tilePosition, wallsTilemap, ObjectsLayers.Walls);
                wallsTilemap.SetTile(tilePosition, tile);
            }
            else if (layer == ObjectsLayers.BackgroundWalls)
            {
                RemoveTile(tilePosition, backgroundWalls, ObjectsLayers.BackgroundWalls);
                backgroundWalls.SetTile(tilePosition, tile);
            }
            else if (layer == ObjectsLayers.Ladder)
            {
                ladderTilemap.SetTile(tilePosition, tile);
                BuildingData.ladder.Add(new Vector2(x, y));
            }
            else if (layer == ObjectsLayers.FrontObjects)
            {
                frontTiles.SetTile(tilePosition, tile);
            }
            else if (layer == ObjectsLayers.BackwardObjects)
            {
                backwardTiles.SetTile(tilePosition, tile);
            }

            int tileIndex = tileDataBase.GetTileIndex(tile);
            networkTileSetter.AddTileInfo(tileIndex, x, y, layer);
        }

        public void SetPlatfromTile(Tile tile, int x, int y)
        {
            Vector3Int tilePosition = new Vector3Int(x, y, 10);
            platformsTilemap.SetTile(tilePosition, tile);

            int tileIndex = tileDataBase.GetTileIndex(tile);
            networkTileSetter.AddPlatformTileInfo(tileIndex, x, y);
        }

        public void RotateTile(int x, int y, float angle)
        {
            Vector3Int tilePosition = new Vector3Int(x, y, 10);
            Matrix4x4 matrix = wallsTilemap.GetTransformMatrix(tilePosition);

            Matrix4x4 rotationMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, angle), Vector3.one);

            wallsTilemap.SetTransformMatrix(tilePosition, rotationMatrix);

            networkTileSetter.AddTileToRotate(tilePosition, angle);
        }

        #region First build room
        public void SetRoomTiles(Room room)
        {
            Tile[] tiles = room.GetTiles();

            SetTilesOnRightPartOfRoom(room, tiles);
            SetTilesOnLeftPartOfRoom(room, tiles);
            SetTileOfRightRoomWall(room, tiles);
            SetTileOfLeftRoomWall(room, tiles);
            SetTilesInsideRoom(room, tiles);
            SetAngleTiles(room, tiles);
            MakeEntrance(room);
        }

        private void SetTilesOnRightPartOfRoom(Room room, Tile[] tile)
        {
            //upper part
            for (int i = 1; i < room.wallsInfo.countOfWallsRight; i++)
            {
                SetTile(tile[2], (int)room.entryPoint.x + i, (int)room.entryPoint.y + room.wallsInfo.countOfWallsUp, ObjectsLayers.Walls);
                buildingGenerator.AddPlaceToOccupiedPlaces(new Vector2((int)room.entryPoint.x + i, (int)room.entryPoint.y + room.wallsInfo.countOfWallsUp));
            }
            // lower part
            for (int i = 1; i < room.wallsInfo.countOfWallsRight; i++)
            {
                SetTile(tile[6], (int)room.entryPoint.x + i, (int)room.entryPoint.y - room.wallsInfo.countOfWallsDown, ObjectsLayers.Walls);
                buildingGenerator.AddPlaceToOccupiedPlaces(new Vector2((int)room.entryPoint.x + i, (int)room.entryPoint.y - room.wallsInfo.countOfWallsDown));
            }
        }

        private void SetTilesOnLeftPartOfRoom(Room room, Tile[] tile)
        {
            // upper part
            for (int i = 0; i < room.wallsInfo.countOfWallsLeft; i++)
            {
                SetTile(tile[2], (int)room.entryPoint.x - i, (int)room.entryPoint.y + room.wallsInfo.countOfWallsUp, ObjectsLayers.Walls);
                buildingGenerator.AddPlaceToOccupiedPlaces(new Vector2((int)room.entryPoint.x - i, (int)room.entryPoint.y + room.wallsInfo.countOfWallsUp));
            }
            //lower part
            for (int i = 0; i < room.wallsInfo.countOfWallsLeft; i++)
            {
                SetTile(tile[6], (int)room.entryPoint.x - i, (int)room.entryPoint.y - room.wallsInfo.countOfWallsDown, ObjectsLayers.Walls);
                buildingGenerator.AddPlaceToOccupiedPlaces(new Vector2((int)room.entryPoint.x - i, (int)room.entryPoint.y - room.wallsInfo.countOfWallsDown));
            }
        }

        private void SetTileOfRightRoomWall(Room room, Tile[] tile)
        {
            // set up
            for (int i = 0; i < room.wallsInfo.countOfWallsUp; i++)
            {
                SetTile(tile[4], (int)room.entryPoint.x + room.wallsInfo.countOfWallsRight, (int)room.entryPoint.y + i, ObjectsLayers.Walls);
                buildingGenerator.AddPlaceToOccupiedPlaces(new Vector2((int)room.entryPoint.x + room.wallsInfo.countOfWallsRight, (int)room.entryPoint.y + i));
            }
            // set down
            for (int i = 0; i < room.wallsInfo.countOfWallsDown; i++)
            {
                SetTile(tile[4], (int)room.entryPoint.x + room.wallsInfo.countOfWallsRight, (int)room.entryPoint.y - i, ObjectsLayers.Walls);
                buildingGenerator.AddPlaceToOccupiedPlaces(new Vector2((int)room.entryPoint.x + room.wallsInfo.countOfWallsRight, (int)room.entryPoint.y - i));
            }
        }

        private void SetTileOfLeftRoomWall(Room room, Tile[] tile)
        {
            // set up
            for (int i = 0; i < room.wallsInfo.countOfWallsUp; i++)
            {
                SetTile(tile[0], (int)room.entryPoint.x - room.wallsInfo.countOfWallsLeft, (int)room.entryPoint.y + i, ObjectsLayers.Walls);
                buildingGenerator.AddPlaceToOccupiedPlaces(new Vector2((int)room.entryPoint.x - room.wallsInfo.countOfWallsLeft, (int)room.entryPoint.y + i));
            }
            // set down
            for (int i = 0; i < room.wallsInfo.countOfWallsDown; i++)
            {
                SetTile(tile[0], (int)room.entryPoint.x - room.wallsInfo.countOfWallsLeft, (int)room.entryPoint.y - i, ObjectsLayers.Walls);
                buildingGenerator.AddPlaceToOccupiedPlaces(new Vector2((int)room.entryPoint.x - room.wallsInfo.countOfWallsLeft, (int)room.entryPoint.y - i));
            }
        }

        private void SetAngleTiles(Room room, Tile[] tile)
        {
            Vector2 positionOfRightUpperTile = new Vector2(room.entryPoint.x + room.wallsInfo.countOfWallsRight, room.entryPoint.y + room.wallsInfo.countOfWallsUp);
            Vector2 positionOfRightDownTile = new Vector2(room.entryPoint.x + room.wallsInfo.countOfWallsRight, room.entryPoint.y - room.wallsInfo.countOfWallsDown);
            Vector2 positionOfLeftUpperTile = new Vector2(room.entryPoint.x - room.wallsInfo.countOfWallsLeft, room.entryPoint.y + room.wallsInfo.countOfWallsUp);
            Vector2 positionOfLeftDownTile = new Vector2(room.entryPoint.x - room.wallsInfo.countOfWallsLeft, room.entryPoint.y - room.wallsInfo.countOfWallsDown);

            SetTile(tile[3], (int)room.entryPoint.x + room.wallsInfo.countOfWallsRight, (int)room.entryPoint.y + room.wallsInfo.countOfWallsUp, ObjectsLayers.Walls);
            SetTile(tile[5], (int)room.entryPoint.x + room.wallsInfo.countOfWallsRight, (int)room.entryPoint.y - room.wallsInfo.countOfWallsDown, ObjectsLayers.Walls);
            SetTile(tile[1], (int)room.entryPoint.x - room.wallsInfo.countOfWallsLeft, (int)room.entryPoint.y + room.wallsInfo.countOfWallsUp, ObjectsLayers.Walls);
            SetTile(tile[7], (int)room.entryPoint.x - room.wallsInfo.countOfWallsLeft, (int)room.entryPoint.y - room.wallsInfo.countOfWallsDown, ObjectsLayers.Walls);

            buildingGenerator.AddPlaceToOccupiedPlaces(positionOfRightUpperTile);
            buildingGenerator.AddPlaceToOccupiedPlaces(positionOfRightDownTile);
            buildingGenerator.AddPlaceToOccupiedPlaces(positionOfLeftUpperTile);
            buildingGenerator.AddPlaceToOccupiedPlaces(positionOfLeftDownTile);
        }

        private void SetTilesInsideRoom(Room room, Tile[] tile)
        {
            int positionOfLeftWall = (int)(room.entryPoint.x - room.wallsInfo.countOfWallsLeft);
            int positionOfRightWall = (int)(room.entryPoint.x + room.wallsInfo.countOfWallsRight);
            int positionOfFloor = (int)room.entryPoint.y - room.wallsInfo.countOfWallsDown;
            int positionOfCeiling = (int)room.entryPoint.y + room.wallsInfo.countOfWallsUp;

            for (int i = positionOfLeftWall + 1; i < positionOfRightWall; i++)
            {
                for (int j = positionOfFloor + 1; j < positionOfCeiling; j++)
                {
                    SetTile(tile[8], i, j, ObjectsLayers.BackgroundWalls);
                    buildingGenerator.AddPlaceToOccupiedPlaces(new Vector2(i, j));
                }
            }
        }

        public void MakeEntrance(Room room)
        {
            Tile backTile = room.GetTiles()[8];

            // remove wall
            RemoveTile(new Vector3Int((int)room.entryPoint.x, (int)room.entryPoint.y, 10), backgroundWalls, ObjectsLayers.BackgroundWalls);
            RemoveWall(new Vector3Int((int)room.entryPoint.x, (int)room.entryPoint.y, 10));

            BuildingData.node.Add(new Vector2((int)room.entryPoint.x + 0.5f, (int)room.entryPoint.y + 0.5f));

            SetTile(backTile, (int)room.entryPoint.x, (int)room.entryPoint.y, ObjectsLayers.BackgroundWalls);
        }

        public void CreateLadderPathToNextRoom(Vector2 beginPos, Room room, int roomFloorY)
        {
            int beginX = (int)beginPos.x;
            int beginY = (int)beginPos.y;

            Tile[] tile = room.GetTiles();

            BuildingData.node.Add(new Vector2(beginX + 0.5f, beginY + 1 + 0.5f));

            SetTile(tile[16], beginX, beginY, ObjectsLayers.Ladder);
            BuildingData.ladder.Add(beginPos);

            for (int y = beginY - 1; y > roomFloorY + 1; y--)
            {
                SetTile(tile[17], beginX, y, ObjectsLayers.Ladder);
                BuildingData.ladder.Add(new Vector2(beginX, y));
            }

            BuildingData.node.Add(new Vector2(beginX + 0.5f, roomFloorY + 1 + 0.5f));

            SetTile(tile[18], beginX, roomFloorY + 1, ObjectsLayers.Ladder);
            BuildingData.ladder.Add(new Vector2(beginX, roomFloorY + 1));
        }
        #endregion

        #region Set Biom Tiles

        public void MakeRoomHerBiom(Room room)
        {
            int leftX = (int)room.GetLeftUpperAngle().x;
            int rightX = (int)room.GetRightBottomAngle().x;
            int upperY = (int)room.GetLeftUpperAngle().y;
            int bottomY = (int)room.GetRightBottomAngle().y;

            Tile[] tiles = room.GetTiles();

            for (int i = leftX; i <= rightX; i++)
            {
                RemoveTile(new Vector3Int(i, upperY, 10), wallsTilemap, ObjectsLayers.Walls);
                RemoveTile(new Vector3Int(i, bottomY, 10), wallsTilemap, ObjectsLayers.Walls);

                SetTile(tiles[2], i, upperY, ObjectsLayers.Walls);
                SetTile(tiles[6], i, bottomY, ObjectsLayers.Walls);
            }
            for (int i = bottomY; i <= upperY; i++)
            {
                RemoveTile(new Vector3Int(leftX, i, 10), wallsTilemap, ObjectsLayers.Walls);
                RemoveTile(new Vector3Int(rightX, i, 10), wallsTilemap, ObjectsLayers.Walls);

                SetTile(tiles[0], leftX, i, ObjectsLayers.Walls);
                SetTile(tiles[4], rightX, i, ObjectsLayers.Walls);
            }

            RemoveTile(new Vector3Int(leftX, upperY, 10), wallsTilemap, ObjectsLayers.Walls);
            RemoveTile(new Vector3Int(rightX, upperY, 10), wallsTilemap, ObjectsLayers.Walls);
            RemoveTile(new Vector3Int(rightX, bottomY, 10), wallsTilemap, ObjectsLayers.Walls);
            RemoveTile(new Vector3Int(leftX, bottomY, 10), wallsTilemap, ObjectsLayers.Walls);

            SetTile(tiles[1], leftX, upperY, ObjectsLayers.Walls);
            SetTile(tiles[3], rightX, upperY, ObjectsLayers.Walls);
            SetTile(tiles[5], rightX, bottomY, ObjectsLayers.Walls);
            SetTile(tiles[7], leftX, bottomY, ObjectsLayers.Walls);

            SetTilesInsideRoomBiom(room, tiles);

            MakeEntrance(room);
        }

        private void SetTilesInsideRoomBiom(Room room, Tile[] tile)
        {
            int positionOfLeftWall = (int)(room.entryPoint.x - room.wallsInfo.countOfWallsLeft);
            int positionOfRightWall = (int)(room.entryPoint.x + room.wallsInfo.countOfWallsRight);
            int positionOfFloor = (int)room.entryPoint.y - room.wallsInfo.countOfWallsDown;
            int positionOfCeiling = (int)room.entryPoint.y + room.wallsInfo.countOfWallsUp;

            for (int i = positionOfLeftWall + 1; i < positionOfRightWall; i++)
            {
                for (int j = positionOfFloor + 1; j < positionOfCeiling; j++)
                {
                    RemoveTile(new Vector3Int(i, j, 10), backgroundWalls, ObjectsLayers.BackgroundWalls);
                    SetTile(tile[8], i, j, ObjectsLayers.BackgroundWalls);
                }
            }
        }

        #endregion
    }
}
