using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

[CustomEditor(typeof(CaveTextureGenerator))]

public class MazeGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CaveTextureGenerator mazeGen = (CaveTextureGenerator)target;
        GUILayout.Label("---Configure Maze---", EditorStyles.largeLabel);
        EditorGUI.BeginChangeCheck();
        GUILayout.Space(10);

        if(DrawDefaultInspector())
        {
            mazeGen.GenerateImageOfNewMaze();
        }

        if(GUILayout.Button("Generate New Mze"))
        {
            mazeGen.GenerateImageOfNewMaze();
        }
    }
}

public class MazeGeneratorWindow : EditorWindow
{
    public int InitialMazeScale = 32;

    [MenuItem("Tools/Generate Maze By Scale")]

    public static void ShowWindow()
    {
        GetWindow<MazeGeneratorWindow>();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(10);
        GUILayout.Label("Maze Generator, will generate maze in scene");
        EditorGUILayout.Space(10);
        GUILayout.Label("Configure Maze:");

        InitialMazeScale = EditorGUILayout.IntField("Scale: ", InitialMazeScale);

        if(GUILayout.Button("Generate"))
        {
            GameObject newObj = new("Generated Maze of scale: " + InitialMazeScale);
            Undo.RegisterCreatedObjectUndo(newObj, "Hehe");

            Material mat = new Material(Shader.Find("Standard"));

            CaveTextureGenerator generator = newObj.AddComponent<CaveTextureGenerator>();

            //generator.AddComponent<MeshFilter>().mesh = ProGenTools.CreatePlaneMesh(InitialMazeScale, InitialMazeScale);

            newObj.AddComponent<MeshRenderer>().material = mat;

            generator.CaveWidthAndHeight = InitialMazeScale;
            generator.GenerateImageOfNewMaze();
        }
    }
}
