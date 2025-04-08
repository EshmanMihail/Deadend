using Assets.Scripts.BuildingScripts.BuildingTypes;
using Assets.Scripts.BuildingScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LampSpawner : MonoBehaviour
{
    [SerializeField] Light2D metalRoomLightLamp;
    [SerializeField] Light2D frozenRoomLightLamp;
    [SerializeField] Light2D grassRoomLightLamp;

    public void SpawnLamps()
    {
        Quaternion rotation = Quaternion.Euler(0, 0, 180);

        foreach (var l in BuildingData.lamp)
        {
            Vector2 postion = new Vector2(l.Item1.x + 0.5f, l.Item1.y + 0.5f);
            if (l.Item2 == RoomBiom.metal)
            {
                Instantiate(metalRoomLightLamp, postion, rotation);
            }
            if (l.Item2 == RoomBiom.frozen)
            {
                Instantiate(frozenRoomLightLamp, postion, rotation);
            }
            if (l.Item2 == RoomBiom.grass)
            {
                Instantiate(grassRoomLightLamp, postion, rotation);
            }
        }
    }
}
