using Assets.Scripts.BuildingScripts.BuildingTypes;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build.Inner_rooms.InnerRoomStructs
{
    public class LampsSpawner
    {
        public static void SpawnLamps(Vector2 leftWall, Vector2 rightWall, int roomFloorY, int roomCeilingY, Room room, System.Random rand)
        {
            if ((int)(rightWall.x - leftWall.x) <= 9)
            {
                SpawnLampInCenter(leftWall, rightWall, roomFloorY, roomCeilingY, room);
            }
            else
            {
                SpawnTwoLamps(leftWall, rightWall, roomFloorY, roomCeilingY, room);
            }
        }

        private static void SpawnLampInCenter(Vector2 leftWall, Vector2 rightWall, int roomFloorY, int roomCeilingY, Room room)
        {
            Tile[] roomTiles = room.GetTiles();

            int centerX = ((int)(rightWall.x + leftWall.x)) / 2;
            int y = roomCeilingY - 1;

            if (roomCeilingY - roomFloorY >= 6) y = (roomCeilingY + roomFloorY) / 2;

            room.tileSetter.SetTile(roomTiles[9], centerX, y, ObjectsLayers.BackgroundWalls);
            BuildingData.lamp.Add(new Vector2(centerX, y));
        }

        private static void SpawnTwoLamps(Vector2 leftWall, Vector2 rightWall, int roomFloorY, int roomCeilingY, Room room)
        {
            Tile[] roomTiles = room.GetTiles();

            int centerX = ((int)(rightWall.x + leftWall.x)) / 2;

            int x1 = (centerX + (int)rightWall.x) / 2;
            int y1 = roomCeilingY - 1;
            if (roomCeilingY - roomFloorY >= 6) y1 = (roomCeilingY + roomFloorY) / 2;

            int x2 = ((int)leftWall.x + centerX) / 2;
            int y2 = roomCeilingY - 1;
            if (roomCeilingY - roomFloorY >= 6) y2 = (roomCeilingY + roomFloorY) / 2;

            room.tileSetter.SetTile(roomTiles[9], x1, y1, ObjectsLayers.BackgroundWalls);
            BuildingData.lamp.Add(new Vector2(x1, y1));

            room.tileSetter.SetTile(roomTiles[9], x2, y2, ObjectsLayers.BackgroundWalls);
            BuildingData.lamp.Add(new Vector2(x2, y2));
        }
    }
}
