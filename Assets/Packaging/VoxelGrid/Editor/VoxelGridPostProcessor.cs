using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class VoxelGridPostProcessor: AssetPostprocessor
{
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
	{
		foreach(string asset in importedAssets)
		{
			MagicaVoxelFile voxfilecontent = AssetDatabase.LoadAssetAtPath < MagicaVoxelFile > (asset);
			if (voxfilecontent != null) {
				if (!voxfilecontent.flags.Contains ("VoxelGridPostProcessor_done")) {
					if (!voxfilecontent.flags.Contains ("VoxelGridPostProcessor_running")) {
						voxfilecontent.flags.Add ("VoxelGridPostProcessor_running");
						AssetDatabase.SaveAssets ();
						AssetDatabase.Refresh ();
						Dictionary<int,VoxelGrid.Material> mats = new Dictionary<int, VoxelGrid.Material> ();
						for (int i = 0; i < voxfilecontent.content.main.models.Length; i++) {
							VoxelGrid.Mesh voxel = voxfilecontent.ToVoxelGrid (i);
							foreach (var mat in voxel.materials) {
								mats.Add (mat.id, mat);
							}
							voxel.name = Path.GetFileNameWithoutExtension (asset) + " - sub" + i;
							AssetDatabase.AddObjectToAsset (voxel, asset);
						}
						foreach (var item in mats) {
							Material mat = item.Value.ToMaterial (Shader.Find ("Standard"));
							mat.name = Path.GetFileNameWithoutExtension (asset) + " - mat" + item.Key;
							AssetDatabase.AddObjectToAsset (mat, asset);
						}
						voxfilecontent.flags.Remove ("VoxelGridPostProcessor_running");
					}
					voxfilecontent.flags.Add ("VoxelGridPostProcessor_done");
					AssetDatabase.SaveAssets ();
					AssetDatabase.Refresh ();
				}
				/*
				*/
			}
		}
	}
}
