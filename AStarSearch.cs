using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class AStarShip : MonoBehaviour
{
    [SerializeField] private bool showPath = true;
    private float nodeSize = 0.1f;
    private Node[,] grid;
    private Vector2 gridSize, gridNodes;
    private bool pathFound, searching;
    private LayerMask obstacleLayer;
    private List<Node> openSet, closedSet, path;
    private Node currentNode;
    private Vector3 rootNodePos, goalNodePos;

    private void Start()
    {
        gridSize = transform.localScale * 10f;
        gridNodes = new Vector2(Mathf.RoundToInt(gridSize.x / nodeSize), Mathf.RoundToInt(gridSize.y / nodeSize));
        obstacleLayer = LayerMask.GetMask("Obstacle");
        CreateGrid();
    }

    // returns the path from objectA to objectB
    public List<Node> RequestPath(GameObject objectA, GameObject objectB)
    {
        if (searching)
        {
            AStarPathFind();
        }
        rootNodePos = objectA.transform.position;
        goalNodePos = objectB.transform.position;

        CreateGrid();
        AStarPathFind();

        return path;
    }

    private void CreateGrid()
    {
        // create 2d matrix to store nodes
        grid = new Node[(int)gridNodes.x, (int)gridNodes.y];

        // location bottom left of the grid space
        Vector3 gridBottomLeft = transform.position - Vector3.right * gridSize.x / 2
                                                    - Vector3.forward * gridSize.y / 2;

        for (int i = 0; i < gridNodes.x; i++)
        {
            for (int j = 0; j < gridNodes.y; j++)
            {
                // find position in the grid this node needs to be
                Vector3 nodePos = gridBottomLeft + Vector3.right * (i * nodeSize + (nodeSize / 2))
                                                 + Vector3.forward * (j * nodeSize + (nodeSize / 2));

                bool traversable = !(Physics.CheckSphere(nodePos, nodeSize / 2, obstacleLayer));

                // add node to grid.
                grid[i, j] = new Node(nodePos, traversable, i, j);
            }
        }
    }

    // returns node in grid matrix from transform.position
    public Node NodePositionInGrid(Vector3 gridPosition)
    {
        float pX = Mathf.Clamp01((gridPosition.x - ((gridSize.x / gridNodes.x) / 2) + (gridSize.x / 2)) / gridSize.x);
        float pY = Mathf.Clamp01((gridPosition.z - ((gridSize.y / gridNodes.y) / 2) + (gridSize.y / 2)) / gridSize.y);

        int nX = (int)Mathf.Clamp(Mathf.RoundToInt(gridNodes.x * pX), 0, gridNodes.x - 1);
        int nY = (int)Mathf.Clamp(Mathf.RoundToInt(gridNodes.y * pY), 0, gridNodes.y - 1);

        return grid[nX, nY];
    }

    // A* search
    private void AStarPathFind()
    {
        Node rootNode = NodePositionInGrid(rootNodePos);
        Node goalNode = NodePositionInGrid(goalNodePos);
        openSet = new();
        closedSet = new();
        pathFound = false;
        openSet.Add(rootNode);
        currentNode = new Node(Vector3.zero, false, -1, -1);
        searching = true;

        while (openSet.Count > 0 && searching)
        {
            currentNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].f < currentNode.f || (openSet[i].f == currentNode.f && openSet[i].h < currentNode.h))
                    currentNode = openSet[i];
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);
        
            if (currentNode == goalNode)
            {
                RetracePath(rootNode, goalNode);
                pathFound = true;
                searching = false;
                break;
            }
            else
            {
                foreach (Node neighbour in GetNeighbours(currentNode))
                {
                    if (!neighbour.traversable || closedSet.Contains(neighbour))
                        continue;

                    float newMoveCost = currentNode.g + GetDistance(currentNode, neighbour);

                    if (newMoveCost < neighbour.g || !openSet.Contains(neighbour))
                    {
                        neighbour.g = newMoveCost;
                        neighbour.h = GetDistance(neighbour, goalNode);
                        neighbour.parentNode = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }
        }
        searching = false;
    }

    // sets path
    private void RetracePath(Node rNode, Node gNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = gNode;

        while (currentNode != rNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parentNode;
        }

        path.Reverse();
        this.path = path;
    }

    // searches neighbours
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }

                int pX = node.x + i;
                int pY = node.y + j;

                if (pX >= 0 && pX < gridNodes.x && pY >= 0 && pY < gridNodes.y)
                {
                    neighbours.Add(grid[pX, pY]);
                }
            }
        }
        return neighbours;
    }

    // returns distance between nodeA and nodeB based on heuristic class
    public float GetDistance(Node nodeA, Node nodeB)
    {
        float rValue = 0;
        rValue = Heuristic.GetDistanceEuclidean(nodeA, nodeB);
        return rValue;
    }
}
