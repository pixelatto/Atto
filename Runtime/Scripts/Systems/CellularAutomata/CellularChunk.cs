using System;
using UnityEngine;

[System.Serializable]
public class CellularChunk
{
    public string chunkName = "";
    [HideInInspector] public Vector2 worldPosition;
    [HideInInspector] public Vector2Int pixelSize;
    public CellMaterial[,] cells { get; private set; }

    public CellularChunk(CellularChunk cloneFrom)
    {
        this.worldPosition = cloneFrom.worldPosition;
        this.pixelSize = cloneFrom.pixelSize;
        this.chunkName = cloneFrom.chunkName;
        cells = cloneFrom.cells;
    }

    public CellularChunk(Vector2 position, Vector2Int size, string chunkName = "")
    {
        this.worldPosition = position;
        this.pixelSize = size;
        this.chunkName = chunkName;
        ClearCells();
    }

    void ClearCells()
    {
        cells = new CellMaterial[pixelSize.x, pixelSize.y];
        for (int i = 0; i < pixelSize.x; i++)
        {
            for (int j = 0; j < pixelSize.y; j++)
            {
                cells[i, j] = CellMaterial.None;
            }
        }
    }

    public void SetValue(Vector2Int position, CellMaterial value)
    {
        SetValue(position.x, position.y, value);
    }

    void SetValue(int x, int y, CellMaterial value)
    {
        if (x < 0 || y < 0 || x > pixelSize.x - 1 || y > pixelSize.y - 1)
        {
            return;
        }
        else
        {
            cells[x, y] = value;
        }
    }

    public CellMaterial GetValue(Vector2Int position)
    {
        return GetValue(position.x, position.y);
    }

    CellMaterial GetValue(int x, int y)
    {
        if (x < 0 || y < 0 || x > pixelSize.x - 1 || y > pixelSize.y - 1)
        {
            return CellMaterial.None;
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

    public void OverrideCells(CellMaterial[,] cells)
    {
        this.cells = cells;
    }

    public bool IsEmpty(Vector2Int position)
    {
        return GetValue(position) == CellMaterial.None;
    }

    public bool IsStatic(Vector2Int position)
    {
        var material = GetValue(position);
        if (material == CellMaterial.None) { return false; }
        var movement = CellularAutomata.instance.materials.FindMaterial(material).movement;
        return movement == CellMovement.Static;
    }

    public bool IsSolid(Vector2Int position)
    {
        var material = GetValue(position);
        if (material == CellMaterial.None) { return false; }
        var movement = CellularAutomata.instance.materials.FindMaterial(material).movement;
        return movement == CellMovement.Static || movement == CellMovement.Granular;
    }

    public bool IsGranular(Vector2Int position)
    {
        var material = GetValue(position);
        if (material == CellMaterial.None) { return false; }
        var movement = CellularAutomata.instance.materials.FindMaterial(material).movement;
        return movement == CellMovement.Granular;
    }

    public bool IsLiquid(Vector2Int position)
    {
        var material = GetValue(position);
        if (material == CellMaterial.None) { return false; }
        var movement = CellularAutomata.instance.materials.FindMaterial(material).movement;
        return movement == CellMovement.Fluid;
    }

    public bool CanDisplace(Vector2Int origin, Vector2Int target)
    {
        var originMaterial = GetValue(origin);
        var targetMaterial = GetValue(target);
        if (targetMaterial == originMaterial)
        {
            return false;
        }
        else if (targetMaterial == CellMaterial.None)
        {
            return true;
        }
        else
        {
            var originMaterialProps = CellularAutomata.instance.materials.FindMaterial(originMaterial);
            var targetMaterialProps = CellularAutomata.instance.materials.FindMaterial(targetMaterial);
            if (targetMaterialProps.movement == CellMovement.Static)
            {
                return false;
            }
            else if (originMaterialProps.movement == CellMovement.Granular && targetMaterialProps.movement == CellMovement.Fluid)
            {
                return true;
            }
        }
        return false;
    }
}