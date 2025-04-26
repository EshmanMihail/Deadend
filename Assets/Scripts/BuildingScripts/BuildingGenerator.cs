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
using Unity.VisualScripting;


public class BuildingGenerator : NetworkBehaviour
{
    private Building building;
    private TilesSetter tilesSetter;
    private RoomFactoryManager roomFactoryManager;

    #region serializeFields
    private System.Random rand;
    [SerializeField] private Vector2 startPosition;

    [SerializeField] private GameObject networkTileSetterObject;
    private NetworkTileSetter networkTileSetter;
    [SerializeField] private Tilemap wallsTilemap;
    [SerializeField] private Tilemap backgroundWalls;
    [SerializeField] private Tilemap ladder;
    [SerializeField] private Tilemap platforms;
    [SerializeField] private Tilemap frontTiles;
    [SerializeField] private Tilemap backwardTiles;

    [SerializeField] private GameObject tileDataBase;
    [SerializeField] private GameObject buildingMainDoor;

    [SerializeField] private GameObject doorSpawner;
    [SerializeField] private GameObject lampSpawner;
    [SerializeField] private GameObject lootSpawner;
    [SerializeField] private GameObject nodeSpawner;
    [SerializeField] private GameObject roomBackgroundChanger;
    [SerializeField] private GameObject roomBackgroundObjectsPlacer;

    [SerializeField] private Tile[] metalRoomTiles = new Tile[16];
    [SerializeField] private GameObject[] metalRoomObjects = new GameObject[1];

    [SerializeField] private Tile[] grassRoomTiles = new Tile[11];
    [SerializeField] private GameObject[] grassRoomObjects = new GameObject[1];


    [SerializeField] private Tile[] frozenRoomTiles = new Tile[11];
    [SerializeField] private GameObject[] frozenRoomObjects = new GameObject[1];

    [SerializeField] private Tile backgroundTunnelTile;

    [SerializeField] private int chanceToCheckToGenerateNextPathes = 80;
    [SerializeField] private int chanceToCheckToStopGenerate = 80;
    [SerializeField] private int chanceToSpawnNextRoom = 100;
    [SerializeField] private int minusChanceToSpawnNextRoom = 1;
    #endregion

    private int roomCount = 0;
    [SerializeField] private int maxRoomCount = 20;
    private List<Room> roomList = new List<Room>();

    private List<Vector2> occupiedPlaces = new List<Vector2>();

    void Start()
    {
        rand = new System.Random(Guid.NewGuid().GetHashCode());
        networkTileSetter = networkTileSetterObject.GetComponent<NetworkTileSetter>();
        tilesSetter = new TilesSetter(this, wallsTilemap, backgroundWalls, ladder, platforms, frontTiles, backwardTiles, networkTileSetter, tileDataBase);
        InitializeRoomFactories();
        if (isServer)
        {
            RoomType firstRoomType = RoomType.Right;

            building = DetermineBuildingType();
            GenerateBuilding(startPosition, firstRoomType);

            networkTileSetter.CmdSendTilesInfo();
            networkTileSetter.CmdSetPlatformsTiles();
            networkTileSetter.CmdRotateTile();
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
            { RoomBiom.metal, new MetalRoomFactory(metalRoomTiles, metalRoomObjects, tilesSetter, rand) },
            { RoomBiom.grass, new GrassRoomFactory(grassRoomTiles, grassRoomObjects, tilesSetter, rand) },
            { RoomBiom.frozen, new FrozenRoomFactory(frozenRoomTiles, frozenRoomObjects, tilesSetter, rand) }
        };

        roomFactoryManager = new RoomFactoryManager(factories);
    }

    private void GenerateBuilding(Vector2 startPosition, RoomType roomType)
    {
        GenerateRoom(roomType, startPosition, chanceToSpawnNextRoom);

        CreateRoomStructure();

        CreatePassagesBetweenRooms();
        GenerateTunnels();

        lampSpawner.GetComponent<LampSpawner>().SpawnLamps();

        roomBackgroundChanger.GetComponent<RoomBackGroundChanger>().ChangeBackGround(roomList);

        AddEntryDoors();
        doorSpawner.GetComponent<DoorSpawner>().Spawn();

        AddPositionsForLootSpawn();
        lootSpawner.GetComponent<LootSpawner>().Spawn(rand);

        roomBackgroundObjectsPlacer.GetComponent<BackgroundObjectPlacerScript>().Spawn(roomList, rand);

        nodeSpawner.GetComponent<NodeSpawner>().SpawnNodes();

        CmdReplaceMainDoor(startPosition + new Vector2(0.5f, 0.5f));
    }

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
            for (int j = positionOfFloor; j <= positionOfCeiling; j++)
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

