using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tilemap overlayTilemap;

    [SerializeField] private Sprite rustSprite;

    [SerializeField] private Vector3Int startRustTilePos;

    private GameManager gameManager;

    private Vector3Int[] directions = {
        new Vector3Int(1, 0, 0),  
        new Vector3Int(-1, 0, 0), 
        new Vector3Int(0, 1, 0),  
        new Vector3Int(0, -1, 0), 
    };


    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            CustomTile tile = tilemap.GetTile<CustomTile>(pos);

            if (tile != null && tile.isStart) 
            {
                ChangeTile(startRustTilePos, tile);
            }
        }
    }

    private void ChangeTile(Vector3Int pos, CustomTile tile)
    {
        Tile overlayTile = ScriptableObject.CreateInstance<Tile>();
        overlayTile.sprite = rustSprite;
        overlayTilemap.SetTile(pos, overlayTile);

        gameManager.AddToRustList(pos, tile);
    }

    public void ChceskNeighbor(Vector3Int pos)
    {
        foreach (Vector3Int direction in directions)
        {
            Vector3Int neighborPos = pos + direction;
            CustomTile neighborTile = tilemap.GetTile<CustomTile>(neighborPos);

            if (neighborTile != null && !neighborTile.isRust)
            {
                ChangeTile(neighborPos, neighborTile);         
            }
        }
    }
}
