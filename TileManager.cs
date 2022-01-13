using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public static class TileManager 
{
    public static Tile basictile;
    public static Tilemap solidTileMap;

    public static void init(Tile basic, Tilemap stilemap) {
        basictile = basic;
        solidTileMap = stilemap;
    }

    public static void AddTile(Vector2Int pos) {
        solidTileMap.SetTile((Vector3Int) pos, basictile);
    }
    public static void RemoveTile(Vector2Int pos) {
        solidTileMap.SetTile((Vector3Int) pos, null);
    }

}