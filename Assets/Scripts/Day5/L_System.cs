using System.Collections.Generic;
using UnityEngine;

public class L_System : MonoBehaviour
{
    // Small struct to hold transform state for push/pop operations.
    private struct TransformData
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    public GameObject Turtle;
    public GameObject prefab;
    public float length;
    public int iterations;
    public float angle;
    public string axiom;
    public Dictionary<char, string> recursionRules = new();
    private Stack<TransformData> transformData = new();

    private void Awake()
    {
        Turtle = this.gameObject;
        recursionRules.Add('X', "F+[[X]-X]-F[-FX]+X");
        recursionRules.Add('F', "FF");
    }

    private void Start()
    {
        GenerateString();
    }

    private void GenerateString()
    {
        string currentString = axiom;
        System.Text.StringBuilder newString = new();

        for (int i = 0; i < iterations; i++)
        {
            foreach (char c in currentString)
            {
                if (recursionRules.ContainsKey(c))
                {
                    newString.Append(recursionRules[c]);
                }
                else
                {
                    newString.Append(c);
                }
            }

            currentString = newString.ToString();
            newString.Clear();
        }

        Debug.Log(currentString);

        ApplyRules(currentString);
    }

    private void ApplyRules(string s)
    {
        foreach (char c in s)
        {
            switch (c)
            {
                case 'X':
                    // Do nothing for 'X'
                    break;
                case 'F':
                    // Move forward and draw a line using linereader component
                    Vector3 initialPosition = transform.position;
                    transform.Translate(Vector3.up * length);
                    GameObject temp = Instantiate(prefab, transform.position, Quaternion.identity);
                    var tempLR = temp.GetComponent<LineRenderer>();
                    tempLR.SetPosition(0, initialPosition);
                    tempLR.SetPosition(1, transform.position);

                    break;
                case '+':
                    // Turn right
                    transform.Rotate(Vector3.forward * angle);
                    break;
                case '-':
                    // Turn left
                    transform.Rotate(Vector3.forward * -angle);
                    break;
                case '[':
                    // Push current transform state onto stack
                    TransformData tI = new()
                    {
                        position = transform.position,
                        rotation = transform.rotation
                    };
                    transformData.Push(tI);
                    break;
                case ']':
                    // Pop transform state from stack and restore it
                    TransformData tF = transformData.Pop();
                    transform.position = tF.position;
                    transform.rotation = tF.rotation;
                    break;
                default:
                    Debug.LogWarning($"Unrecognized character: {c}");
                    break;
            }
        }
    }
}
