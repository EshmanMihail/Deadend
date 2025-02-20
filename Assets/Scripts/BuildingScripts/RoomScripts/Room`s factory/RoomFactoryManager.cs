using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Room_s_factory
{
    public class RoomFactoryManager
    {
        private Dictionary<RoomBiom, IRoomFactory> factories;

        public RoomFactoryManager(Dictionary<RoomBiom, IRoomFactory> factories)
        {
            this.factories = factories;
        }

        public Room CreateRoom(Vector2 entryPoint, RoomType roomType, RoomWallsInfo wallsInfo, RoomBiom roomBiom)
        {
            if (!factories.ContainsKey(roomBiom))
                throw new ArgumentException($"No factory found for biom: {roomBiom}");

            var factory = factories[roomBiom];
            var room = factory.CreateRoom(entryPoint, roomType, wallsInfo);
            factory.ConfigureRoom(room);
            return room;
        }
    }
}
