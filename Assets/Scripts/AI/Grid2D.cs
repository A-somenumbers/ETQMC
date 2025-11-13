using System.Collections.Generic;
using UnityEngine;

public class Grid2D : MonoBehaviour
{
    [Header("Grid Settings")]
    public Vector2 worldBottomLeft = new Vector2(-10, -5);
    public Vector2 worldTopRight   = new Vector2( 10,  5);
    public float cellSize = 0.5f;
    public LayerMask obstacleMask;  // set this to Walls layer in Inspector

    bool[,] walkable;
    int width, height;

    void Awake()
    {
        BuildGrid();
    }


    public void BuildGrid(){
        width  = Mathf.CeilToInt((worldTopRight.x - worldBottomLeft.x) / cellSize);
        height = Mathf.CeilToInt((worldTopRight.y - worldBottomLeft.y) / cellSize);

        walkable = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 worldPos = CellToWorld(x, y);

                // Make walls "fatter" for pathfinding: any overlap blocks this tile.
                Vector2 boxSize = Vector2.one * (cellSize * 0.9f);
                Collider2D hit = Physics2D.OverlapBox(
                    worldPos,
                    boxSize,
                    0f,
                    obstacleMask
                );

                walkable[x, y] = (hit == null);
            }
        }
    }




    public Vector2 CellToWorld(int x, int y)
    {
        return worldBottomLeft + new Vector2((x + 0.5f) * cellSize, (y + 0.5f) * cellSize);
    }

    public bool WorldToCell(Vector2 worldPos, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPos.x - worldBottomLeft.x) / cellSize);
        y = Mathf.FloorToInt((worldPos.y - worldBottomLeft.y) / cellSize);

        if (x < 0 || y < 0 || x >= width || y >= height)
            return false;

        return true;
    }

    public bool IsWalkable(int x, int y)
    {
        if (x < 0 || y < 0 || x >= width || y >= height)
            return false;

        return walkable[x, y];
    }

    public List<(int x, int y)> GetNeighbors(int x, int y){
        var result = new List<(int, int)>();

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                int nx = x + dx;
                int ny = y + dy;

                if (!IsWalkable(nx, ny))
                    continue;

                // If moving diagonally, don't allow cutting through corners:
                // we require both horizontal and vertical neighbors to be walkable.
                bool isDiagonal = (dx != 0 && dy != 0);
                if (isDiagonal)
                {
                    if (!IsWalkable(x + dx, y) || !IsWalkable(x, y + dy))
                        continue;
                }

                result.Add((nx, ny));
            }
        }

        return result;
    }


    void OnDrawGizmosSelected()
    {
        if (walkable == null) return;

        Gizmos.color = new Color(0f, 1f, 0f, 0.1f);
        for (int x = 0; x < walkable.GetLength(0); x++)
        {
            for (int y = 0; y < walkable.GetLength(1); y++)
            {
                if (walkable[x, y])
                    Gizmos.DrawCube(CellToWorld(x, y), Vector3.one * cellSize * 0.9f);
            }
        }
    }

}
