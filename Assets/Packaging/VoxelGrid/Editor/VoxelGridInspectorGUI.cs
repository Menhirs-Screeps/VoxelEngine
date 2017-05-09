using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VoxelGrid.Mesh))]
public class VoxelGridInspectorGUI : Editor {
	VoxelGrid.Mesh mesh;
	static bool scaleFromX;
	static bool centerMeshOnFloorPlane;
	static VoxelGeneratorStrategy strategy;
	void OnEnable()
	{
		mesh = (VoxelGrid.Mesh)target;
	}
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI ();
		scaleFromX = EditorGUILayout.Toggle("Scale model using X size", scaleFromX);
		centerMeshOnFloorPlane = EditorGUILayout.Toggle("Center mesh on floor plane", centerMeshOnFloorPlane);
		strategy = (VoxelGeneratorStrategy) EditorGUILayout.EnumPopup ("Generation strategy", strategy);
		if (GUILayout.Button ("Generate voxel prefab")) {
			mesh.GeneratePrefab_VoxelGrid (scaleFromX, centerMeshOnFloorPlane, strategy);
		}
	}
}
