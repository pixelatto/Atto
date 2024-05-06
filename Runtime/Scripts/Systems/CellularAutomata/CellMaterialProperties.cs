using UnityEngine;

[System.Serializable]
public class CellMaterialProperties
{
    public CellMaterial cellMaterial = CellMaterial.None;
    public CellMovement movement = CellMovement.Undefined;
    public int fluidity = 1;
    public Sprite appearance;
    public Color32 identifierColor = Color.white;
    [Range(0f, 1f)]public float opacity = 1;
}
