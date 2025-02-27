using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using Assets.Scripts.BuildingScripts;
using UnityEngine.Rendering.Universal;
using Assets.Scripts.BuildingScripts.BuildingTypes;
using System.Linq;
using Assets.Scripts.BuildingScripts.RoomScripts;
using Assets.Scripts.BuildingScripts.RoomScripts.Room_s_factory;
using Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_build;


public class BuildingGenerator : NetworkBehaviour
{
    private Building building;
    private TilesSetter tilesSetter;
    private RoomFactoryManager roomFactoryManager;

    private System.Random rand;
    private bool isGenerationEnd;
    [SerializeField] private Tilemap wallsTilemap;
    [SerializeField] private Tilemap backgroundWalls;
    [SerializeField] private Tilemap ladder;

    [SerializeField] private Tile wallMetalTile;
    [SerializeField] private Tile lampTile;
    [SerializeField] private Tile startTile;

    [SerializeField] private Tile[] metalRoomTiles = new Tile[16];
    [SerializeField] private GameObject[] metalRoomObjects = new GameObject[1];

    [SerializeField] private Tile[] grassRoomTiles = new Tile[11];
    [SerializeField] private GameObject[] grassRoomObjects = new GameObject[1];


    [SerializeField] private Tile[] frozenRoomTiles = new Tile[11];
    [SerializeField] private GameObject[] frozenRoomObjects = new GameObject[1];


    [SerializeField] private int chanceToCheckToGenerateNextPathes = 80;
    [SerializeField] private int chanceToCheckToStopGenerate = 80;
    [SerializeField] private int chanceToSpawnNextRoom = 100;
    [SerializeField] private int minusChanceToSpawnNextRoom = 1;

    private int roomCount = 0;
    [SerializeField] private int maxRoomCount = 20;
    private List<Room> roomList = new List<Room>(); 

    private List<Vector2> occupiedPlaces = new List<Vector2>();

    void Start()
    {
        rand = new System.Random(Guid.NewGuid().GetHashCode());
        tilesSetter = new TilesSetter(this, wallsTilemap, backgroundWalls, ladder, metalRoomTiles, lampTile);
        InitializeRoomFactories();

        isGenerationEnd = false;

        if (isServer)
        {
            Debug.Log("server start generating");
            Vector2 startPositionOfGeneration = new Vector2(0, 0);
            RoomType firstRoomType = RoomType.Right;

            building = DetermineBuildingType();
            GenerateBuilding(startPositionOfGeneration, firstRoomType);
            isGenerationEnd = true;
            Debug.Log(roomCount.ToString());

            Debug.Log("server end generation");
        }
    }

    private Building DetermineBuildingType()
    {
        Array values = Enum.GetValues(typeof(BuildingTypeStructure));
        int randomIndex = rand.Next(values.Length);
        BuildingTypeStructure randomStructure = (BuildingTypeStructure)values.GetValue(randomIndex);

        Debug.Log(randomStructure.ToString());
        randomStructure = 0;
        if (randomStructure == 0) return new NormalBuilding();
        return new RandedBuilding();
    }

    private void InitializeRoomFactories()
    {
        var factories = new Dictionary<RoomBiom, IRoomFactory>
        {
            { RoomBiom.metal, new MetalRoomFactory(new RoomStructureGenerator(rand), metalRoomTiles, metalRoomObjects, tilesSetter) },
            { RoomBiom.grass, new GrassRoomFactory(new RoomStructureGenerator(rand), grassRoomTiles, grassRoomObjects, tilesSetter) },
            { RoomBiom.frozen, new FrozenRoomFactory(new RoomStructureGenerator(rand), frozenRoomTiles, frozenRoomObjects, tilesSetter) }
        };

        roomFactoryManager = new RoomFactoryManager(factories);

        Debug.Log("");
    }

    private void GenerateBuilding(Vector2 startPosition, RoomType roomType)
    {
        GenerateRoom(roomType, startPosition, chanceToSpawnNextRoom);
        CreateRoomStructure();
        SpawnRoomsBioms();
        //StartCoroutine(GenerateRooms());
    }

    //IEnumerator GenerateRooms()
    //{
    //    for (int i = 0; i < roomList.Count; i++)
    //    {
    //        if (roomList[i].roomBiom != RoomBiom.metal)
    //        {
    //            MakeRoomHerBiom(roomList[i]);
    //            yield return StartCoroutine(PauseAndMakeBiom(roomList[i], i));
    //        }
    //    }
    //}
    //IEnumerator PauseAndMakeBiom(Room room, int i)
    //{
    //    yield return new WaitForSeconds(2f); // Пауза на 2 секунды
    //    MakeRoomHerBiom(room);
    //}


