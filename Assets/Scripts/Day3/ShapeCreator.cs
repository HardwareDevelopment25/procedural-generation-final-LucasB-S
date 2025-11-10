using UnityEngine;
using UnityEngine.UIElements;

public class ShapeCreator : MonoBehaviour
{
    public int sizeOfGrid = 1;
    public float heightMultiplier = 5f;
    public int seed = 42;

    private float[,] noiseMap;
    private float[,] falloffMap;

    TextureGenerator textureGenerator;


    void Start()
    {
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();

        noiseMap = NoiseMapGenerator.GenerateNoiseMap(sizeOfGrid + 1, sizeOfGrid + 1, 10f, 4, 2f, 0.5f, seed, Vector2.zero);

       //textureGenerator.CreateLayeredPerlinPatteren(noiseMap);
        falloffMap = NoiseMapGenerator.GenerateFalloffMap(sizeOfGrid + 1, textureGenerator.falloffCurve);

        meshRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        meshRenderer.material.color = Color.whiteSmoke;
        //meshFilter.mesh = MeshGenerator.GenerateGridMesh(noiseMap, heightMultiplier).CreateMesh();
    }
}
