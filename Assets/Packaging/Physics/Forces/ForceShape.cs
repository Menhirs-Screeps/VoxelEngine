using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ForceShape
{
	public virtual void Draw (Transform transform) {
	}
	public virtual bool Affected(Transform transform, Particle p) {
		return false;
	}
	public virtual Vector3 Affect(Transform transform, Particle p) {
		return Vector3.zero;
	}
	public virtual bool Affected(Matrix4x4 w2l, Matrix4x4 l2w, Particle p) {
		return false;
	}
	public virtual Vector3 Affect(Matrix4x4 w2l, Matrix4x4 l2w, Particle p) {
		return Vector3.zero;
	}
}
