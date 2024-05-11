using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CellularMaterials", menuName = "Atto/CellularAutomataMaterial")]
public class CellularMaterials : ScriptableObject
{
    static public CellularMaterials instance { get { if (instance_ == null) { instance_ = Resources.Load<CellularMaterials>("CellularMaterials"); }; return instance_; } }
    static private CellularMaterials instance_;

    public List<CellMaterialProperties> materials;

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
