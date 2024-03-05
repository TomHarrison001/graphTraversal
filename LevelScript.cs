using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PMLevelScript : MonoBehaviour
{
    UIControllerScript uiController;

    public GameObject[] levelComponents;
    private const int width = 30;
    private const int height = 33;
    private const int nNodes = width * height;
    private string levelString;
    char[,] map;
    bool[,] adjmat;
    GameObject[,] nodePos;
    List<GameObject> nodeList = new List<GameObject>();
    public bool searchingPath;

    private void Start()
    {
        uiController = GameObject.Find("ConsoleCanvas").GetComponent<UIControllerScript>(); //IGNORE

        ReadLevelFile();
        StoreMap();
        InitAdjMat();
        DrawLevelBoard();
        CompleteAdjMat();
        PrintAdjMatTraversalTotal();
    }

    private void ReadLevelFile()
    {
        levelString = "";
        levelString = FileReader.ReadString("Level1.txt");
    }

    private void StoreMap()
    {
        map = new char[width, height];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                map[j, i] = levelString[((i * width) + j)];
            }
        }
    }

    private void InitAdjMat()
    {
        adjmat = new bool[nNodes, nNodes];
        for (int i = 0; i < nNodes; i++)
        {
            for (int j = 0; j < nNodes; j++)
            {
                adjmat[i, j] = false;
            }
        }
    }

    /* Draw the map

    The map is represented by an array of characters
    Each of this characters is represented by a 19x19 pixel square
    Each map square is drawn differently dependant on the character in the map
    0 - nothing
    1 - a dot
    P - proton pill
    - - horizontal line
    | - vertical line
    / - top left corner
    \ - top right corner
    L - bottom left corner
    I - bottom right corner
    = - horizonal pink line

    */

    private void DrawLevelBoard()
    {
        Vector2 boardPos = new Vector2(0, 0);
        nodePos = new GameObject[width, height];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                boardPos.x = (j * 0.25f) - 8;
                boardPos.y = (i * -0.25f) + 4;
                switch (map[j, i])
                {
                    case '0': // Draw blank
                        nodePos[j, i] = Instantiate(levelComponents[0], boardPos, levelComponents[0].transform.rotation, transform);
                        nodeList.Add(nodePos[j, i]);
                        break;

                    case '1': // Draw edible dot
                        nodePos[j, i] = Instantiate(levelComponents[1], boardPos, levelComponents[1].transform.rotation, transform);
                        nodeList.Add(nodePos[j, i]);
                        break;

                    case 'P': // Draw a proton pill
                        nodePos[j, i] = Instantiate(levelComponents[2], boardPos, levelComponents[2].transform.rotation, transform);
                        nodeList.Add(nodePos[j, i]);

                        break;
                    case '-': // Draw a horizontal line
                        nodePos[j, i] = Instantiate(levelComponents[3], boardPos, levelComponents[3].transform.rotation, transform);
                        nodeList.Add(nodePos[j, i]);
                        break;

                    case '=': // Draw a horizontal line with the alternative colour
                        nodePos[j, i] = Instantiate(levelComponents[4], boardPos, levelComponents[4].transform.rotation, transform);
                        nodeList.Add(nodePos[j, i]);
                        break;

                    case '|': // Draw a vertical line
                        nodePos[j, i] = Instantiate(levelComponents[5], boardPos, levelComponents[5].transform.rotation, transform);
                        nodeList.Add(nodePos[j, i]);
                        break;

                    case '/': // Draw a top left corner
                        nodePos[j, i] = Instantiate(levelComponents[6], boardPos, levelComponents[6].transform.rotation, transform);
                        nodeList.Add(nodePos[j, i]);
                        break;

                    case '\\': // Draw a top right corner
                        nodePos[j, i] = Instantiate(levelComponents[7], boardPos, levelComponents[7].transform.rotation, transform);
                        nodeList.Add(nodePos[j, i]);
                        break;

                    case 'L': // Draw a bottom left corner
                        nodePos[j, i] = Instantiate(levelComponents[8], boardPos, levelComponents[8].transform.rotation, transform);
                        nodeList.Add(nodePos[j, i]);
                        break;

                    case 'I': // Draw a bottom right corner
                        nodePos[j, i] = Instantiate(levelComponents[9], boardPos, levelComponents[9].transform.rotation, transform);
                        nodeList.Add(nodePos[j, i]);
                        break;

                }
            }
        }
    }

    private void CompleteAdjMat()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (Traversable(map[j, i]))
                {
                    int position = i * width + j;

                    if (j > 0)
                    {
                        if (Traversable(map[j - 1, i]))
                            adjmat[position, position - 1] = true;
                    }
                    if (j < width - 1)
                    {
                        if (Traversable(map[j + 1, i]))
                            adjmat[position, position + 1] = true;
                    }
                    if (i > 0)
                    {
                        if (Traversable(map[j, i - 1]))
                        {
                            int abovePosition = (i - 1) * width + j;
                            adjmat[position, abovePosition] = true;
                        }
                    }
                    if (i < height - 1)
                    {
                        if (Traversable(map[j, i + 1]))
                        {
                            int belowPosition = (i + 1) * width + j;
                            adjmat[position, belowPosition] = true;
                        }
                    }
                }
            }
        }
    }

    public bool Traversable(char pos)
    {
        return (pos == '0' || pos == '1' || pos == 'P');
    }

    public void PMTouchingNode(int x, int y)
    {
        nodePos[y, x].GetComponent<SpriteRenderer>().sprite = levelComponents[0].GetComponent<SpriteRenderer>().sprite;
    }

    private void PrintAdjMatTraversalTotal()
    {
        int traversalTotal = 0;

        for (int i = 0; i < nNodes; i++)
        {
            for (int j = 0; j < nNodes; j++)
            {
                if (adjmat[i, j] == true)
                    traversalTotal++;
            }
        }

        uiController.PrintLine("Total Traversable Nodes: " + traversalTotal);
    }

    public List<int> BFSPath(int rootNode, int goalNode)
    {
        searchingPath = true;

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

            if (currentNode == goalNode) break;

            for (int i = 0; i < nNodes; i++)
            {
                if (adjmat[currentNode, i] && !visitedNodes[i])
                {
                    nodeQueue.Enqueue(i);
                    visitedNodes[i] = true;
                    parentNode[i] = currentNode;
                }
            }
        }

        int j = goalNode;
        while (j != -1)
        {
            path.Add(j);
            j = parentNode[j];
        }
        path.Reverse();

        searchingPath = false;
        return path;
    }

    public bool[,] GetAdjMat
    {
        get
        {
            return adjmat;
        }
    }

    public GameObject[,] GetNodePos
    {
        get
        {
            return nodePos;
        }
    }

    public int GetWidth
    {
        get
        {
            return width;
        }
    }

    public int GetHeight
    {
        get
        {
            return height;
        }
    }

    public GameObject GetNodeGameObject(int nodePos)
    {
        return nodeList[nodePos];
    }

    public char GetMapChar(int j, int i)
    {
        return map[j, i];
    }
}