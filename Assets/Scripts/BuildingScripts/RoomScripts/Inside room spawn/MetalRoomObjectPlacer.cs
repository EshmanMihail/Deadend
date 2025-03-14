using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_spawn
{
    public class MetalRoomObjectPlacer : IRoomObjectPlacer
    {
        System.Random rand;

        public MetalRoomObjectPlacer(System.Random rand)
        {
            this.rand = rand;
        }

        public void SetRoomObjects(Room room)
        {
            
        }
    }
}