    private Room GenerateRoom(RoomType roomType, Vector2 entryPoint, double chanceToSpawnNextRoom)
    {
        if (roomCount >= maxRoomCount) return null;
        if (rand.Next(0, chanceToCheckToStopGenerate) > chanceToSpawnNextRoom) return null;

        Room room = CreateRoom(roomType, entryPoint);

        if (!IsRoomCanExsite(room)) return null;

        tilesSetter.SetRoomTiles(room);
        roomList.Add(room);

        roomCount++;

        bool isHavePathToUpperRoom = false, isHavePathToLowerRoom = false, isHavePathToRightRoom = false, isHavePathToLeftRoom = false;
        DetermineNextRoomPaths(roomType, ref isHavePathToUpperRoom, ref isHavePathToLowerRoom, ref isHavePathToRightRoom, ref isHavePathToLeftRoom);
        chanceToSpawnNextRoom -= minusChanceToSpawnNextRoom;

        RandomizeNextRoomsPathes(entryPoint, room, chanceToSpawnNextRoom,
            isHavePathToUpperRoom, isHavePathToLowerRoom, isHavePathToRightRoom, isHavePathToLeftRoom);

        CreatePathToUpperForBottomRoom(room);

        return room;
    }

    private Room CreateRoom(RoomType roomType, Vector2 entryPoint)
    {
        int countOfWallsUp = 0, countOfWallsDown = 0, countOfWallsLeft = 0, countOfWallsRight = 0;
        building.GenerateRoomsSize(roomType, rand, ref countOfWallsUp, ref countOfWallsDown, ref countOfWallsLeft, ref countOfWallsRight);

        Vector2 entryPointAfterCorrection = DetermineEntryPointForCorrection(roomType, entryPoint);
        CorrectionOfRoomSize(entryPointAfterCorrection, ref countOfWallsUp, ref countOfWallsDown, ref countOfWallsLeft, ref countOfWallsRight);

        RoomWallsInfo roomWalls = new RoomWallsInfo
        {
            countOfWallsUp = countOfWallsUp,
            countOfWallsDown = countOfWallsDown,
            countOfWallsLeft = countOfWallsLeft,
            countOfWallsRight = countOfWallsRight
        };

        Room room = DetermineRoomBiom(entryPoint, roomType, roomWalls);

        return room;
    }

    #region Room scale correction
    private Vector2 DetermineEntryPointForCorrection(RoomType roomType, Vector2 entryPoint)
    {
        Vector2 vecForCorrection = new Vector2(0, 0);

        if (roomType == RoomType.Upper) vecForCorrection = new Vector2(entryPoint.x, entryPoint.y + 1);

        if (roomType == RoomType.Bottom) vecForCorrection = new Vector2(entryPoint.x, entryPoint.y - 1);

        if (roomType == RoomType.Right) vecForCorrection = new Vector2(entryPoint.x + 1, entryPoint.y);

        if (roomType == RoomType.Left) vecForCorrection = new Vector2(entryPoint.x - 1, entryPoint.y);

        return vecForCorrection;
    }

    private void CorrectionOfRoomSize(Vector2 entryPoint, ref int countOfWallsUp, ref int countOfWallsDown, ref int countOfWallsLeft, ref int countOfWallsRight)
    {
        CorrectOnTheUpper(entryPoint, ref countOfWallsUp, countOfWallsLeft, countOfWallsRight);

        CorrectOnTheDown(entryPoint, ref countOfWallsDown, countOfWallsLeft, countOfWallsRight);

        CorrectOnTheRight(entryPoint, ref countOfWallsRight, countOfWallsUp, countOfWallsDown);

        CorrectOnTheLeft(entryPoint, ref countOfWallsLeft, countOfWallsUp, countOfWallsDown);
    }

    private void CorrectOnTheUpper(Vector2 entryPoint, ref int countOfWallsUp, int countOfWallsLeft, int countOfWallsRight)
    {
        int positionOfLeftWall = (int)(entryPoint.x - countOfWallsLeft);
        int positionOfRightWall = (int)(entryPoint.x + countOfWallsRight);
        int positionOfFloor = (int)entryPoint.y;
        int positionOfCeiling = (int)(entryPoint.y + countOfWallsUp);

        int minPossibleHeightY = (int)(entryPoint.y + countOfWallsUp);

        for (int i = positionOfLeftWall; i <= positionOfRightWall; i++)
        {
            for(int j = positionOfFloor; j <= positionOfCeiling; j++)
            {
                Vector2 position = new Vector2(i, j);

                if (occupiedPlaces.Contains(position))
                {
                    if (minPossibleHeightY > j)
                    {
                        minPossibleHeightY = j;
                        //Debug.Log("up x y min = " + position.x.ToString() + " " + position.y.ToString() + " " + minPossibleHeightY.ToString());
                    }
                }
            }
        }
        countOfWallsUp = Math.Abs(minPossibleHeightY - (int)entryPoint.y);
    }

