using System.Collections.Generic;
using UnityEngine;

public class Distribution : MonoBehaviour
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
        Vector2Int newPos;

        for (int i = 0; i < 500; i++)
        {
            newPos = GetRandomPoint();
            positions.Add(newPos);
            Instantiate(orbPrefab, new Vector3(newPos.x, 0, newPos.y), Quaternion.identity);
        }
    }
}
