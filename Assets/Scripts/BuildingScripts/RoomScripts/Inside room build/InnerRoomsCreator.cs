using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build
{
    public class InnerRoomsCreator
    {
        private Room room;

        private System.Random rand;

        private enum InnerRoomType
        {
            TopLeft = 0, TopRight = 1, BottomLeft = 2, BottomRight = 3
        }

        private List<Vector2> ocupiedPlaces;
        private List<Vector2> laderPlaces;

        private int chanceToSpawnRoom = 80;
        private int reduceChance = 20;

        public InnerRoomsCreator(Room room, System.Random random)
        {
            this.room = room;
            this.rand = random;
            ocupiedPlaces = new List<Vector2>();
            laderPlaces = new List<Vector2>();
        }

        public void CreateInnerRooms()
        {
            while (rand.Next(0, chanceToSpawnRoom) < chanceToSpawnRoom)
            {


                chanceToSpawnRoom -= reduceChance;
            }
        }

        private void CraeteLeftTopRoom()
        {

        }

        private void CraeteRightTopRoom()
        {

        }
        private void CraeteLeftBottomRoom()
        {

        }
        private void CraeteRightBottomRoom()
        {

        }
    }
}
