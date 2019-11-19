using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class World : MonoBehaviour
{
    [Serializable]
    public class GameTiles : SerializableDictionary<Vector3Int, GameTile>
    {
    }
    
    [Serializable]
    public class Observer
    {
        public Action action;
        public Observer(Action action)
        {
            this.action = action;
        }
        public void Notify()
        {
            action();
        }
    }

    [HideInInspector]
    public List<Observer> tileMapObservers = new List<Observer>();

    [HideInInspector]
    public bool tileMapInitialized = false;
    private Grid grid;

    private GameTiles gameTiles;

    private void Awake()
    {
        Instance = this;
        grid = GetComponent<Grid>();
    }

    private IEnumerator Start()
    {
        yield return null;
        InitializeWorld();
        tileMapObservers.ForEach(o => o.Notify());
    }

    private void InitializeWorld()
    {
        var obstacles = new Dictionary<Vector3Int, List<Obstacle>>();
        var allObstacles = GetComponentsInChildren<Obstacle>();
        foreach (var obstacle in allObstacles)
        {
            var position = Vector3Int.FloorToInt(obstacle.transform.position);
            if (!obstacles.ContainsKey(position))
            {
                obstacles.Add(position, new List<Obstacle>());
            }

            obstacles[position].Add(obstacle);
        }

        gameTiles = new GameTiles();
        var allTiles = GetComponentsInChildren<GameTile>();

        foreach (var tile in allTiles)
        {
            var position = Vector3Int.FloorToInt(tile.transform.position);
            gameTiles.Add(position, tile);
            tile.Initialize(obstacles.ContainsKey(position) ? obstacles[position] : new List<Obstacle>());
        }

        tileMapInitialized = true;
    }

    public (GameTile.CantMoveReason, Obstacle, Unit) CanWalk(Vector3Int position)
    {
        return !gameTiles.ContainsKey(position)
            ? (GameTile.CantMoveReason.NonWalkable, null, null)
            : gameTiles[position].CanWalk();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(new Vector3(0.5f, 0.5f, 0), new Vector3(1, 1, 0.2f));
    }

    public Vector3 GetCellCenterWorld(Vector3Int position)
    {
        return grid.GetCellCenterWorld(position);
    }

    public void OccupyTargetTile(Vector3Int currentPosition, Unit unit)
    {
        if (gameTiles.ContainsKey(currentPosition))
        {
            gameTiles[currentPosition].BecomeOccupied(unit);
        }
        else
        {
            Debug.LogError($"Can't occupy. No such tile exist: {currentPosition}");
        }
    }

    public void UnoccupyTargetTile(Vector3Int currentPosition)
    {
        if (gameTiles.ContainsKey(currentPosition))
        {
            gameTiles[currentPosition].Unoccupied();
        }
        else
        {
            Debug.LogError($"Can't unoccupy. No such tile exist: {currentPosition}");
        }
    }

    public static World Instance { get; private set; }
}