﻿using System;
using UnityEngine;

[System.Serializable]
public class Cell
{
    public CellMaterial material = CellMaterial.None;
    public Color overrideColor = Color.clear;
    public bool blocksLight = false;
    
    public CellMovement movement => material == CellMaterial.None ? CellMovement.Static : materialProperties.movement;
    public int fluidity => materialProperties.fluidity;

    CellMaterialProperties materialProperties { get { if (_materialProperties == null) { _materialProperties = CellularMaterials.instance.FindMaterial(material); }; return _materialProperties; } }
    CellMaterialProperties _materialProperties;

    public uint lastUpdateTick = 0;
    public int  gravity => materialProperties.gravity;

    public bool wasUpdatedThisTick => lastUpdateTick == CellularAutomata.currentTick;

    public CellRenderLayer renderLayer => materialProperties.renderLayer;
    public int startLifetime => materialProperties.startLifetime;
    public int elapsedLifetime = 0;
    public bool lifetimeTimeout => (startLifetime != -1) && (elapsedLifetime >= startLifetime);

    public Cell(CellMaterial material)
    {
        this.material = material;
    }

    public Color GetColor()
    {
        if (material == CellMaterial.None)
        {
            return Color.clear;
        }
        else if (overrideColor != Color.clear)
        {
            overrideColor.a = materialProperties.opacity;
            return overrideColor;
        }
        else if (materialProperties.appearance != null)
        {
            overrideColor = materialProperties.GetColor();
            return overrideColor;
        }
        else
        {
            return materialProperties.identifierColor;
        }
    }

    public bool IsEmpty()
    {
        return material == CellMaterial.None;
    }

    public bool IsStatic()
    {
        return movement == CellMovement.Static;
    }

    public bool IsSolid()
    {
        return material != CellMaterial.None && (movement == CellMovement.Static || movement == CellMovement.Granular);
    }

    public bool IsGranular()
    {
        return movement == CellMovement.Granular;
    }

    public bool IsLiquid()
    {
        return movement == CellMovement.Fluid;
    }

    public bool IsGas()
    {
        return movement == CellMovement.Gas;
    }

    public bool IsHotterThan(int threshold)
    {
        return materialProperties != null && materialProperties.temperature > threshold;
    }

    public bool IsColderThan(int threshold)
    {
        return materialProperties != null && materialProperties.temperature < threshold;
    }

    public void Destroy()
    {
        material = CellMaterial.None;
    }
}