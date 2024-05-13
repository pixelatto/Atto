using UnityEngine;

[System.Serializable]
public class CellMaterialProperties
{
    public CellMaterial cellMaterial = CellMaterial.None;
    public CellMovement movement = CellMovement.Undefined;
    public int gravity = -1;
    public int fluidity = 1;
    public int temperature = 0;
    public float lightRadius = 0;
    public float lightEmission = 0;
    public Sprite appearance;
    public Color32 identifierColor = Color.white;
    [Range(0f, 1f)]public float opacity = 1;
    public int startLifetime = -1;
    public CellRenderLayer renderLayer = CellRenderLayer.Main;

    public Color GetColor()
    {
        var color = appearance.texture.GetPixel(Random.Range(0, 32), Random.Range(0, 32));
        color.a = opacity;
        return color;
    }
}

public enum CellRenderLayer { Main, Back, Front }