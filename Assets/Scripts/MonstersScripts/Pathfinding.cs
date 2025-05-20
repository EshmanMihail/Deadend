using Assets.Scripts.BuildingScripts.BuildingTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MonstersScripts
{
    public class Pathfinding
    {
        public static List<Vector2> FindPath(Vector2 start, Vector2 goal)
        {
            Vector2 startNode = FindClosestNode(start);
            Vector2 goalNode = FindClosestNode(goal);

            if (startNode == goalNode)
                return new List<Vector2> { startNode };

            Queue<Vector2> queue = new();
            queue.Enqueue(startNode);

            HashSet<Vector2> visited = new();
            visited.Add(startNode);

            Dictionary<Vector2, Vector2> cameFrom = new();

            while (queue.Count > 0)
            {
                Vector2 current = queue.Dequeue();

                if (current == goalNode)
                    return ReconstructPath(cameFrom, current);

                foreach (Vector2 neighbor in GetNeighbors(current))
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                        cameFrom[neighbor] = current;
                    }
                }
            }

            return new List<Vector2>();
        }

        private static Vector2 FindClosestNode(Vector2 point)
        {
            return BuildingData.node.OrderBy(node => Vector2.Distance(node, point)).FirstOrDefault();
        }

        private static List<Vector2> GetNeighbors(Vector2 node)
        {
            float maxDistance = 1f;
            return BuildingData.node
                .Where(n => n != node && Vector2.Distance(n, node) <= maxDistance)
                .ToList();
        }

        private static List<Vector2> ReconstructPath(Dictionary<Vector2, Vector2> cameFrom, Vector2 current)
        {
            List<Vector2> path = new() { current };
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Add(current);
            }
            path.Reverse();
            return path;
        }
    }
}
