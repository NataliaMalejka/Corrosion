using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Dictionary<Vector3Int, CustomTile> rustTiles = new Dictionary<Vector3Int, CustomTile>();
    private TileManager tileManager;

    private float maxTime = 2.5f;
    private float currentTime;

    private void Awake()
    {
        tileManager = FindObjectOfType<TileManager>();
    }

    private void Start()
    {
        currentTime = 0;
    }

    private void FixedUpdate()
    {
        currentTime += Time.fixedDeltaTime;

        if(currentTime >= maxTime)
        {
            currentTime = 0;
            ChangeTurn();
        }
    }

    private void ChangeTurn()
    {
        List<Vector3Int> currentRustTiles = new List<Vector3Int>(rustTiles.Keys); 

        foreach (Vector3Int tilePos in currentRustTiles) 
        {
            tileManager.ChceskNeighbor(tilePos);
        }
    }

    public void AddToRustList(Vector3Int pos, CustomTile tile)
    {
        if (!rustTiles.ContainsKey(pos))
        {
            rustTiles[pos] = tile;
        }
    }

    public void RemoveFromList(Vector3Int pos)
    {
        rustTiles.Remove(pos);
    }
}
