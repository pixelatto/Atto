using System;
using UnityEngine;

[System.Serializable]
public class Cell
{
    public CellMaterial material = CellMaterial.Empty;
    public Color overrideColor = Color.clear;
    public bool blocksLight = false;

    public CellMovement movement => material == CellMaterial.Empty ? CellMovement.Static : materialProperties.movement;
    public int fluidity => materialProperties.fluidity;
    public float lightEmission => materialProperties.lightEmission;
    public float lightRadius => materialProperties.lightRadius;
    public float thermalConductivity => materialProperties.thermalConductivity;

    public float temperature { get { return temperature_; } set { temperature_ = value; } }
    [SerializeField] [Label("Temperature")] [ReadOnly] private float temperature_;

    public CellMaterialProperties materialProperties { get { if (_materialProperties == null) { _materialProperties = CellularMaterials.instance.FindMaterial(material); }; return _materialProperties; } }
    CellMaterialProperties _materialProperties;

    public uint lastUpdateTick = 0;
    public int gravity => materialProperties.gravity;

    public bool wasUpdatedThisTick => lastUpdateTick == CellularAutomata.currentTick;

    public CellRenderLayer renderLayer => materialProperties.renderLayer;
    public int startLifetime => materialProperties.lifespan;
    public float startTemperature => materialProperties.temperature;
    public int elapsedLifetime = 0;
    public bool lifetimeTimeout => (startLifetime != -1) && (elapsedLifetime >= startLifetime);

    public bool isStructurallyConnected = false;
    public bool needsUpdate = true;

    public Cell(CellMaterial material)
    {
        this.material = material;
    }

    public Color GetColor()
    {
        if (material == CellMaterial.Empty)
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
        return material == CellMaterial.Empty;
    }

    public bool IsStatic()
    {
        return movement == CellMovement.Static || (movement == CellMovement.Structural && isStructurallyConnected);
    }

    public bool IsSolid()
    {
        return material != CellMaterial.Empty &&
            (movement == CellMovement.Static || movement == CellMovement.Granular || movement == CellMovement.Structural);
    }

    public bool IsGranular()
    {
        return movement == CellMovement.Granular;
    }

    public bool IsFluid()
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
        material = CellMaterial.Empty;
    }
}
