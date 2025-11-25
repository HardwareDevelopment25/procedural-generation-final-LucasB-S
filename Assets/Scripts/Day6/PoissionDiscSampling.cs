using System.Collections.Generic;
using UnityEngine;

public class PoissionDiscSampling : MonoBehaviour
{
    public int mapWidth;
    public int mapHeight;

    public int seed;
    private System.Random rand;

    private List<Vector2Int> positions = new();

    public GameObject orbPrefab;

    private void Awake()
    {
        rand = new System.Random(seed);
    }

    private void Start()
    {
        Generate();
    }

    private Vector2Int GetRandomPoint()
    {
        int x = rand.Next(0, mapWidth);
        int y = rand.Next(0, mapHeight);

        return new Vector2Int(x, y);
    }

    private void Generate()
    {
        for (int j = 0; j < 500; j++)
        {
            float bestDistance = -1f;
            Vector2Int bestCandidate = Vector2Int.zero;

            for (int i = 0; i < 30; i++)
            {
                Vector2Int candidate = GetRandomPoint();
                Vector2Int closestSample = FindClosestSample(candidate);
                float distance = GetDistance(candidate, closestSample);
                if (distance > bestDistance)
                {
                    bestDistance = distance;
                    bestCandidate = candidate;
                }
            }

            positions.Add(bestCandidate);
        }

        foreach (var pos in positions)
        {
            Instantiate(orbPrefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
        }
    }

    private float GetDistance(Vector2Int a, Vector2Int b)
    {
        return Vector2Int.Distance(a, b);
    }

    private Vector2Int FindClosestSample(Vector2Int a)
    {
        if (positions.Count == 0)
            return a;

        float currentClosestDistance = float.MaxValue;
        Vector2Int closestSample = positions[0];
        foreach (var sample in positions)
        {
            float distance = GetDistance(a, sample);
            if (distance < currentClosestDistance)
            {
                currentClosestDistance = distance;
                closestSample = sample;
            }
        }

        return closestSample;
    }
}