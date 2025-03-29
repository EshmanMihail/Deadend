using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build
{
    public struct PositionProperty
    {
        public int X;
        public int Y;

        public int widthRight;
        public int height;
    }

    public static class PositionPropertyCreator
    {
        public static PositionProperty Create(int x, int y, int widthRight, int height)
        {
            PositionProperty property = new PositionProperty
            {
                X = x,
                Y = y,
                widthRight = widthRight,
                height = height
            };

            return property;
        }
    }
}