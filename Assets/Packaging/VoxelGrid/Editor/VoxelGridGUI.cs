using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VoxelGridBehaviour))]
public class VoxelGridGUI : Editor {[System.Flags]
	public enum ShaderFlags {
		HideElements	= 0x1,
		GlobalTest		= 0x2,
		DiscardInterior	= 0x4,
		DiscardHidden	= 0x8
	}
	VoxelGridBehaviour voxel;
	bool isPrefab;
	void OnEnable()
	{
		voxel = (VoxelGridBehaviour)target;
		isPrefab = PrefabUtility.GetPrefabType (voxel.gameObject) == PrefabType.Prefab;
	}
	public override void OnInspectorGUI()
	{
		float fvalue;
		bool allHidden = true;
		bool allGlobal = true;
		bool allInterior = true;
		bool allFaces = true;
		float fscale = 0.0f;
		base.OnInspectorGUI ();
		Material[] mats;
		if (isPrefab)
			mats = voxel.gameObject.GetComponent < MeshRenderer> ().sharedMaterials;
		else
			mats = voxel.gameObject.GetComponent < MeshRenderer> ().materials;
		foreach (var mat in mats) {
			allHidden = allHidden && (mat.GetFloat ("_DoNotShow") > 0.0f);
			allGlobal = allGlobal && (mat.GetFloat ("_TestGlobalConnectivity") > 0.0f);
			allInterior = allInterior && (mat.GetFloat ("_DiscardInterior") > 0.0f);
			allFaces = allFaces && (mat.GetFloat ("_DiscardConnectedFaces") > 0.0f);
			fscale += mat.GetFloat ("_ElementScale");
		}
		fscale /= mats.Length;
		ShaderFlags allvalues =
			(allHidden		? ShaderFlags.HideElements	: 0) |
			(allGlobal		? ShaderFlags.GlobalTest		: 0) |
			(allInterior	? ShaderFlags.DiscardInterior	: 0) |
			(allFaces		?	ShaderFlags.DiscardHidden	: 0);
		EditorGUILayout.LabelField ("Global settings");
		EditorGUI.BeginChangeCheck ();
		allvalues = (ShaderFlags) EditorGUILayout.MaskField ( "All flags:", (int) allvalues, new string[] { "hide", "global test", "discard interior", "discard faces" } );
		if (EditorGUI.EndChangeCheck ()) {
			foreach (var mat in mats) {
				if ( ( (allvalues & ShaderFlags.HideElements) > 0) != allHidden ) mat.SetFloat ("_DoNotShow", ( (allvalues & ShaderFlags.HideElements) > 0) ? 1.0f : 0.0f);
				if ( ( (allvalues & ShaderFlags.GlobalTest) > 0) != allGlobal ) mat.SetFloat ("_TestGlobalConnectivity", ( (allvalues & ShaderFlags.GlobalTest) > 0) ? 1.0f : 0.0f);
				if ( ( (allvalues & ShaderFlags.DiscardInterior) > 0) != allInterior ) mat.SetFloat ("_DiscardInterior", ( (allvalues & ShaderFlags.DiscardInterior) > 0) ? 1.0f : 0.0f);
				if ( ( (allvalues & ShaderFlags.DiscardHidden) > 0) != allFaces ) mat.SetFloat ("_DiscardConnectedFaces", ( (allvalues & ShaderFlags.DiscardHidden) > 0) ? 1.0f : 0.0f);
			}
		}
		EditorGUI.BeginChangeCheck ();
		fscale = EditorGUILayout.Slider ("All scale", fscale, 0.0f, 1.0f);
		if (EditorGUI.EndChangeCheck ()) {
			foreach (var mat in mats)
				mat.SetFloat ("_ElementScale", fscale );
		}
		EditorGUILayout.Space ();
		foreach (var mat in mats) {
			ShaderFlags values =
				(mat.GetFloat ("_DoNotShow") > 0.0f ?				ShaderFlags.HideElements	: 0) |
				(mat.GetFloat ("_TestGlobalConnectivity") > 0.0f ?	ShaderFlags.GlobalTest		: 0) |
				(mat.GetFloat ("_DiscardInterior") > 0.0f ?			ShaderFlags.DiscardInterior	: 0) |
				(mat.GetFloat ("_DiscardConnectedFaces") > 0.0f ?	ShaderFlags.DiscardHidden	: 0);
			Color c = mat.GetColor ( "_Color" );
			EditorGUI.BeginDisabledGroup (true);
			EditorGUILayout.ColorField ("Material:", c);
			EditorGUI.EndDisabledGroup ();
			EditorGUI.BeginChangeCheck ();
			values = (ShaderFlags) EditorGUILayout.MaskField ( "Flags:", (int) values, new string[] { "hide", "global test", "discard interior", "discard faces" } );
			if (EditorGUI.EndChangeCheck ()) {
				mat.SetFloat ("_DoNotShow",					((values & ShaderFlags.HideElements)>0)		? 1.0f : 0.0f);
				mat.SetFloat ("_TestGlobalConnectivity",	((values & ShaderFlags.GlobalTest)>0)		? 1.0f : 0.0f);
				mat.SetFloat ("_DiscardInterior",			((values & ShaderFlags.DiscardInterior)>0)	? 1.0f : 0.0f);
				mat.SetFloat ("_DiscardConnectedFaces",		((values & ShaderFlags.DiscardHidden)>0)	? 1.0f : 0.0f);
			}
			fvalue = mat.GetFloat ("_ElementScale");
			EditorGUI.BeginChangeCheck ();
			fvalue = EditorGUILayout.Slider ("Elements scale", fvalue, 0.0f, 1.0f);
			if (EditorGUI.EndChangeCheck ()) {
				mat.SetFloat ("_ElementScale", fvalue );
			}
		}
	}
}