    private void CorrectOnTheDown(Vector2 entryPoint, ref int countOfWallsDown, int countOfWallsLeft, int countOfWallsRight)
    {
        int positionOfLeftWall = (int)(entryPoint.x - countOfWallsLeft);
        int positionOfRightWall = (int)(entryPoint.x + countOfWallsRight);
        int positionOfCeiling = (int)entryPoint.y;
        int positionOfFloor = (int)(entryPoint.y - countOfWallsDown);

        int minPossibleHeightDownY = (int)(entryPoint.y - countOfWallsDown);

        for (int i = positionOfLeftWall; i <= positionOfRightWall; i++)
        {
            for (int j = positionOfCeiling; j >= positionOfFloor; j--) 
            {
                Vector2 position = new Vector2(i, j);

                if (occupiedPlaces.Contains(position))
                {
                    if (minPossibleHeightDownY < j)
                    {
                        minPossibleHeightDownY = j;
                        //Debug.Log("down x y min = " + position.x.ToString() + " " + position.y.ToString() + " " + minPossibleHeightDownY.ToString());
                    }
                }
            }
        }
        countOfWallsDown = Math.Abs(minPossibleHeightDownY - (int)entryPoint.y);
    }

    private void CorrectOnTheRight(Vector2 entryPoint, ref int countOfWallsRight, int countOfWallsUp, int countOfWallsDown)
    {
        int positionOfRightWall = (int)(entryPoint.x + countOfWallsRight);
        int positionOfCeiling = (int)(entryPoint.y + countOfWallsUp);
        int positionOfFloor = (int)(entryPoint.y - countOfWallsDown);
        int positionOfEntryPoint = (int)entryPoint.x;

        int minPossibleWidthRight = (int)(entryPoint.x + countOfWallsRight);

        for (int i = positionOfFloor + 1; i <= positionOfCeiling - 1; i++)
        {
            for (int j = positionOfEntryPoint; j <= positionOfRightWall; j++)
            {
                Vector2 position = new Vector2(j, i);

                if (occupiedPlaces.Contains(position))
                {
                    if (minPossibleWidthRight > j)
                    {
                        minPossibleWidthRight = j;
                    }
                }
            }
        }
        countOfWallsRight = Math.Abs(minPossibleWidthRight - (int)entryPoint.x);
    }

    private void CorrectOnTheLeft(Vector2 entryPoint, ref int countOfWallsLeft, int countOfWallsUp, int countOfWallsDown)
    {
        int positionOfLeftWall = (int)(entryPoint.x + countOfWallsLeft);
        int positionOfCeiling = (int)(entryPoint.y + countOfWallsUp);
        int positionOfFloor = (int)(entryPoint.y - countOfWallsDown);
        int positionOfEntryPoint = (int)entryPoint.x;

        int minPossibleWidthLeft = (int)(entryPoint.x - countOfWallsLeft);

        for (int i = positionOfFloor; i <= positionOfCeiling; i++)
        {
            for (int j = positionOfEntryPoint; j >= positionOfLeftWall; j--)
            {
                Vector2 position = new Vector2(j, i);

                if (occupiedPlaces.Contains(position))
                {
                    if (minPossibleWidthLeft < j)
                    {
                        minPossibleWidthLeft = j;
                    }
                }
            }
        }
        countOfWallsLeft = Math.Abs((int)entryPoint.x - minPossibleWidthLeft);
    }
    #endregion

    private bool IsRoomCanExsite(Room room)
    {
        if ((room.wallsInfo.countOfWallsRight + room.wallsInfo.countOfWallsLeft) * (room.wallsInfo.countOfWallsUp + room.wallsInfo.countOfWallsDown) < 16) return false;
        if (room.wallsInfo.countOfWallsUp + room.wallsInfo.countOfWallsDown == 1) return false;
        return true;
    }

    private Room DetermineRoomBiom(Vector2 entryPoint, RoomType roomType, RoomWallsInfo wallsInfo)
    {
        Array values = Enum.GetValues(typeof(RoomBiom));
        int randomIndex = rand.Next(values.Length);
        RoomBiom randomBiom = (RoomBiom)values.GetValue(randomIndex);

        //randomBiom = RoomBiom.metal;

        return roomFactoryManager.CreateRoom(entryPoint, roomType, wallsInfo, randomBiom);
    }

