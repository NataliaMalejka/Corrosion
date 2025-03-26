using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public enum GameState{
    PlayerTurn,
    Busy,
    EnemyMoving,
    TileUpdate,
    Animation
}

public class GameManager : MonoBehaviour
{
    //can be hash set?
    private Dictionary<Vector3Int, CustomTile> rustTiles = new Dictionary<Vector3Int, CustomTile>();
    
    public HashSet<GameObject> Enemies = new HashSet<GameObject>();

    [SerializeField] private GameObject enemyPrefab; 

    public UnityEvent<HashSet<Vector3Int>, HashSet<Vector3Int>> OnPlayerTurnEnded;
    public UnityEvent<HashSet<Vector3Int>> OnEnemyTurnEnded;

    private int movingEnemyCount = 0;
    HashSet<Vector3Int> enemyPositions = new HashSet<Vector3Int>();

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    private GameState state = GameState.PlayerTurn;
    public GameState State
    {
        get
        {
            return state;
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

    private void Start()
    {
        state = GameState.PlayerTurn;
    }

    private void Update()
    {
        switch (state)
        {
            case GameState.PlayerTurn:
                //player turn logic
                //currently state is changed in TileManager on click
                break;
            case GameState.Busy:
                //currently - enemy moves, enemy cleans, rust spreads
                //more aggressive enemies - enemy cleans, enemy moves, rust spreads
                //rust overwhelms enemy -  rust spreads, enemy moves, enemy cleans
                movingEnemyCount = Enemies.Count;
                enemyPositions = HandleEnemyMove();
                state = GameState.EnemyMoving;
                break;
            case GameState.EnemyMoving:
                //wait for enemy movement to finish
                if (movingEnemyCount == 0)
                {
                    state = GameState.TileUpdate;
                }
                break;
            case GameState.TileUpdate:
                OnEnemyTurnEnded.Invoke(enemyPositions);
                OnPlayerTurnEnded.Invoke(new HashSet<Vector3Int>(rustTiles.Keys), enemyPositions);

                state = GameState.Animation;
                break;
            case GameState.Animation:
                //animation and other logic if needed
                Debug.Log("Animation state");
                state = GameState.PlayerTurn;
                break;
        }
    }

    public void OnEnemyFinishMoving()
    {
        movingEnemyCount--;
    }

    private HashSet<Vector3Int> HandleEnemyMove()
    {
        HashSet<Vector3Int> enemyPositions = new HashSet<Vector3Int>();
        foreach (var enemy in Enemies)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            enemyController.OnItsTurn(new HashSet<Vector3Int>(rustTiles.Keys));
            enemyPositions.Add(enemyController.Position);
        }
        return enemyPositions;
    }

    public void UpdateState(GameState newState)
    {
        state = newState;
    }

    public void AddToRustList(Vector3Int pos, CustomTile tile)
    {
        if (!rustTiles.ContainsKey(pos))
        {
            rustTiles[pos] = tile;
        }
    }

    public bool CheckIfTileHasRust(Vector3Int pos)
    {
        return rustTiles.ContainsKey(pos);
    }

    public void SpawnEnemy(Vector3Int pos)
    {
        GameObject enemy = Instantiate(enemyPrefab, TileManager.Instance.FloorTilemap.CellToWorld(pos), Quaternion.identity);
        Enemies.Add(enemy);
    }

    public void RemoveFromRustList(Vector3Int pos)
    {
        rustTiles.Remove(pos);
    }
}
