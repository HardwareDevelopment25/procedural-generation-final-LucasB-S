using System;
using UnityEngine;

public class PlaneGenerator : MonoBehaviour
{
    [Range(64, 512)]
    public int textureSize = 256;

    [Range(1, 100)]
    public float scale;

    [Range(1, 10)]
    public int LevelOfDetail = 1;

    private Texture2D texture;
    private float[,] finalMap;

    public int seed;

    [SerializeField]
    public AnimationCurve falloffCurve = new AnimationCurve();

    [System.Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color color;
    }

    public TerrainType[] terrainTypes;

    private void Start()
    {
        texture = new Texture2D(textureSize, textureSize);

        CreateLayeredPerlinPatteren();

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = MeshGenerator.GenerateGridMesh(finalMap, 10, falloffCurve, LevelOfDetail).CreateMesh();

        GetComponent<MeshRenderer>().material.mainTexture = texture;
    }
    private void CreateFractalPatteren()
    {
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                Color color = ((x & y) != 0) ? Color.white : Color.black;
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
    }

    private void CreateRandomPatteren()
    {
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                Color color = UnityEngine.Random.value > 0.5f ? Color.white : Color.black;
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
    }

    private void CreatePerlinPatteren()
    {
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                float xCoord = (float)x / texture.width * scale;
                float yCoord = (float)y / texture.height * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                Color color = new Color(sample, sample, sample);
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
    }

    public void CreateLayeredPerlinPatteren()
    {
        float[,] nm = NoiseMapGenerator.GenerateNoiseMap(textureSize, textureSize, scale, 6, 2f, 0.5f, seed, Vector2.zero);
        float[,] falloffMap = NoiseMapGenerator.GenerateFalloffMap(textureSize, falloffCurve);

        finalMap = new float[textureSize, textureSize];
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                finalMap[x, y] = Mathf.Clamp01(nm[x, y] - falloffMap[x, y]);
            }
        }


        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                float sample = finalMap[x, y];
                Color color = Color.black;

                for (int i = 0; i < terrainTypes.Length; i++)
                {
                    if (sample <= terrainTypes[i].height)
                    {
                        color = terrainTypes[i].color;
                        break;
                    }
                }
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
    }
}

