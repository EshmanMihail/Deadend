using Assets.Scripts.BuildingScripts.BuildingTypes;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace Assets.Scripts.BuildingScripts
{
    public class NetworkTileSetter : NetworkBehaviour
    {
        [SerializeField] private Tilemap wallsTilemap;
        [SerializeField] private Tilemap backgroundWalls;
        [SerializeField] private Tilemap ladderTilemap;
        [SerializeField] private Tilemap platformsTilemap;
        [SerializeField] private Tilemap frontTiles;
        [SerializeField] private Tilemap backwardTiles;
        [SerializeField] private GameObject tileDataBaseObject;

        private TileDataBase tileDataBase;

        private List<int> tilesIndexes = new();
        private List<Vector2> tilesPositions = new();
        private List<int> layersIndexes = new();

        private List<int> platformsTilesIndex = new();
        private List<Vector2> platformsPositions = new();

        private List<Vector3Int> tilePositionToRotate = new();
        private List<float> angles = new();

        private void Start()
        {
            tileDataBase = tileDataBaseObject.GetComponent<TileDataBase>();
        }

        public void AddTileInfo(int index, int x, int y, int layer)
        {
            tilesIndexes.Add(index);
            tilesPositions.Add(new Vector2(x, y));
            layersIndexes.Add(layer);
        }


        public void AddPlatformTileInfo(int index, int x, int y)
        {
            platformsTilesIndex.Add(index);
            platformsPositions.Add(new Vector2(x, y));
        }

        public void AddTilesToRemove(Vector3Int positionToRemove, int tilemap)
        {
            Vector2 position2D = new Vector2(positionToRemove.x, positionToRemove.y);
            for (int i = 0; i < tilesPositions.Count; i++)
            {
                if (tilesPositions[i] == position2D && layersIndexes[i] == tilemap)
                {
                    tilesIndexes.RemoveAt(i);
                    tilesPositions.RemoveAt(i);
                    layersIndexes.RemoveAt(i);
                    break;
                }
            }
        }

        public void AddTileToRotate(Vector3Int position, float angle)
        {
            tilePositionToRotate.Add(position);
            angles.Add(angle);
        }

        #region Set Tiles
        [Command(requiresAuthority = false)]
        public void CmdSendTilesInfo()
        {
            SendLevelToClients();
        }

        [Server]
        private void SendLevelToClients()
        {
            //StartCoroutine(SyncLevelTilesInChunks(tilesIndexes, tilesPositions, layersIndexes));
            StartCoroutine(SendLevelWithDelay(tilesIndexes, tilesPositions, layersIndexes));
        }

        private IEnumerator SendLevelWithDelay(List<int> tilesIndexes, List<Vector2> tilesPositions, List<int> layersIndexes)
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(SyncLevelTilesInChunks(tilesIndexes, tilesPositions, layersIndexes));
        }

        private IEnumerator SyncLevelTilesInChunks(List<int> tilesIndexes, List<Vector2> tilesPositions, List<int> layersIndexes, int chunkSize = 100)
        {
            int count = tilesIndexes.Count;

            for (int i = 0; i < count; i += chunkSize)
            {
                List<int> chunkIndexes = tilesIndexes.GetRange(i, Mathf.Min(chunkSize, count - i));
                List<Vector2> chunkPositions = tilesPositions.GetRange(i, Mathf.Min(chunkSize, count - i));
                List<int> chunkLayers = layersIndexes.GetRange(i, Mathf.Min(chunkSize, count - i));

                RpcSyncTiles(chunkIndexes, chunkPositions, chunkLayers);
                yield return null; // Разделяем отправку между кадрами
            }
        }

        [ClientRpc]
        private void RpcSyncTiles(List<int> tilesIndexes, List<Vector2> tilesPositions, List<int> layersIndexes)
        {
            for (int i = 0; i < tilesIndexes.Count; i++)
            {
                Tile tile = tileDataBase.GetTileByIndex(tilesIndexes[i]);
                Vector3Int position = new Vector3Int((int)tilesPositions[i].x, (int)tilesPositions[i].y, 10);

                if (layersIndexes[i] == ObjectsLayers.Walls)
                {
                    wallsTilemap.SetTile(position, tile);
                }
                else if (layersIndexes[i] == ObjectsLayers.BackgroundWalls)
                {
                    backgroundWalls.SetTile(position, tile);
                }
                else if (layersIndexes[i] == ObjectsLayers.Ladder)
                {
                    ladderTilemap.SetTile(position, tile);
                }
                else if (layersIndexes[i] == ObjectsLayers.FrontObjects)
                {
                    frontTiles.SetTile(position, tile);
                }
                else if (layersIndexes[i] == ObjectsLayers.BackwardObjects)
                {
                    backwardTiles.SetTile(position, tile);
                }
            }
        }
        #endregion

        #region Rotate tiles
        [Command(requiresAuthority = false)]
        public void CmdRotateTile()
        {
            RotateTilesToClients();
        }

        [Server]
        private void RotateTilesToClients()
        {
            //StartCoroutine(SyncRotationTilesInChunks(tilePositionToRotate, angles));
            StartCoroutine(RotateTileWithDelay(tilePositionToRotate, angles));
        }

        private IEnumerator RotateTileWithDelay(List<Vector3Int> tilePositionToRotate, List<float> angles)
        {
            yield return new WaitForSeconds(1f);
            StartCoroutine(SyncRotationTilesInChunks(tilePositionToRotate, angles));
        }

        private IEnumerator SyncRotationTilesInChunks(List<Vector3Int> tilePositionToRotate, List<float> angles, int chunkSize = 100)
        {
            int count = tilePositionToRotate.Count;

            for (int i = 0; i < count; i += chunkSize)
            {
                List<Vector3Int> chunkPosition = tilePositionToRotate.GetRange(i, Mathf.Min(chunkSize, count - i));
                List<float> chunkAngles = angles.GetRange(i, Mathf.Min(chunkSize, count - i));

                RpcSyncRotationTiles(chunkPosition, chunkAngles);
                yield return null;
            }
        }

        [ClientRpc]
        private void RpcSyncRotationTiles(List<Vector3Int> tilePositionToRotate, List<float> angles)
        {
            for (int i = 0; i < tilePositionToRotate.Count; i++)
            {
                Matrix4x4 matrix = wallsTilemap.GetTransformMatrix(tilePositionToRotate[i]);

                Matrix4x4 rotationMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, angles[i]), Vector3.one);

                wallsTilemap.SetTransformMatrix(tilePositionToRotate[i], rotationMatrix);
            }
        }
        #endregion

        #region Set tiles of platforms
        [Command(requiresAuthority = false)]
        public void CmdSetPlatformsTiles()
        {
            SetPlatformsTilesToClients();
        }

        [Server]
        private void SetPlatformsTilesToClients()
        {
            //StartCoroutine(SyncPlaformTilesInChunks(platformsTilesIndex, platformsPositions));
            StartCoroutine(SetPlatformsTilesWithDelay(platformsTilesIndex, platformsPositions));
        }

        private IEnumerator SetPlatformsTilesWithDelay(List<int> platformsTilesIndex, List<Vector2> platformsPositions)
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(SyncPlaformTilesInChunks(platformsTilesIndex, platformsPositions));
        }

        private IEnumerator SyncPlaformTilesInChunks(List<int> platformsTilesIndex, List<Vector2> platformsPositions, int chunkSize = 100)
        {
            int count = platformsTilesIndex.Count;

            for (int i = 0; i < count; i += chunkSize)
            {
                List<int> chunkPlatformsIndex = platformsTilesIndex.GetRange(i, Mathf.Min(chunkSize, count - i));
                List<Vector2> chunkPlatformsPositions = platformsPositions.GetRange(i, Mathf.Min(chunkSize, count - i));

                RpcSyncPlatformsTiles(chunkPlatformsIndex, chunkPlatformsPositions);
                yield return null;
            }
        }

        [ClientRpc]
        private void RpcSyncPlatformsTiles(List<int> platformsTilesIndex, List<Vector2> platformsPositions)
        {
            for (int i = 0; i < platformsTilesIndex.Count; i++)
            {
                Vector3Int position = new Vector3Int((int)platformsPositions[i].x, (int)platformsPositions[i].y, 10);
                Tile tile = tileDataBase.GetTileByIndex(platformsTilesIndex[i]);
                platformsTilemap.SetTile(position, tile);
            }
        }

        #endregion
    }
}
