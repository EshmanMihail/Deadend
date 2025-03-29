using Assets.Scripts.BuildingScripts;
using Assets.Scripts.BuildingScripts.BuildingTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DoorSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] doors;

    public void Spawn()
    {
        var doorList = BuildingData.door;
        foreach(var door in doorList)
        {
            Vector2 positionToSpawn = new Vector2(door.Item1.x + 0.5f, door.Item1.y + 0.5f);
            switch (door.Item2)
            {
                case RoomBiom.metal:
                    UnityEngine.Object.Instantiate(doors[0], positionToSpawn, Quaternion.identity);
                    break;
                case RoomBiom.grass:
                    UnityEngine.Object.Instantiate(doors[1], positionToSpawn, Quaternion.identity);
                    break;
                case RoomBiom.frozen:
                    UnityEngine.Object.Instantiate(doors[2], positionToSpawn, Quaternion.identity);
                    break;
            }
        }
    }
}
