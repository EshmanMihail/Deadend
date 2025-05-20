using Assets.Scripts.BuildingScripts.BuildingTypes;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject node;

    public void SpawnNodes()
    {
        SpawnNodesOnLadders();

        for (int i = 0; i < BuildingData.node.Count; i++)
        {
            BuildingData.node[i] += new Vector2 (0, -0.25f);
        }

        for (int i = 0; i < BuildingData.node.Count; i++)
        {
            Instantiate(node, BuildingData.node[i], Quaternion.identity);
        }
    }

    private void SpawnNodesOnLadders()
    {
        for (int i = 0; i < BuildingData.ladder.Count; i++)
        {
            if (!BuildingData.node.Contains(new Vector2(BuildingData.ladder[i].x + 0.5f, BuildingData.ladder[i].y + 0.5f)))
                BuildingData.node.Add(new Vector2(BuildingData.ladder[i].x + 0.5f, BuildingData.ladder[i].y + 0.5f));
        }
    }
}
