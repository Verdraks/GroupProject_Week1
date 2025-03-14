﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TilemapData[] tilemapsData;

    [Space(10)] 
    
    [SerializeField] private RSF_AccessNextTile rsfAccessNextTile;
    
    
    private readonly Dictionary<Vector2Int, List<TilemapData.TileType>> _tiles = new ();
    
    private void Awake()
    {
        GenerateLevel();
    }

    private void OnEnable()
    {
        rsfAccessNextTile.Action += CheckAccessNextTile;
    }
    
    
    private void OnDisable() => rsfAccessNextTile.Action -= CheckAccessNextTile;

    private bool CheckAccessNextTile(Vector2 nextTile)
    {
        Vector2Int nextTileInt = new Vector2Int(Mathf.FloorToInt(nextTile.x), Mathf.FloorToInt(nextTile.y));
        
        if (_tiles.TryGetValue(nextTileInt, out var tileTypes))
        {
            foreach (var tileType in tileTypes)
            {
                
                switch (tileType)
                {
                    case TilemapData.TileType.Wall:
                        return false;
                    case TilemapData.TileType.Floor:
                        break;
                    case TilemapData.TileType.Interactible:
                        break;
                    case TilemapData.TileType.None:
                        return false;
                }
            }
            
            return true;
            
        }

        return false;
    }

    private void GenerateLevel()
    {
        if (tilemapsData.Length == 0)
        {
            Debug.LogError("No tilemaps data provided.");
            return;
        }
        
        _tiles.Clear();

        foreach (var tilemapData in tilemapsData)
        {
            if (!tilemapData.tilemap)
            {
                Debug.LogError($"Tilemap not assign for {tilemapData.tilemapTile}");
                continue;
            }

            foreach (var position in tilemapData.tilemap.cellBounds.allPositionsWithin)
            {
                if (tilemapData.tilemap.HasTile(position))
                {
                    Vector2Int posInt = new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));
                    
                    if (!_tiles.ContainsKey(posInt))
                    {
                        _tiles[posInt] = new List<TilemapData.TileType>(){Capacity = 2};
                    }
                    _tiles[posInt].Add(tilemapData.tilemapTile);
                }
            }
        }
        
    }
}

[Serializable]
public struct TilemapData
{
    public TileType tilemapTile;
    public Tilemap tilemap;
    
    public enum TileType : byte
    {
        None,
        Wall,
        Floor,
        Interactible
    }
}