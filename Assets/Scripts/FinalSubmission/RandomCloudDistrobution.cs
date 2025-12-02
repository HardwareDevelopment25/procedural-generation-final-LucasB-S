using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RandomCloudDistrobution : MonoBehaviour
{
    private int mapWidth;
    private int mapLength;

    public int CloudCount;
    public int CloudScale;
    public float CloudHeight;
    public int CloudCandidateCycles = 30;

    [SerializeField] private TextMeshProUGUI CloudCountUIVal;
    [SerializeField] private TextMeshProUGUI CloudScaleUIVal;
    [SerializeField] private TextMeshProUGUI CloudHeightUIVal;
    [SerializeField] private Toggle RandomCloudRot;
    [SerializeField] private Toggle RandomCloudPrefab;
    [SerializeField] private Toggle RandomCloudPos;
    [SerializeField] private TMP_InputField CloudSeed;

    [SerializeField] private int defaultSeed = 0;
    private int seed;
    private System.Random rand;

    private List<Vector2Int> positions = new();
    public GameObject[] CloudPrefabs = new GameObject[3];
    [SerializeField] private PlaneGenerator Island;


    private void Awake()
    {
        seed = defaultSeed;
        rand = new System.Random(seed);
    }

    private void Start()
    {
        Generate();
    }

    private Vector2Int GetRandomPoint()
    {
        int x = rand.Next(0, mapWidth);
        int y = rand.Next(0, mapLength);

        return new Vector2Int(x, y);
    }

    private void Generate()
    {
        mapWidth = Island.textureSize;
        mapLength = Island.textureSize;
        this.transform.position = new Vector3(0, 0, 0);

        if(CloudCount != positions.Count || RandomCloudPos)
        {
            positions.Clear();

            for (int j = 0; j < CloudCount; j++)
            {
                float bestDistance = -1f;
                Vector2Int bestCandidate = Vector2Int.zero;

                for (int i = 0; i < CloudCandidateCycles; i++)
                {
                    Vector2Int candidate = GetRandomPoint();

                    if (RandomCloudPos == null)
                        Debug.Log("RandomCloudPos is empty");
                    else
                        if(!RandomCloudPos.isOn)
                            candidate =  new Vector2Int(((seed + 10) * i * 10) % mapWidth, ((seed + 10) * j * 10) % mapLength);

                    Vector2Int closestSample = FindClosestSample(candidate);
                    float distance = Vector2Int.Distance(candidate, closestSample);
                    if (distance > bestDistance)
                    {
                        bestDistance = distance;
                        bestCandidate = candidate;
                    }
                }

                positions.Add(bestCandidate);
            }
        }

        Vector3 cloudRot = Vector3.zero;
        int cloudPrefabIndex = 1;

        int cloudnum = 0;
        foreach (var pos in positions)
        {
            if (RandomCloudRot != null)
                if (RandomCloudRot.isOn)
                    cloudRot = new Vector3(0, rand.Next(0, 360), 0);
                else
                    cloudRot = new Vector3(0, (seed + cloudnum * 5) % 360, 0);
            else
                Debug.Log("RandomCloudRot is empty");

            if (RandomCloudPrefab != null)
                if (RandomCloudPrefab.isOn)
                    cloudPrefabIndex = rand.Next(0, 3);
                else
                    cloudPrefabIndex = cloudnum % 3;
            else
                Debug.Log("RandomCloudPrefab is empty");

            cloudnum += 1;
            GameObject cloud = Instantiate(CloudPrefabs[cloudPrefabIndex], new Vector3(pos.x, 0, pos.y), Quaternion.Euler(cloudRot),this.transform);
            cloud.name = $"Cloud_{cloudnum}: using prefab{CloudPrefabs[cloudPrefabIndex].name}";
            cloud.transform.localScale = new Vector3(CloudScale,CloudScale / 2,CloudScale);
        }

        this.transform.position = new Vector3(-mapWidth / 2, CloudHeight, -mapLength / 2);
    }

    private Vector2Int FindClosestSample(Vector2Int a)
    {
        if (positions.Count == 0)
            return a;

        float currentClosestDistance = float.MaxValue;
        Vector2Int closestSample = positions[0];
        foreach (var sample in positions)
        {
            float distance = Vector2Int.Distance(a, sample);
            if (distance < currentClosestDistance)
            {
                currentClosestDistance = distance;
                closestSample = sample;
            }
        }

        return closestSample;
    }

    public void ReGenerate()
    {
        if (CloudCountUIVal != null)
            CloudCount = int.Parse(CloudCountUIVal.text);
        else
            Debug.Log("CloudCountUIVal is empty");

        if (CloudScaleUIVal != null)
            CloudScale = int.Parse(CloudScaleUIVal.text);
        else
            Debug.Log("CloudScaleUIVal is empty");

        if (CloudHeightUIVal != null)
            CloudHeight = float.Parse(CloudHeightUIVal.text);
        else
            Debug.Log("CloudHeightUIVal is empty");

        if(CloudSeed != null)
        {
            seed = int.Parse(CloudSeed.text);
        }
        else
        {
            seed = defaultSeed;
            Debug.Log("CloudSeed is empty");
        }

        foreach (Transform cloud in transform)
            Destroy(cloud.gameObject);

        Generate();
    }
}
