using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding : MonoBehaviour
{
    public static List<Node> FindPath(Node startNode, Node targetNode)
    {
        List<Node> open = new List<Node>();
        HashSet<Node> closed = new HashSet<Node>();

        open.Add(startNode);

        while (open.Count > 0)
        {
            Node current = open[0];
            for (int i = 1; i < open.Count; i++)
            {
                if (open[i].fCost < current.fCost ||
                   (open[i].fCost == current.fCost && open[i].hCost < current.hCost))
                {
                    current = open[i];
                }
            }

            open.Remove(current);
            closed.Add(current);

            if (current == targetNode)
            {
                return Retrace(startNode, targetNode);
            }

            foreach (Node neighbour in GridManager.Instance.GetNeighbours(current))
            {
                if (!neighbour.walkable || closed.Contains(neighbour))
                    continue;

                int newCost = current.gCost + 10;

                if (newCost < neighbour.gCost || !open.Contains(neighbour))
                {
                    neighbour.gCost = newCost;
                    neighbour.hCost = Heuristic(neighbour, targetNode);
                    neighbour.parent = current;

                    if (!open.Contains(neighbour))
                        open.Add(neighbour);
                }
            }
        }

        return null; // Không tìm được đường
    }

    private static List<Node> Retrace(Node start, Node end)
    {
        List<Node> path = new List<Node>();
        Node current = end;

        while (current != start)
        {
            path.Add(current);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }

    private static int Heuristic(Node a, Node b)
    {
        return Mathf.Abs(a.gridX - b.gridX) + Mathf.Abs(a.gridY - b.gridY);
    }
}
