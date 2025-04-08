using Assets.Scripts.BuildingScripts.BuildingTypes;
using Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_spawn
{
    public class MetalRoomObjectPlacer : RoomObjectPlacer
    {
        public MetalRoomObjectPlacer(System.Random rand) : base(rand) { }

        public override void SetRoomObjects(Room room)
        {
            SetPositionsAndObjectsOfRoom(room);
            base.SetRoomObjects(room);
        }
    }
}
