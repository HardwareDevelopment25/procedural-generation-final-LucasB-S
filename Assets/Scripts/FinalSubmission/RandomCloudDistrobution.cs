using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RandomCloudDistrobution : MonoBehaviour
{
    private int mapWidth;
    private int mapLength;
    public int CloudHeight;
    public int CloudCount;
    public int CloudScale;
    public int CloudCandidateCycles = 30;

    public int seed;
    private System.Random rand;

    private List<Vector2Int> positions = new();
    public GameObject[] CloudPrefabs = new GameObject[3];
    public PlaneGenerator Island;

    private void Awake()
    {
        rand = new System.Random(seed);
        mapWidth = Island.textureSize;
        mapLength = Island.textureSize;
    }

    private void Start()
    {
        Generate();

        this.transform.position = new Vector3(-mapWidth / 2, CloudHeight, -mapLength / 2);
    }

    private Vector2Int GetRandomPoint()
    {
        int x = rand.Next(0, mapWidth);
        int y = rand.Next(0, mapLength);

        return new Vector2Int(x, y);
    }

    private void Generate()
    {
        for (int j = 0; j < CloudCount; j++)
        {
            float bestDistance = -1f;
            Vector2Int bestCandidate = Vector2Int.zero;

            for (int i = 0; i < CloudCandidateCycles; i++)
            {
                Vector2Int candidate = GetRandomPoint();
                Vector2Int closestSample = FindClosestSample(candidate);
                float distance = Vector2Int.Distance(candidate, closestSample);
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
            GameObject cloud = Instantiate(CloudPrefabs[rand.Next(0,3)], new Vector3(pos.x, 0, pos.y), Quaternion.Euler(new Vector3(0,rand.Next(0,360),0)),this.transform);
            cloud.transform.localScale = new Vector3(CloudScale,CloudScale / 2,CloudScale);

        }
    }

    private Vector2Int FindClosestSample(Vector2Int a)
    {
        if (positions.Count == 0)
            return a;

        float currentClosestDistance = float.MaxValue;
        Vector2Int closestSample = positions[0];
        foreach (var sample in positions)
        {
            float distance = Vector2Int.Distance(a, sample);
            if (distance < currentClosestDistance)
            {
                currentClosestDistance = distance;
                closestSample = sample;
            }
        }

        return closestSample;
    }
}
