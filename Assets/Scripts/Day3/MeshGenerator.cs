using UnityEngine;

public static class MeshGenerator
{ 
    public static MeshData GenerateGridMesh(float[,] heightMap, float heightMultiplier, AnimationCurve curve, int LOD)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        int simplificationIncrement = (LOD == 0) ? 1 : LOD * 2;
        int verticesPerLine = (width - 1) / simplificationIncrement + 1;

        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);

        int vertexIndex = 0;
        int yIndex = 0;

        for (int y = 0; y < height; y += simplificationIncrement, yIndex++)
        {
            int xIndex = 0;
            for (int x = 0; x < width; x += simplificationIncrement, xIndex++)
            {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, curve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)(width - 1), y / (float)(height - 1));

                if (xIndex < verticesPerLine - 1 && yIndex < verticesPerLine - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData;
    }
}

public class MeshData
{
    public Vector3[] vertices;  
    public Vector2[] uvs;
    public int[] triangles;

    int triangleIndex = 0;

    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }
}