    private void DetermineNextRoomPaths(RoomType roomType, ref bool upperRoom, ref bool downRoom, ref bool rightRoom, ref bool leftRoom)
    {
        if (rand.Next(0, chanceToCheckToGenerateNextPathes) <= chanceToSpawnNextRoom) upperRoom = true;
        if (rand.Next(0, chanceToCheckToGenerateNextPathes) <= chanceToSpawnNextRoom) rightRoom = true;
        if (rand.Next(0, chanceToCheckToGenerateNextPathes) <= chanceToSpawnNextRoom) leftRoom = true;
        if (rand.Next(0, chanceToCheckToGenerateNextPathes) <= chanceToSpawnNextRoom) downRoom = true;

        if (roomType == RoomType.Upper) downRoom = false;
        if (roomType == RoomType.Bottom) upperRoom = false;
        if (roomType == RoomType.Right) leftRoom = false;
        if (roomType == RoomType.Left) rightRoom = false;
    }

    private void RandomizeNextRoomsPathes(Vector2 entryPoint, Room room, double chanceToSpawnNextRoom,
        bool isHavePathToUpperRoom, bool isHavePathToLowerRoom, bool isHavePathToRightRoom, bool isHavePathToLeftRoom)
    {
        var actions = new (bool condition, Action action)[]
        {
            (isHavePathToUpperRoom, () => GeneratePathToNextRoomAndCreateNextRoom(entryPoint, RoomType.Upper, room, chanceToSpawnNextRoom)),
            (isHavePathToLowerRoom, () => GeneratePathToNextRoomAndCreateNextRoom(entryPoint, RoomType.Bottom, room, chanceToSpawnNextRoom)),
            (isHavePathToLeftRoom, () => GeneratePathToNextRoomAndCreateNextRoom(entryPoint, RoomType.Left, room, chanceToSpawnNextRoom)),
            (isHavePathToRightRoom, () => GeneratePathToNextRoomAndCreateNextRoom(entryPoint, RoomType.Right, room, chanceToSpawnNextRoom)),
        };

        var shuffledActions = actions.OrderBy(_ => rand.Next()).ToList();

        foreach (var (condition, action) in shuffledActions)
        {
            if (condition)
            {
                action();
            }
        }
    }

    private void GeneratePathToNextRoomAndCreateNextRoom(Vector2 entryPoint, RoomType NextRoomType, Room room, double chanceToSpawnNextRoom)
    {
        Vector2 nextRoomEntryPoint = building.GeneratePathToNextRoom(entryPoint, NextRoomType, room, rand);
        //StartCoroutine(PauseAndExecute(NextRoomType, nextRoomEntryPoint, chanceToSpawnNextRoom));
        Room nextRoom = GenerateRoom(NextRoomType, nextRoomEntryPoint, chanceToSpawnNextRoom);

        if (nextRoom != null && nextRoom.roomType == RoomType.Upper)
        {
            tilesSetter.CreateLadderPathToNextRoom(nextRoom.entryPoint, room, (int)(room.entryPoint.y - room.wallsInfo.countOfWallsDown));
        }
    }

    private void CreatePathToUpperForBottomRoom(Room room)
    {
        if (room.roomType == RoomType.Bottom)
        {
            tilesSetter.CreateLadderPathToNextRoom(room.entryPoint, room, (int)(room.entryPoint.y - room.wallsInfo.countOfWallsDown));
        }
    }

    IEnumerator PauseAndExecute(RoomType roomtype, Vector2 nextRoomEntryPoint, double chanceToSpawnNextRoom)
    {
        yield return new WaitForSeconds(5f); // Пауза на 2 секунды
        //Debug.Log("Next room");
        GenerateRoom(roomtype, nextRoomEntryPoint, chanceToSpawnNextRoom);
    }

    public void AddPlaceToOccupiedPlaces(Vector2 position)
    {
        occupiedPlaces.Add(position);
    }

    #region Make rooms bioms

    private void SpawnRoomsBioms()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].roomBiom != RoomBiom.metal)
            {
                tilesSetter.MakeRoomHerBiom(roomList[i]);
            }
        }

        MakeEntranceForMetalRooms();
    }

    private void MakeEntranceForMetalRooms()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].roomBiom == RoomBiom.metal)
            {
                tilesSetter.MakeEntrance(roomList[i]);
            }
        }
    }

    private void CreateRoomStructure()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            /*if (roomList[i].roomBiom == RoomBiom.metal)*/ roomList[i].GenerateRoomStructure();
        }
    }
    #endregion
}