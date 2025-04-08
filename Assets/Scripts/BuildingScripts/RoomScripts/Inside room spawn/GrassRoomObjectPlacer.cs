using Assets.Scripts.BuildingScripts.BuildingTypes;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_spawn
{
    public class GrassRoomObjectPlacer : RoomObjectPlacer
    {
        public GrassRoomObjectPlacer(Random rand) : base(rand) { }

        public override void SetRoomObjects(Room room)
        {
            SetPositionsAndObjectsOfRoom(room);
            SpawnGrassOnFloor(room);
            base.SetRoomObjects(room);
        }

        private void SpawnGrassOnFloor(Room room)
        {
            Tile[] tile = room.GetTiles();

            Tile[] grass = { tile[20], tile[21], tile[22] };
            for (int i = 0; i < positions.Count; i++)
            {
                int randIndex = rand.Next(0, grass.Length);
                room.tileSetter.SetTile(grass[randIndex], positions[i].X, positions[i].Y, ObjectsLayers.FrontObjects);
            }

            int leftX = (int)room.entryPoint.x - room.wallsInfo.countOfWallsLeft;
            int rightX = (int)room.entryPoint.x + room.wallsInfo.countOfWallsRight;
            int floorY = (int)room.entryPoint.y - room.wallsInfo.countOfWallsDown + 1;
            Tile[] grassAndBushes = { tile[20], tile[21], tile[22], tile[23], tile[24], tile[25], tile[26], tile[27] };
            for (int x = leftX + 1; x < rightX; x++)
            {
                int randIndex = rand.Next(0, grassAndBushes.Length);
                room.tileSetter.SetTile(grassAndBushes[randIndex], x, floorY, ObjectsLayers.FrontObjects);
            }
        }
    }
}
