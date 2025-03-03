using Assets.Scripts.BuildingScripts.BuildingTypes;
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

        private Tilemap wallsTilemap;
        private Tilemap backgroundWalls;
        private Tilemap ladderTilemap;
        private Tilemap platformsTilemap;

        private Tile[] metalRoomTiles;
        private Tile lampTile;

        public TilesSetter(BuildingGenerator buildingGenerator, Tilemap wallsTilemap, Tilemap backgroundWalls,
            Tilemap ladder, Tilemap platformsTilmap, Tile[] metalRoomTiles, Tile lampTile)
        {
            this.buildingGenerator = buildingGenerator;
            this.wallsTilemap = wallsTilemap;
            this.ladderTilemap = ladder;
            this.platformsTilemap = platformsTilmap;
            this.backgroundWalls = backgroundWalls;
            this.metalRoomTiles = metalRoomTiles;
            this.lampTile = lampTile;
        }

        public void RemoveWall(Vector3Int positionToRemove)
        {
            wallsTilemap.SetTile(positionToRemove, null);
            BuildingData.RemoveTileFromTileListData(positionToRemove);
        }

        private void RemoveTile(Vector3Int positionToRemove, Tilemap tilemap)
        {
            tilemap.SetTile(positionToRemove, null);
            BuildingData.RemoveTileFromTileListData(positionToRemove);
        }

        public void SetTile(Tile tile, int x, int y, int layer)
        {
            Vector3Int tilePosition = new Vector3Int(x, y, 10);
            if (layer == ObjectsLayers.Walls)
            {
                RemoveTile(tilePosition, wallsTilemap);
                wallsTilemap.SetTile(tilePosition, tile);

                BuildingData.AddTileToTileListData(tilePosition, tile, layer);
            }
            else if (layer == ObjectsLayers.BackgroundWalls)
            {
                RemoveTile(tilePosition, backgroundWalls);
                backgroundWalls.SetTile(tilePosition, tile);

                BuildingData.AddTileToTileListData(tilePosition, tile, layer);
            }
            else if (layer == ObjectsLayers.Ladder)
            {
                ladderTilemap.SetTile(tilePosition, tile);
                BuildingData.ladder.Add(new Vector2(x, y));
                BuildingData.AddTileToTileListData(tilePosition, tile, ObjectsLayers.Ladder);
            }
        }

        public void SetPlatfromTile(Tile tile, int x, int y)
        {
            Vector3Int tilePosition = new Vector3Int(x, y, 10);
            platformsTilemap.SetTile(tilePosition, tile);
        }

        public void RotateTile(int x, int y, float angle)
        {
            Vector3Int tilePosition = new Vector3Int(x, y, 10);
            Matrix4x4 matrix = wallsTilemap.GetTransformMatrix(tilePosition);

            Matrix4x4 rotationMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, angle), Vector3.one);

            wallsTilemap.SetTransformMatrix(tilePosition, rotationMatrix);
        }

        public void SetObject(GameObject gameObject, int x, int y, int layer)
        {

        }

        #region First build room
        public void SetRoomTiles(Room room)
        {
            SetTilesOnRightPartOfRoom(room, metalRoomTiles);
            SetTilesOnLeftPartOfRoom(room, metalRoomTiles);
            SetTileOfRightRoomWall(room, metalRoomTiles);
            SetTileOfLeftRoomWall(room, metalRoomTiles);
            SetTilesInsideRoom(room, metalRoomTiles);
            SetAngleTiles(room, metalRoomTiles);
            MakeEntrance(room);
        }

        private void SetTilesOnRightPartOfRoom(Room room, Tile[] tile)
        {
            //upper part
            for (int i = 1; i < room.wallsInfo.countOfWallsRight; i++)
            {
                Vector3Int tilePosition = new Vector3Int((int)room.entryPoint.x + i, (int)room.entryPoint.y + room.wallsInfo.countOfWallsUp, 10);
                wallsTilemap.SetTile(tilePosition, tile[2]);

                BuildingData.AddTileToTileListData(tilePosition, tile[2], ObjectsLayers.Walls);
                buildingGenerator.AddPlaceToOccupiedPlaces(new Vector2((int)room.entryPoint.x + i, (int)room.entryPoint.y + room.wallsInfo.countOfWallsUp));
            }
            // lower part
            for (int i = 1; i < room.wallsInfo.countOfWallsRight; i++)
            {
                Vector3Int tilePosition = new Vector3Int((int)room.entryPoint.x + i, (int)room.entryPoint.y - room.wallsInfo.countOfWallsDown, 10);
                wallsTilemap.SetTile(tilePosition, tile[6]);

                BuildingData.AddTileToTileListData(tilePosition, tile[6], ObjectsLayers.Walls);
                buildingGenerator.AddPlaceToOccupiedPlaces(new Vector2((int)room.entryPoint.x + i, (int)room.entryPoint.y - room.wallsInfo.countOfWallsDown));
            }
        }

        private void SetTilesOnLeftPartOfRoom(Room room, Tile[] tile)
        {
            // upper part
            for (int i = 0; i < room.wallsInfo.countOfWallsLeft; i++)
            {
                Vector3Int tilePosition = new Vector3Int((int)room.entryPoint.x - i, (int)room.entryPoint.y + room.wallsInfo.countOfWallsUp, 10);
                wallsTilemap.SetTile(tilePosition, tile[2]);

                BuildingData.AddTileToTileListData(tilePosition, tile[2], ObjectsLayers.Walls);
                buildingGenerator.AddPlaceToOccupiedPlaces(new Vector2((int)room.entryPoint.x - i, (int)room.entryPoint.y + room.wallsInfo.countOfWallsUp));
            }
            //lower part
            for (int i = 0; i < room.wallsInfo.countOfWallsLeft; i++)
            {
                Vector3Int tilePosition = new Vector3Int((int)room.entryPoint.x - i, (int)room.entryPoint.y - room.wallsInfo.countOfWallsDown, 10);
                wallsTilemap.SetTile(tilePosition, tile[6]);

                BuildingData.AddTileToTileListData(tilePosition, tile[6], ObjectsLayers.Walls);
                buildingGenerator.AddPlaceToOccupiedPlaces(new Vector2((int)room.entryPoint.x - i, (int)room.entryPoint.y - room.wallsInfo.countOfWallsDown));
            }
        }

        private void SetTileOfRightRoomWall(Room room, Tile[] tile)
        {
            // set up
            for (int i = 0; i < room.wallsInfo.countOfWallsUp; i++)
            {
                Vector3Int tilePosition = new Vector3Int((int)room.entryPoint.x + room.wallsInfo.countOfWallsRight, (int)room.entryPoint.y + i, 10);
                wallsTilemap.SetTile(tilePosition, tile[4]);

                BuildingData.AddTileToTileListData(tilePosition, tile[4], ObjectsLayers.Walls);
                buildingGenerator.AddPlaceToOccupiedPlaces(new Vector2((int)room.entryPoint.x + room.wallsInfo.countOfWallsRight, (int)room.entryPoint.y + i));
            }
            // set down
            for (int i = 0; i < room.wallsInfo.countOfWallsDown; i++)
            {
                Vector3Int tilePosition = new Vector3Int((int)room.entryPoint.x + room.wallsInfo.countOfWallsRight, (int)room.entryPoint.y - i, 10);
                wallsTilemap.SetTile(tilePosition, tile[4]);

                BuildingData.AddTileToTileListData(tilePosition, tile[4], ObjectsLayers.Walls);
                buildingGenerator.AddPlaceToOccupiedPlaces(new Vector2((int)room.entryPoint.x + room.wallsInfo.countOfWallsRight, (int)room.entryPoint.y - i));
            }
        }

        private void SetTileOfLeftRoomWall(Room room, Tile[] tile)
        {
            // set up
            for (int i = 0; i < room.wallsInfo.countOfWallsUp; i++)
            {
                Vector3Int tilePosition = new Vector3Int((int)room.entryPoint.x - room.wallsInfo.countOfWallsLeft, (int)room.entryPoint.y + i, 10);
                wallsTilemap.SetTile(tilePosition, tile[0]);

                BuildingData.AddTileToTileListData(tilePosition, tile[0], ObjectsLayers.Walls);
                buildingGenerator.AddPlaceToOccupiedPlaces(new Vector2((int)room.entryPoint.x - room.wallsInfo.countOfWallsLeft, (int)room.entryPoint.y + i));
            }
            // set down
            for (int i = 0; i < room.wallsInfo.countOfWallsDown; i++)
            {
                Vector3Int tilePosition = new Vector3Int((int)room.entryPoint.x - room.wallsInfo.countOfWallsLeft, (int)room.entryPoint.y - i, 10);
                wallsTilemap.SetTile(tilePosition, tile[0]);

                BuildingData.AddTileToTileListData(tilePosition, tile[0], ObjectsLayers.Walls);
                buildingGenerator.AddPlaceToOccupiedPlaces(new Vector2((int)room.entryPoint.x - room.wallsInfo.countOfWallsLeft, (int)room.entryPoint.y - i));
            }
        }

        private void SetAngleTiles(Room room, Tile[] tile)
        {
            Vector2 positionOfRightUpperTile = new Vector2(room.entryPoint.x + room.wallsInfo.countOfWallsRight, room.entryPoint.y + room.wallsInfo.countOfWallsUp);
            Vector2 positionOfRightDownTile = new Vector2(room.entryPoint.x + room.wallsInfo.countOfWallsRight, room.entryPoint.y - room.wallsInfo.countOfWallsDown);
            Vector2 positionOfLeftUpperTile = new Vector2(room.entryPoint.x - room.wallsInfo.countOfWallsLeft, room.entryPoint.y + room.wallsInfo.countOfWallsUp);
            Vector2 positionOfLeftDownTile = new Vector2(room.entryPoint.x - room.wallsInfo.countOfWallsLeft, room.entryPoint.y - room.wallsInfo.countOfWallsDown);

            Vector3Int positionOfRightUpperTileINT = new Vector3Int((int)room.entryPoint.x + room.wallsInfo.countOfWallsRight, (int)room.entryPoint.y + room.wallsInfo.countOfWallsUp, 10);
            Vector3Int positionOfRightDownTileINT = new Vector3Int((int)room.entryPoint.x + room.wallsInfo.countOfWallsRight, (int)room.entryPoint.y - room.wallsInfo.countOfWallsDown, 10);
            Vector3Int positionOfLeftUpperTileINT = new Vector3Int((int)room.entryPoint.x - room.wallsInfo.countOfWallsLeft, (int)room.entryPoint.y + room.wallsInfo.countOfWallsUp, 10);
            Vector3Int positionOfLeftDownTileINT = new Vector3Int((int)room.entryPoint.x - room.wallsInfo.countOfWallsLeft, (int)room.entryPoint.y - room.wallsInfo.countOfWallsDown, 10);

            wallsTilemap.SetTile(positionOfRightUpperTileINT, tile[3]);
            wallsTilemap.SetTile(positionOfRightDownTileINT, tile[5]);
            wallsTilemap.SetTile(positionOfLeftUpperTileINT, tile[1]);
            wallsTilemap.SetTile(positionOfLeftDownTileINT, tile[7]);

            BuildingData.AddTileToTileListData(positionOfRightUpperTileINT, tile[3], ObjectsLayers.Walls);
            BuildingData.AddTileToTileListData(positionOfRightDownTileINT, tile[5], ObjectsLayers.Walls);
            BuildingData.AddTileToTileListData(positionOfLeftUpperTileINT, tile[1], ObjectsLayers.Walls);
            BuildingData.AddTileToTileListData(positionOfLeftDownTileINT, tile[7], ObjectsLayers.Walls);

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
                    backgroundWalls.SetTile(new Vector3Int(i, j, 10), tile[8]);
                    BuildingData.AddTileToTileListData(new Vector3Int(i, j, 10), tile[8], ObjectsLayers.BackgroundWalls);
                    buildingGenerator.AddPlaceToOccupiedPlaces(new Vector2(i, j));
                }
            }
        }

        public void MakeEntrance(Room room)
        {
            wallsTilemap.SetTile(new Vector3Int((int)room.entryPoint.x, (int)room.entryPoint.y, 10), null);

            RemoveTile(new Vector3Int((int)room.entryPoint.x, (int)room.entryPoint.y, 10), backgroundWalls);
            backgroundWalls.SetTile(new Vector3Int((int)room.entryPoint.x, (int)room.entryPoint.y, 10), lampTile);
        }

        public void CreateLadderPathToNextRoom(Vector2 beginPos, Room room, int roomFloorY)
        {
            int beginX = (int)beginPos.x;
            int beginY = (int)beginPos.y;

            Tile[] tile = room.GetTiles();

            Vector3Int firstLadderposition = new Vector3Int(beginX, beginY, 10);
            ladderTilemap.SetTile(firstLadderposition, tile[16]);
            BuildingData.ladder.Add(beginPos);
            BuildingData.AddTileToTileListData(firstLadderposition, tile[16], ObjectsLayers.Ladder);

            for (int y = beginY - 1; y > roomFloorY + 1; y--)
            {
                Vector3Int ladderposition = new Vector3Int(beginX, y, 10);
                ladderTilemap.SetTile(ladderposition, tile[17]);

                BuildingData.ladder.Add(new Vector2(beginX, y));
                BuildingData.AddTileToTileListData(ladderposition, tile[17], ObjectsLayers.Ladder);
            }

            Vector3Int lastLadderposition = new Vector3Int(beginX, roomFloorY + 1, 10);
            ladderTilemap.SetTile(lastLadderposition, tile[18]);
            BuildingData.ladder.Add(new Vector2(beginX, roomFloorY + 1));
            BuildingData.AddTileToTileListData(lastLadderposition, tile[18], ObjectsLayers.Ladder);
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
                RemoveTile(new Vector3Int(i, upperY, 10), wallsTilemap);
                RemoveTile(new Vector3Int(i, bottomY, 10), wallsTilemap);

                wallsTilemap.SetTile(new Vector3Int(i, upperY, 10), tiles[2]);
                wallsTilemap.SetTile(new Vector3Int(i, bottomY, 10), tiles[6]);

                BuildingData.AddTileToTileListData(new Vector3Int(i, upperY, 10), tiles[2], ObjectsLayers.Walls);
                BuildingData.AddTileToTileListData(new Vector3Int(i, bottomY, 10), tiles[6], ObjectsLayers.Walls);
            }
            for (int i = bottomY; i <= upperY; i++)
            {
                RemoveTile(new Vector3Int(leftX, i, 10), wallsTilemap);
                RemoveTile(new Vector3Int(rightX, i, 10), wallsTilemap);

                wallsTilemap.SetTile(new Vector3Int(leftX, i, 10), tiles[0]);
                wallsTilemap.SetTile(new Vector3Int(rightX, i, 10), tiles[4]);

                BuildingData.AddTileToTileListData(new Vector3Int(leftX, i, 10), tiles[0], ObjectsLayers.Walls);
                BuildingData.AddTileToTileListData(new Vector3Int(rightX, i, 10), tiles[4], ObjectsLayers.Walls);
            }

            RemoveTile(new Vector3Int(leftX, upperY, 10), wallsTilemap);
            RemoveTile(new Vector3Int(rightX, upperY, 10), wallsTilemap);
            RemoveTile(new Vector3Int(rightX, bottomY, 10), wallsTilemap);
            RemoveTile(new Vector3Int(leftX, bottomY, 10), wallsTilemap);

            wallsTilemap.SetTile(new Vector3Int(leftX, upperY, 10), tiles[1]);
            wallsTilemap.SetTile(new Vector3Int(rightX, upperY, 10), tiles[3]);
            wallsTilemap.SetTile(new Vector3Int(rightX, bottomY, 10), tiles[5]);
            wallsTilemap.SetTile(new Vector3Int(leftX, bottomY, 10), tiles[7]);

            BuildingData.AddTileToTileListData(new Vector3Int(leftX, upperY, 10), tiles[1], ObjectsLayers.Walls);
            BuildingData.AddTileToTileListData(new Vector3Int(rightX, upperY, 10), tiles[3], ObjectsLayers.Walls);
            BuildingData.AddTileToTileListData(new Vector3Int(rightX, bottomY, 10), tiles[5], ObjectsLayers.Walls);
            BuildingData.AddTileToTileListData(new Vector3Int(leftX, bottomY, 10), tiles[7], ObjectsLayers.Walls);

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
                    RemoveTile(new Vector3Int(i, j, 10), backgroundWalls);

                    backgroundWalls.SetTile(new Vector3Int(i, j, 10), tile[8]);
                    BuildingData.AddTileToTileListData(new Vector3Int(i, j, 10), tile[8], ObjectsLayers.BackgroundWalls);
                }
            }
        }

        #endregion
    }
}
