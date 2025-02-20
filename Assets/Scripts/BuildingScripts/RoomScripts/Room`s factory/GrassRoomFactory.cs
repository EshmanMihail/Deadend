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

        public Room CreateRoom(Vector2 entryPoint, RoomType roomType, RoomWallsInfo wallsInfo)
        {
            return new GrassRoom(entryPoint, roomType, wallsInfo, RoomBiom.grass);
        }

        public void ConfigureRoom(Room room)
        {
            room.SetTilesAndTileSetter(tiles, tilesSetter);
            room.SetGameObjects(objects);
        }
    }
}
