using System;
using UnityEngine;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Room_s_factory
{
    public interface IRoomFactory
    {
        Room CreateRoom(Vector2 entryPoint, RoomType roomType, RoomWallsInfo wallsInfo);

        void ConfigureRoom(Room room);
    }
}
