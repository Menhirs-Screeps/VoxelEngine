using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MagicaVoxelPostProcessor : AssetPostprocessor
{
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
	{
		foreach(string asset in importedAssets)
		{
			if (Path.GetExtension (asset) == ".vox") {
				string fname = Path.Combine ( Path.GetDirectoryName (asset), Path.GetFileNameWithoutExtension(asset) + ".asset");
				FileStream fs = new FileStream(asset, FileMode.Open);
				int len = (int)fs.Length;
				byte[] fileBytes = new byte[len];
				fs.Read(fileBytes, 0, len);
				fs.Close ();
				MagicaVoxelFile voxfilecontent = ScriptableObject.CreateInstance<MagicaVoxelFile> ();
				voxfilecontent.content = new MagicaVoxel.File ();
				voxfilecontent.content.PopulateFromBuffer ( fileBytes, 0 );
				voxfilecontent.filename = asset;
				voxfilecontent.flags = new List<string> ();
				AssetDatabase.CreateAsset (voxfilecontent, fname);
				AssetDatabase.SaveAssets ();
				AssetDatabase.Refresh ();
				AssetDatabase.ImportAsset(fname,ImportAssetOptions.ForceUpdate);
			}
		}
	}
}