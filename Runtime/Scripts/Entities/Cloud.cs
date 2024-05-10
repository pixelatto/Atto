using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Cloud : MonoBehaviour
{
    public bool isRaining = false;

    public Vector2Int minSize = new Vector2Int(8, 4);
    public Vector2Int maxSize = new Vector2Int(64, 16);

    public float scale = 10f;
    [Range(0, 1)] public float density = 0.5f;
    public int neighborThreshold = 4;
    public int iterations = 5;

    private Vector2Int size;
    private Texture2D texture;
    private int[,] labels;

    BoxCollider2D boxCollider;


    public CellularSpawner spawner { get { if (spawner_ == null) { spawner_ = GetComponentInChildren<CellularSpawner>(); }; return spawner_; } }
    private CellularSpawner spawner_;


    void Start()
    {
        GenerateCloud();
    }

    private void Update()
    {
        if (texture == null)
        {
            GenerateCloud();
        }
        if (Application.isPlaying)
        {
            spawner.enabled = isRaining;
        }
    }

    [Button]
    void GenerateCloud()
    {
        var width = Random.Range(minSize.x, maxSize.x);
        var height = Random.Range(minSize.y, maxSize.y);
        width -= width % 2;
        height -= height % 2;
        size = new Vector2Int(width, height);
        labels = new int[width, height];

        texture = new Texture2D(size.x, size.y);
        texture.filterMode = FilterMode.Point;
        GetComponentInChildren<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, width, height), new Vector2(0.5f, 0.5f), 8);

        // Initial Perlin noise generation
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color color = CalculateColor(x, y);
                texture.SetPixel(x, y, color);
            }
        }
        for (int i = 2; i < width-2; i++)
        {
            texture.SetPixel(i, height / 4, Color.white);
            texture.SetPixel(i, height / 4 + 1, Color.white);
        }

        // Apply cellular automaton
        ApplyCellularAutomaton();

        texture.Apply();

        // Identify the largest connected component
        FindLargestConnectedComponent();

        // Update BoxCollider
        UpdateBoxCollider();
    }

    Color CalculateColor(int x, int y)
    {
        float xCoord = (float)x / size.x * scale + Random.Range(-0.5f, 0.5f);
        float yCoord = (float)y / size.y * scale + Random.Range(-0.5f, 0.5f);

        float sample = Mathf.PerlinNoise(xCoord, yCoord);
        return new Color(1, 1, 1, sample > density ? 1f : 0);
    }

    void ApplyCellularAutomaton()
    {
        for (int i = 0; i < iterations; i++)
        {
            Color[] newPixels = new Color[size.x * size.y];
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    int index = x + y * size.x;
                    int neighborCount = CountNeighbors(x, y);

                    if (neighborCount > neighborThreshold)
                        newPixels[index] = new Color(1, 1, 1, 1);
                    else
                        newPixels[index] = new Color(1, 1, 1, 0);
                }
            }
            // Update texture
            for (int j = 0; j < newPixels.Length; j++)
            {
                texture.SetPixel(j % size.x, j / size.x, newPixels[j]);
            }
        }
        texture.Apply();
    }

    int CountNeighbors(int x, int y)
    {
        int count = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;
                int nx = (x + i + size.x) % size.x;
                int ny = (y + j + size.y) % size.y;
                if (texture.GetPixel(nx, ny).a > 0.5f)
                    count++;
            }
        }
        return count;
    }

    void UpdateBoxCollider()
    {
        int minX = size.x, minY = size.y, maxX = 0, maxY = 0;
        int maxLabel = FindMaxLabel();

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (labels[x, y] == maxLabel)
                {
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }
            }
        }

        if (minX < maxX && minY < maxY) // Check if we have any visible pixels
        {
            GameObject colliderObject = gameObject.FindOrAddChild("Collider");
            Vector2 size = new Vector2((maxX - minX + 1) / Global.pixelsPerUnit, (maxY - minY + 1) / Global.pixelsPerUnit);
            Vector2 center = new Vector2((minX + maxX) / 2.0f / Global.pixelsPerUnit - this.size.x / 2.0f / Global.pixelsPerUnit, (minY + maxY) / 2.0f / Global.pixelsPerUnit - this.size.y / 2.0f / Global.pixelsPerUnit);
            colliderObject.transform.localPosition = center + Vector2.down / Global.pixelsPerUnit;
            boxCollider = colliderObject.GetOrAddComponent<BoxCollider2D>();
            size.x -= 2f.PixelsToUnits();
            size.y = 1f.PixelsToUnits();
            boxCollider.size = size;
        }
        else
        {
            Debug.LogWarning("No visible pixels! Collider was not generated");
        }
    }

    // Helper functions for finding the largest connected component
    private void FindLargestConnectedComponent()
    {
        int labelCount = 1;
        Dictionary<int, int> labelSize = new Dictionary<int, int>();

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                if (texture.GetPixel(x, y).a > 0.5f && labels[x, y] == 0)
                {
                    // Use a flood fill to label this component
                    int count = FloodFill(x, y, labelCount);
                    labelSize[labelCount] = count;
                    labelCount++;
                }
            }
        }
    }

    private int FloodFill(int x, int y, int label)
    {
        if (x < 0 || y < 0 || x >= size.x || y >= size.y || labels[x, y] != 0 || texture.GetPixel(x, y).a <= 0.5f)
            return 0;

        labels[x, y] = label;
        return 1 + FloodFill(x + 1, y, label) + FloodFill(x - 1, y, label) +
               FloodFill(x, y + 1, label) + FloodFill(x, y - 1, label);
    }

    private int FindMaxLabel()
    {
        Dictionary<int, int> area = new Dictionary<int, int>();
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                int label = labels[x, y];
                if (label > 0)
                {
                    if (!area.ContainsKey(label))
                        area[label] = 0;
                    area[label]++;
                }
            }
        }

        int maxLabel = 0;
        int maxArea = 0;
        foreach (var pair in area)
        {
            if (pair.Value > maxArea)
            {
                maxArea = pair.Value;
                maxLabel = pair.Key;
            }
        }
        return maxLabel;
    }
}
