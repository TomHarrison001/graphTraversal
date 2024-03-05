using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class Heuristic
{
    public static float GetDistanceEuclidean(Node nodeA, Node nodeB)
    {
        float total = Mathf.Pow(nodeB.x - nodeA.x, 2) + Mathf.Pow(nodeB.y - nodeA.y, 2);
        return Mathf.Sqrt(total);
    }

    public static float GetDistanceEuclideanNoSqr(Node nodeA, Node nodeB)
    {
        return Mathf.Pow(nodeB.x - nodeA.x, 2) + Mathf.Pow(nodeB.y - nodeA.y, 2);
    }

    public static float GetDistanceManhattan(Node nodeA, Node nodeB)
    {
        int cost = 1;
        int dist = Mathf.Abs(nodeB.x - nodeA.x) + Mathf.Abs(nodeB.y - nodeA.y);
        return cost + dist;
    }

    public static float GetDistanceDiag(Node nodeA, Node nodeB)
    {
        int cost = 1;
        int x = Mathf.Abs(nodeB.x - nodeA.x);
        int y = Mathf.Abs(nodeB.y - nodeA.y);
        return cost * Mathf.Max(x, y);
    }
    public static float GetDistanceDiagShort(Node nodeA, Node nodeB)
    {
        float cost = 1.41f;
        int x = Mathf.Abs(nodeB.x - nodeA.x);
        int y = Mathf.Abs(nodeB.y - nodeA.y);
        return cost * Mathf.Min(x, y) + Mathf.Abs(x - y);
    }
}