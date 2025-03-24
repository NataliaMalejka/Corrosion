using UnityEngine;
using UnityEngine.Tilemaps;

public enum CellType
{
    Empty,
    Rust,
    Obstacle,
    Enemy
}

[CreateAssetMenu(menuName = "Tiles/CustomTile")]
public class CustomTile : Tile
{
    public CellType occupancy;
    public bool isStart;
    public bool isFinish;
}
