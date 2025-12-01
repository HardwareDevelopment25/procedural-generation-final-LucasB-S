using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;

public class CellularAutomata : MonoBehaviour
{
    public int gridSize = 64;
    public float fillProbability = 0.4f;
    private int[,] grid;
    public int seed;
    System.Random rand;
    public float iterationDelay;
    public GameObject Rockprefab;
    public GameObject Airprefab;
    public GameObject Grassprefab;
    public GameObject Spikeprefab;
    public GameObject MarchinPrefab;
    public int iterations;
    public Sprite[] MarchinSprites;

    void Awake()
    {
        rand = new System.Random(seed);
        grid = new int[gridSize, gridSize];
    }

    void Start()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                grid[x, y] = (rand.NextDouble() < fillProbability) ? 1 : 0;
            }
        }

        GenerateCave(grid);
        SetValuesInCave(grid);
        GenerateMarchinSquares();
        //DrawCave(grid);
        GetComponent<Renderer>().material.mainTexture = IntToBoolTexture(grid);
    }

    public static Texture2D IntToBoolTexture(int[,] grid)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        Texture2D texture = new Texture2D(width, height);

        CreateBorder(grid, 1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //Color color = (grid[x, y] == 1) ? Color.black : Color.white;
                //texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        return texture;
    }

    public static void CreateBorder(int[,] grid, int borderWidth)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x < borderWidth || x >= width - borderWidth || y < borderWidth || y >= height - borderWidth)
                {
                    grid[x, y] = 1;
                }
            }
        }
    }

    public static int CheckNeighbors(int x, int y, int[,] grid)
    {
        int activeNeighbors = 0;

        if (x > 2 && x < grid.GetLength(0) - 1 && y > 2 && y < grid.GetLength(1) - 1)
        {
            if (grid[x - 1, y] == 1) // Left
                activeNeighbors++;

            if (grid[x + 1, y] == 1) // Right
                activeNeighbors++;

            if (grid[x, y - 1] == 1) // Down
                activeNeighbors++;

            if (grid[x, y + 1] == 1) // Up
                activeNeighbors++;

            if (grid[x - 1, y - 1] == 1) // Down-Left
                activeNeighbors++;

            if (grid[x - 1, y + 1] == 1) // Up-Left
                activeNeighbors++;

            if (grid[x + 1, y - 1] == 1) // Down-Right
                activeNeighbors++;

            if (grid[x + 1, y + 1] == 1) // Up-Right
                activeNeighbors++;

            else if (y == 2)
                activeNeighbors -= 3;

            else if (y == grid.GetLength(1) - 2)
                activeNeighbors -= 3;

            else if (x == 2)
                activeNeighbors -= 3;

            else if (x == grid.GetLength(0) - 2)
                activeNeighbors -= 3;

        }

        return activeNeighbors;

    }

    public IEnumerator<WaitForSeconds> SecondsDelay()
    {
        yield return new WaitForSeconds(iterationDelay);
    }

    public void GenerateCave(int[,] grid)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);


        for (int step = 0; step < iterations; step++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int activeNeighbors = CheckNeighbors(x, y, grid);

                    if (activeNeighbors > 4)
                    {
                        grid[x, y] = 1;
                    }

                    else if (activeNeighbors < 4)
                    {
                        grid[x, y] = 0;
                    }
                }
            }
        }
    }

    public void SetValuesInCave(int[,] grid)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == 1)
                {
                    if (grid[x, y + 1] == 0)
                    {
                        // Set to Grass
                        grid[x, y] = 2;
                    }

                    if (grid[x, y - 1] == 0)
                    {
                        // Set to Spike
                        grid[x, y] = 3;
                    }
                }
            }
        }
    }

    void GenerateMarchinSquares()
    {
        for (int x = 1; x < gridSize - 1; x++)
        {
            for (int y = 1; y < gridSize - 1; y++)
            {
                PlaceCell(x, y, getConfigIndext(x, y));

                Debug.Log(getConfigIndext(x, y));
            }
        }

    }

    void PlaceCell(int x, int y, int index)
    {
        Vector3 position = new Vector3(x, 0, y);
        GameObject cell = GameObject.Instantiate(MarchinPrefab, position, Quaternion.identity, this.transform);
        cell.transform.rotation = Quaternion.Euler(90, 0, 0);
        SpriteRenderer spriteRenderer = cell.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = MarchinSprites[index];
    }

    int getConfigIndext(int x, int y)
    {
        int index = 0;

        if (grid[x, y] > 0)
        {
            index |= 1;
        }


        if (grid[x + 1, y] > 0)
        {
            index |= 2;
        }

        if (grid[x + 1, y + 1] > 0)
        {
            index |= 4;
        }

        if (grid[x, y + 1] > 0)
        {
            index |= 8;
        }

        return index;
    }

    public void DrawCave(int[,] grid)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int activeNeighbors = CheckNeighbors(x, y, grid);

                if (grid[x, y] == 1)
                {

                    GameObject.Instantiate(Rockprefab, new Vector3(x, 0, y), Quaternion.Euler(90,0,0), this.transform);

                }
                if (grid[x, y] == 0)
                {

                    GameObject.Instantiate(Airprefab, new Vector3(x, 0, y), Quaternion.Euler(90, 0, 0), this.transform);

                }
                if (grid[x, y] == 2)
                {

                    GameObject.Instantiate(Grassprefab, new Vector3(x, 0, y), Quaternion.Euler(90, 0, 0), this.transform);
                }
                if (grid[x, y] == 3)
                {

                    GameObject.Instantiate(Spikeprefab, new Vector3(x, 0, y), Quaternion.Euler(90, 0, 0), this.transform);
                }

            }
        }
    }

    public void RunGameOfLifeIteration(int[,] grid)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        int[,] newGrid = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int activeNeighbors = CheckNeighbors(x, y, grid);
                if (grid[x, y] == 1)
                {
                    // Cell is alive
                    if (activeNeighbors < 2 || activeNeighbors > 3)
                    {
                        newGrid[x, y] = 0; // Cell dies
                    }
                    else
                    {
                        newGrid[x, y] = 1; // Cell lives
                    }
                }
                else
                {
                    // Cell is dead
                    if (activeNeighbors == 3)
                    {
                        newGrid[x, y] = 1; // Cell becomes alive
                    }
                    else
                    {
                        newGrid[x, y] = 0; // Cell remains dead
                    }
                }
            }
        }
        // Copy newGrid back to grid
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = newGrid[x, y];
            }
        }




    }
}
