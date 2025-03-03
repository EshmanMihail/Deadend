using Assets.Scripts.BuildingScripts.BuildingTypes;
using UnityEngine;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build.Inner_rooms.InnerRoomStructs
{
    public class FreePositionsCollector
    {
        public static void CollectFreePositions(int leftX, int rightX, int freePositionY)
        {
            for (int x = leftX + 1; x < rightX; x++)
            {
                BuildingData.freePlace.Add(new Vector2(x, freePositionY));
            }
        }
    }
}
