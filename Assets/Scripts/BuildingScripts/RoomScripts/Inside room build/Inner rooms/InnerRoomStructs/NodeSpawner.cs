using Assets.Scripts.BuildingScripts.BuildingTypes;
using UnityEngine;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build.Inner_rooms.InnerRoomStructs
{
    public class NodeSpawner
    {
        public static void SpawnNode(int x, int y)
        {
            BuildingData.node.Add(new Vector2(x, y));
        }
    }
}
