using NUnit.Framework.Internal;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] private int defaultSeed = 0;
    public int seed;

    [SerializeField]
    public AnimationCurve falloffCurve = new AnimationCurve();

    MeshFilter meshFilter;

    [SerializeField] private TextMeshProUGUI IslandSizeVal;
    [SerializeField] private TextMeshProUGUI IslandNormalMapScaleVal;
    [SerializeField] private TextMeshProUGUI IslandLODMultiplierVal;
    [SerializeField] private Toggle RegenCloudsWithIsland;
    [SerializeField] private TMP_InputField islandSeed;

    [SerializeField] private RandomCloudDistrobution Clouds;

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
        seed = defaultSeed;
        texture = new Texture2D(textureSize, textureSize);

        CreateLayeredPerlinPatteren();

        meshFilter = gameObject.AddComponent<MeshFilter>();
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

    public void ReGenerate()
    {
        if (IslandSizeVal != null)
            textureSize = int.Parse(IslandSizeVal.text);
        else
            Debug.Log("IslandSize is empty");

        if (IslandNormalMapScaleVal != null)
            scale = int.Parse(IslandNormalMapScaleVal.text);
        else
            Debug.Log("IslandNormalMapScale is empty");

        if (IslandLODMultiplierVal != null)
            LevelOfDetail = int.Parse(IslandLODMultiplierVal.text);
        else
            Debug.Log("CloudCountUIVal is empty");

        if (islandSeed != null)
        {
            seed = int.Parse(islandSeed.text);
        }
        else
        {
            seed = defaultSeed;
            Debug.Log("CloudSeed is empty");
        }

        texture = new Texture2D(textureSize, textureSize);

        CreateLayeredPerlinPatteren();

        meshFilter.mesh = new Mesh();
        meshFilter.mesh = MeshGenerator.GenerateGridMesh(finalMap, 10, falloffCurve, LevelOfDetail).CreateMesh();

        GetComponent<MeshRenderer>().material.mainTexture = texture;

        if (RegenCloudsWithIsland == null)
            Debug.Log("CloudCountUIVal is empty");
        else
            if (RegenCloudsWithIsland.isOn)
                Clouds.ReGenerate();
    }
}

