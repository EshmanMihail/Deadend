using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build.Inner_rooms
{
    public abstract class InnerRoom
    {
        protected Room room;
        protected System.Random rand;
        protected Tile[] roomTiles;
        protected List<Vector2> ocupiedPlaces;
        protected List<Vector2> laderPlaces;

        public InnerRoom(Room room, System.Random rand, Tile[] roomTiles, List<Vector2> ocupiedPlaces, List<Vector2> laderPlaces)
        {
            this.room = room;
            this.rand = rand;
            this.roomTiles = roomTiles;
            this.ocupiedPlaces = ocupiedPlaces;
            this.laderPlaces = laderPlaces;
        }

        public abstract void CraeteRoom();

        protected bool IsInnerRoomCanExist(int countOfWallsLeft, int countOfWallsRight, int countOfWallsDown, int countOfWallsUp)
        {
            if ((countOfWallsLeft + countOfWallsRight) * (countOfWallsDown + countOfWallsUp) < 4)
                return false;

            return true;
        }
    }
}
