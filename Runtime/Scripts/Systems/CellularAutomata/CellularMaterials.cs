﻿using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CellularMaterials", menuName = "Atto/CellularAutomataMaterial")]
public class CellularMaterials : ScriptableObject
{
    public List<CellMaterialProperties> materials;

    Dictionary<CellMaterial, CellMaterialProperties> lookupDictionary = new Dictionary<CellMaterial, CellMaterialProperties>();

    [System.Serializable]
    public class CellMaterialProperties
    {
        public CellMaterial cellMaterial = CellMaterial.None;
        public CellMovement movement = CellMovement.Undefined;
        public Color color = Color.white;
        public int fluidity = 1;
    }

    public CellMaterialProperties FindMaterial(CellMaterial cellMaterial)
    {
        if (cellMaterial == CellMaterial.None) { return null; }
        if (!lookupDictionary.ContainsKey(cellMaterial))
        {
            lookupDictionary.Add(cellMaterial, materials.Find(x => x.cellMaterial == cellMaterial));
        }
        return lookupDictionary[cellMaterial];
    }
}