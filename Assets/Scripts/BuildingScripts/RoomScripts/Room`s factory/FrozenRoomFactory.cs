using Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Room_s_factory
{
    public class FrozenRoomFactory : IRoomFactory
    {
        private Tile[] tiles;
        private GameObject[] objects;
        private TilesSetter tilesSetter;
        private System.Random rand;

        public FrozenRoomFactory(Tile[] tiles, GameObject[] objects, TilesSetter tilesSetter, System.Random rand)
        {
            this.tiles = tiles;
            this.objects = objects;
            this.tilesSetter = tilesSetter;
            this.rand = rand;
        }

        public Room CreateRoom(Vector2 entryPoint, RoomType roomType, RoomWallsInfo wallsInfo)
        {
            return new FrozenRoom(entryPoint, roomType, wallsInfo, RoomBiom.frozen, new RoomStructureGenerator(rand));
        }

        public void ConfigureRoom(Room room)
        {
            room.SetTilesAndTileSetter(tiles, tilesSetter);
            room.SetGameObjects(objects);
        }
    }
}
