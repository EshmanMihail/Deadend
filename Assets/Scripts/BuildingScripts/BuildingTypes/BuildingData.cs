using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.BuildingScripts.BuildingTypes
{
    public static class BuildingData
    {
        public static List<TileDataInfo> mapData = new List<TileDataInfo>();

        public static List<Vector2> freePlace = new List<Vector2>();

        public static List<Vector2> node = new List<Vector2>();

        public static List<Vector2> ladder = new List<Vector2>();

        public static List<Vector2> lamp = new List<Vector2>();


        public static void AddTileToTileListData(Vector3Int position, Tile tile, int tileLayer)
        {
            TileDataInfo tileData = new TileDataInfo
            {
                position = position,
                tile = tile,
                tileLayer = tileLayer
            };
            mapData.Add(tileData);
        }

        public static void RemoveTileFromTileListData(Vector3Int positionToRemove)
        {
            for (int i = 0; i < mapData.Count; ++i)
            {
                if (mapData[i].position == positionToRemove)
                {
                    mapData.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
