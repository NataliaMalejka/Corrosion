using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class TileManager : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tilemap overlayTilemap;

    [SerializeField] private Sprite rustSprite;
    [SerializeField] private Sprite DebugSprite;

    [SerializeField] private Vector3Int startRustTilePos;

    private Vector3Int[] directions = {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0),
    };

    // Triggered by GameManager Event
    private void HandleRustSpread(List<Vector3Int> tilePosition)
    {
        foreach (Vector3Int tilePos in tilePosition)
        {
            CheckNeighbor(tilePos);
        }
    }

    private void Start()
    {
        BoundsInt bounds = tilemap.cellBounds;

        //Initial rust placement, clear tile states
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            CustomTile tile = tilemap.GetTile<CustomTile>(pos);

            if (tile != null)
            {
                tile.occupancy = CellType.Empty;
                if (tile.isStart)
                {
                    tile.occupancy = CellType.Rust;
                    ChangeTile(startRustTilePos, tile);
                }
            }
        }

        GameManager.Instance.OnPlayerTurnEnded.AddListener(HandleRustSpread);
    }

    // Players clicks on any tile
    public void OnClick(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.State != GameState.PlayerTurn || !context.performed)
            return;
        GetComponent<PlayerInput>().DeactivateInput();
        
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0));

        Vector3Int clickPos = tilemap.WorldToCell(mousePos);
        CustomTile clickedTile = tilemap.GetTile<CustomTile>(clickPos);

        if (clickedTile != null && clickedTile.occupancy.Equals(CellType.Empty))
        {
            //TO DO
            Debug.Log("Empty tile clicked, Do something, rotate etc");
            CustomTile overlayTile = ScriptableObject.CreateInstance<CustomTile>();
            overlayTile.sprite = DebugSprite;
            overlayTilemap.SetTile(clickPos, overlayTile);

            GameManager.Instance.UpdateState(GameState.Busy);
        }
        GetComponent<PlayerInput>().ActivateInput();
    }

    private void ChangeTile(Vector3Int pos, CustomTile tile)
    {
        Tile overlayTile = ScriptableObject.CreateInstance<Tile>();
        overlayTile.sprite = rustSprite;
        overlayTilemap.SetTile(pos, overlayTile);

        GameManager.Instance.AddToRustList(pos, tile);
    }

    public void CheckNeighbor(Vector3Int pos)
    {
        foreach (Vector3Int direction in directions)
        {
            Vector3Int neighborPos = pos + direction;
            CustomTile neighborTile = tilemap.GetTile<CustomTile>(neighborPos);

            if (neighborTile != null && neighborTile.occupancy.Equals(CellType.Empty))
            {
                ChangeTile(neighborPos, neighborTile);
            }
        }
    }
}
