using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TruncatedSphereShape : ForceShape
{
	[Range(0.0f,1.0f)]
	public float minimumDistance = 0.0f;
	public override void Draw (Transform transform) {
		Vector3 oldMax = Vector3.zero;
		for (float angle = 0.0f; angle <= 360.0f; angle += 20.0f) {
			Vector3 pMax = transform.TransformPoint (0.5f * new Vector3 (Mathf.Cos (Mathf.Deg2Rad * angle), 0.0f, Mathf.Sin (Mathf.Deg2Rad * angle)));
			if (angle > 0.0f) {
				Gizmos.DrawLine (oldMax, pMax);
			}
			oldMax = pMax;
		}
		oldMax = Vector3.zero;
		for (float angle = 0.0f; angle <= 360.0f; angle += 20.0f) {
			Vector3 pMax = transform.TransformPoint (0.5f * new Vector3 (Mathf.Cos (Mathf.Deg2Rad * angle), Mathf.Sin (Mathf.Deg2Rad * angle), 0.0f));
			if (angle > 0.0f) {
				Gizmos.DrawLine (oldMax, pMax);
			}
			oldMax = pMax;
		}
		oldMax = Vector3.zero;
		for (float angle = 0.0f; angle <= 360.0f; angle += 20.0f) {
			Vector3 pMax = transform.TransformPoint (0.5f * minimumDistance * new Vector3 (Mathf.Cos (Mathf.Deg2Rad * angle), 0.0f, Mathf.Sin (Mathf.Deg2Rad * angle)));
			if (angle > 0.0f) {
				Gizmos.DrawLine (oldMax, pMax);
			}
			oldMax = pMax;
		}
		oldMax = Vector3.zero;
		for (float angle = 0.0f; angle <= 360.0f; angle += 20.0f) {
			Vector3 pMax = transform.TransformPoint (0.5f * minimumDistance * new Vector3 (Mathf.Cos (Mathf.Deg2Rad * angle), Mathf.Sin (Mathf.Deg2Rad * angle), 0.0f));
			if (angle > 0.0f) {
				Gizmos.DrawLine (oldMax, pMax);
			}
			oldMax = pMax;
		}
	}
	public override bool Affected(Transform transform, Particle p) {
		Vector3 pos = transform.InverseTransformPoint (p.position);
		if (pos.magnitude > 0.5f)
			return false;
		if (pos.magnitude < 0.5f * minimumDistance)
			return false;
		return true;
	}
	public override Vector3 Affect(Transform transform, Particle p) {
		Vector3 pos = transform.InverseTransformPoint (p.position);
		Vector3 d = - pos;
		Vector3 res = Vector3.zero;
		float l = (d.magnitude - minimumDistance*0.5f) / (0.5f - minimumDistance*0.5f);
		float f = 1.0f - l;
		d.Normalize ();
		d = transform.TransformDirection (d);
		res = f * d;
		return d;
	}
	public override bool Affected(Matrix4x4 w2l, Matrix4x4 l2w, Particle p) {
		Vector3 pos = w2l.MultiplyPoint(p.position);
		if (pos.magnitude > 0.5f)
			return false;
		if (pos.magnitude < 0.5f * minimumDistance)
			return false;
		return true;
	}
	public override Vector3 Affect(Matrix4x4 w2l, Matrix4x4 l2w, Particle p) {
		Vector3 pos = w2l.MultiplyPoint(p.position);
		Vector3 d = - pos;
		Vector3 res = Vector3.zero;
		float l = (d.magnitude - minimumDistance*0.5f) / (0.5f - minimumDistance*0.5f);
		float f = 1.0f - l;
		d.Normalize ();
		d = l2w.MultiplyVector(d);
		res = f * d;
		return d;
	}

}
