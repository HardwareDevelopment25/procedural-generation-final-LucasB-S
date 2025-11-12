using System.Collections;
using UnityEngine;

public class CelularAutonima : MonoBehaviour
{
    public int gridSize = 0;
    public float percentageOfOnes = 0.0f;
    public int seed = 0;

    private System.Random rand;

    private int[,] intGrid;
    private void Awake()
    {
        intGrid = new int[gridSize, gridSize];

        Random.InitState(seed);
    }

    private void Start()
    {
        for (int i = 0; i < gridSize; i++)
            for (int j = 0; j < gridSize; j++)
                intGrid[i, j] = Random.value < percentageOfOnes ? 1 : 0;

        GetComponent<Renderer>().material.mainTexture = RenderIntArrayAs2DTexture(intGrid);

        StartCoroutine(SecondsDelay(5));
    }

    private IEnumerator SecondsDelay(float seconds)
    {
        while (true)
        {
            intGrid = GenerateCave(intGrid);
            //intGrid = RunGameOfLifeIteration(intGrid);
            GetComponent<Renderer>().material.mainTexture = RenderIntArrayAs2DTexture(intGrid);
            yield return new WaitForSeconds(seconds);
        }
    }

    public static Texture2D RenderIntArrayAs2DTexture(int[,] grid)
    {
        int length = grid.GetLength(0);
        int width = grid.GetLength(1);

        Texture2D texture = new(length, width);

        grid = CreateBorder(grid);

        for (int x = 0; x < width; x++)
            for (int z = 0; z < length; z++)
            {
                Color colour = grid[x, z] == 1 ? Color.lightGray : Color.grey;
                texture.SetPixel(x, z, colour);
            }

        texture.Apply();

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        return texture;
    }

    public static int[,] CreateBorder(int[,] grid)
    {
        int length = grid.GetLength(0);
        int width = grid.GetLength(1);
        for (int x = 0; x < width; x++)
        {
            grid[x, 0] = 0;
            grid[x, length - 1] = 0;
        }
        for (int z = 0; z < length; z++)
        {
            grid[0, z] = 0;
            grid[width - 1, z] = 0;
        }

        return grid;
    }

    public static int GetAllActiveNeighbouringCells(int[,] grid, int x, int y)
    {
        int activeNeighbours = 0;

            if (grid[x, y + 1] == 1) // Top Cell
                activeNeighbours++;

            if (grid[x + 1, y + 1] == 1) // Top-Right Cell
                activeNeighbours++;

            if (grid[x + 1, y] == 1) // Right Cell
                activeNeighbours++;

            if (grid[x + 1, y - 1] == 1) // Bottom-Right Cell
                activeNeighbours++;

            if (grid[x, y - 1] == 1) // Bottom Cell
                activeNeighbours++;

            if (grid[x - 1, y - 1] == 1) // Bottom-Left Cell
                activeNeighbours++;

            if (grid[x - 1, y] == 1) // Left Cell
                activeNeighbours++;

            if (grid[x - 1, y + 1] == 1) // Top-Left Cell
                activeNeighbours++;

            bool LeftBorder = (x == 1);
            bool RightBorder = (x == grid.GetLength(0) - 1);

            bool TopBorder = (y == 1);
            bool BottomBorder = (y == grid.GetLength(1) - 1);

            if (activeNeighbours >= 5)
                if (LeftBorder || RightBorder)
                    if (TopBorder || BottomBorder)
                        activeNeighbours -= 5;

            if (activeNeighbours >= 3)
                if (LeftBorder || RightBorder || TopBorder || BottomBorder)
                    activeNeighbours -= 3;


        return activeNeighbours;
    }

    public static int[,] GenerateCave(int[,] grid)
    {
        int length = grid.GetLength(0);
        int width = grid.GetLength(1);

        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < length - 1; y++)
            {
                int activeNeighbours = GetAllActiveNeighbouringCells(grid, x, y);
                if (grid[x, y] == 1) // Cell is a wall
                {
                    if (activeNeighbours > 4)
                        grid[x, y] = 1; // Cell becomes wall
                }
                else // Cell is air
                    if (activeNeighbours < 4) 
                        grid[x, y] = 0; // Cell becomes air

            }
        }

        return grid;
    }

    public static int[,] RunGameOfLifeIteration(int[,] grid)
    {
        int length = grid.GetLength(0);
        int width = grid.GetLength(1);

        int[,] newGrid = new int[length, width];
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < length - 1; y++)
            {
                int activeNeighbours = GetAllActiveNeighbouringCells(grid, x, y);
                if (grid[x, y] == 1) // Cell is currently alive
                {
                    if (activeNeighbours < 2 || activeNeighbours > 3)
                        newGrid[x, y] = 0; // Cell dies
                }
                else // Cell is currently dead
                {
                    if (activeNeighbours == 3)
                        newGrid[x, y] = 1; // Cell becomes alive
                }
            }
        }
        // Copy newGrid to grid
        for (int x = 0; x < width; x++)
            for (int y = 0; y < length; y++)
                grid[x, y] = newGrid[x, y];

        return grid;
    }

}