    public void AddPlaceToOccupiedPlaces(Vector2 position)
    {
        occupiedPlaces.Add(position);
    }

    #region Make rooms bioms and structure
    private void CreateRoomStructure()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            roomList[i].GenerateRoomStructure();
            roomList[i].SpawnRoomObjects();
        }
    }
    private void AddEntryDoors()
    {
        for (int i = 1; i < roomList.Count; i++)
        {
            if (roomList[i].roomType == RoomType.Right || roomList[i].roomType == RoomType.Left)
            {
                Vector2 position = new Vector2(roomList[i].entryPoint.x, roomList[i].entryPoint.y);
                BuildingData.door.Add((position, roomList[i].roomBiom));
            }
        }
    }

    private void AddPositionsForLootSpawn()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            roomList[i].SetPositionForLoot();
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdReplaceMainDoor(Vector2 newDoorPosition)
    {
        buildingMainDoor.transform.position = newDoorPosition;
        RpcReplaceMainDoor(newDoorPosition);
    }

    [ClientRpc]
    private void RpcReplaceMainDoor(Vector2 newDoorPosition)
    {
        buildingMainDoor.transform.position = newDoorPosition;
    }
    #endregion

    #region Create Pathes between rooms
    private void CreatePassagesBetweenRooms()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            Room roomA = roomList[i];
            for (int j = i + 1; j < roomList.Count; j++)
            {
                Room roomB = roomList[j];

                Vector2 roomALeftUpper = roomA.GetLeftUpperAngle();
                Vector2 roomARightBottom = roomA.GetRightBottomAngle();
                Vector2 roomBLeftUpper = roomB.GetLeftUpperAngle();
                Vector2 roomBRightBottom = roomB.GetRightBottomAngle();

                if (IsRectanglesIntersect(roomALeftUpper, roomARightBottom, roomBLeftUpper, roomBRightBottom))
                {
                    Vector2 intersectionLeftUpper = new Vector2(
                        Mathf.Max(roomALeftUpper.x, roomBLeftUpper.x),
                        Mathf.Min(roomALeftUpper.y, roomBLeftUpper.y)
                    );

                    Vector2 intersectionRightBottom = new Vector2(
                        Mathf.Min(roomARightBottom.x, roomBRightBottom.x),
                        Mathf.Max(roomARightBottom.y, roomBRightBottom.y)
                    );

                    float intersectionWidth = intersectionRightBottom.x - intersectionLeftUpper.x;
                    float intersectionHeight = intersectionLeftUpper.y - intersectionRightBottom.y;

                    //горизонтально
                    if (intersectionWidth > intersectionHeight)
                    {
                        CreateHorizontalPassageWithPlatform(intersectionLeftUpper, intersectionRightBottom, roomA, roomB);
                    }
                    else // вертикально
                    {
                        CreateVerticalPassage(intersectionLeftUpper, intersectionRightBottom, roomA, roomB);
                    }
                }
            }
        }
    }

    private void CreateHorizontalPassageWithPlatform(Vector2 intersectionLeftUpper, Vector2 intersectionRightBottom, Room roomA, Room roomB)
    {
        int passageY = Mathf.FloorToInt((intersectionLeftUpper.y + intersectionRightBottom.y) / 2);

        Tile[] tiles = roomA.GetTiles();

        List<int> positionsX = new List<int>();
        for (int x = Mathf.FloorToInt(intersectionLeftUpper.x) + 1; x < Mathf.FloorToInt(intersectionRightBottom.x); x++)
        {
            positionsX.Add(x);
        }

        if (positionsX.Count == 0) return;
        int countPassages = rand.Next(0, 3);

        for (int i = 0; i < countPassages; i++)
        {
            if (positionsX.Count == 0) break;

            int randIndex = rand.Next(0, positionsX.Count);

            roomA.tileSetter.RemoveWall(new Vector3Int(positionsX[randIndex], passageY, 10));
            roomB.tileSetter.RemoveWall(new Vector3Int(positionsX[randIndex], passageY, 10));
            roomA.tileSetter.SetPlatfromTile(tiles[19], positionsX[randIndex], passageY);

            positionsX.RemoveAt(randIndex);
        }
    }

    private void CreateVerticalPassage(Vector2 intersectionLeftUpper, Vector2 intersectionRightBottom, Room roomA, Room roomB)
    {
        List<PositionProperty> roomAPostionsToSpawn = roomA.GetPositionsToPlaceObjects();
        List<Vector2> postionsA = SetPositionsToVector(roomAPostionsToSpawn);

        List<PositionProperty> roomBPostionsToSpawn = roomB.GetPositionsToPlaceObjects();
        List<Vector2> positiosB = SetPositionsToVector(roomBPostionsToSpawn);

        for (int i = 0; i < postionsA.Count; i++)
        {
            bool f = true;
            if (positiosB.Contains(new Vector2(postionsA[i].x + 2, postionsA[i].y + 1)) || positiosB.Contains(new Vector2(postionsA[i].x + 3, postionsA[i].y + 1)))
            {
                roomA.tileSetter.RemoveWall(new Vector3Int((int)postionsA[i].x + 1, (int)postionsA[i].y + 1, 10));
                roomB.tileSetter.RemoveWall(new Vector3Int((int)postionsA[i].x + 1, (int)postionsA[i].y + 1, 10));
                f = false;
            }
            if (f && positiosB.Contains(new Vector2(postionsA[i].x + 2, postionsA[i].y)) || positiosB.Contains(new Vector2(postionsA[i].x + 3, postionsA[i].y)))
            {
                roomA.tileSetter.RemoveWall(new Vector3Int((int)postionsA[i].x + 1, (int)postionsA[i].y, 10));
                roomB.tileSetter.RemoveWall(new Vector3Int((int)postionsA[i].x + 1, (int)postionsA[i].y, 10));
            }
            if (f && positiosB.Contains(new Vector2(postionsA[i].x + 2, postionsA[i].y - 1)) || positiosB.Contains(new Vector2(postionsA[i].x + 3, postionsA[i].y - 1)))
            {
                roomA.tileSetter.RemoveWall(new Vector3Int((int)postionsA[i].x + 1, (int)postionsA[i].y, 10));
                roomB.tileSetter.RemoveWall(new Vector3Int((int)postionsA[i].x + 1, (int)postionsA[i].y, 10));
            }
        }
    }

    private List<Vector2> SetPositionsToVector(List<PositionProperty> roomAPostionsToSpawn)
    {
        List<Vector2> resalt = new();
        for (int i = 0; i < roomAPostionsToSpawn.Count; ++i)
        {
            resalt.Add(new Vector2(roomAPostionsToSpawn[i].X, roomAPostionsToSpawn[i].Y));
        }
        return resalt;
    }

    // Проверка пересечения двух прямоугольников
    private bool IsRectanglesIntersect(Vector2 leftUpperA, Vector2 rightBottomA, Vector2 leftUpperB, Vector2 rightBottomB)
    {
        return leftUpperA.x <= rightBottomB.x && rightBottomA.x >= leftUpperB.x &&
               leftUpperA.y >= rightBottomB.y && rightBottomA.y <= leftUpperB.y;
    }
    #endregion

    #region Make tunnels between rooms
    private List<Vector2> occupiedTunnelPositions;

    private void GenerateTunnels()
    {
        occupiedTunnelPositions = new List<Vector2>();
        for (int i = 0; i < roomList.Count; i++)
        {
            Room roomA = roomList[i];
            Vector2 roomAUpperLeft = roomA.GetLeftUpperAngle();
            Vector2 roomALowerRight = roomA.GetRightBottomAngle();

            for (int j = 0; j < roomList.Count; j++)
            {
                if (i == j) continue;

                Room roomB = roomList[j];
                Vector2 roomBUpperLeft = roomB.GetLeftUpperAngle();
                Vector2 roomBLowerRight = roomB.GetRightBottomAngle();

                if (IsRoomBelow(roomAUpperLeft, roomALowerRight, roomBUpperLeft, roomBLowerRight))
                {
                    CreateTunnel(roomA, roomB);
                }
                if (IsRoomRight(roomAUpperLeft, roomALowerRight, roomBUpperLeft, roomBLowerRight))
                {
                    CreateHorizontalTunnel(roomA, roomB);
                }
            }
        }
    }

    private bool IsRoomBelow(Vector2 upperLeftA, Vector2 lowerRightA, Vector2 upperLeftB, Vector2 lowerRightB)
    {
        bool horizontalOverlap = upperLeftA.x < lowerRightB.x && lowerRightA.x > upperLeftB.x;
        bool isBelow = upperLeftB.y < lowerRightA.y;

        return horizontalOverlap && isBelow;
    }

    private void CreateTunnel(Room roomA, Room roomB)
    {
        int tunnelStartX = Mathf.Max((int)roomA.GetLeftUpperAngle().x, (int)roomB.GetLeftUpperAngle().x);
        int tunnelEndX = Mathf.Min((int)roomA.GetRightBottomAngle().x, (int)roomB.GetRightBottomAngle().x);

        int tunnelStartY = (int)roomA.GetRightBottomAngle().y; // Нижняя точка roomA
        int tunnelEndY = (int)roomB.GetLeftUpperAngle().y;    // Верхняя точка roomB

        if (tunnelEndX - tunnelStartX < 3) return;

        int randPositionX = rand.Next(tunnelStartX + 1, tunnelEndX - 1);
        if (!IsTunnelCanBe(randPositionX, tunnelStartY, tunnelEndY)) return;

        //int tunnelLength = tunnelStartY - tunnelEndY;
        //bool isNeedLight = true;
        //if (tunnelLength <= 8) isNeedLight = false; 

        Tile[] tiles = roomA.GetTiles();

        roomA.tileSetter.RemoveWall(new Vector3Int(randPositionX, tunnelStartY, 10));
        roomA.tileSetter.SetTile(tiles[16], randPositionX, tunnelStartY, ObjectsLayers.Ladder);
        //int countToLight = 0;
        for (int y = tunnelStartY - 1; y >= tunnelEndY; y--)
        {
            if (!occupiedPlaces.Contains(new Vector2(randPositionX - 1, y)) && !occupiedTunnelPositions.Contains(new Vector2(randPositionX - 1, y)))
                roomA.tileSetter.SetTile(tiles[0], randPositionX - 1, y, ObjectsLayers.Walls);

            occupiedTunnelPositions.Add(new Vector2(randPositionX, y));
            roomA.tileSetter.RemoveWall(new Vector3Int(randPositionX, y, 10));
            roomA.tileSetter.SetTile(tiles[17], randPositionX, y, ObjectsLayers.Ladder);
            roomA.tileSetter.SetTile(backgroundTunnelTile, randPositionX, y, ObjectsLayers.BackgroundWalls);

            if (!occupiedPlaces.Contains(new Vector2(randPositionX + 1, y)) && !occupiedTunnelPositions.Contains(new Vector2(randPositionX + 1, y)))
                roomA.tileSetter.SetTile(tiles[4], randPositionX + 1, y, ObjectsLayers.Walls);

            //if (isNeedLight && countToLight >= 5)
            //{
            //    countToLight = 0;
            //    roomA.tileSetter.SetTile(tiles[9], randPositionX, y, ObjectsLayers.BackgroundWalls);
            //    BuildingData.lamp.Add((new Vector2(randPositionX, y), roomA.roomBiom));
            //}
            //countToLight++;
        }
        roomA.tileSetter.RemoveWall(new Vector3Int(randPositionX, tunnelEndY, 10));

        FinishBuildingLadderPath(roomB, randPositionX);
    }

    private void FinishBuildingLadderPath(Room room, int x)
    {
        List<PositionProperty> roomPostionsToSpawn = room.GetPositionsToPlaceObjects();
        List<Vector2> objectPostions = SetPositionsToVector(roomPostionsToSpawn);

        List<Vector2> occupiedPostions = room.GetOccupiedPlaces();

        int floorY = (int)room.GetRightBottomAngle().y;
        int ceilingY = (int)room.GetLeftUpperAngle().y;
        Tile[] tiles = room.GetTiles();

        bool f = true;
        int y = ceilingY - 1;
        for (y = ceilingY - 1; y > floorY + 1; y--)
        {
            if (objectPostions.Contains(new Vector2(x, y)))
            {
                room.tileSetter.SetTile(tiles[18], x, y, ObjectsLayers.Ladder);
                f = false;
                break;
            }
            else
            {
                room.tileSetter.RemoveWall(new Vector3Int(x, y, 10));
                room.tileSetter.SetTile(tiles[17], x, y, ObjectsLayers.Ladder);
            }
        }
        if (f) room.tileSetter.SetTile(tiles[18], x, floorY + 1, ObjectsLayers.Ladder);
    }

    private bool IsTunnelCanBe(int tunnelStartX, int tunnelStartY, int tunnelEndY)
    {
        bool resalt = true;
        for (int y = tunnelStartY - 1; y > tunnelEndY; y--)
        {
            if (occupiedPlaces.Contains(new Vector2(tunnelStartX, y)))
            {
                resalt = false;
                break;
            }
        }

        return resalt;
    }

    private void CreateHorizontalTunnel(Room roomA, Room roomB)
    {
        int startX = (int)roomA.GetRightBottomAngle().x; // Правая точка roomA
        int endX = (int)roomB.GetLeftUpperAngle().x;    // Левая точка roomB

        List<Vector2> positions = GetPositionsForRightTunnel(roomA, startX, roomB);
        if (positions.Count == 0) return;

        int randIndex = rand.Next(0, positions.Count);
        int tunnelY = (int)positions[randIndex].y;

        if (!IsRightTunnelCanBe(startX, endX, tunnelY)) return;

        Tile[] tiles = roomA.GetTiles();
        roomA.tileSetter.RemoveWall(new Vector3Int(startX, tunnelY, 10));
        for (int x = startX + 1; x < endX; x++)
        {
            if (!occupiedPlaces.Contains(new Vector2(x, tunnelY - 1)) && !occupiedTunnelPositions.Contains(new Vector2(x, tunnelY - 1)))
                roomA.tileSetter.SetTile(tiles[2], x, tunnelY - 1, ObjectsLayers.Walls);

            occupiedTunnelPositions.Add(new Vector2(x, tunnelY));
            roomA.tileSetter.RemoveWall(new Vector3Int(x, tunnelY, 10));
            roomA.tileSetter.SetTile(backgroundTunnelTile, x, tunnelY, ObjectsLayers.BackgroundWalls);

            if (!occupiedPlaces.Contains(new Vector2(x, tunnelY + 1)) && !occupiedTunnelPositions.Contains(new Vector2(x, tunnelY + 1)))
                roomA.tileSetter.SetTile(tiles[6], x, tunnelY + 1, ObjectsLayers.Walls);
        }
        roomA.tileSetter.RemoveWall(new Vector3Int(endX, tunnelY, 10));
        roomA.tileSetter.RemoveWall(new Vector3Int(endX + 1, tunnelY, 10));
    }

    private bool IsRoomRight(Vector2 upperLeftA, Vector2 lowerRightA, Vector2 upperLeftB, Vector2 lowerRightB)
    {
        // Проверяем пересечение по вертикали
        bool verticalOverlap = upperLeftA.y > lowerRightB.y && lowerRightA.y < upperLeftB.y;

        // Проверяем, что roomB находится справа от roomA
        bool isRight = upperLeftB.x > lowerRightA.x;

        return verticalOverlap && isRight;
    }

    private bool IsRightTunnelCanBe(int startX, int endX, int y)
    {
        bool resalt = true;
        for (int x  = startX + 1; x < endX; x++)
        {
            if (occupiedPlaces.Contains(new Vector2(x, y)))
            {
                resalt = false;
                break;
            }
        }
        return resalt;
    }

    private List<Vector2> GetPositionsForRightTunnel(Room roomA, int startX, Room roomB)
    {
        List<PositionProperty> roomPostionsToSpawn = roomA.GetPositionsToPlaceObjects();
        List<Vector2> objectPostions = SetPositionsToVector(roomPostionsToSpawn);
        List<Vector2> resalt = new();

        for (int i = 0; i < objectPostions.Count; i++)
        {
            if (objectPostions[i].x + 1 == startX)
            {
                if ((int)objectPostions[i].y < (int)roomB.GetLeftUpperAngle().y && (int)objectPostions[i].y > (int)roomB.GetRightBottomAngle().y)
                {
                    resalt.Add(objectPostions[i]);
                }
            }
        }

        return resalt;
    }
    #endregion
}