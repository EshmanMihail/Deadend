using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.BuildingScripts.BuildingTypes
{
    public static class BuildingData
    {
        public static List<TileDataInfo> mapData = new List<TileDataInfo>();

        public static List<Vector2> freePlaces = new List<Vector2>();

        public static List<Vector2> nodes = new List<Vector2>();
    }
}
