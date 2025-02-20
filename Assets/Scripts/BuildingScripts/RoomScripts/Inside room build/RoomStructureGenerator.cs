using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build
{
    public class RoomStructureGenerator
    {
        protected System.Random rand;

        public RoomStructureGenerator(System.Random random)
        {
            rand = random;
        }

        public virtual void Generate(Room room)
        {
            InnerRoomsCreator innerRoomsCreator = new InnerRoomsCreator(room, rand);
            innerRoomsCreator.CreateInnerRooms();

            CreatePlatformsOfWalls();
        }

        

        public void CreatePlatformsOfWalls()
        {

        }
    }
}
