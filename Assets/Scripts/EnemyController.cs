using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
public class EnemyController : MonoBehaviour
{
    public Vector3Int Position { get; private set; }
    private Vector2 boxSize = new Vector2(0.8f, 0.8f);
    [SerializeField] private LayerMask CollisionLayer; 
    [SerializeField] private LayerMask EnemyLayer; 
    [SerializeField] private float MoveSpeed = 3.0f;
    [SerializeField] private SpriteRenderer SpriteRenderer;
    [SerializeField] private Animator Animator;
    private void Start()
    {
        Position = TileManager.Instance.FloorTilemap.WorldToCell(transform.position);
    }

    public bool OnItsTurn(HashSet<Vector3Int> rustPositions)
    {
        return MoveTowardInfestation(rustPositions);
    }

    private bool MoveTowardInfestation(HashSet<Vector3Int> rustPositions)
    {
        bool hasToMove = false;
        Vector3Int targetTile = GetClosestInfestedTile(rustPositions);
        if (targetTile == Position) return hasToMove;
        List<Vector3Int> nextSteps = GetNextGradedStepTowards(targetTile);
        foreach (var step in nextSteps)
        {
            // Debug.Log("Is tile "+step+" valid? "+TileManager.Instance.FloorTilemap.HasTile(step));
            // Debug.Log("Is tile "+step+" rusted? "+rustPositions.Contains(step));
            // Debug.Log("Is tile "+step+" free? "+ (Physics2D.OverlapBox(TileManager.Instance.FloorTilemap.CellToWorld(step), boxSize, 0, collisionLayer) == null));

            bool wallHit = Physics.Raycast(Position + new Vector3(0.5f, 0.5f, 0), step - Position, out RaycastHit hitInfo, 1, CollisionLayer);

            if (TileManager.Instance.FloorTilemap.HasTile(step) 
                && !rustPositions.Contains(step)
                && Physics2D.OverlapBox(TileManager.Instance.FloorTilemap.CellToWorld(step), boxSize, 0, EnemyLayer) == null
                && !wallHit)
            {                
                if(step.x - Position.x == -1)
                { SpriteRenderer.flipX = true; }
                else if (step.x - Position.x == 1)
                { SpriteRenderer.flipX = false; }

                Position = step;
                hasToMove = true;
                StartCoroutine(SmoothMove(TileManager.Instance.FloorTilemap.CellToWorld(Position)));
                break;
            }
        }
        return hasToMove;
    }

    private List<Vector3Int> GetNextGradedStepTowards(Vector3Int target)
    {
        List<Vector3Int> possibleMoves = new List<Vector3Int>{
            Position + Vector3Int.up,
            Position + Vector3Int.down,
            Position + Vector3Int.left,
            Position + Vector3Int.right
        };
        return possibleMoves.OrderBy(move => Vector3Int.Distance(move, target)).ToList();
    }

    private Vector3Int GetClosestInfestedTile(HashSet<Vector3Int> rustPositions)
    {
        Vector3Int closestTile = Vector3Int.zero;
        float shortestDist = float.MaxValue;
        foreach (var rustPos in rustPositions)
        {
            float dist = Vector3Int.Distance(Position, rustPos);
            if (dist < shortestDist)
            {
                shortestDist = dist;
                closestTile = rustPos;
            }
        }
        return closestTile;
    }

    IEnumerator SmoothMove(Vector3 targetWorldPos)
    {
        Animator.SetBool("move", true);
        while (Vector3.Distance(transform.position, targetWorldPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWorldPos, MoveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetWorldPos;
        Animator.SetBool("move", false);
        GameManager.Instance.OnEnemyFinishMoving();
    }
}
