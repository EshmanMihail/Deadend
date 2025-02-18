using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Room_s_factory
{
    public class MetalRoomFactory : IRoomFactory
    {
        private Tile[] tiles;
        private GameObject[] objects;
        private TilesSetter tilesSetter;

        public MetalRoomFactory(Tile[] tiles, GameObject[] objects, TilesSetter tilesSetter)
        {
            this.tiles = tiles;
            this.objects = objects;
            this.tilesSetter = tilesSetter;
        }

        public Room CreateRoom(Vector2 entryPoint, RoomType roomType, int countOfWallsUp, int countOfWallsDown, int countOfWallsLeft, int countOfWallsRight)
        {
            return new MetalRoom(entryPoint, roomType, countOfWallsUp, countOfWallsDown, countOfWallsLeft, countOfWallsRight, RoomBiom.metal);
        }

        public void ConfigureRoom(Room room)
        {
            room.GetTilesAndTileSetter(tiles, tilesSetter);
            room.GetGameObjects(objects);
        }
    }
}
