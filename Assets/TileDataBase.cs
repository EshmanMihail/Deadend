using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileDataBase : MonoBehaviour
{
    [SerializeField] private Tile[] tiles;

    public int GetTileIndex(Tile tile)
    {
        int index = 0;

        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] == tile)
            {
                index = i; 
                break;
            }
        }

        return index;
    }

    public Tile GetTileByIndex(int index)
    {
        return tiles[index];
    }
}
