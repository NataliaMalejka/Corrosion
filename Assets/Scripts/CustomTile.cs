using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/CustomTile")]
public class CustomTile : Tile
{
    public bool isRust;
    public bool isStart;
    public bool isFinish;
}
