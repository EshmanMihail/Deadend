using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build
{
    public interface IRoomStructure
    {
        void Generate(Room room);

        void SetChancesForStructures(int chanceToCreateInnerRooms, int chanceToCreateWallsPlatforms);
    }
}
