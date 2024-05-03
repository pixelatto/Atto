using UnityEngine;

public class Cell
{
    public CellMaterial material = CellMaterial.None;
    public Color color = Color.clear;
    
    public CellMovement movement => materialProperties.movement;
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
}