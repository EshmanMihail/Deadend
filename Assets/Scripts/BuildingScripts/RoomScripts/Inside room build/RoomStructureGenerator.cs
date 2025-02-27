using System;
using UnityEngine;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build
{
    public class RoomStructureGenerator : IRoomStructure
    {
        protected System.Random rand;

        public RoomStructureGenerator(System.Random random)
        {
            rand = random;
            
        }

        public virtual void Generate(Room room)
        {
            if (room.wallsInfo.countOfWallsDown + room.wallsInfo.countOfWallsUp > 5)
            {
                InnerRoomsCreator innerRoomsCreator = new InnerRoomsCreator(room, rand);
                innerRoomsCreator.CreateInnerRooms();
            }

            CreatePlatformsOfWalls();
        }

        

        public void CreatePlatformsOfWalls()
        {

        }
    }
}
