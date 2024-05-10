using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CellularSensor : MonoBehaviour
{
    public Vector2 pixelOffset;

    Vector3 lastPosition;
    Vector2Int globalPixelPosition;
    Vector2Int chunkAddress;
    CellularChunk chunk = null;
    Vector2Int? chunkCoords = null;
    bool validPosition => chunk != null && chunkCoords != null;

    public Cell currentValue;

    public System.Action<Cell> OnCellChanged;

    Vector2 sensorPosition => (Vector2)transform.position + pixelOffset / Global.pixelsPerUnit;

    private void Update()
    {
        if (transform.position != lastPosition)
        {
            lastPosition = transform.position;
            globalPixelPosition = CellularAutomata.WorldToPixelPosition(sensorPosition);
            chunkAddress = CellularAutomata.GetPixelChunkAddress(globalPixelPosition);
            chunk = CellularAutomata.FindChunk(chunkAddress);
            chunkCoords = CellularAutomata.GlobalPixelToChunkCoords(globalPixelPosition, chunk);
        }

        var newValue = validPosition ? chunk[((Vector2Int)chunkCoords).x, ((Vector2Int)chunkCoords).y] : null;
        if (newValue != currentValue)
        {
            currentValue = newValue;
            OnCellChanged.InvokeIfNotNull(currentValue);
        }
    }

    private void OnDrawGizmos()
    {
        Draw.Pixel(sensorPosition, Color.yellow);
    }

}
