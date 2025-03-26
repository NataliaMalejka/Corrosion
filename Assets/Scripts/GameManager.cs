using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum GameState{
    PlayerTurn,
    Busy,
    EnemyMoving,
    TileUpdate,
    Animation,
    GameEnd,
    IdleBrokenLoop
}

public class GameManager : MonoBehaviour
{
    //can be hash set?
    private Dictionary<Vector3Int, CustomTile> rustTiles = new Dictionary<Vector3Int, CustomTile>();
    
    public HashSet<GameObject> Enemies = new HashSet<GameObject>();

    [SerializeField] private GameObject enemyPrefab; 
    [SerializeField] private int maxTurns = 20; 
    [SerializeField] private GameObject EndScreenPrefab; 
    [SerializeField] private UiController UiController; 


    public UnityEvent<HashSet<Vector3Int>, HashSet<Vector3Int>> OnPlayerTurnEnded;
    public UnityEvent<HashSet<Vector3Int>> OnEnemyTurnEnded;

    private int movingEnemyCount = 0;
    private int turnCount = 0;
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
        UiController.UpdateTurnCounter(maxTurns);
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
                enemyPositions = HandleEnemyMove();
                UpdateState(GameState.EnemyMoving);
                break;
            case GameState.EnemyMoving:
                //wait for enemy movement to finish
                if (movingEnemyCount == 0)
                {
                    UpdateState(GameState.TileUpdate);
                }
                break;
            case GameState.TileUpdate:
                OnEnemyTurnEnded.Invoke(enemyPositions);
                OnPlayerTurnEnded.Invoke(new HashSet<Vector3Int>(rustTiles.Keys), enemyPositions);

                UpdateState(GameState.Animation);
                break;
            case GameState.Animation:
                //animation and other logic if needed
                turnCount++;
                UiController.UpdateTurnCounter(maxTurns - turnCount);
                if (turnCount >= maxTurns || rustTiles.Count == 0)
                {
                    UpdateState(GameState.GameEnd);
                }
                else
                {
                    UpdateState(GameState.PlayerTurn);
                }
                break;
            case GameState.GameEnd:
                GameObject endScreenInstance = Instantiate(EndScreenPrefab);
                endScreenInstance.GetComponent<GameEndController>().EndGame(turnCount < maxTurns && rustTiles.Count != 0, rustTiles.Count == 0);
                state = GameState.IdleBrokenLoop;
                break;
            case GameState.IdleBrokenLoop:
                //idle state, when logic must end permanently
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
            movingEnemyCount += enemyController.OnItsTurn(new HashSet<Vector3Int>(rustTiles.Keys)) ? 1 : 0;
            enemyPositions.Add(enemyController.Position);
        }
        return enemyPositions;
    }

    public void UpdateState(GameState newState)
    {
        if (State != GameState.GameEnd && State != GameState.IdleBrokenLoop)
        {
            state = newState;
        }
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
