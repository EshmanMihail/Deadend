using Assets.Scripts.BuildingScripts.BuildingTypes;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject node;

    public void SpawnNodes()
    {
        for (int i = 0; i < BuildingData.node.Count; i++)
        {
            Instantiate(node, BuildingData.node[i], Quaternion.identity);
        }
    }
}
