using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class TileManager : MonoBehaviour
{
    public Tilemap FloorTilemap;
    [SerializeField] private Tilemap overlayTilemap;

    [SerializeField] private Sprite RustSprite;
    [SerializeField] private Sprite DebugSprite;
    [SerializeField] private Vector3Int startRustTilePos;
    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] private float distance;

    private static TileManager instance;
    public static TileManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private Vector3Int[] directions = {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0),
    };

    // Triggered by GameManager Event
    private void HandleRustSpread(HashSet<Vector3Int> tilePosition, HashSet<Vector3Int> enemyPositions)
    {
        foreach (Vector3Int tilePos in tilePosition)
        {
            CheckNeighbor(tilePos, enemyPositions);
        }
    }

    private void HandleEnemyCleaning(HashSet<Vector3Int> enemyPositions)
    {
        foreach (Vector3Int enemyPos in enemyPositions)
        {
            RemoveRust(enemyPos);
        }
    }

    private void Start()
    {
        BoundsInt bounds = FloorTilemap.cellBounds;

        //Initial rust placement, clear tile states
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            CustomTile tile = FloorTilemap.GetTile<CustomTile>(pos);

            if (tile != null)
            {
                tile.occupancy = CellState.Empty;
                if (tile.cellType == CellType.Start)
                {
                    tile.occupancy = CellState.Rust;
                    ChangeTile(startRustTilePos, tile);
                }
                else if (tile.cellType == CellType.EnemySpawn)
                {
                    GameManager.Instance.SpawnEnemy(pos);
                }
            }
        }

        GameManager.Instance.OnPlayerTurnEnded.AddListener(HandleRustSpread);
        GameManager.Instance.OnEnemyTurnEnded.AddListener(HandleEnemyCleaning);
    }

    // Players clicks on any tile
    public void OnClick(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.State != GameState.PlayerTurn || !context.performed)
            return;
        GetComponent<PlayerInput>().DeactivateInput();
        
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0));

        Vector3Int clickPos = FloorTilemap.WorldToCell(mousePos);
        CustomTile clickedTile = FloorTilemap.GetTile<CustomTile>(clickPos);
        Debug.Log("Clicked tile at " + clickPos);

        if (clickedTile != null && clickedTile.occupancy.Equals(CellState.Empty) && !GameManager.Instance.CheckIfTileHasRust(clickPos))
        {
            //TO DO
            Debug.Log("Empty tile clicked, Do something, rotate etc");
            CustomTile overlayTile = ScriptableObject.CreateInstance<CustomTile>();
            overlayTile.sprite = DebugSprite;
            overlayTilemap.SetTile(clickPos, overlayTile);
        }
        GameManager.Instance.UpdateState(GameState.Busy);
        GetComponent<PlayerInput>().ActivateInput();
    }

    private void ChangeTile(Vector3Int pos, CustomTile tile)
    {
        Tile overlayTile = ScriptableObject.CreateInstance<Tile>();
        overlayTile.sprite = RustSprite;
        overlayTilemap.SetTile(pos, overlayTile);

        GameManager.Instance.AddToRustList(pos, tile);
    }

    public void CheckNeighbor(Vector3Int pos, HashSet<Vector3Int> enemyPositions)
    {
        foreach (Vector3Int direction in directions)
        {
            Vector3Int neighborPos = pos + direction;
            CustomTile neighborTile = FloorTilemap.GetTile<CustomTile>(neighborPos);

            Vector3 startpos = pos + new Vector3(0.5f, 0.5f, 0);
            Vector3 dir = direction;
            bool hit = Physics.Raycast(startpos, dir, out RaycastHit hitInfo, distance, collisionLayer);

            Debug.DrawRay(startpos, dir * distance, hit ? Color.red : Color.green, 2f);

            if (neighborTile != null && neighborTile.occupancy.Equals(CellState.Empty)
                && !enemyPositions.Contains(neighborPos) && !hit)
            {
                ChangeTile(neighborPos, neighborTile);
            }
        }
    }

    //pos is the position of the enemy
    public void RemoveRust(Vector3Int pos)
    {
        foreach (Vector3Int direction in directions)
        {
            Vector3Int neighborPos = pos + direction;
            CustomTile neighborTile = FloorTilemap.GetTile<CustomTile>(neighborPos);
            if (neighborTile != null && GameManager.Instance.CheckIfTileHasRust(neighborPos))
            {
                overlayTilemap.SetTile(neighborPos, null);
                GameManager.Instance.RemoveFromRustList(neighborPos);
            }
        }
    }
}
