using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            
        }
    }
}
