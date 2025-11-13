using System.Collections.Generic;
using UnityEngine;

// A* pathfinding on a Grid2D
public class AStar2D : MonoBehaviour
{
    public Grid2D grid;   // assign PathfindingGrid here in Inspector

    class Node
    {
        public int x, y;
        public float g;   // cost from start
        public float h;   // heuristic to goal
        public Node parent;
        public float f => g + h;

        public Node(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public List<Vector2> FindPath(Vector2 startWorld, Vector2 goalWorld)
    {
        var finalPath = new List<Vector2>();

        if (!grid.WorldToCell(startWorld, out int sx, out int sy)) return finalPath;
        if (!grid.WorldToCell(goalWorld, out int gx, out int gy)) return finalPath;

        var open   = new List<Node>();                 // Open list
        var closed = new HashSet<(int, int)>();        // Closed list
        var all    = new Dictionary<(int, int), Node>();

        Node startNode = new Node(sx, sy);
        startNode.g = 0;
        startNode.h = Heuristic(sx, sy, gx, gy);
        open.Add(startNode);
        all[(sx, sy)] = startNode;

        while (open.Count > 0)
        {
            // pick node with lowest F = G + H
            open.Sort((a, b) => a.f.CompareTo(b.f));
            Node current = open[0];
            open.RemoveAt(0);

            if (current.x == gx && current.y == gy)
            {
                // reached goal, reconstruct path via parent pointers
                var stack = new Stack<Vector2>();
                Node n = current;
                while (n != null)
                {
                    stack.Push(grid.CellToWorld(n.x, n.y));
                    n = n.parent;
                }
                while (stack.Count > 0)
                    finalPath.Add(stack.Pop());
                break;
            }

            closed.Add((current.x, current.y));

            foreach (var (nx, ny) in grid.GetNeighbors(current.x, current.y))
            {
                if (closed.Contains((nx, ny))) continue;

                float stepCost = (nx != current.x && ny != current.y) ? 14f : 10f; // diagonal vs straight
                float newG = current.g + stepCost;

                if (!all.TryGetValue((nx, ny), out Node neighbor))
                {
                    neighbor = new Node(nx, ny);
                    all[(nx, ny)] = neighbor;
                    neighbor.g = newG;
                    neighbor.h = Heuristic(nx, ny, gx, gy);
                    neighbor.parent = current;
                    open.Add(neighbor);
                }
                else if (newG < neighbor.g)
                {
                    neighbor.g = newG;
                    neighbor.parent = current;
                }
            }
        }

        return finalPath;
    }

    // Manhattan heuristic scaled to match G cost (10 per straight step)
    float Heuristic(int x, int y, int gx, int gy)
    {
        int dx = Mathf.Abs(x - gx);
        int dy = Mathf.Abs(y - gy);
        return 10f * (dx + dy);
    }
}
