using UnityEngine;
using UnityEditor;

using System.IO;

[UnityEditor.Experimental.AssetImporters.ScriptedImporter(1, "hjson")]
public class HjsonImporter : UnityEditor.Experimental.AssetImporters.ScriptedImporter
{
	public override void OnImportAsset(UnityEditor.Experimental.AssetImporters.AssetImportContext context)
	{
		TextAsset subAsset = new TextAsset(File.ReadAllText(context.assetPath));
		context.AddObjectToAsset("text", subAsset);
		context.SetMainObject(subAsset);
	}
}