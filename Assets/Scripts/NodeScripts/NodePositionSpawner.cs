using Assets.Scripts.BuildingScripts.BuildingTypes;
using UnityEngine;

namespace Assets.Scripts.NodeScripts
{
    public static class NodePositionSpawner
    {
        private static int stepX = 2;

        public static void SpawnNodes(int startX, int endX, int y)
        {
            for (int x = startX; x <= endX; x += stepX)
            {
                if (!BuildingData.node.Contains(new Vector2(x, y)))
                {
                    BuildingData.node.Add(new Vector2(x + 0.5f, y + 0.5f));
                }
            }
        }
    }
}
