using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelCollider : MonoBehaviour {
	public AABB shape;
	void Start () {
	}
	void Update () {
	}
	void OnDrawGizmosSelected () {
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube (shape.center, shape.halfSize * 2.0f);
	}
}
