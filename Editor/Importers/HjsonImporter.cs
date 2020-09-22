#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using System.IO;

namespace Atto.Editor.Importers
{
	[UnityEditor.AssetImporters.ScriptedImporter(1, "hjson")]
	public class HjsonImporter : UnityEditor.AssetImporters.ScriptedImporter
	{
		public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext context)
		{
			TextAsset subAsset = new TextAsset(File.ReadAllText(context.assetPath));
			context.AddObjectToAsset("text", subAsset);
			context.SetMainObject(subAsset);
		}
	}
}
#endif
