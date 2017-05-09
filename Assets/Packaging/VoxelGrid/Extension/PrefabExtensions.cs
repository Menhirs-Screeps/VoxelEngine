using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static partial class PrefabExtensions {
	public static void GeneratePrefab_VoxelGrid ( this VoxelGrid.Mesh mesh, bool scaleFromX, bool centerMeshOnFloorPlane, VoxelGeneratorStrategy strategy ) {
		string prefabFilename = AssetDatabase.GenerateUniqueAssetPath( Path.Combine(Path.GetDirectoryName(AssetDatabase.GetAssetPath (mesh)),mesh.name+" - Voxel.prefab"));
		Vector3 center = new Vector3 (0, 0, 0);
		float scale = 1.0f;
		if ( scaleFromX ) scale /= mesh.dimensions.x;
		if ( centerMeshOnFloorPlane )
		{
			center.x = mesh.dimensions.x / 2.0f;
			center.z = mesh.dimensions.z / 2.0f;
		}
		Mesh unityMesh = mesh.ToMesh ( scale, center, strategy );
		unityMesh.name = mesh.name + "- voxels";

		List<Material> mats = new List < Material > ();
		foreach (var mat in mesh.materials) {
			Material unityMat = mat.ToMaterial (Shader.Find ("Voxel/Standard"));
			unityMat.SetFloat ("_ElementSize", scale);
			unityMat.name = mesh.name + "- mat (voxels) -" + mat.id;
			mats.Add (unityMat);
			AssetDatabase.AddObjectToAsset(unityMat, prefabFilename );
		}

		UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab(prefabFilename);
		AssetDatabase.AddObjectToAsset(unityMesh, prefabFilename );

		GameObject gameObject = new GameObject();
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		meshFilter.mesh = unityMesh;
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.materials = mats.ToArray ();
		VoxelGridBehaviour grid = gameObject.AddComponent<VoxelGridBehaviour>();
		grid.gridSize = mesh.dimensions;
		grid.gridReference = center;
		grid.elementSize = scale;
		EditorUtility.SetDirty(gameObject);
		GameObject prefabGameObject = PrefabUtility.ReplacePrefab(gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
		GameObject.DestroyImmediate(gameObject);
	}
}
