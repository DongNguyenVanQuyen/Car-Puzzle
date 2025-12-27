using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public int gridSizeX = 10;
    public int gridSizeY = 10;
    public float cellSize = 1f;
    public LayerMask obstacleMask;

    public Node[,] grid;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CreateGrid();
    }

    public void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 bottomLeft = transform.position;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = bottomLeft + new Vector3(x * cellSize, 0, y * cellSize);

                bool walkable = !Physics.CheckSphere(worldPoint, 0.4f, obstacleMask);

                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public Node WorldToNode(Vector3 worldPos)
    {
        float percentX = Mathf.Clamp01((worldPos.x - transform.position.x) / (gridSizeX * cellSize));
        float percentY = Mathf.Clamp01((worldPos.z - transform.position.z) / (gridSizeY * cellSize));

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    public void UpdateObstacles()
    {
        // Reset
        foreach (Node n in grid)
            n.walkable = true;

        // Gán xe vào grid là vật cản
        foreach (CarController car in FindObjectsOfType<CarController>())
        {
            Node n = WorldToNode(car.transform.position);
            n.walkable = false;
        }
    }

    public System.Collections.Generic.List<Node> GetNeighbours(Node node)
    {
        var neighbours = new System.Collections.Generic.List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                // bỏ đường chéo (Car Jam không chạy chéo)
                if (Mathf.Abs(x) + Mathf.Abs(y) != 1) continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX &&
                    checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    private void OnDrawGizmos()
    {
        if (grid == null) return;

        foreach (Node n in grid)
        {
            Gizmos.color = n.walkable ? Color.white : Color.red;
            Gizmos.DrawCube(n.worldPosition, Vector3.one * (cellSize * 0.9f));
        }
    }
}
