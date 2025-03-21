using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_spawn
{
    public class GrassRoomObjectPlacer : IRoomObjectPlacer
    {
        System.Random rand;

        public GrassRoomObjectPlacer(System.Random rand)
        {
            this.rand = rand;
        }

        public void SetRoomObjects(Room room)
        {
            SpawnGrassOnRoomFloor(room);
        }

        private void SpawnGrassOnRoomFloor(Room room)
        {
            Tile[] tile = room.GetTiles();
            Tile[] grass = { tile[20], tile[21], tile[22], tile[23], tile[24], tile[25], tile[26], tile[27] };

            int leftX = (int)room.entryPoint.x - room.wallsInfo.countOfWallsLeft;
            int rightX = (int)room.entryPoint.x + room.wallsInfo.countOfWallsRight;
            int roomFloorY = (int)room.entryPoint.y - room.wallsInfo.countOfWallsDown;

            for (int x = leftX + 1; x < rightX; x++)
            {
                int randIndex = rand.Next(0, grass.Length);
                room.tileSetter.SetTile(grass[randIndex], x, roomFloorY + 1, ObjectsLayers.FrontObjects);
            }
        }
    }
}
