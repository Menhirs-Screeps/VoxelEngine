using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VoxelGridPhysicSimulator))]
public class VoxelGridPhysicSimulatorGUI : Editor {
	VoxelGridPhysicSimulator simulator;
	void OnEnable()
	{
		simulator = (VoxelGridPhysicSimulator)target;
	}
	public override void OnInspectorGUI()
	{
		if (!simulator.threadRunning) {
			if (GUILayout.Button ("Start engine")) {
				simulator.StartThread ();
			}
		} else {
			if (GUILayout.Button ("Stop engine")) {
				simulator.StopThread ();
			}
		}
		foreach ( var body in simulator.bodies ) {
			EditorGUILayout.LabelField (body.name);
			if ( ! body.animated ) {
				if (GUILayout.Button ("Animate body")) {
					body.Animate ();
				}
			} else {
				if (GUILayout.Button ("Stop animation")) {
					body.Stop ();
				}
			}
			if (GUILayout.Button ("Reset body")) {
				body.Reset ();
			}
		}
		base.OnInspectorGUI ();
	}
}
