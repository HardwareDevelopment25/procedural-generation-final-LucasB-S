using UnityEngine;
using UnityEngine;
using static ProGenTools;

public class ShapeCreator : MonoBehaviour
{
    void Start()
    {
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();

        meshFilter.mesh = CreateCube(10);

        meshRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        meshRenderer.material.color = Color.whiteSmoke;
    }
}
