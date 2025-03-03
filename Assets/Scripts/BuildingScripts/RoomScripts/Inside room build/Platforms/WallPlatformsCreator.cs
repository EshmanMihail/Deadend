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

        public void CreatePlatfroms()
        {
            LeftSideWallPlatform leftSideWallPlatform = new LeftSideWallPlatform(room, rand, occupiedPlaces);
            leftSideWallPlatform.CreatePlatformsOnLeftSide();

            RightSideWallPlatfrom rightSideWallPlatfrom = new RightSideWallPlatfrom(room, rand, occupiedPlaces);
            rightSideWallPlatfrom.CreatePlatformsOnRightSide();
        }
    }
}
