using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConeShape : ForceShape
{
	public float minimumDistance = 0.0f;
	public float maximumDistance = 1.0f;
	public float angle = 15.0f;
	public float initialRadius = 0.0f;
	public override void Draw (Transform transform) {
		float rMax = initialRadius+maximumDistance * Mathf.Sin (angle * Mathf.PI / 180.0f);
		float rMin = initialRadius+minimumDistance * Mathf.Sin (angle * Mathf.PI / 180.0f);
		Vector3 oldMin = Vector3.zero;
		Vector3 oldMax = Vector3.zero;
		for (float a = 0.0f; a <= 360.0f; a += 30.0f) {
			float ar = a * Mathf.PI / 180.0f;
			Vector3 pMin = transform.TransformPoint (new Vector3 (rMin * Mathf.Cos (ar), minimumDistance * Mathf.Cos (angle * Mathf.PI / 180.0f), rMin * Mathf.Sin (ar)));
			Vector3 pMax = transform.TransformPoint (new Vector3 (rMax * Mathf.Cos (ar), maximumDistance * Mathf.Cos (angle * Mathf.PI / 180.0f), rMax * Mathf.Sin (ar)));
			if (a > 0.0f) {
				Gizmos.color = Color.red;
				Gizmos.DrawLine (pMin, oldMin);
				Gizmos.color = Color.blue;
				Gizmos.DrawLine (oldMax, pMax);
			}
			Gizmos.color = Color.green;
			Gizmos.DrawLine (pMin, pMax);
			oldMin = pMin;
			oldMax = pMax;
		}
	}
	public override bool Affected(Transform transform, Particle p) {
		Vector3 pos = transform.position;
		Vector3 d = pos - p.position;
		if (d.magnitude < minimumDistance)
			return false;
		if (d.magnitude > maximumDistance)
			return false;
		float dist = d.magnitude;
		d.Normalize ();
		if (Vector3.Angle (transform.up, -d) > angle)
			return false;
		return true;
	}
	public override Vector3 Affect(Transform transform, Particle p) {
		Vector3 pos = transform.position;
		Vector3 d = pos - p.position;
		float dist = d.magnitude;
		d.Normalize ();
		float l = (dist - minimumDistance) / (maximumDistance - minimumDistance);
		float f = 1.0f - l;
		d.Normalize ();
		//d = transform.TransformDirection (d);
		return d;
	}
}
