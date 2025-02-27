using Assets.Scripts.BuildingScripts.BuildingTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.EventSystems.EventTrigger;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build.Inner_rooms
{
    public abstract class InnerRoom
    {
        protected Room room;
        protected System.Random rand;
        protected Tile[] roomTiles;
        protected List<Vector2> ocupiedPlaces;

        public InnerRoom(Room room, System.Random rand, Tile[] roomTiles, List<Vector2> ocupiedPlaces)
        {
            this.room = room;
            this.rand = rand;
            this.roomTiles = roomTiles;
            this.ocupiedPlaces = ocupiedPlaces;
        }

        public abstract void CraeteRoom();

        protected bool IsInnerRoomCanExist(int countOfWallsLeft, int countOfWallsRight, int countOfWallsDown, int countOfWallsUp)
        {
            if ((countOfWallsLeft + countOfWallsRight) * (countOfWallsDown + countOfWallsUp) < 4)
                return false;

            return true;
        }

        protected bool IsOnLadderPosition(int x, int y)
        {
            Vector2 position = new Vector2(x, y);
            return BuildingData.ladder.Contains(position);
        }

        protected void AddTileToMapData(int x, int y, Tile tile, int layer)
        {
            Vector3Int position = new Vector3Int(x, y, 10);
            BuildingData.AddTileToTileListData(position, tile, layer);
        }
    }
}
