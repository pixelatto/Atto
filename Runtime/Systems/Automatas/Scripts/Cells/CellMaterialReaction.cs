using UnityEngine;

[System.Serializable]
public class CellMaterialReaction
{
    public string reactionName;
    public CellMaterial reactorA;
    public CellMaterial reactorB;
    public CellMaterial productA;
    public CellMaterial productB;
    [Range(0, 1)] public float chance = 1;
}