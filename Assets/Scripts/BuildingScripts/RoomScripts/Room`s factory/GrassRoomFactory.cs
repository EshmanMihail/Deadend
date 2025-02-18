using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Room_s_factory
{
    public class GrassRoomFactory : IRoomFactory
    {
        private Tile[] tiles;
        private GameObject[] objects;
        private TilesSetter tilesSetter;

        public GrassRoomFactory(Tile[] tiles, GameObject[] objects, TilesSetter tilesSetter)
        {
            this.tiles = tiles;
            this.objects = objects;
            this.tilesSetter = tilesSetter;
        }

        public Room CreateRoom(Vector2 entryPoint, RoomType roomType, int countOfWallsUp, int countOfWallsDown, int countOfWallsLeft, int countOfWallsRight)
        {
            return new GrassRoom(entryPoint, roomType, countOfWallsUp, countOfWallsDown, countOfWallsLeft, countOfWallsRight, RoomBiom.grass);
        }

        public void ConfigureRoom(Room room)
        {
            room.GetTilesAndTileSetter(tiles, tilesSetter);
            room.GetGameObjects(objects);
        }
    }
}
