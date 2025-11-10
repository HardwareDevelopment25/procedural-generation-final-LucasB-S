using UnityEngine;

public class ProGenTools
{
    public static Mesh CreateRightAngledTriangle(float sizeOfTriangle)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[3]
        {
            new(0, 0, 0),
            new(sizeOfTriangle, 0, 0),
            new(0, 0, sizeOfTriangle)
        };

        Vector2[] uvs = new Vector2[3]
        {
            new(0, 0),
            new(1, 0),
            new(1, 1)
        };

        int[] triangles = new int[3]
        {
            0, 1, 2
        };

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        return mesh;
    }

    public static Mesh CreateSquare(float sizeOfSquare)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4]
        {
            new(0, 0, 0),
            new(sizeOfSquare, 0, 0),
            new(0, 0, sizeOfSquare),
            new(sizeOfSquare, 0, sizeOfSquare)
        };

        Vector2[] uvs = new Vector2[4]
        {
            new(0, 0),
            new(1, 0),
            new(0, 1),
            new(1, 1)
        };

        int[] faces = new int[6]
        {
            0, 2, 1,
            2, 3, 1
        };

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = faces;

        return mesh;
    }

    public static Mesh CreateCube(float sizeOfCube)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[8]
        {
            //Bottom vertices
            new(0, 0, 0),
            new(sizeOfCube, 0, 0),


            // 
            new(sizeOfCube, sizeOfCube, 0),

            new(0, sizeOfCube, 0),

            new(0, 0, sizeOfCube),

            new(sizeOfCube, 0, sizeOfCube),

            new(sizeOfCube, sizeOfCube, sizeOfCube),

            new(0, sizeOfCube, sizeOfCube)
        };
        int[] faces = new int[36]
        {
            //Front 
            0, 2, 1, 
            0, 3, 2,

            //Top
            1, 2, 6, 
            6, 5, 1,

            //
            4, 5, 6, 
            6, 7, 4,


            2, 3, 7, 
            7, 6, 2,

            0, 7, 3, 
            0, 4, 7,

            0, 1, 5, 
            0, 5, 4
        };
        mesh.vertices = vertices;
        mesh.triangles = faces;
        return mesh;
    }


    public static Texture2D RenderBoolArrayAs2DTexture(bool[,] maze)
    {
        int length = maze.GetLength(0);
        int width = maze.GetLength(1);

        Texture2D texture = new (maze.GetLength(0), maze.GetLength(1));

        for (int x = 0; x < width; x++)
            for (int z = 0; z < length; z++)
            {
                if (maze[x, z] == false)
                    texture.SetPixel(x, z, Color.red);
                else
                    texture.SetPixel(x, z, Color.blue);
            }

        texture.Apply();

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        return texture;
    }

    public static Texture2D RenderNoiseArrayAs2DTexture(float[,] maze)
    {
        int length = maze.GetLength(0);
        int width = maze.GetLength(1);

        Texture2D texture = new(maze.GetLength(0), maze.GetLength(1));

        for (int x = 0; x < width; x++)
            for (int z = 0; z < length; z++)
            {
               // texture.SetPixel(maze[x,z]., maze);
            }

        texture.Apply();

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        return texture;
    }
}
