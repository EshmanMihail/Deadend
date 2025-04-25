using Assets.Scripts.BuildingScripts;
using Assets.Scripts.BuildingScripts.BuildingTypes;
using Mirror;
using UnityEngine;

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
                    GameObject spawnedDoor0 = UnityEngine.Object.Instantiate(doors[0], positionToSpawn, Quaternion.identity);
                    NetworkServer.Spawn(spawnedDoor0);
                    break;
                case RoomBiom.grass:
                    GameObject spawnedDoor1 = UnityEngine.Object.Instantiate(doors[1], positionToSpawn, Quaternion.identity);
                    NetworkServer.Spawn(spawnedDoor1);
                    break;
                case RoomBiom.frozen:
                    GameObject spawnedDoor2 = UnityEngine.Object.Instantiate(doors[2], positionToSpawn, Quaternion.identity);
                    NetworkServer.Spawn(spawnedDoor2);
                    break;
            }
        }
    }
}
