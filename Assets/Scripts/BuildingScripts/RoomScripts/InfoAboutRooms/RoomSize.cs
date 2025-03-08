using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.BuildingScripts
{
    public static class RoomSizes
    {
        public static int MinCountWallsRightLength { get; } = 15;
        public static int MaxCountWallsRightLength { get; } = 20;

        public static int MinCountWallsLeftLength { get; } = 15;
        public static int MaxCountWallsLeftLength { get; } = 20;

        public static int MinCountWallsUpLength { get; } = 5;
        public static int MaxCountWallsUpLength { get; } = 10;
        public static int MinCountWallsUpInUpperRoom { get; } = 10;
        public static int MaxCountWallsUpInUpperRoom { get; } = 20;

        public static int MinCountWallsDownLength { get; } = 5;
        public static int MaxCountWallsDownLength { get; } = 10;
        public static int MinCountWallsDownInBottomRoom { get; } = 10;
        public static int MaxCountWallsDownInBottomRoom { get; } = 20;
    }
}
