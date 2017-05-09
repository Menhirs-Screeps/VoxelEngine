using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public struct Shape
{
	public ForceShapeType type;
	public SphereShape sphere;
	public TruncatedSphereShape truncsphere;
	public BoxShape box;
	public ConeShape cone;
	public void Draw (Transform transform) {
		if (type == ForceShapeType.Sphere)
			sphere.Draw (transform);
		if (type == ForceShapeType.TruncatedSphere)
			truncsphere.Draw (transform);
		if (type == ForceShapeType.Box)
			box.Draw (transform);
		if (type == ForceShapeType.Cone)
			cone.Draw (transform);
	}
	public bool Affected(Transform transform, Particle p) {
		if (type == ForceShapeType.Sphere)
			return sphere.Affected (transform, p);
		if (type == ForceShapeType.TruncatedSphere)
			return truncsphere.Affected (transform,p);
		if (type == ForceShapeType.Box)
			return box.Affected (transform, p);
		if (type == ForceShapeType.Cone)
			return cone.Affected (transform, p);
		return false;
	}
	public Vector3 Affect(Transform transform, Particle p) {
		if (type == ForceShapeType.Sphere)
			return sphere.Affect (transform, p);
		if (type == ForceShapeType.TruncatedSphere)
			return truncsphere.Affect (transform,p);
		if (type == ForceShapeType.Box)
			return box.Affect (transform, p);
		if (type == ForceShapeType.Cone)
			return cone.Affect (transform, p);
		return Vector3.zero;
	}


	public bool Affected(Matrix4x4 w2l, Matrix4x4 l2w, Particle p) {
		if (type == ForceShapeType.Sphere)
			return sphere.Affected (w2l, l2w, p);
		if (type == ForceShapeType.TruncatedSphere)
			return truncsphere.Affected (w2l, l2w,p);
		if (type == ForceShapeType.Box)
			return box.Affected (w2l, l2w, p);
		if (type == ForceShapeType.Cone)
			return cone.Affected (w2l, l2w, p);
		return false;
	}
	public Vector3 Affect(Matrix4x4 w2l, Matrix4x4 l2w, Particle p) {
		if (type == ForceShapeType.Sphere)
			return sphere.Affect (w2l, l2w, p);
		if (type == ForceShapeType.TruncatedSphere)
			return truncsphere.Affect (w2l, l2w,p);
		if (type == ForceShapeType.Box)
			return box.Affect (w2l, l2w, p);
		if (type == ForceShapeType.Cone)
			return cone.Affect (w2l, l2w, p);
		return Vector3.zero;
	}
}

public class VoxelGridForce : MonoBehaviour {
	public ForceType directionalEffect;
	public ForceDirection type;
	public float maximumMagnitude;
	public bool impulse;
	public bool active;
	public Shape shape;
	public Matrix4x4 LocalToWorld;
	public Matrix4x4 WorldToLocal;
	public Vector3 up;
	void Awake () {
	}
	void Start ()
	{
		up = transform.up;
		LocalToWorld = transform.localToWorldMatrix;
		WorldToLocal = transform.worldToLocalMatrix;
		VoxelGridPhysicSimulator.Instance.AddForce (this);
	}
	#if false
	public virtual Vector3 Affect(Particle p) {
		if (!active)
			return Vector3.zero;
		//Debug.Log (p.position + " " + transform.InverseTransformPoint (p.position) + " " + (transform.worldToLocalMatrix.MultiplyPoint(p.position)));
		if (!shape.Affected (transform, p))
			return Vector3.zero;
		Vector3 f;
		if (directionalEffect == ForceType.Directional)
			f = maximumMagnitude * (-transform.up);
		else if (directionalEffect == ForceType.ParticleDirection)
			f = maximumMagnitude * p.speed * p.velocity;
		else {
			f = shape.Affect (transform, p);
			f = f * maximumMagnitude;
		}
		if (type == ForceDirection.Repulsive)
			f = -f;
		p.forces += f;
		return f;
	}
	#else
	public virtual Vector3 Affect(Particle p) {
		if (!active)
			return Vector3.zero;
		if (!shape.Affected (/*transform*/WorldToLocal, LocalToWorld, p))
			return Vector3.zero;
		Vector3 f;
		if (directionalEffect == ForceType.Directional)
			f = maximumMagnitude * (-up);
		else if (directionalEffect == ForceType.ParticleDirection)
			f = maximumMagnitude * p.speed * p.velocity;
		else {
			f = shape.Affect (/*transform*/WorldToLocal, LocalToWorld, p);
			f = f * maximumMagnitude;
		}
		if (type == ForceDirection.Repulsive)
			f = -f;
		p.forces += f;
		return f;
	}
	#endif
	void OnDrawGizmosSelected () {
		if (type == ForceDirection.Attractive)
			Gizmos.color = Color.blue;
		else
			Gizmos.color = Color.red;
		shape.Draw (transform);
	}
}
