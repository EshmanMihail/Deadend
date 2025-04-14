using Assets.Scripts.BuildingScripts;
using Assets.Scripts.BuildingScripts.BuildingTypes;
using Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BackgroundObjectPlacerScript : MonoBehaviour
{
    [SerializeField] private Tile[] pipeTiles;
    [SerializeField] private Tile[] pipeTilesDark;

    [SerializeField] private Tile[] metalBackObjectsTiles;
    [SerializeField] private Tile[] grassBackObjectsTiles;
    [SerializeField] private Tile[] frozenBackObjectsTiles;

    [SerializeField] private GameObject[] backObjects;

    [SerializeField] private int chanceToSpawn = 5;
    [SerializeField] private int chaceToSpawnPipe = 10;
    [SerializeField] private int chanceToRotatePipe = 10;

    private List<Vector2> occupiedPostions;
    private List<Vector2> occupiedPositionsByPipes;

    public void Spawn(List<Room> rooms, System.Random rand)
    {
        occupiedPostions = new();
        occupiedPositionsByPipes = new();

        for (int i = 0; i < rooms.Count; i++)
        {
            SpawnTiles(rooms[i], rand);
            SpawnObjects(rooms[i], rand);
        }
    }

    private void SpawnTiles(Room room, System.Random rand)
    {
        Tile[] tiles = DetermineTileArray(room.roomBiom);

        int floorY = (int)room.GetRightBottomAngle().y;
        int ceilingY = (int)room.GetLeftUpperAngle().y;
        int leftX = (int)room.GetLeftUpperAngle().x;
        int rightX = (int)room.GetRightBottomAngle().x;

        List<PositionProperty> positions = room.GetPositionsToPlaceObjects();

        for (int i = 0; i < positions.Count; i++)
        {
            int x = positions[i].X;
            int y = positions[i].Y;
            if (!IsLampPosition(new Vector2(x, y)) && rand.Next(0, 100) <= chanceToSpawn)
            {
                int index = rand.Next(0, tiles.Length);

                int randY = y + rand.Next(1, positions[i].height);

                room.tileSetter.SetTile(tiles[index], x, randY, ObjectsLayers.BackwardObjects);

                occupiedPostions.Add(new Vector2(x, randY));
            }
        }

        //for (int y = floorY + 1; y < ceilingY; y++)
        //{
        //    for (int x = leftX + 1; x < rightX; x++)
        //    {
        //        if (!IsLampPosition(new Vector2(x, y)) && rand.Next(0, 100) <= chanceToSpawn)
        //        {
        //            int index = rand.Next(0, tiles.Length);

        //            room.tileSetter.SetTile(tiles[index], x, y, ObjectsLayers.BackwardObjects);

        //            occupiedPostions.Add(new Vector2(x, y));
        //        }
        //    }
        //}
        SpawnPipes(room, rand);
    }

    private Tile[] DetermineTileArray(RoomBiom roomBiom)
    {
        if (roomBiom == RoomBiom.grass) return grassBackObjectsTiles;
        else if (roomBiom == RoomBiom.frozen) return frozenBackObjectsTiles;

        return metalBackObjectsTiles;
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

    private void SpawnPipes(Room room, System.Random rand)
    {
        int floorY = (int)room.GetRightBottomAngle().y;
        int ceilingY = (int)room.GetLeftUpperAngle().y;
        int leftX = (int)room.GetLeftUpperAngle().x;
        int rightX = (int)room.GetRightBottomAngle().x;

        Tile[] tiles = DeterminePipeArray(room);
 
        for (int y = floorY + 2; y < ceilingY; y++)
        {
            if (rand.Next(0, 100) <= chaceToSpawnPipe)
            {
                SpawnHorizontalPipe(leftX + 1, rightX - 1, y, room, rand, tiles);
            }
        }

        for (int x = leftX + 1; x < rightX; x++)
        {
            if (x != (int)room.entryPoint.x && !BuildingData.ladder.Contains(new Vector2(x, floorY)) && rand.Next(0, 100) <= chaceToSpawnPipe)
            {
                SpawnVerticalPipe(x, floorY + 1, ceilingY - 1, room, rand, tiles);
            }
        }
    }

    private Tile[] DeterminePipeArray(Room room)
    {
        if ((int)room.entryPoint.y < -22) return pipeTilesDark;

        return pipeTiles;
    }
    
    private void SpawnHorizontalPipe(int startX, int endX, int y, Room room, System.Random rand, Tile[] tiles)
    {
        room.tileSetter.SetTile(tiles[10], startX - 1, y, ObjectsLayers.BackwardObjects);

        for (int x = startX; x <= endX; x++)
        {
            if (!occupiedPostions.Contains(new Vector2(x, y)))
            {
                Tile tile = tiles[0];
                if (rand.Next(0, 100) <= 2) tile = tiles[3];

                room.tileSetter.SetTile(tile, x, y, ObjectsLayers.BackwardObjects);
                occupiedPositionsByPipes.Add(new Vector2(x, y));
            }
        }

        room.tileSetter.SetTile(tiles[11], endX + 1, y, ObjectsLayers.BackwardObjects);
    }

    private void SpawnVerticalPipe(int x, int startY, int endY, Room room, System.Random rand, Tile[] tiles)
    {
        room.tileSetter.SetTile(tiles[9], x, startY - 1, ObjectsLayers.BackwardObjects);

        for (int y = startY; y <= endY; y++)
        {
            if (!occupiedPostions.Contains(new Vector2(x, y)))
            {
                Tile tile = tiles[1];
                if (rand.Next(0, 100) <= 3) tile = tiles[4];

                tile = DeterminePipeTile(x, y, tile, tiles);

                room.tileSetter.SetTile(tile, x, y, ObjectsLayers.BackwardObjects);
            }
        }
        room.tileSetter.SetTile(tiles[8], x, endY + 1, ObjectsLayers.BackwardObjects);
    }

    private Tile DeterminePipeTile(int x, int y, Tile currentTile, Tile[] tiles)
    {
        Tile tile = currentTile;
        if (occupiedPositionsByPipes.Contains(new Vector2(x - 1, y)) && occupiedPositionsByPipes.Contains(new Vector2(x + 1, y)))
        {
            tile = tiles[5];
        }
        else if (occupiedPositionsByPipes.Contains(new Vector2(x - 1, y)))
        {
            tile = tiles[7];
        }
        else if (occupiedPositionsByPipes.Contains(new Vector2(x + 1, y)))
        {
            tile = tiles[6];
        }
        return tile;
    }

    private void SpawnObjects(Room room, System.Random rand)
    {

    }
}
