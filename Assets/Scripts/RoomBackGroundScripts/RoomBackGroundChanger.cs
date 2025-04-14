using Assets.Scripts.BuildingScripts;
using Assets.Scripts.BuildingScripts.BuildingTypes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomBackGroundChanger : MonoBehaviour
{
    [SerializeField] private TilePair[] metalRoomTiles;
    [SerializeField] private TilePair[] grassRoomTiles;
    [SerializeField] private TilePair[] frozenRoomTiles;

    public void ChangeBackGround(List<Room> rooms)
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            ChangeRoomBackground(rooms[i], RoomTilesIndex(rooms[i]));
        }
    }

    private int RoomTilesIndex(Room room)
    {
        int index = 0;

        if (room.roomBiom == RoomBiom.grass) index = 1;
        else if (room.roomBiom == RoomBiom.frozen) index = 2;

        return index;
    }

    private void ChangeRoomBackground(Room room, int tilesIndex)
    {
        int entryRoomY = (int)room.entryPoint.y;
        TilePair[] tiles = GetBackTile(tilesIndex);

        if (entryRoomY > -22)
        {
            ChangeBackground(room, tiles[0].backTile, tiles[0].backLampTile);
        }
        else if (entryRoomY < -22 && entryRoomY > -62)
        {
            ChangeBackground(room, tiles[1].backTile, tiles[1].backLampTile);
        }
        else
        {
            ChangeBackground(room, tiles[2].backTile, tiles[2].backLampTile);
        }
    }

    private TilePair[] GetBackTile(int index)
    {
        TilePair[] tilePair = metalRoomTiles;

        if (index == 1) tilePair = grassRoomTiles;
        else if (index == 2) tilePair = frozenRoomTiles;

        return tilePair;
    }

    private void ChangeBackground(Room room, Tile back, Tile lamp)
    {
        int floorY = (int)room.GetRightBottomAngle().y;
        int ceilingY = (int)room.GetLeftUpperAngle().y;
        int leftX = (int)room.GetLeftUpperAngle().x;
        int rightX = (int)room.GetRightBottomAngle().x;

        for (int y = floorY; y <= ceilingY; y++)
        {
            for (int x = leftX; x <= rightX; x++)
            {
                if (!IsLampPosition(new Vector2(x, y)))
                {
                    room.tileSetter.SetTile(back, x, y, ObjectsLayers.BackgroundWalls);
                }
                else
                {
                    room.tileSetter.SetTile(lamp, x, y, ObjectsLayers.BackgroundWalls);
                }
            }
        }
    }

    private bool IsLampPosition(Vector2 tilePosition)
    {
        bool resalt = false;
        for (int i = 0; i < BuildingData.lamp.Count; i++)
        {
            if (tilePosition == BuildingData.lamp[i].Item1)
            {
                resalt = true;
                break;
            }
        }
        return resalt;
    }
}

[System.Serializable]
public class TilePair
{
    public Tile backTile;
    public Tile backLampTile;
}
