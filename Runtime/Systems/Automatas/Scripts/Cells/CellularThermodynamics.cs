using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class CellularThermodynamics : MonoBehaviour
{
    public float ambientTemperature = 20;
    public bool debugTemperatures = false;
    private Texture2D debugTexture;
    private SpriteRenderer debugSpriteRenderer;

    private float[,] temperatures;
    private int textureWidth;
    private int textureHeight;

    CellularAutomata cellularAutomata;
    CellularChunk[] chunks;
    Vector2Int globalSize;

    private void Start()
    {
        globalSize = Global.roomPixelSize;
        temperatures = new float[globalSize.x, globalSize.y];

        InitializeDebugTexture();

        cellularAutomata = CellularAutomata.instance;
        chunks = FindObjectsOfType<CellularChunk>();
    }

    private void InitializeDebugTexture()
    {
        textureWidth = Global.roomPixelSize.x;
        textureHeight = Global.roomPixelSize.y;

        debugTexture = new Texture2D(textureWidth, textureHeight);
        debugTexture.filterMode = FilterMode.Point;

        var debugGameObject = new GameObject("DebugTexture");
        debugSpriteRenderer = debugGameObject.AddComponent<SpriteRenderer>();
        debugSpriteRenderer.sprite = Sprite.Create(debugTexture, new Rect(0, 0, textureWidth, textureHeight), Vector2.one * 0.5f, Global.pixelsPerUnit);
        debugSpriteRenderer.color = new Color(1, 1, 1, 0.5f);
        debugSpriteRenderer.sortingLayerName = "UI";

        var pixelCamera = FindObjectOfType<RoomPixelCamera>();
        if (pixelCamera != null)
        {
            debugSpriteRenderer.transform.position = pixelCamera.transform.position;
        }
        debugSpriteRenderer.transform.localScale = Vector3.one;
    }

    private void Update()
    {
        UpdateTemperatures();
        TriggerMaterialChanges();
        if (debugTemperatures)
        {
            UpdateDebugTexture();
        }
    }

    private void UpdateTemperatures()
    {
        Global.ambientTemperature = ambientTemperature;

        Parallel.ForEach(chunks, chunk =>
        {
            for (int y = 0; y < chunk.pixelSize.y; y++)
            {
                for (int x = 0; x < chunk.pixelSize.x; x++)
                {
                    Vector2Int globalPixelPosition = new Vector2Int(chunk.pixelPosition.x + x, chunk.pixelPosition.y + y);
                    var cell = cellularAutomata.GetCell(globalPixelPosition);

                    if (cell != null)
                    {
                        float newTemperature = CalculateNewTemperature(globalPixelPosition, cell);
                        temperatures[globalPixelPosition.x, globalPixelPosition.y] = newTemperature;
                    }
                }
            }
        });

        for (int y = 0; y < globalSize.y; y++)
        {
            for (int x = 0; x < globalSize.x; x++)
            {
                var cell = cellularAutomata.GetCell(new Vector2Int(x, y));
                if (cell != null)
                {
                    cell.temperature = temperatures[x, y];
                }
            }
        }
    }

    private float CalculateNewTemperature(Vector2Int position, Cell cell)
    {
        var neighbors = GetNeighboringCells(position);
        float sumTemperatures = 0f;
        float sumConductivities = 0f;
        float ambientTemperature = Global.ambientTemperature;
        float emptyCellConductivity = cell.thermalConductivity;

        foreach (var neighborPosition in neighbors)
        {
            if (neighborPosition.x >= 0 && neighborPosition.x < globalSize.x && neighborPosition.y >= 0 && neighborPosition.y < globalSize.y)
            {
                var neighborCell = cellularAutomata.GetCell(neighborPosition);
                if (neighborCell != null)
                {
                    float neighborTemperature = temperatures[neighborPosition.x, neighborPosition.y];
                    sumTemperatures += neighborTemperature * neighborCell.thermalConductivity;
                    sumConductivities += neighborCell.thermalConductivity;
                }
            }
        }

        if (cell.material == CellMaterial.Empty)
        {
            float perlinNoise = Mathf.PerlinNoise(position.x * 0.1f, position.y * 0.1f) * Global.ambientTemperatureDelta;
            float adjustedAmbientTemperature = ambientTemperature + perlinNoise;

            sumTemperatures += adjustedAmbientTemperature * emptyCellConductivity;
            sumConductivities += emptyCellConductivity;
        }

        if (sumConductivities == 0 || cell.thermalConductivity == 0)
        {
            return cell.temperature;
        }

        float averageNeighborTemperature = sumTemperatures / sumConductivities;
        float temperatureChange = (averageNeighborTemperature - cell.temperature) * (cell.thermalConductivity / (cell.thermalConductivity + sumConductivities));

        return cell.temperature + temperatureChange;
    }

    private Vector2Int[] GetNeighboringCells(Vector2Int position)
    {
        return new Vector2Int[]
        {
            new Vector2Int(position.x, position.y + 1),
            new Vector2Int(position.x, position.y - 1),
            new Vector2Int(position.x + 1, position.y),
            new Vector2Int(position.x - 1, position.y)
        };
    }

    private void UpdateDebugTexture()
    {
        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                var cell = cellularAutomata.GetCell(new Vector2Int(x, y));
                if (cell != null)
                {
                    float temperature = cell.temperature;
                    Color color = GetTemperatureColor(temperature);
                    debugTexture.SetPixel(x, y, color);
                }
            }
        }
        debugTexture.Apply();
    }

    private Color GetTemperatureColor(float temperature)
    {
        Color coldColor = Color.cyan;
        Color coolColor = Color.blue;
        Color neutralColor = Color.green;
        Color warmColor = Color.red;
        Color hotColor = new Color(1f, 0f, 1f);

        if (temperature <= -20)
        {
            return coldColor;
        }
        else if (temperature <= 0)
        {
            float t = Mathf.InverseLerp(-20, 0, temperature);
            return Color.Lerp(coldColor, coolColor, t);
        }
        else if (temperature <= 20)
        {
            float t = Mathf.InverseLerp(0, 20, temperature);
            return Color.Lerp(coolColor, neutralColor, t);
        }
        else if (temperature <= 100)
        {
            float t = Mathf.InverseLerp(20, 100, temperature);
            return Color.Lerp(neutralColor, warmColor, t);
        }
        else
        {
            float t = Mathf.InverseLerp(100, 120, temperature);
            return Color.Lerp(warmColor, hotColor, t);
        }
    }

    private void TriggerMaterialChanges()
    {
        foreach (var chunk in chunks)
        {
            for (int y = 0; y < chunk.pixelSize.y; y++)
            {
                for (int x = 0; x < chunk.pixelSize.x; x++)
                {
                    Vector2Int globalPixelPosition = new Vector2Int(chunk.pixelPosition.x + x, chunk.pixelPosition.y + y);
                    var cell = cellularAutomata.GetCell(globalPixelPosition);

                    if (cell.material != CellMaterial.Empty && cell.elapsedLifetime > 1)
                    {
                        CheckAndTriggerChange(cell, globalPixelPosition);
                    }
                }
            }
        }
    }

    private void CheckAndTriggerChange(Cell cell, Vector2Int position)
    {
        float temperature = cell.temperature;
        CellMaterialProperties properties = cell.materialProperties;

        if (temperature >= properties.heatPoint && properties.heatMaterial != CellMaterial.Empty)
        {
            cellularAutomata.CreateCell(position, properties.heatMaterial);
        }
        else if (temperature <= properties.coldPoint && properties.coldMaterial != CellMaterial.Empty)
        {
            cellularAutomata.CreateCell(position, properties.coldMaterial);
        }
    }
}
