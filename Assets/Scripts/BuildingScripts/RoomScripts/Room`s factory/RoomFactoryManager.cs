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

        public Room CreateRoom(Vector2 entryPoint, RoomType roomType,
            int countOfWallsUp, int countOfWallsDown, int countOfWallsLeft, int countOfWallsRight, RoomBiom roomBiom)
        {
            if (!factories.ContainsKey(roomBiom))
                throw new ArgumentException($"No factory found for biom: {roomBiom}");

            var factory = factories[roomBiom];
            var room = factory.CreateRoom(entryPoint, roomType, countOfWallsUp, countOfWallsDown, countOfWallsLeft, countOfWallsRight);
            factory.ConfigureRoom(room);
            return room;
        }
    }
}
