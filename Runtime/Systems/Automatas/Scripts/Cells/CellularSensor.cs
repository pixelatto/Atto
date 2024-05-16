using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CellularSensor : MonoBehaviour
{
    public Cell firstCell;

    public Vector2Int pixelOffset;
    public Vector2Int pixelSize = new Vector2Int(1, 1);

    Vector3 lastPosition;
    Vector2Int globalPixelPosition;
    Vector2Int chunkAddress;
    CellularChunk chunk = null;
    Vector2Int? chunkCoords = null;
    bool isInsideChunk => chunk != null && chunkCoords != null;

    public Cell[,] sensedCells = null;

    Vector2 sensorBasePosition => (Vector2)transform.position + (Vector2)pixelOffset / Global.pixelsPerUnit;

    private void Start()
    {
        sensedCells = new Cell[pixelSize.x, pixelSize.y];

        for (int i = 0; i < pixelSize.x; i++)
        {
            for (int j = 0; j < pixelSize.y; j++)
            {
                sensedCells[i, j] = CellularAutomata.emptyCell;
            }
        }
    }

    private void Update()
    {
        if (transform.position != lastPosition)
        {
            lastPosition = transform.position;
            globalPixelPosition = CellularAutomata.WorldToPixelPosition(sensorBasePosition);
            chunkAddress = CellularAutomata.GetPixelChunkAddress(globalPixelPosition);
            chunk = CellularAutomata.FindChunk(chunkAddress);
            chunkCoords = CellularAutomata.GlobalPixelToChunkCoords(globalPixelPosition, chunk);
        }

        if (isInsideChunk && sensedCells != null)
        {
            if (sensedCells == null || sensedCells.GetLength(0) != pixelSize.x || sensedCells.GetLength(1) != pixelSize.y)
            {
                sensedCells = new Cell[pixelSize.x, pixelSize.y];
            }

            for (int i = 0; i < sensedCells.GetLength(0); i++)
            {
                for (int j = 0; j < sensedCells.GetLength(1); j++)
                {
                    sensedCells[i, j] = chunk[((Vector2Int)chunkCoords).x + i, ((Vector2Int)chunkCoords).y + j];
                }
            }
        }

        firstCell = sensedCells[0, 0];
    }

    public bool ContainsAny(System.Predicate<Cell> CellCondition)
    {
        for (int i = 0; i < sensedCells.GetLength(0); i++)
        {
            for (int j = 0; j < sensedCells.GetLength(1); j++)
            {
                if (CellCondition(sensedCells[i, j]))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsFilledWith(System.Predicate<Cell> CellCondition)
    {
        for (int i = 0; i < sensedCells.GetLength(0); i++)
        {
            for (int j = 0; j < sensedCells.GetLength(1); j++)
            {
                if (!CellCondition(sensedCells[i, j]))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < pixelSize.x; i++)
        {
            for (int j = 0; j < pixelSize.y; j++)
            {
                Draw.Pixel(sensorBasePosition + new Vector2(i, j) / Global.pixelsPerUnit, Color.yellow);
            }
        }
    }

}
