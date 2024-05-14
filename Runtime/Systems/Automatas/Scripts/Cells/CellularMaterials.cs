using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CellularMaterials", menuName = "Atto/CellularAutomataMaterial")]
public class CellularMaterials : ScriptableObject
{
    static public CellularMaterials instance { get { if (instance_ == null) { instance_ = Resources.Load<CellularMaterials>("CellularMaterials"); }; return instance_; } }
    static private CellularMaterials instance_;

    public List<CellMaterialProperties> materials;
    public List<CellMaterialReaction> reactions;

    static Dictionary<CellMaterial, CellMaterialProperties> lookupDictionary = new Dictionary<CellMaterial, CellMaterialProperties>();

    public CellMaterialProperties FindMaterial(CellMaterial cellMaterial)
    {
        if (!lookupDictionary.ContainsKey(cellMaterial))
        {
            lookupDictionary.Add(cellMaterial, instance.materials.Find(x => x.cellMaterial == cellMaterial));
        }
        
        return lookupDictionary[cellMaterial];
    }
}

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