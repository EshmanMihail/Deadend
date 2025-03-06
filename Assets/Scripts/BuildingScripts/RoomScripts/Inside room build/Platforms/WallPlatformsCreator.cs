using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build.Platforms
{
    public class WallPlatformsCreator
    {
        private Room room;
        private Tile[] roomTiles;
        private System.Random rand;
        private List<Vector2> occupiedPlaces;

        private List<Vector2> wallPlaces = new List<Vector2>();

        private enum PlatformType
        {
            LeftSide, RightSide
        }

        public WallPlatformsCreator(Room room, System.Random random, List<Vector2> occupiedPlaces)
        {
            this.room = room;
            this.rand = random;
            this.occupiedPlaces = occupiedPlaces;
            
            roomTiles = room.GetTiles();
        }

        public List<Vector2> CreatePlatfroms()
        {
            LeftSideWallPlatform leftSideWallPlatform = new LeftSideWallPlatform(room, rand, occupiedPlaces);
            List<Vector2> wallsFromLeftPlatforms = leftSideWallPlatform.CreatePlatformsOnLeftSide();

            for (int i = 0; i < wallsFromLeftPlatforms.Count; i++)
            {
                wallPlaces.Add(wallsFromLeftPlatforms[i]);
            }

            RightSideWallPlatfrom rightSideWallPlatfrom = new RightSideWallPlatfrom(room, rand, occupiedPlaces);
            List<Vector2> wallsFromRightPlatforms = rightSideWallPlatfrom.CreatePlatformsOnRightSide();

            for (int i = 0; i < wallsFromRightPlatforms.Count; i++)
            {
                wallPlaces.Add(wallsFromRightPlatforms[i]);
            }

            return wallPlaces;
        }
    }
}
