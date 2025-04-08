using Assets.Scripts.BuildingScripts.BuildingTypes;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.BuildingScripts.RoomScripts.Inside_room_spawn
{
    public class FrozenRoomObjectPlacer : RoomObjectPlacer
    {
        public FrozenRoomObjectPlacer(System.Random rand) : base(rand) { }

        public override void SetRoomObjects(Room room)
        {
            SetPositionsAndObjectsOfRoom(room);
            SpawnFrozenGenerator(room);
            base.SetRoomObjects(room);
        }

        private void SpawnFrozenGenerator(Room room)
        {
            List<Vector2> positionsForSpawn = new List<Vector2>();
            for (int i = 0; i < positions.Count; i++)
            {
                positionsForSpawn.Add(new Vector2(positions[i].X, positions[i].Y));
            }

            if (positions.Count == 0)
            {
                positionsForSpawn = GetRoomFloorPositions(room);
            }

            GameObject[] gameObjects = room.GetObjects();
            for (int i = 0; i < gameObjects.Length; i++)
            {
                if (gameObjects[i].CompareTag("GeneratorOfSnow"))
                {
                    int randomPositionIndex = rand.Next(0, positionsForSpawn.Count);
                    Vector2 randomPosition = new Vector2 (positionsForSpawn[randomPositionIndex].x, positionsForSpawn[randomPositionIndex].y);

                    UnityEngine.Object.Instantiate(gameObjects[i], randomPosition, Quaternion.identity);

                    if (positions.Count > 0) positions.RemoveAt(randomPositionIndex);

                    break;
                }
            }
        }

        private List<Vector2> GetRoomFloorPositions(Room room)
        {
            int leftX = (int)room.entryPoint.x - room.wallsInfo.countOfWallsLeft;
            int rightX = (int)room.entryPoint.x + room.wallsInfo.countOfWallsRight;
            int floorY = (int)room.entryPoint.y - room.wallsInfo.countOfWallsDown + 1;

            List<Vector2> floorPositions = new();

            for (int x = leftX + 2; x < rightX - 1; x++)
            {
                if (!BuildingData.ladder.Contains(new Vector2(x, floorY)))
                {
                    floorPositions.Add(new Vector2(x, floorY));
                }
            }

            return floorPositions;
        }
    }
}
