using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CellularMaterials", menuName = "Atto/CellularAutomataMaterial")]
public class CellularMaterials : ScriptableObject
{
    static public CellularMaterials instance { get { if (instance_ == null) { instance_ = Resources.Load<CellularMaterials>("CellularMaterials"); }; return instance_; } }
    static private CellularMaterials instance_;

    public List<CellMaterialProperties> materials;

    static Dictionary<CellMaterial, CellMaterialProperties> materialLookupDictionary = new Dictionary<CellMaterial, CellMaterialProperties>();
    
    private void OnEnable()
    {
        InitializeDictionaries();
    }

    private void InitializeDictionaries()
    {
        materialLookupDictionary.Clear();

        foreach (var material in materials)
        {
            materialLookupDictionary[material.cellMaterial] = material;
        }
    }

    public CellMaterialProperties FindMaterial(CellMaterial cellMaterial)
    {
        return materialLookupDictionary[cellMaterial];
    }
}
