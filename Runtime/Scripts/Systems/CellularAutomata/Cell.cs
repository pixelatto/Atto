using UnityEngine;

[System.Serializable]
public class Cell
{
    public CellMaterial material = CellMaterial.None;
    public Color color = Color.clear;
    public bool blocksLight = false;
    
    public CellMovement movement => material == CellMaterial.None ? CellMovement.Static : materialProperties.movement;
    public int fluidity => materialProperties.fluidity;

    CellMaterialProperties materialProperties { get { if (_materialProperties == null) { _materialProperties = CellularAutomata.instance.materials.FindMaterial(material); }; return _materialProperties; } }
    CellMaterialProperties _materialProperties;

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
        else if (color != Color.clear)
        {
            return color;
        }
        else
        {
            return CellularAutomata.instance.materials.FindMaterial(material).color;
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
}