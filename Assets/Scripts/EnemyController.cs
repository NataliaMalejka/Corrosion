using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class EnemyController : MonoBehaviour
{
    public Vector3Int Position { get; private set; }
    private Vector2 boxSize = new Vector2(0.8f, 0.8f);
    [SerializeField] private LayerMask collisionLayer; 
    private void Start()
    {
        Position = TileManager.Instance.FloorTilemap.WorldToCell(transform.position);
    }

    public void OnItsTurn(HashSet<Vector3Int> rustPositions)
    {
        MoveTowardInfestation(rustPositions);
    }

    private void MoveTowardInfestation(HashSet<Vector3Int> rustPositions)
    {
        Vector3Int targetTile = GetClosestInfestedTile(rustPositions);
        if (targetTile == Position) return;
        List<Vector3Int> nextSteps = GetNextGradedStepTowards(targetTile);
        foreach (var step in nextSteps)
        {
            // Debug.Log("Is tile "+step+" valid? "+TileManager.Instance.FloorTilemap.HasTile(step));
            // Debug.Log("Is tile "+step+" rusted? "+rustPositions.Contains(step));
            // Debug.Log("Is tile "+step+" free? "+ (Physics2D.OverlapBox(TileManager.Instance.FloorTilemap.CellToWorld(step), boxSize, 0, collisionLayer) == null));

            if (TileManager.Instance.FloorTilemap.HasTile(step) 
                && !rustPositions.Contains(step)
                && Physics2D.OverlapBox(TileManager.Instance.FloorTilemap.CellToWorld(step), boxSize, 0, collisionLayer) == null)
            {                
                Position = step;
                transform.position = TileManager.Instance.FloorTilemap.CellToWorld(Position);
                break;
            }
        }
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
}
