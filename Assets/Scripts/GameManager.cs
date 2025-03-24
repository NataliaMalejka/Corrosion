using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum GameState{
    PlayerTurn,
    Busy,
    Animation
}

public class GameManager : MonoBehaviour
{
    private Dictionary<Vector3Int, CustomTile> rustTiles = new Dictionary<Vector3Int, CustomTile>();

    // private float maxTime = 2.5f;
    // private float currentTime;

    public UnityEvent<List<Vector3Int>> OnPlayerTurnEnded;

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
                //currently state is changed in TileManager
                break;
            case GameState.Busy:
                OnPlayerTurnEnded.Invoke(new List<Vector3Int>(rustTiles.Keys));
                //busy logic
                Debug.Log("Busy state");

                state = GameState.Animation;
                break;
            case GameState.Animation:
                //animation logic
                Debug.Log("Animation state");
                state = GameState.PlayerTurn;
                break;
        }
    }

    public void UpdateState(GameState newState)
    {
        state = newState;
    }

    // private void FixedUpdate()
    // {
    //     currentTime += Time.fixedDeltaTime;

    //     if(currentTime >= maxTime)
    //     {
    //         currentTime = 0;
    //         // ChangeTurn();
    //     }
    // }

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
