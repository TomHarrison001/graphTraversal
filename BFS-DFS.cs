using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFSDFSScript : MonoBehaviour
{
    private const int nNodes = 8;
    private UIControllerScript uiController;
    private bool[,] adjMat = new bool[nNodes, nNodes];

    private void Start()
    {
        uiController = GameObject.Find("ConsoleCanvas").GetComponent<UIControllerScript>();
        InitAdjMat();
        SetAdjMat();
        uiController.PrintLine("Input node into input field and select search type");
    }

    private void InitAdjMat()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                adjMat[i, j] = false;
            }
        }
    }

    private void SetAdjMat()
    {
        adjMat[0, 1] = true;

        adjMat[1, 0] = true;
        adjMat[1, 2] = true;
        adjMat[1, 3] = true;

        adjMat[2, 1] = true;

        adjMat[3, 1] = true;
        adjMat[3, 4] = true;
        adjMat[3, 6] = true;

        adjMat[4, 3] = true;
        adjMat[4, 5] = true;

        adjMat[5, 4] = true;
        adjMat[5, 6] = true;

        adjMat[6, 3] = true;
        adjMat[6, 5] = true;
        adjMat[6, 7] = true;

        adjMat[7, 6] = true;
    }

    public void BFS(int rootNode)
    {
        bool[] visitedNodes = new bool[nNodes];
        Queue<int> nodeQueue = new Queue<int>();
        int currentNode = rootNode;

        for (int i = 0; i < nNodes; i++)
        {
            visitedNodes[i] = false;
        }

        nodeQueue.Enqueue(currentNode);
        visitedNodes[currentNode] = true;

        while (nodeQueue.Count > 0)
        {
            currentNode = nodeQueue.Peek();
            nodeQueue.Dequeue();
            uiController.PrintLine(Convert.ToChar(currentNode + 65).ToString());
            for (int i = 0; i < nNodes; i++)
            {
                if (adjMat[currentNode, i] && !visitedNodes[i])
                {
                    nodeQueue.Enqueue(i);
                    visitedNodes[i] = true;
                }
            }
        }

        uiController.PrintLine("BFS Done! Root Node: " + Convert.ToChar(rootNode + 65).ToString());
        uiController.PrintLine("----------------------- \n");
    }

    public void BFSPath(int rootNode, int goalNode)
    {
        uiController.Clear();

        bool[] visitedNodes = new bool[nNodes];
        Queue<int> nodeQueue = new Queue<int>();
        int currentNode = rootNode;
        int[] parentNode = new int[nNodes];
        parentNode[currentNode] = -1;
        List<int> path = new List<int>();

        for (int i = 0; i < nNodes; i++)
        {
            visitedNodes[i] = false;
        }

        nodeQueue.Enqueue(currentNode);
        visitedNodes[currentNode] = true;

        while (nodeQueue.Count > 0)
        {
            currentNode = nodeQueue.Peek();
            nodeQueue.Dequeue();
            uiController.PrintLine(Convert.ToChar(currentNode + 65).ToString());

            if (currentNode == goalNode) break;

            for (int i = 0; i < nNodes; i++)
            {
                if (adjMat[currentNode, i] && !visitedNodes[i])
                {
                    nodeQueue.Enqueue(i);
                    visitedNodes[i] = true;
                    parentNode[i] = currentNode;
                }
            }
        }
        uiController.PrintLine("BFS Done!");
        uiController.NewLine();
        uiController.PrintLine("Path");
        
        int j = goalNode;
        while (j != -1)
        {
            path.Add(j);
            j = parentNode[j];
        }
        path.Reverse();

        foreach (int item in path)
        {
            uiController.PrintLine(Convert.ToChar(item + 65).ToString());
        }
        uiController.PrintLine("-----------------");
    }

    public void DFS(int rootNode)
    {
        bool[] visitedNodes = new bool[nNodes];
        Stack<int> nodeStack = new Stack<int>();
        int currentNode = rootNode;

        for (int i = 0; i < nNodes; i++)
        {
            visitedNodes[i] = false;
        }

        visitedNodes[rootNode] = true;
        nodeStack.Push(currentNode);

        while (nodeStack.Count != 0)
        {
            currentNode = nodeStack.Peek();
            nodeStack.Pop();

            uiController.PrintLine(Convert.ToChar(currentNode + 65).ToString());

            for (int i = 0; i < nNodes; i++)
            {
                if (adjMat[currentNode, i])
                {
                    if (!visitedNodes[i])
                    {
                        nodeStack.Push(i);
                        visitedNodes[i] = true;
                    }
                }
            }
        }

        uiController.PrintLine("DFS Done!");
        uiController.PrintLine("-----------------");
    }

    public void DFSRecursiveCall(int rootNode)
    {
        bool[] vNodes = new bool[nNodes];

        DFSRecursive(rootNode, vNodes);

        uiController.PrintLine("DFS Recursive Done!");
        uiController.PrintLine("-----------------");
    }

    public void DFSRecursive(int cNode, bool[] visitedNodes)
    {
        visitedNodes[cNode] = true;

        uiController.PrintLine(Convert.ToChar(cNode + 65).ToString());

        for (int i = 0; i < nNodes; i++)
        {
            if (adjMat[cNode, i] && !visitedNodes[i])
            {
                DFSRecursive(i, visitedNodes);
            }
        }
    }
}
