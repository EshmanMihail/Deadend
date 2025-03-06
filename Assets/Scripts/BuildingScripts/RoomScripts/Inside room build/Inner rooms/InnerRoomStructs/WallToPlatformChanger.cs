using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build.Inner_rooms
{
    public class WallToPlatformChanger
    {
        public static List<Vector2> MakeWallToPlatform(Room room, int leftX, int rightX, int y, System.Random rand, int chanceToMakePlatform)
        {
            List<Vector2> platformsPositions = new List<Vector2>();
            Tile platformTile = room.GetTiles()[19];

            for (int x = leftX; x < rightX; x++)
            {
                if (rand.Next(0, 100) < chanceToMakePlatform)
                {
                    platformsPositions.Add(new Vector2(x, y));

                    room.tileSetter.RemoveWall(new Vector3Int(x, y, 10));
                    room.tileSetter.SetPlatfromTile(platformTile, x, y);
                }
            }

            return platformsPositions;
        }
    }
}
