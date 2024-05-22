using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CellularMaterialLibrary", menuName = "Atto/CellularMaterialLibrary")]
public class CellularMaterialLibrary : ScriptableObject
{
    static public CellularMaterialLibrary instance { get { if (instance_ == null) { instance_ = Resources.Load<CellularMaterialLibrary>("CellularMaterials"); }; return instance_; } }
    static private CellularMaterialLibrary instance_;

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

    public Color[] MaterialColors()
    {
        List<Color> result = new List<Color>();
        foreach (var material in materials)
        {
            result.Add(material.identifierColor);
        }
        return result.ToArray();
    }

    public CellMaterial GetMaterialFromColor(Color32 color)
    {
        foreach (var material in materials)
        {
            if (ColorsAreEqual(material.identifierColor, color))
            {
                return material.cellMaterial;
            }
        }
        return CellMaterial.Empty;
    }

    private bool ColorsAreEqual(Color32 color1, Color32 color2)
    {
        return color1.r == color2.r && color1.g == color2.g && color1.b == color2.b && color1.a == color2.a;
    }

}
