using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CaveGenerator : MonoBehaviour
{
    public int CaveWidthAndHeight; // Has to be odd if you want outer maze perimeter to be 1 wall thick
    private int caveWidth;
    private int caveHeight;
    public int seed;

    private List<int> directions = new List<int>();
    private Stack<Vector2Int> minerPath = new Stack<Vector2Int>();

    private bool[,] caveGrid;
    private Vector2Int minerPos;

    public GameObject WallPrefab;
    public GameObject FloorPrefab;
    public GameObject Camera;

    System.Random rand;

    private void Awake()
    {
        rand = new System.Random(seed);
    }
    void Start()
    {
        // cave width and cave height need to be odd numbers
        caveWidth = CaveWidthAndHeight;
        caveHeight = CaveWidthAndHeight;

        caveGrid = new bool[caveWidth, caveHeight];
        Camera.transform.position = new Vector3((int)(caveWidth / 2), (caveWidth + caveHeight) / 2, (int)(caveHeight / 2));
        minerPos = new Vector2Int(caveWidth / 2, caveHeight / 2);
        minerPath.Push(minerPos);

        caveGrid[minerPos.x, minerPos.y] = true;

        GenerateCaveLayout();
        RenderCave();
    }

    private void GenerateCaveLayout()
    {
        while(minerPath.Count > 0)
        {
            Getneighbours();

            if (directions.Count > 0)
            {
                int move = rand.Next(0, directions.Count);

                switch (directions[move])
                {
                    case 0: // Up
                        minerPos.y += 2;
                        caveGrid[minerPos.x, minerPos.y - 1] = true;
                        caveGrid[minerPos.x, minerPos.y] = true;
                        minerPath.Push(minerPos);
                        break;

                    case 1: //Right
                        minerPos.x += 2;
                        caveGrid[minerPos.x - 1, minerPos.y] = true;
                        caveGrid[minerPos.x, minerPos.y] = true;
                        minerPath.Push(minerPos);
                        break;

                    case 2: //Down
                        minerPos.y -= 2;
                        caveGrid[minerPos.x, minerPos.y + 1] = true;
                        caveGrid[minerPos.x, minerPos.y] = true;
                        minerPath.Push(minerPos);
                        break;

                    case 3: //Left
                        minerPos.x -= 2;
                        caveGrid[minerPos.x + 1, minerPos.y] = true;
                        caveGrid[minerPos.x, minerPos.y] = true;
                        minerPath.Push(minerPos);
                        break;
                }
            }
            else
            {
                minerPath.Pop();
                if(minerPath.Count > 0)
                    minerPos = minerPath.Peek();
            }

            directions = new List<int>(0);
        }
    }

    private void Getneighbours()
    {
        if (minerPos.y + 2 < caveHeight - 1 && caveGrid[minerPos.x, minerPos.y + 2] == false) //Up
            directions.Add(0);

        if (minerPos.x + 2 < caveWidth - 1 && caveGrid[minerPos.x + 2, minerPos.y] == false) //Right
            directions.Add(1);

        if (minerPos.y - 1 > 1 && caveGrid[minerPos.x, minerPos.y - 2] == false) //Down
            directions.Add(2);

        if (minerPos.x - 1 > 1 && caveGrid[minerPos.x - 2, minerPos.y] == false) //Left
            directions.Add(3);
    }



    private void RenderCave()
    {
        for (int x = 0; x < caveWidth; x++)
            for(int z = 0; z < caveHeight; z++)
            {
                Vector3 pos = new Vector3(x, 0, z);

                if (caveGrid[x, z] == false)
                    GameObject.Instantiate(WallPrefab, pos, Quaternion.identity);
                else
                    GameObject.Instantiate(FloorPrefab, pos, Quaternion.identity);
            }
    }
}
