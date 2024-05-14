using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CellularMaterials", menuName = "Atto/CellularAutomataMaterial")]
public class CellularMaterials : ScriptableObject
{
    static public CellularMaterials instance { get { if (instance_ == null) { instance_ = Resources.Load<CellularMaterials>("CellularMaterials"); }; return instance_; } }
    static private CellularMaterials instance_;

    public List<CellMaterialProperties> materials;
    public List<CellMaterialReaction> reactions;

    static Dictionary<CellMaterial, CellMaterialProperties> materialLookupDictionary = new Dictionary<CellMaterial, CellMaterialProperties>();
    static Dictionary<CellMaterial, Dictionary<CellMaterial, CellMaterialReaction>> reactionLookupDictionary = new Dictionary<CellMaterial, Dictionary<CellMaterial, CellMaterialReaction>>();

    private void OnEnable()
    {
        InitializeDictionaries();
    }

    private void InitializeDictionaries()
    {
        materialLookupDictionary.Clear();
        reactionLookupDictionary.Clear();

        foreach (var material in materials)
        {
            materialLookupDictionary[material.cellMaterial] = material;
        }

        foreach (var reaction in reactions)
        {
            if (!reactionLookupDictionary.ContainsKey(reaction.reactorA))
            {
                reactionLookupDictionary[reaction.reactorA] = new Dictionary<CellMaterial, CellMaterialReaction>();
            }
            if (!reactionLookupDictionary.ContainsKey(reaction.reactorB))
            {
                reactionLookupDictionary[reaction.reactorB] = new Dictionary<CellMaterial, CellMaterialReaction>();
            }

            reactionLookupDictionary[reaction.reactorA][reaction.reactorB] = reaction;
            reactionLookupDictionary[reaction.reactorB][reaction.reactorA] = reaction; // Ensure both orders are stored
        }
    }

    public CellMaterialProperties FindMaterial(CellMaterial cellMaterial)
    {
        return materialLookupDictionary[cellMaterial];
    }

    public CellMaterialReaction FindReaction(CellMaterial reactorA, CellMaterial reactorB)
    {
        if (reactionLookupDictionary.ContainsKey(reactorA) && reactionLookupDictionary[reactorA].ContainsKey(reactorB))
        {
            return reactionLookupDictionary[reactorA][reactorB];
        }
        else if (reactionLookupDictionary.ContainsKey(reactorB) && reactionLookupDictionary[reactorB].ContainsKey(reactorA))
        {
            return reactionLookupDictionary[reactorB][reactorA];
        }
        else
        {
            return null; // No reaction found
        }
    }
}
