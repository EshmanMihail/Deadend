using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build
{
    public interface IRoomStructure
    {
        void Generate(Room room);

        void SetChancesForStructures(int chanceToCreateInnerRooms, int chanceToCreateWallsPlatforms);

        List<Vector2> GetPlacesToSetObjects();
    }
}
