using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DungeonEdging : MonoBehaviour
{
    private class Node
    {
        // Final products of split space that rooms can be placed in

        public RectInt rect;
        public Node right;
        public Node left;
        public RectInt? room;

        public Node(RectInt r)
        {
            rect = r;
        }

        public bool IsLeaf()
        {
            return left == null && right == null;
        }

        public IEnumerable<Node> GetLeaf()
        {
            if (IsLeaf())
            {
                yield return this;
                yield break;
            }
            else
            {
                if (left != null)
                {
                    foreach (var l in left.GetLeaf())
                    {
                        yield return l;
                    }
                }
                if (right != null)
                {
                    foreach (var r in right.GetLeaf())
                    {
                        yield return r;
                    }
                }
            }
        }

        public RectInt? GetAnyRoom()
        {
            if (room.HasValue)
                return room;

            RectInt? r = left?.GetAnyRoom();
            if( r.HasValue)
                return r;

            return right?.GetAnyRoom();
        }
    }

    [System.Serializable]
    public struct Edge
    {
        public Vector2Int start;
        public Vector2Int end;
        public char face;

        public Edge(Vector2Int s, Vector2Int e, char f)
        {
            start = s;
            end = e;
            face = f;
        }
    }

    [SerializeField]
    public List<Edge> edges = new();

    public int mapWidth = 20;
    public int mapHeight = 20;
    public int mapBorder = 2;

    public int maxDepth = 5;
    public int minLeafSize = 5;

    public Vector2Int maxRoomSize = new Vector2Int(10, 10);
    public Vector2Int minRoomSize = new Vector2Int(3, 3);
    public int smallestPossibleRoomSize = 6;

    public float tileSize = 1.0f;
    public int corridorWidth = 1;

    public float BiasToLongerRooms = 0.5f;

    public Material floorMat;

    private bool[,] grid;
    private System.Random rand;

    private List<RectInt> rooms = new();
    private List<RectInt> corridors = new();
    private List<RectInt> leafs = new();

    private GameObject floorParent;
    public GameObject wallPrefab;

    private void Start()
    {
        Generate();

        edges = SimplifiyEdges(GroupNeighbouringEdges(DetectEdges(grid)));
        MarksSpawnWalls(edges);
    }

    private List<Edge> SimplifiyEdges(List<Edge> list)
    {
        List<Edge> finalEdges = new();

        Edge edge = new();

        for (int i = 0; i <= list.Count - 1; i++)
        {
            edge = list[i];

            if (list[i].face == 'N' || list[i].face == 'S') // North or South
            {
                while (i + 1 < list.Count && list[i].start.y == list[i + 1].start.y && (list[i].start.x + 1) == list[i + 1].start.x)
                {
                    edge.end = list[i].end;
                    i += 1;
                }
            }
            else if (list[i].face == 'E' || list[i].face == 'W')
            {
                while (i + 1 < list.Count && list[i].start.x == list[i + 1].start.x && (list[i].start.y + 1) == list[i + 1].start.y)
                {
                    edge.end = list[i].end;
                    i += 1;
                }
            }

            finalEdges.Add(edge);
        }

        return finalEdges;
    }

    private List<Edge> DetectEdges(bool[,] grid)
    {
        List<Edge> edges = new();

        for (int j = 0; j < mapHeight; j++)
        {
            for (int i = 0; i < mapWidth; i++)
            {
                if (grid[i, j] == true)
                {
                    if (j == 0 || grid[i,j - 1] == false) //North
                    {
                        edges.Add(new Edge(new Vector2Int(i,j), new Vector2Int(i + 1,j), 'N'));
                    }    

                    if (j == mapHeight - 1 || grid[i, j+1] == false) //South
                    {
                        edges.Add(new Edge(new Vector2Int(i, j + 1), new Vector2Int(i + 1, j + 1), 'S'));
                    }

                    if (i == mapWidth - 1 || grid[i + 1, j] == false) //East
                    {
                        edges.Add(new Edge(new Vector2Int(i + 1, j), new Vector2Int(i + 1, j + 1), 'E'));
                    }

                    if (i == 0 || grid[i - 1, j] == false) //West
                    {
                        edges.Add(new Edge(new Vector2Int(i, j), new Vector2Int(i, j + 1), 'W'));
                    }
                }
            }
        }

        return edges;
    }

    private List<Edge> GroupNeighbouringEdges(List<Edge> list)
    {
        List<Edge> finalList = new();

        int n = 0;
        int e = 0;
        int s = 0;
        int w = 0;

        foreach (Edge edge in list)
        {
            if(edge.face == 'N')
            {
                finalList.Insert(n, edge);
                n++;
                e++;
                s++;
                w++;
            }
            else if(edge.face == 'E')
            {
                finalList.Insert(e, edge);
                e++;
                s++; 
                w++;
            }
            else if(edge.face == 'S')
            {
                finalList.Insert(s, edge);
                s++;
                w++;
            }
            else
            {
                finalList.Insert(w, edge);
                w++;
            }
        }

        return finalList;
    }

    private void SpawnWalls()
    {
        foreach (Edge edge in edges)
        {
            float size;
            float midPoint;
            Quaternion rotation;
            Vector3 position;
            GameObject obj;

            char face = edge.face;
            if (face == 'N' ||  face == 'S')
            { 
                size = Math.Abs((edge.start.x - (tileSize / 2)) - edge.end.x + (tileSize / 2)) * tileSize;
                midPoint = edge.start.x - (tileSize / 2) + ((size + 1) / 2);

                if (face == 'N')
                    rotation = Quaternion.Euler(90, 0, 0);
                else
                    rotation = Quaternion.Euler(-90, 0, 0);

                position = new Vector3(midPoint, 0.5f, edge.start.y - (tileSize / 2));
            }
            else
            {
                size = Math.Abs(Vector2Int.Distance(edge.start, edge.end) * tileSize);

                midPoint = edge.start.y + (size / 2);

                if (face == 'E')
                    rotation = Quaternion.Euler(90, 90, 0);
                else
                    rotation = Quaternion.Euler(-90, -90, 0);

                position = new Vector3(midPoint, 0.5f, edge.start.y - (tileSize / 2));
            }

            obj = GameObject.Instantiate(wallPrefab, position, rotation);
            obj.transform.localScale = new Vector3(size * 0.1f, 1f, 0.1f);
            obj.transform.parent = this.transform;
            obj.name = $"Wall_{edge.face}";
        }
    }
    private void MarksSpawnWalls(List<Edge> merged)
    {
        var parentWalls = new GameObject("Walls");
        parentWalls.transform.SetParent(transform, false);

        foreach (var e in merged)
        {
            bool horizontal = (e.start.y == e.end.y);

            // Length in grid units
            float gridLength = horizontal
                ? Mathf.Abs(e.end.x - e.start.x)
                : Mathf.Abs(e.end.y - e.start.y);

            // Midpoint in grid coords
            float midX = (e.start.x + e.end.x) * 0.5f;
            float midY = (e.start.y + e.end.y) * 0.5f;

            // Convert grid → world (x,z plane)
            Vector3 worldPos = new Vector3(
                midX * tileSize,
                0f,
                midY * tileSize
            );

            Quaternion rot = horizontal
                ? Quaternion.Euler(0f, 0f, 0f)     // along world X
                : Quaternion.Euler(0f, 90f, 0f);   // along world Z

            var wall = Instantiate(wallPrefab, worldPos, rot, parentWalls.transform);

            // Scale so X dimension covers length * tileSize
            Vector3 s = wall.transform.localScale;
            s.x = gridLength * tileSize;
            wall.transform.localScale = s;
        }
    }

    private void Generate()
    {
        ClearPrevSpawnedTiles();

        rand = new System.Random();

        RectInt rootRect = new RectInt(mapBorder, mapBorder,
            Mathf.Max(smallestPossibleRoomSize, mapWidth - mapBorder),
            Mathf.Max(smallestPossibleRoomSize, mapHeight - mapBorder));

        Node rootNode = new Node(rootRect);

        SplitRecursive(rootNode, 0);

        foreach (var leaf in rootNode.GetLeaf())
        {
            var room = CreateRoomInsideLeaf(leaf.rect);
            leaf.room = room;
            rooms.Add(room);
            leafs.Add(leaf.rect);
        }

        grid = new bool[mapWidth, mapHeight];

        ConnectTree(rootNode);
        DrawRoomsAndCorridors();

        //Instantiate floor cubes from _grid

        SpawnFloorCubes();
    }

    private void FillRect(RectInt r, bool value)
    {
        // Use r.xMax/r.yMax as exclusive upper bounds (RectInt.xMax is already exclusive)
        int startX = Mathf.Clamp(r.xMin, 0, mapWidth);
        int endX = Mathf.Clamp(r.xMax, 0, mapWidth);
        int startY = Mathf.Clamp(r.yMin, 0, mapHeight);
        int endY = Mathf.Clamp(r.yMax, 0, mapHeight);

        for(int y = startY; y < endY; y++)
            for(int x = startX; x < endX; x++)
            {
                grid[x, y] = value;
            }
    }

    private void DrawRoomsAndCorridors()
    {
        foreach (var room in rooms)
        { 
            FillRect(room, true);
        }

        foreach (var corridor in corridors)
        {
            FillRect(corridor, true);
        }
    }

    private void CreateCorridor(Vector2Int a, Vector2Int b)
    {
        if(a.y == b.y)
        {
            int StartX = Mathf.Min(a.x, b.x);
            int w = Mathf.Abs(a.x - b.x) + 1;
            RectInt corridor = new RectInt(StartX - corridorWidth, a.y - corridorWidth / 2, w, corridorWidth);
            corridors.Add(corridor);
            return;
        }

        if(a.x == b.x)
        {
            int StartY = Mathf.Min(a.y, b.y);
            int h = Mathf.Abs(a.y - b.y) + 1;
            RectInt corridor = new RectInt(a.x - corridorWidth / 2, StartY, corridorWidth, h);
            corridors.Add(corridor);
            return;
        }
    }

    private void ConnectTree(Node node)
    {
        if (node == null || node.IsLeaf()) 
            return;

        ConnectTree(node.left);
        ConnectTree(node.right);

        var leftRoom = node.left.GetAnyRoom();
        var rightRoom = node.right.GetAnyRoom();

        var a = GetRoomCenter(leftRoom.Value);
        var b = GetRoomCenter(rightRoom.Value);

        var middlePoint = new Vector2Int(b.x, a.y);

        CreateCorridor(a, middlePoint);
        CreateCorridor(middlePoint, b);
    }

    private Vector2Int GetRoomCenter(RectInt r)
    {
        return new Vector2Int(Mathf.RoundToInt(r.center.x), Mathf.RoundToInt(r.center.y));
    }

    private void SpawnFloorCubes()
    {
        var floorParent = new GameObject("BSP Dungeon Floor").transform;

        for (int y = 0; y < mapHeight; y++)
            for (int x = 0; x < mapWidth; x++)
            {
                if (grid != null && grid[x,y])
                {
                    var floorCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    floorCube.name = $"Floor_{x}_{y}";
                    floorCube.transform.position = new Vector3(x * tileSize, 0, y * tileSize);
                    floorCube.transform.localScale = new Vector3(tileSize, 0.1f, tileSize);
                    floorCube.transform.parent = floorParent; 
                }
            }
    }

    private void ClearPrevSpawnedTiles()
    {
        GameObject.Destroy(floorParent);
    }

    private void SplitRecursive(Node node, int depth)
    {
        if (depth > maxDepth ||
            node.rect.width < minLeafSize * 2 && node.rect.height < minLeafSize * 2)
            return;

        // Can fit 2 leafs in horizontally / vertically
        bool canSplitH = node.rect.height >= minLeafSize * 2;
        bool canSplitV = node.rect.width >= minLeafSize * 2;
        if (!canSplitH && !canSplitV)
            return;

        bool splitVert;

        if (node.rect.width > node.rect.height)
        {
            splitVert = true;
        }
        else if (node.rect.height > node.rect.width)
        {
            splitVert = false;
        }
        else
        {
            splitVert = rand.NextDouble() < BiasToLongerRooms;
        }

        if (splitVert)
        {
            // Split vertically
            int minX = node.rect.xMin + minLeafSize;
            int maxX = node.rect.xMax - minLeafSize;

            // double checking bounds
            if (minX >= maxX)
                return;

            int splitX = rand.Next(minX, maxX);

            var left = new RectInt(node.rect.xMin, node.rect.yMin,
                splitX - node.rect.xMin, node.rect.height);
            var right = new RectInt(splitX, node.rect.yMin,
                node.rect.xMax - splitX, node.rect.height);

            node.left = new Node(left);
            node.right = new Node(right);
        }
        else
        {
            // Split horizontally
            int minY = node.rect.yMin + minLeafSize;
            int maxY = node.rect.yMax - minLeafSize;

            if (minY >= maxY)
                return;

            int splitY = rand.Next(minY, maxY);

            var top = new RectInt(node.rect.xMin, node.rect.yMin,
                node.rect.width, splitY - node.rect.yMin);

            var bottom = new RectInt(node.rect.xMin, splitY,
                node.rect.width, node.rect.yMax - splitY);

            node.left = new Node(top);
            node.right = new Node(bottom);

        }

        SplitRecursive(node.left, depth + 1);
        SplitRecursive(node.right, depth + 1);
    }

    private RectInt CreateRoomInsideLeaf(RectInt leaf)
    {
        int maxRoomWidth = Mathf.Min(maxRoomSize.x, leaf.width - 2 * mapBorder);
        int maxRoomHeight = Mathf.Min(maxRoomSize.y, leaf.height - 2 * mapBorder);

        int minRoomWidth = Mathf.Min(minRoomSize.x, maxRoomWidth);
        int minRoomHeight = Mathf.Min(minRoomSize.y, maxRoomHeight);

        if(minRoomWidth <= 0 || minRoomHeight <= 0)
        {
            return new RectInt((int)leaf.center.x, (int)leaf.center.y, 1, 1);
        }

        int roomWidth = rand.Next(minRoomWidth, maxRoomWidth) + 1;
        int roomHeight = rand.Next(minRoomHeight, maxRoomHeight) + 1;

        int roomX = rand.Next(leaf.xMin + mapBorder, leaf.xMax - mapBorder - roomWidth + 1);
        int roomY = rand.Next(leaf.yMin + mapBorder, leaf.yMax - mapBorder - roomHeight + 1);

        return new RectInt(roomX, roomY, roomWidth, roomHeight);
    }
}
