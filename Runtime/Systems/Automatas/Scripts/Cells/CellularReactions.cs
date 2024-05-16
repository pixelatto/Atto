using System.Collections.Generic;
using UnityEngine;

public class CellularReactions : MonoBehaviour
{
    public bool debugTemperatures = false;

    private void Update()
    {
        UpdateTemperatures();
        TriggerMaterialChanges();
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
                    var cell = CellularAutomata.instance.GetCell(globalPixelPosition);

                    if (cell.material != CellMaterial.Empty)
                    {
                        float oldTemperature = cell.temperature;
                        float newTemperature = CalculateNewTemperature(globalPixelPosition, cell);
                        float temperatureChange = newTemperature - oldTemperature;

                        if (temperatureChange != 0)
                        {
                            DistributeTemperatureChange(globalPixelPosition, temperatureChange);
                            cell.temperature = newTemperature;
                        }

                        if (debugTemperatures)
                        {
                            DrawTemperatureDebug(globalPixelPosition, newTemperature);
                        }
                    }
                }
            }
        }
    }

    private float CalculateNewTemperature(Vector2Int position, Cell cell)
    {
        var neighbors = GetNeighboringCells(position);
        float sumTemperatures = 0f;
        float sumConductivities = 0f;

        foreach (var neighbor in neighbors)
        {
            if (neighbor != null && neighbor.material != CellMaterial.Empty)
            {
                sumTemperatures += neighbor.temperature * neighbor.thermalConductivity;
                sumConductivities += neighbor.thermalConductivity;
            }
        }

        if (sumConductivities == 0 || cell.thermalConductivity == 0)
        {
            // Si la conductividad térmica de la célula o sus vecinos es cero, no cambia la temperatura
            return cell.temperature;
        }

        float averageNeighborTemperature = sumTemperatures / sumConductivities;
        float temperatureChange = (averageNeighborTemperature - cell.temperature) * (cell.thermalConductivity / (cell.thermalConductivity + sumConductivities));

        return cell.temperature + temperatureChange;
    }

    private void DistributeTemperatureChange(Vector2Int position, float temperatureChange)
    {
        var cell = CellularAutomata.instance.GetCell(position);
        var neighbors = GetNeighboringCells(position);
        float totalConductivity = 0f;

        foreach (var neighbor in neighbors)
        {
            if (neighbor != null && neighbor.material != CellMaterial.Empty)
            {
                totalConductivity += neighbor.thermalConductivity;
            }
        }

        if (totalConductivity == 0)
        {
            return;
        }

        foreach (var neighbor in neighbors)
        {
            if (neighbor != null && neighbor.material != CellMaterial.Empty)
            {
                float proportion = neighbor.thermalConductivity / totalConductivity;
                neighbor.temperature -= temperatureChange * proportion;
            }
        }
    }

    private List<Cell> GetNeighboringCells(Vector2Int position)
    {
        List<Cell> neighbors = new List<Cell>();

        Vector2Int[] offsets = {
            new Vector2Int(0, 1), // Up
            new Vector2Int(0, -1), // Down
            new Vector2Int(1, 0), // Right
            new Vector2Int(-1, 0) // Left
        };

        foreach (var offset in offsets)
        {
            var neighborPosition = position + offset;
            var neighborCell = CellularAutomata.instance.GetCell(neighborPosition);
            neighbors.Add(neighborCell);
        }

        return neighbors;
    }

    private void DrawTemperatureDebug(Vector2Int position, float temperature)
    {
        Color color = GetTemperatureColor(temperature);
        Vector3 worldPosition = CellularAutomata.PixelToWorldPosition(position);
        Draw.Pixel(worldPosition, color);
    }

    private Color GetTemperatureColor(float temperature)
    {
        if (temperature <= -20)
        {
            return Color.cyan;
        }
        else if (temperature <= 0)
        {
            return Color.blue;
        }
        else if (temperature <= 20)
        {
            return Color.green;
        }
        else if (temperature <= 100)
        {
            return Color.red;
        }
        else
        {
            return new Color(1f, 0f, 1f); // Violet
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
                    var cell = CellularAutomata.instance.GetCell(globalPixelPosition);

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
            CellularAutomata.instance.CreateCell(position, properties.heatMaterial);
        }
        else if (temperature <= properties.coldPoint && properties.coldMaterial != CellMaterial.Empty)
        {
            CellularAutomata.instance.CreateCell(position, properties.coldMaterial);
        }
    }
}
