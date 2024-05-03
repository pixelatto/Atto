using System;
using UnityEngine;

[System.Serializable]
public class CellularChunk
{
    public string chunkName = "";
    [HideInInspector] public Vector2 worldPosition;
    [HideInInspector] public Vector2Int pixelSize;
    public Cell[,] cells { get; private set; }

    public CellularChunk(Vector2 position, Vector2Int size, Cell[,] cells = null, string chunkName = "")
    {
        this.worldPosition = position;
        this.pixelSize = size;
        this.chunkName = chunkName;
        if (cells != null) { this.cells = cells; } else { ClearCells(); }
    }

    void ClearCells()
    {
        cells = new Cell[pixelSize.x, pixelSize.y];
        for (int i = 0; i < pixelSize.x; i++)
        {
            for (int j = 0; j < pixelSize.y; j++)
            {
                cells[i, j] = new Cell(CellMaterial.None);
            }
        }
    }

    public void SetValue(Vector2Int position, Cell value)
    {
        SetValue(position.x, position.y, value);
    }

    void SetValue(int x, int y, Cell cell)
    {
        if (x < 0 || y < 0 || x > pixelSize.x - 1 || y > pixelSize.y - 1)
        {
            return;
        }
        else
        {
            cells[x, y] = cell;
        }
    }

    public Cell GetCell(Vector2Int position)
    {
        return GetCell(position.x, position.y);
    }

    Cell GetCell(int x, int y)
    {
        if (x < 0 || y < 0 || x > pixelSize.x - 1 || y > pixelSize.y - 1)
        {
            return new Cell(CellMaterial.None);
        }
        else
        {
            return cells[x, y];
        }
    }

    public Vector2Int WorldToPixelPosition(Vector3 position)
    {
        return new Vector2Int(Mathf.RoundToInt((position.x - worldPosition.x) * 8f), Mathf.RoundToInt((position.y - worldPosition.y) * 8f));
    }

    public void OverrideCellMaterial(Cell[,] cells)
    {
        this.cells = cells;
    }

    public bool IsEmpty(Vector2Int position)
    {
        return GetCell(position).material == CellMaterial.None;
    }

    public bool IsStatic(Vector2Int position)
    {
        var cell = GetCell(position);
        if (cell.material == CellMaterial.None) { return false; }
        var movement = cell.movement;
        return movement == CellMovement.Static;
    }

    public bool IsSolid(Vector2Int position)
    {
        var cell = GetCell(position);
        if (cell.material == CellMaterial.None) { return false; }
        var movement = cell.movement;
        return movement == CellMovement.Static || movement == CellMovement.Granular;
    }

    public bool IsGranular(Vector2Int position)
    {
        var cell = GetCell(position);
        if (cell.material == CellMaterial.None) { return false; }
        var movement = cell.movement;
        return movement == CellMovement.Granular;
    }

    public bool IsLiquid(Vector2Int position)
    {
        var cell = GetCell(position);
        if (cell.material == CellMaterial.None) { return false; }
        var movement = cell.movement;
        return movement == CellMovement.Fluid;
    }

    public bool CanDisplace(Vector2Int origin, Vector2Int target)
    {
        var originCell = GetCell(origin);
        var targetCell = GetCell(target);
        if (targetCell == originCell)
        {
            return false;
        }
        else if (targetCell.material == CellMaterial.None)
        {
            return true;
        }
        else
        {
            if (targetCell.movement == CellMovement.Static)
            {
                return false;
            }
            else if (originCell.movement == CellMovement.Granular && targetCell.movement == CellMovement.Fluid)
            {
                return true;
            }
        }
        return false;
    }
}
