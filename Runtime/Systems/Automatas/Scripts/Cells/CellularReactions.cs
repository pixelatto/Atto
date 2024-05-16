using System.Collections.Generic;
using UnityEngine;

public class CellularReactions : MonoBehaviour
{
    public bool debugTemperatures = false;
    private Texture2D debugTexture;
    private SpriteRenderer debugSpriteRenderer;

    private Dictionary<Vector2Int, float> currentTemperatures;
    private Dictionary<Vector2Int, float> nextTemperatures;
    private int textureWidth;
    private int textureHeight;

    CellularAutomata cellularAutomata;

    private void Start()
    {
        currentTemperatures = new Dictionary<Vector2Int, float>();
        nextTemperatures = new Dictionary<Vector2Int, float>();

        InitializeDebugTexture();

        cellularAutomata = CellularAutomata.instance;
    }

    private void InitializeDebugTexture()
    {
        textureWidth = Global.roomPixelSize.x;
        textureHeight = Global.roomPixelSize.y;

        // Crear la textura de depuración
        debugTexture = new Texture2D(textureWidth, textureHeight);
        debugTexture.filterMode = FilterMode.Point;

        // Crear un SpriteRenderer para mostrar la textura de depuración
        var debugGameObject = new GameObject("DebugTexture");
        debugSpriteRenderer = debugGameObject.AddComponent<SpriteRenderer>();
        debugSpriteRenderer.sprite = Sprite.Create(debugTexture, new Rect(0, 0, textureWidth, textureHeight), Vector2.one * 0.5f, Global.pixelsPerUnit);
        debugSpriteRenderer.color = new Color(1, 1, 1, 0.5f);
        debugSpriteRenderer.sortingLayerName = "UI";

        // Ajustar la posición y escala del SpriteRenderer
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
        var chunks = FindObjectsOfType<CellularChunk>();
        foreach (var chunk in chunks)
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
                        nextTemperatures[globalPixelPosition] = newTemperature;
                    }
                }
            }
        }

        // Swap buffers
        var temp = currentTemperatures;
        currentTemperatures = nextTemperatures;
        nextTemperatures = temp;

        // Clear nextTemperatures for the next update
        nextTemperatures.Clear();

        // Update the actual temperatures of the cells
        foreach (var kvp in currentTemperatures)
        {
            var cell = cellularAutomata.GetCell(kvp.Key);
            cell.temperature = kvp.Value;
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
            var neighborCell = cellularAutomata.GetCell(neighborPosition);
            if (neighborCell != null)
            {
                float neighborTemperature = currentTemperatures.ContainsKey(neighborPosition) ? currentTemperatures[neighborPosition] : neighborCell.temperature;
                sumTemperatures += neighborTemperature * neighborCell.thermalConductivity;
                sumConductivities += neighborCell.thermalConductivity;
            }
        }

        // Añadir la célula adicional de temperatura ambiente con ruido Perlin si la célula es vacía
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

    private void DistributeTemperatureChange(Vector2Int position, float temperatureChange)
    {
        var neighbors = GetNeighboringCells(position);
        float totalConductivity = 0f;

        foreach (var neighborPosition in neighbors)
        {
            var neighborCell = cellularAutomata.GetCell(neighborPosition);
            if (neighborCell != null)
            {
                totalConductivity += neighborCell.thermalConductivity;
            }
        }

        if (totalConductivity == 0)
        {
            return;
        }

        foreach (var neighborPosition in neighbors)
        {
            var neighborCell = cellularAutomata.GetCell(neighborPosition);
            if (neighborCell != null)
            {
                float proportion = neighborCell.thermalConductivity / totalConductivity;
                neighborCell.temperature -= temperatureChange * proportion;
            }
        }
    }

    private List<Vector2Int> GetNeighboringCells(Vector2Int position)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        Vector2Int[] offsets = {
            new Vector2Int(0, 1), // Up
            new Vector2Int(0, -1), // Down
            new Vector2Int(1, 0), // Right
            new Vector2Int(-1, 0) // Left
        };

        foreach (var offset in offsets)
        {
            var neighborPosition = position + offset;
            neighbors.Add(neighborPosition);
        }

        return neighbors;
    }

    private void UpdateDebugTexture()
    {
        var chunks = FindObjectsOfType<CellularChunk>();
        foreach (var chunk in chunks)
        {
            for (int y = 0; y < chunk.pixelSize.y; y++)
            {
                for (int x = 0; x < chunk.pixelSize.x; x++)
                {
                    Vector2Int globalPixelPosition = new Vector2Int(chunk.pixelPosition.x + x, chunk.pixelPosition.y + y);
                    var cell = cellularAutomata.GetCell(globalPixelPosition);

                    if (cell != null)
                    {
                        float temperature = cell.temperature;
                        Color color = GetTemperatureColor(temperature);

                        int textureX = globalPixelPosition.x;
                        int textureY = globalPixelPosition.y;

                        // Verificar que la posición esté dentro de los límites de la textura
                        if (textureX >= 0 && textureX < textureWidth && textureY >= 0 && textureY < textureHeight)
                        {
                            debugTexture.SetPixel(textureX, textureY, color);
                        }
                    }
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
        Color hotColor = new Color(1f, 0f, 1f); // Violet

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
            float t = Mathf.InverseLerp(100, 120, temperature); // Assuming temperatures above 100 go up to 120 for interpolation
            return Color.Lerp(warmColor, hotColor, t);
        }
    }

    private void TriggerMaterialChanges()
    {
        var chunks = FindObjectsOfType<CellularChunk>();
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
