using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public LayerMask unwalkable;
    public Vector2 rawGridSize;
    public float nodeRadius;
    public float walkableCheckRadius;
    public Node[,] grid;

    public bool drawGizmos;

    int gridSizeX, gridSizeY;

    void Start()
    {
        gridSizeX = Mathf.RoundToInt(rawGridSize.x / (nodeRadius*2));
        gridSizeY = Mathf.RoundToInt(rawGridSize.y / (nodeRadius*2));
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 bottomLeft = transform.position - new Vector3(rawGridSize.x/2, rawGridSize.y/2, 0);

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 pos = bottomLeft + new Vector3((2*x + 1)*nodeRadius, (2*y + 1)*nodeRadius, 0);
                bool walkable = !Physics2D.OverlapCircle(pos, nodeRadius+walkableCheckRadius, unwalkable);
                grid[x, y] = new Node(walkable, pos, x, y);
            }
        }
    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;
                
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    neighbors.Add(grid[checkX, checkY]);
            }
        }
        return neighbors;
    }

    public Node NodeFromWorldPoint(Vector3 pos)
    {
        float percentX = (pos.x + rawGridSize.x/2) / rawGridSize.x;
        float percentY = (pos.y + rawGridSize.y/2) / rawGridSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX-1)*percentX);
        int y = Mathf.RoundToInt((gridSizeY-1)*percentY);

        Debug.Log("x: " + x + ", y: " + y);
        Debug.Log(grid);
        return grid[x, y];
    }

    void OnDrawGizmos()
    {
        if (grid != null && drawGizmos)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = n.walkable?Color.white:Color.red;
                Gizmos.DrawWireCube(n.position, Vector3.one * (2*nodeRadius - 0.1f));
            }
        }
    }
}
