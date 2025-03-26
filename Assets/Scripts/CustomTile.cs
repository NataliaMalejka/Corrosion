using UnityEngine;
using UnityEngine.Tilemaps;

public enum CellState
{
    Empty,
    Rust,
    Obstacle,
    Enemy
}

public enum CellType
{
    Neutral,
    Start,
    Finish,
    EnemySpawn
}

[CreateAssetMenu(menuName = "Tiles/CustomTile")]
public class CustomTile : Tile
{
    //most likely useless, tilemap has build in flags apparently idk
    public CellState occupancy;
    public CellType cellType;
}
