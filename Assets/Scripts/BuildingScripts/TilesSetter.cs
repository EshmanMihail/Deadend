﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace Assets.Scripts.BuildingScripts
{
    public class TilesSetter
    {
        private BuildingGenerator buildingGenerator;
        private Tilemap wallsTilemap;
        private Tilemap backgroundWalls;
        private Tile[] metalRoomTiles;
        private Tile lampTile;

        public TilesSetter(BuildingGenerator buildingGenerator, Tilemap wallsTilemap, Tilemap backgroundWalls, Tile[] metalRoomTiles, Tile lampTile)
        {
            this.buildingGenerator = buildingGenerator;
            this.wallsTilemap = wallsTilemap;
            this.backgroundWalls = backgroundWalls;
            this.metalRoomTiles = metalRoomTiles;
            this.lampTile = lampTile;
        }

        private void RemoveTile(Vector3Int positionToRemove, Tilemap tilemap)
        {
            tilemap.SetTile(positionToRemove, null);
            buildingGenerator.RemoveTileFromTileListData(positionToRemove);
        }

        public void SetTile(Tile tile, int x, int y, int layer)
        {
            Vector3Int tilePosition = new Vector3Int(x, y, 10);
            if (layer == 0)
            {
                RemoveTile(tilePosition, wallsTilemap);
                wallsTilemap.SetTile(tilePosition, tile);

                buildingGenerator.AddTileToTileListData(tilePosition, tile, layer);
            }
            else if (layer == -1)
            {
                RemoveTile(tilePosition, backgroundWalls);
                backgroundWalls.SetTile(tilePosition, tile);

                buildingGenerator.AddTileToTileListData(tilePosition, tile, layer);
            }
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
            for (int i = 1; i < room.countOfWallsRight; i++)
            {
                Vector3Int tilePosition = new Vector3Int((int)room.entryPoint.x + i, (int)room.entryPoint.y + room.countOfWallsUp, 10);
                wallsTilemap.SetTile(tilePosition, tile[2]);

                buildingGenerator.AddTileToTileListData(tilePosition, tile[2], 0);
                buildingGenerator.AddPlaceToOccupiedPlaces(new Vector2((int)room.entryPoint.x + i, (int)room.entryPoint.y + room.countOfWallsUp));
            }
            // lower part
            for (int i = 1; i < room.countOfWallsRight; i++)
            {
                Vector3Int tilePosition = new Vector3Int((int)room.entryPoint.x + i, (int)room.entryPoint.y - room.countOfWallsDown, 10);
                wallsTilemap.SetTile(tilePosition, tile[6]);

                buildingGenerator.AddTileToTileListData(tilePosition, tile[6], 0);
                buildingGenerator.AddPlaceToOccupiedPlaces(new Vector2((int)room.entryPoint.x + i, (int)room.entryPoint.y - room.countOfWallsDown));

                AddPositionsOfFloor(new Vector2((int)room.entryPoint.x + i, (int)room.entryPoint.y - room.countOfWallsDown));
            }
        }

        private void SetTilesOnLeftPartOfRoom(Room room, Tile[] tile)
        {
            // upper part
            for (int i = 0; i < room.countOfWallsLeft; i++)
            {
                Vector3Int tilePosition = new Vector3Int((int)room.entryPoint.x - i, (int)room.entryPoint.y + room.countOfWallsUp, 10);
                wallsTilemap.SetTile(tilePosition, tile[2]);

                buildingGenerator.AddTileToTileListData(tilePosition, tile[2], 0);
                buildingGenerator.AddPlaceToOccupiedPlaces(new Vector2((int)room.entryPoint.x - i, (int)room.entryPoint.y + room.countOfWallsUp));
            }
            //lower part
            for (int i = 0; i < room.countOfWallsLeft; i++)
            {
                Vector3Int tilePosition = new Vector3Int((int)room.entryPoint.x - i, (int)room.entryPoint.y - room.countOfWallsDown, 10);
                wallsTilemap.SetTile(tilePosition, tile[6]);

                buildingGenerator.AddTileToTileListData(tilePosition, tile[6], 0);
                buildingGenerator.AddPlaceToOccupiedPlaces(new Vector2((int)room.entryPoint.x - i, (int)room.entryPoint.y - room.countOfWallsDown));

                AddPositionsOfFloor(new Vector2((int)room.entryPoint.x - i, (int)room.entryPoint.y - room.countOfWallsDown));
            }
        }

        private void AddPositionsOfFloor(Vector2 position)
        {
            buildingGenerator.AddWallPositionToListOfWallsPositions(position);
        }

        private void SetTileOfRightRoomWall(Room room, Tile[] tile)
        {
            // set up
            for (int i = 0; i < room.countOfWallsUp; i++)
            {
                Vector3Int tilePosition = new Vector3Int((int)room.entryPoint.x + room.countOfWallsRight, (int)room.entryPoint.y + i, 10);
                wallsTilemap.SetTile(tilePosition, tile[4]);

                buildingGenerator.AddTileToTileListData(tilePosition, tile[4], 0);
                buildingGenerator.AddPlaceToOccupiedPlaces(new Vector2((int)room.entryPoint.x + room.countOfWallsRight, (int)room.entryPoint.y + i));
            }
            // set down
            for (int i = 0; i < room.countOfWallsDown; i++)
            {
                Vector3Int tilePosition = new Vector3Int((int)room.entryPoint.x + room.countOfWallsRight, (int)room.entryPoint.y - i, 10);
                wallsTilemap.SetTile(tilePosition, tile[4]);

                buildingGenerator.AddTileToTileListData(tilePosition, tile[4], 0);
                buildingGenerator.AddPlaceToOccupiedPlaces(new Vector2((int)room.entryPoint.x + room.countOfWallsRight, (int)room.entryPoint.y - i));
            }
        }

        private void SetTileOfLeftRoomWall(Room room, Tile[] tile)
        {
            // set up
            for (int i = 0; i < room.countOfWallsUp; i++)
            {
                Vector3Int tilePosition = new Vector3Int((int)room.entryPoint.x - room.countOfWallsLeft, (int)room.entryPoint.y + i, 10);
                wallsTilemap.SetTile(tilePosition, tile[0]);

                buildingGenerator.AddTileToTileListData(tilePosition, tile[0], 0);
                buildingGenerator.AddPlaceToOccupiedPlaces(new Vector2((int)room.entryPoint.x - room.countOfWallsLeft, (int)room.entryPoint.y + i));
            }
            // set down
            for (int i = 0; i < room.countOfWallsDown; i++)
            {
                Vector3Int tilePosition = new Vector3Int((int)room.entryPoint.x - room.countOfWallsLeft, (int)room.entryPoint.y - i, 10);
                wallsTilemap.SetTile(tilePosition, tile[0]);

                buildingGenerator.AddTileToTileListData(tilePosition, tile[0], 0);
                buildingGenerator.AddPlaceToOccupiedPlaces(new Vector2((int)room.entryPoint.x - room.countOfWallsLeft, (int)room.entryPoint.y - i));
            }
        }

        private void SetAngleTiles(Room room, Tile[] tile)
        {
            Vector2 positionOfRightUpperTile = new Vector2(room.entryPoint.x + room.countOfWallsRight, room.entryPoint.y + room.countOfWallsUp);
            Vector2 positionOfRightDownTile = new Vector2(room.entryPoint.x + room.countOfWallsRight, room.entryPoint.y - room.countOfWallsDown);
            Vector2 positionOfLeftUpperTile = new Vector2(room.entryPoint.x - room.countOfWallsLeft, room.entryPoint.y + room.countOfWallsUp);
            Vector2 positionOfLeftDownTile = new Vector2(room.entryPoint.x - room.countOfWallsLeft, room.entryPoint.y - room.countOfWallsDown);

            Vector3Int positionOfRightUpperTileINT = new Vector3Int((int)room.entryPoint.x + room.countOfWallsRight, (int)room.entryPoint.y + room.countOfWallsUp, 10);
            Vector3Int positionOfRightDownTileINT = new Vector3Int((int)room.entryPoint.x + room.countOfWallsRight, (int)room.entryPoint.y - room.countOfWallsDown, 10);
            Vector3Int positionOfLeftUpperTileINT = new Vector3Int((int)room.entryPoint.x - room.countOfWallsLeft, (int)room.entryPoint.y + room.countOfWallsUp, 10);
            Vector3Int positionOfLeftDownTileINT = new Vector3Int((int)room.entryPoint.x - room.countOfWallsLeft, (int)room.entryPoint.y - room.countOfWallsDown, 10);

            wallsTilemap.SetTile(positionOfRightUpperTileINT, tile[3]);
            wallsTilemap.SetTile(positionOfRightDownTileINT, tile[5]);
            wallsTilemap.SetTile(positionOfLeftUpperTileINT, tile[1]);
            wallsTilemap.SetTile(positionOfLeftDownTileINT, tile[7]);

            buildingGenerator.AddTileToTileListData(positionOfRightUpperTileINT, tile[3], 0);
            buildingGenerator.AddTileToTileListData(positionOfRightDownTileINT, tile[5], 0);
            buildingGenerator.AddTileToTileListData(positionOfLeftUpperTileINT, tile[1], 0);
            buildingGenerator.AddTileToTileListData(positionOfLeftDownTileINT, tile[7], 0);

            buildingGenerator.AddPlaceToOccupiedPlaces(positionOfRightUpperTile);
            buildingGenerator.AddPlaceToOccupiedPlaces(positionOfRightDownTile);
            buildingGenerator.AddPlaceToOccupiedPlaces(positionOfLeftUpperTile);
            buildingGenerator.AddPlaceToOccupiedPlaces(positionOfLeftDownTile);
        }

        private void SetTilesInsideRoom(Room room, Tile[] tile)
        {
            int positionOfLeftWall = (int)(room.entryPoint.x - room.countOfWallsLeft);
            int positionOfRightWall = (int)(room.entryPoint.x + room.countOfWallsRight);
            int positionOfFloor = (int)room.entryPoint.y - room.countOfWallsDown;
            int positionOfCeiling = (int)room.entryPoint.y + room.countOfWallsUp;

            for (int i = positionOfLeftWall + 1; i < positionOfRightWall; i++)
            {
                for (int j = positionOfFloor + 1; j < positionOfCeiling; j++)
                {
                    backgroundWalls.SetTile(new Vector3Int(i, j, 10), tile[8]);
                    buildingGenerator.AddTileToTileListData(new Vector3Int(i, j, 10), tile[8], -1);
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

                buildingGenerator.AddTileToTileListData(new Vector3Int(i, upperY, 10), tiles[2], 0);
                buildingGenerator.AddTileToTileListData(new Vector3Int(i, bottomY, 10), tiles[6], 0);
            }
            for (int i = bottomY; i <= upperY; i++)
            {
                RemoveTile(new Vector3Int(leftX, i, 10), wallsTilemap);
                RemoveTile(new Vector3Int(rightX, i, 10), wallsTilemap);

                wallsTilemap.SetTile(new Vector3Int(leftX, i, 10), tiles[0]);
                wallsTilemap.SetTile(new Vector3Int(rightX, i, 10), tiles[4]);

                buildingGenerator.AddTileToTileListData(new Vector3Int(leftX, i, 10), tiles[0], 0);
                buildingGenerator.AddTileToTileListData(new Vector3Int(rightX, i, 10), tiles[4], 0);
            }

            RemoveTile(new Vector3Int(leftX, upperY, 10), wallsTilemap);
            RemoveTile(new Vector3Int(rightX, upperY, 10), wallsTilemap);
            RemoveTile(new Vector3Int(rightX, bottomY, 10), wallsTilemap);
            RemoveTile(new Vector3Int(leftX, bottomY, 10), wallsTilemap);

            wallsTilemap.SetTile(new Vector3Int(leftX, upperY, 10), tiles[1]);
            wallsTilemap.SetTile(new Vector3Int(rightX, upperY, 10), tiles[3]);
            wallsTilemap.SetTile(new Vector3Int(rightX, bottomY, 10), tiles[5]);
            wallsTilemap.SetTile(new Vector3Int(leftX, bottomY, 10), tiles[7]);

            buildingGenerator.AddTileToTileListData(new Vector3Int(leftX, upperY, 10), tiles[1], 0);
            buildingGenerator.AddTileToTileListData(new Vector3Int(rightX, upperY, 10), tiles[3], 0);
            buildingGenerator.AddTileToTileListData(new Vector3Int(rightX, bottomY, 10), tiles[5], 0);
            buildingGenerator.AddTileToTileListData(new Vector3Int(leftX, bottomY, 10), tiles[7], 0);

            SetTilesInsideRoomBiom(room, tiles);

            MakeEntrance(room);
        }

        private void SetTilesInsideRoomBiom(Room room, Tile[] tile)
        {
            int positionOfLeftWall = (int)(room.entryPoint.x - room.countOfWallsLeft);
            int positionOfRightWall = (int)(room.entryPoint.x + room.countOfWallsRight);
            int positionOfFloor = (int)room.entryPoint.y - room.countOfWallsDown;
            int positionOfCeiling = (int)room.entryPoint.y + room.countOfWallsUp;

            for (int i = positionOfLeftWall + 1; i < positionOfRightWall; i++)
            {
                for (int j = positionOfFloor + 1; j < positionOfCeiling; j++)
                {
                    RemoveTile(new Vector3Int(i, j, 10), backgroundWalls);

                    backgroundWalls.SetTile(new Vector3Int(i, j, 10), tile[8]);
                    buildingGenerator.AddTileToTileListData(new Vector3Int(i, j, 10), tile[8], -1);
                }
            }
        }

        #endregion
    }
}
