using Assets.Scripts.BuildingScripts.BuildingTypes;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build.Inner_rooms.InnerRoomStructs
{
    public class LadderPathBuilder
    {
        public static void MakePathToRoom(Vector2 leftWall, Vector2 rightWall, int roomFloorY, Room room, System.Random rand)
        {
            int entryX = rand.Next((int)leftWall.x + 1, (int)rightWall.x - 1);
            Vector2 entryToInnerRoom = new Vector2(entryX, roomFloorY);

            SetLadder(entryToInnerRoom, room);
        }

        private static void SetLadder(Vector2 entry, Room room)
        {
            Tile[] roomTiles = room.GetTiles();

            int roomFloorY = (int)room.entryPoint.y - room.wallsInfo.countOfWallsDown;

            room.tileSetter.RemoveWall(new Vector3Int((int)entry.x, (int)entry.y, 10));
            room.tileSetter.SetTile(roomTiles[16], (int)entry.x, (int)entry.y, ObjectsLayers.Ladder);
            BuildingData.ladder.Add(entry);

            for (int y = (int)entry.y - 1; y > roomFloorY + 1; y--)
            {
                room.tileSetter.SetTile(roomTiles[17], (int)entry.x, y, ObjectsLayers.Ladder);
                BuildingData.ladder.Add(new Vector2((int)entry.x, y));
            }

            room.tileSetter.SetTile(roomTiles[18], (int)entry.x, roomFloorY + 1, ObjectsLayers.Ladder);
            BuildingData.ladder.Add(entry);
        }
    }
}
