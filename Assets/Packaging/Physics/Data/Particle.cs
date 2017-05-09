using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Particle
{
	public static float sleepEpsilon = 0.01f;
	public int ID;
	public float mass;
	public float inverseMass;
	public float size;
	public bool isFixed;
	//
	public Vector3 initialPosition;
	public Vector3 forces;
	public Vector3 acceleration;
	public Vector3 velocity;
	public float speed;
	public float time;
	public Vector3 position;
	public override string ToString ()
	{
		return ID + " : " + size + " , " + position.ToString("F10") + " , " + forces + " , " + acceleration + " , " + velocity + " / " + speed + " , " + KineticEnergy;
	}
	//
	public float KineticEnergy;
	public float Power;
	public float dKE;
	bool awake = true;
	void SetAwake ( bool awk = true )
	{
		if (awk) {
			awake= true;
			// Add a bit of motion to avoid it falling asleep immediately.
			KineticEnergy = sleepEpsilon*2.0f;
		} else {
			awake = false;
			velocity = Vector3.zero;
			speed = 0.0f;
		}
	}
	bool GetAwake ()
	{
		return awake;
	}
	public bool Awake {
		get { return GetAwake (); }
		set { SetAwake (value); }
	}
	public void MatchAwakeState(Particle body1)
	{
		bool body0awake = GetAwake();
		bool body1awake = body1.GetAwake();

		// Wake up only the sleeping one
		if (body0awake ^ body1awake) {
			if (body0awake) body1.SetAwake();
			else SetAwake();
		}
	}
	public void UpdateForces( List<VoxelGridForce> globalForces, float deltaTime ) {
		forces = Vector3.zero;
		if (isFixed)
			return;
		foreach (var force in globalForces) {
			if (force.Affect (this).magnitude > 0.0f)
				awake = true;
		}
	}
	public void UpdateAccelerations( float deltaTime ) {
		acceleration = forces / mass;
	}
	public void UpdateVelocities( float deltaTime ) {
		Vector3 v = acceleration * deltaTime + speed * velocity;
		speed = v.magnitude;
		velocity = v.normalized;
		time = deltaTime;
	}
	public void UpdatePositions( float deltaTime ) {
		position = position + velocity * speed * deltaTime;
	}
	//
	public bool Collide(out CollisionInfo local, float deltaTime) {
		local = new CollisionInfo ();
		Vector3 p = position;
		Vector3 d = velocity;
		if (VoxelGridPhysicSimulator.Instance.Raycast (p, d, out local, speed + size, speed)) {
			Vector3 planeDirection = local.collisionNormal;
			float planeOffset = - Vector3.Dot ( local.collisionNormal, local.collisionPoint );
			float ballDistance = Vector3.Dot ( planeDirection, position ) - (size/2.0f) - planeOffset;
			if (ballDistance >= 0) return false;
			local.collisionDistance = ballDistance;
			local.penetration = -ballDistance;
			local.collisionPoint = position - planeDirection * ( ballDistance + size/2.0f);
			local.reference = this;
			return true;
		}
		return false;
	}
	public bool FastCollide(Particle hitable, out CollisionInfo local, float deltaTime) {
		#if true
		local = Collider.Collides (new AABB (position, Vector3.one * size / 2.0f), new AABB (hitable.position, Vector3.one * hitable.size / 2.0f));
		//local = Collider.Collides (new Sphere (position, size / 2.0f), new Sphere (hitable.position, hitable.size / 2.0f));
		if (local != null) {
			local.reference = this;
			local.hitSphere = hitable;
		}
		return (local != null);
		#else
		// Cache the sphere positions
		Vector3 positionOne = position;
		Vector3 positionTwo = hitable.position;

		// Find the vector between the objects
		Vector3 midline = positionOne - positionTwo;
		float sz = midline.magnitude;

		// See if it is large enough.
		if (sz < 0.0f)
			return false;
		if (sz > ((size+hitable.size)/2.0f))
			return false;
		// We manually create the normal, because we have the
		// size to hand.
		Vector3 normal = midline.normalized;

		local.collisionNormal = normal;
		local.collisionPoint = positionOne + midline * 0.5f;
		local.collisionDistance = sz;
		local.penetration = (size + hitable.size) / 2.0f - sz;
		local.reference = this;
		local.hitSphere = hitable;
		local.collideWithWorld = false;
		#endif
		return true;
	}
	#if false
	public bool Collide(Particle hitable, out CollisionInfo local, float deltaTime) {
		local = new CollisionInfo ();
		float t = Mathf.Infinity;
		float distance = (position - hitable.position).magnitude;
		Vector3 a = hitable.acceleration - acceleration;
		Vector3 v = hitable.speed * hitable.velocity - speed * velocity;
		Vector3 p = hitable.position - position;
		if (Vector3.Dot (v, p) > Mathf.Epsilon)
			return false;
		float s = (hitable.size + size) / 2.0f;
		Polynomial polynom = new Polynomial ();
		polynom.Add (4, Vector3.Dot (a, a));
		polynom.Add (3, 2*Vector3.Dot (a, v));
		polynom.Add (2, 2*Vector3.Dot (a, p)+Vector3.Dot(v,v));
		polynom.Add (1, 2*Vector3.Dot (v, p));
		polynom.Add (0, Vector3.Dot (p, p)-s);
		if (polynom.Degree == 0)
			return false;
		SturmSequence solver = new SturmSequence (polynom);
		List<double> roots = solver.FindRoots (0.0, deltaTime);
		if (roots != null) {
			float r = (float)roots [0];
			Vector3 p0 = (position + r * speed * velocity + r * r * acceleration);
			Vector3 p1 = (hitable.position + r * hitable.speed * hitable.velocity + r * r * hitable.acceleration);
			local.collisionPoint = (p0 + p1) / 2.0f;
			local.collisionNormal = (p1 - p0).normalized;
			local.collisionTime = r;
			local.reference = this;
			local.hitSphere = hitable;
			local.collideWithWorld = false;
			return true;
		}
		return false;
	}
	float RealDistanceToIntersectionWithPlane(CollisionInfo local) {
		float s = size / 2.0f;
		float S = Vector3.Dot (local.collisionNormal, position - local.collisionPoint);
		float dp = (position - local.collisionPoint).magnitude;
		float dc = (1.0f - s / S) * dp;
		if (dc < 0.0f)
			return -1.0f;
		return dc;
	}
	float TimeToGoTo (Vector3 pc) {
		float c = Vector3.Dot (pc, position) - Vector3.Dot(pc,pc);
		float b = Vector3.Dot (pc, speed * velocity);
		float a = Vector3.Dot (pc, acceleration);
		float delta = b * b - 4 * a * c;
		float t0 = (-b -Mathf.Sqrt(delta))/ (2 * a);
		if (t0 < 0)
			t0 = Mathf.Infinity;
		float t1 = (-b +Mathf.Sqrt(delta))/ (2 * a);
		if (t1 < 0)
			t1 = Mathf.Infinity;
		float t = Mathf.Min (t0, t1);
		return t;
	}
	#endif
}
