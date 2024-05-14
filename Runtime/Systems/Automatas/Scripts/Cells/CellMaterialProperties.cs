using UnityEngine;

[System.Serializable]
public class CellMaterialProperties
{
    public CellMaterial cellMaterial = CellMaterial.None;
    public Color32 identifierColor = Color.white;

    [Header("Physics")]
    public CellMovement movement = CellMovement.Undefined;
    public int gravity = 1;
    public int fluidity = 1;
    public int lifespan = -1;

    [Header("Optics")]
    public Sprite appearance;
    public float lightRadius = 0;
    public float lightEmission = 0;
    [Range(0f, 1f)] public float opacity = 1;
    public CellRenderLayer renderLayer = CellRenderLayer.Main;

    [Header("Thermodynamics")]
    public float temperature = 20;
    public float thermalConductivity = 1;
    public float heatPoint = 100;
    public CellMaterial heatMaterial = CellMaterial.None;
    public float coldPoint = 0;
    public CellMaterial coldMaterial = CellMaterial.None;

    public Color GetColor()
    {
        var color = appearance.texture.GetPixel(Random.Range(0, 32), Random.Range(0, 32));
        color.a = opacity;
        return color;
    }
}

public enum CellRenderLayer { Main, Back, Front }