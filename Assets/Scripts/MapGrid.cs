using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*class Node
{
    public bool isWalkable;
    public Vector3 worldPos;

    public Node(bool _isWalkable, Vector3 _worldPos)
    {
        isWalkable = _isWalkable;
        worldPos = _worldPos;
    }
}

public class MapGrid : MonoBehaviour
{
    [SerializeField] LayerMask unwalkableLayer;
    [SerializeField] Vector2 gridWorldSize;
    [SerializeField] float nodeRadius;

    Node[,] grid;

    float nodeDiameter;
    int gridX;
    int gridZ;

    private void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridZ = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Node[gridX, gridZ];
        Vector3 worldStartPos = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        Vector3 worldPoint;

        for (int x = 0; x < gridX; x++)
        {
            for (int z = 0; z < gridZ; z++)
            {
                worldPoint = worldStartPos + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (z * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableLayer));
                grid[x, z] = new Node(walkable, worldPoint);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 0, gridWorldSize.y));

        if(grid !=null)
        {
            foreach(Node n in grid)
            {
                Gizmos.color = n.isWalkable ? Color.clear : new Color(1,0,0,0.2f);
                Gizmos.DrawCube(n.worldPos, new Vector3(1, 0, 1) * (nodeDiameter - .1f));
            }
        }
    }
}*/
