using Unity.VisualScripting;
using UnityEngine;

public class NoiseMapGenerator
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int octaves, float lacunarity, float persistance, int seed, Vector2 offset)
    {
        // Create 2D array to hold noise values
        float[,] noiseMap = new float[mapWidth, mapHeight];
        if (scale <= 0)
            scale = 0.0001f;

        // Track min and max noise values for normalization
        float minNoiseHeight = float.MaxValue;
        float maxNoiseHeight = float.MinValue;

        System.Random rand = new System.Random(seed);

        Vector2[] octaveOffsets = new Vector2[octaves];

        // Generate random offsets for each octave
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = rand.Next(-100000, 100000) + offset.x;
            float offsetY = rand.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        // Generate noise map
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                // Generate noise value for each octave
                for (int i = 0; i < octaves; i++)
                {
                    // Calculate sample coordinates
                    float sampleX = (x - (mapWidth / 2f)) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - (mapHeight / 2f)) / scale * frequency + octaveOffsets[i].y;

                    // Get Perlin noise value
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    // Accumulate noise height
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;

                }

                // Update min and max noise heights
                if (noiseHeight > maxNoiseHeight)
                    maxNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseHeight)
                    minNoiseHeight = noiseHeight;

                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseHeight);
            }
        }

        return noiseMap;
    }

    public static float[,] GenerateFalloffMap(int size, AnimationCurve ac)
    {
        float[,] falloffMap = new float[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;
                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                falloffMap[i, j] = ac.Evaluate(value);
            }
        }
        return falloffMap;
    }
}

