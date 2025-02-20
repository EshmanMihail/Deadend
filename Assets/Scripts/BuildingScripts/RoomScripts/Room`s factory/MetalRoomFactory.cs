using Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Room_s_factory
{
    public class MetalRoomFactory : IRoomFactory
    {
        private RoomStructureGenerator structureGenerator;
        private Tile[] tiles;
        private GameObject[] objects;
        private TilesSetter tilesSetter;

        public MetalRoomFactory(RoomStructureGenerator structureGenerator, Tile[] tiles, GameObject[] objects, TilesSetter tilesSetter)
        {
            this.structureGenerator = structureGenerator;
            this.tiles = tiles;
            this.objects = objects;
            this.tilesSetter = tilesSetter;
        }

        public Room CreateRoom(Vector2 entryPoint, RoomType roomType, RoomWallsInfo wallsInfo)
        {
            return new MetalRoom(entryPoint, roomType, wallsInfo, RoomBiom.metal);
        }

        public void ConfigureRoom(Room room)
        {
            room.SetStructureGenerator(structureGenerator);
            room.SetTilesAndTileSetter(tiles, tilesSetter);
            room.SetGameObjects(objects);
        }
    }
}
