using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(MeshFilter))]
public class VoxelRigidBody : MonoBehaviour {
	public VoxelGridBehaviour vx;
	public Mesh theMesh;

	public bool animated = false;
	public bool internalCollider = false;
	public bool worldCollider = true;
	public float elementMass;
	public float elementSize;
	public float collisionDampen;

	public Vector3[] gridElements;
	public bool modified = false;

	UnityEngine.Object inSimulation = new UnityEngine.Object();

	public int physicalBodyID;
	void Awake () {
		vx = GetComponent < VoxelGridBehaviour > ();
		theMesh = GetComponent<MeshFilter> ().mesh;
	}
	void Start () {
		physicalBodyID = VoxelGridPhysicSimulator.Instance.AddBody (this);
		gridElements = (Vector3[]) theMesh.vertices;
		particles = new Particle[gridElements.Length];
		for (int i = 0; i < gridElements.Length; i++) {
			particles [i] = new Particle ();
			particles [i].ID = i;
			particles [i].mass = elementMass;
			particles [i].inverseMass = 1.0f / elementMass;
			particles [i].size = elementSize;
			particles [i].isFixed = false;
			particles [i].initialPosition = transform.TransformPoint( gridElements[i] );
			//
			particles [i].position = particles [i].initialPosition;
			particles [i].forces = Vector3.zero;
			particles [i].acceleration = Vector3.zero;
			particles [i].velocity = Vector3.zero;
			particles [i].speed = 0.0f;
			particles [i].time = 0.0f;

			particles [i].Awake = true;
		}
	}
	void Update () {
		lock (inSimulation)
		{
			if (modified)
			{
				for (int i = 0; i < particles.Length; i++) {
					gridElements [i] = transform.InverseTransformPoint ( particles [i].position );
				}
				theMesh.vertices = gridElements;
				if (vx == null) {
					theMesh.RecalculateNormals ();
					theMesh.RecalculateBounds ();
				}
				modified = false;
			}
		}
	}
	public Particle[] particles;
	public void Reset () {
		animated = false;
		if (particles == null)
			return;
		for (int i = 0; i < particles.Length; i++) {
			gridElements [i] = transform.InverseTransformPoint ( particles [i].initialPosition );
			particles [i].position = particles [i].initialPosition;
			particles [i].forces = Vector3.zero;
			particles [i].acceleration = Vector3.zero;
			particles [i].velocity = Vector3.zero;
			particles [i].speed = 0.0f;
			particles [i].time = 0.0f;
		}
		modified = true;
	}
	public void Animate () {
		animated = true;
	}
	public void Stop () {
		animated = false;
	}
	string logStr;
	public bool ParticleCollision ( Particle particle, float remainingTime, out List<CollisionInfo> collisions ) {
		List<CollisionInfo> cols = new List<CollisionInfo> ();
		Vector3 p = particle.position;
		Vector3 d = particle.velocity;
		float dt = Mathf.Infinity;
		CollisionInfo local = new CollisionInfo ();
		if (worldCollider) {
			List<CollisionInfo> cworld = VoxelGridPhysicSimulator.Instance.CollidesWithWorld (particle);
			foreach (var c in cworld)
				cols.Add (c);
			/*
			if (particle.Collide (out local, remainingTime)) {
				dt = local.collisionTime;
				cols.Add (local);
			}
			*/
		}
		if (internalCollider) {
			for (int j = particle.ID + 1; j < particles.Length; j++) {
				if (particle.FastCollide (particles [j], out local, remainingTime)) {
					if (local.collisionTime < dt) {
						cols.Clear ();
						cols.Add (local);
						dt = local.collisionTime;
					} else if (local.collisionTime == dt)
						cols.Add (local);
				}
			}
		}
		collisions = cols.OrderBy(o=>o.collisionTime).ToList();
		return cols.Count > 0;
	}
	public bool GlobalCollision ( float remainingTime, out List<CollisionInfo> collisions )
	{
		collisions = new List<CollisionInfo> ();
		for ( int parti = 0 ; parti < particles.Length ; parti ++ ) {
			List<CollisionInfo> cols = null;
			if (ParticleCollision(particles[parti],remainingTime,out cols)) {
				foreach (var col in cols) {
					collisions.Add (col);
				}
			}
		}
		collisions = collisions.OrderBy(o=>o.collisionTime).ToList();
		return collisions.Count > 0;
	}
	public void UpdateForces ( List<VoxelGridForce> globalForces, float deltaTime ) {
		for (int i = 0; i < particles.Length; i++) {
			particles [i].UpdateForces (globalForces, deltaTime);
		}
	}
	public void UpdateAccelerations( float deltaTime ) {
		for (int i = 0; i < particles.Length; i++) {
			particles [i].UpdateAccelerations (deltaTime);
		}
	}
	public void UpdateVelocities( float deltaTime ) {
		for (int i = 0; i < particles.Length; i++) {
			particles [i].UpdateVelocities (deltaTime);
		}
	}
	public void UpdatePositions( float deltaTime ) {
		for (int i = 0; i < particles.Length; i++) {
			particles [i].UpdatePositions (deltaTime);
			//gridElements [i] = transform.InverseTransformPoint ( particles [i].position );
		}
		modified = true;
	}
	public void UpdateAwakeState (float deltaTime )
	{
		for (int i = 0; i < particles.Length; i++) {
			particles [i].Power = particles [i].speed * particles [i].speed;
			particles [i].dKE = particles [i].mass * particles [i].Power * deltaTime * deltaTime / 2.0f;
			float bias = Mathf.Pow (0.5f, deltaTime);
			particles [i].KineticEnergy = bias * particles [i].KineticEnergy + (1 - bias) * particles [i].Power;
			if (particles [i].KineticEnergy < Particle.sleepEpsilon)
				particles [i].Awake = false;
			else if (particles [i].KineticEnergy > 10 * Particle.sleepEpsilon)
				particles [i].KineticEnergy = 10 * Particle.sleepEpsilon;
		}
	}
	/// <summary>
	/// constraint solver.
	/// </summary>
	/// <param name="collision">Collision.</param>
	/// <param name="deltaTime">Delta time.</param>
	public void ResolveVelocity ( CollisionInfo collision,float deltaTime ) {
		float restitution = 0.9f;
		// Find the velocity in the direction of the contact.
		float separatingVelocity = collision.separationVelocity;
		// Check whether it needs to be resolved.
		if (separatingVelocity > 0)
		{
			// The contact is either separating or stationary - there’s
			// no impulse required.
			return;
		}
		// Calculate the new separating velocity.
		float newSepVelocity = -separatingVelocity * restitution;
		// Check the velocity build-up due to acceleration only.
		Vector3 accCausedVelocity = collision.reference.acceleration;
		if (!collision.collideWithWorld) accCausedVelocity -= collision.hitSphere.acceleration;
		float accCausedSepVelocity = Vector3.Dot (accCausedVelocity, collision.collisionNormal) * deltaTime;
		// If we’ve got a closing velocity due to acceleration build-up,
		// remove it from the new separating velocity.
		if (accCausedSepVelocity < 0)
		{
			newSepVelocity += restitution * accCausedSepVelocity;
			// Make sure we haven’t removed more than was
			// there to remove.
			if (newSepVelocity < 0) newSepVelocity = 0;
		}
		float deltaVelocity = newSepVelocity - separatingVelocity;
		// We apply the change in velocity to each object in proportion to
		// its inverse mass (i.e., those with lower inverse mass [higher
		// actual mass] get less change in velocity).
		float totalInverseMass = collision.reference.inverseMass;
		if (!collision.collideWithWorld) totalInverseMass += collision.hitSphere.inverseMass;
		// If all particles have infinite mass, then impulses have no effect.
		if (totalInverseMass <= 0) return;
		// Calculate the impulse to apply.
		float impulse = deltaVelocity / totalInverseMass;
		// Find the amount of impulse per unit of inverse mass.
		Vector3 impulsePerIMass = collision.collisionNormal * impulse;
		// Apply impulses: they are applied in the direction of the contact,
		// and are proportional to the inverse mass.
		Vector3 v0 = collision.reference.speed * collision.reference.velocity + impulsePerIMass * collision.reference.inverseMass;
		collision.reference.speed = v0.magnitude;
		collision.reference.velocity = v0.normalized;
		if (!collision.collideWithWorld)
		{
			// Particle 1 goes in the opposite direction.
			Vector3 v1 = collision.hitSphere.speed * collision.hitSphere.velocity + impulsePerIMass * (-collision.hitSphere.inverseMass);
			collision.hitSphere.speed = v1.magnitude;
			collision.hitSphere.velocity = v1.normalized;
		}
	}
	public void ResolveInterpenetration ( CollisionInfo collision,float deltaTime ) {
		// If we don't have any penetration, skip this step.
		if (collision.penetration <= 0) return;

		// The movement of each object is based on their inverse mass, so
		// total that.
		float totalInverseMass = collision.reference.inverseMass;
		if (!collision.collideWithWorld) totalInverseMass += collision.hitSphere.inverseMass;

		// If all particles have infinite mass, then we do nothing
		if (totalInverseMass <= 0) return;

		// Find the amount of penetration resolution per unit of inverse mass
		Vector3 movePerIMass = collision.collisionNormal * (collision.penetration / totalInverseMass);

		// Calculate the the movement amounts
		collision.referenceMovement = movePerIMass * collision.reference.inverseMass;
		if (!collision.collideWithWorld) {
			collision.hitMovement = movePerIMass * (-collision.hitSphere.inverseMass);
		} else {
			collision.hitMovement = Vector3.zero;
		}

		// Apply the penetration resolution
		collision.reference.position = collision.reference.position + collision.referenceMovement;
		if (!collision.collideWithWorld) {
			collision.hitSphere.position = collision.hitSphere.position + collision.hitMovement;
		}
	}
	public void Resolve ( CollisionInfo collision,float deltaTime ) {
		#if false// DEBUG Logger
		{//if (!collision.collideWithWorld && collision.penetration > 0.0f) {
			logStr += "\n      * Pre Vel Resolution";
			logStr += "\nref : " + collision.reference + "\nhit : " + collision.hitSphere + "\ncol : " + collision;
		}
		#endif
		ResolveInterpenetration (collision, deltaTime);
		#if false// DEBUG Logger
		{//if (!collision.collideWithWorld && collision.penetration > 0.0f) {
			logStr += "\n      * Post Pen Resolution";
			logStr += "\nref : " + collision.reference + "\nhit : " + collision.hitSphere + "\ncol : " + collision;
		}
		#endif
		ResolveVelocity (collision, deltaTime);
		#if false// DEBUG Logger
		{//if (!collision.collideWithWorld && collision.penetration > 0.0f) {
			logStr += "\n      * Pos Vel Resolution";
			logStr += "\nref : " + collision.reference + "\nhit : " + collision.hitSphere + "\ncol : " + collision;
		}
		#endif
	}
	public void SolveConstraints (List<CollisionInfo> cols,float deltaTime) {
		if (cols.Count == 0)
			return;
		int maxIter = 2*cols.Count;
		#if false// DEBUG Logger
		logStr += "---- before while";
		#endif
		while (maxIter > 0) {
			#if false// DEBUG Logger
			logStr += "\n  * iterate " + maxIter;
			#endif
			float sepVel = Mathf.Infinity;
			CollisionInfo toManage = null;
			#if false// DEBUG Logger
			logStr += "\n    * pre manage";
			#endif
			foreach (var col in cols) {
				Vector3 relativeVelocity = col.reference.speed * col.reference.velocity;
				if (!col.collideWithWorld) relativeVelocity -= col.hitSphere.speed * col.hitSphere.velocity;
				col.separationVelocity = Vector3.Dot(relativeVelocity, col.collisionNormal);
				if (col.separationVelocity < sepVel) {
					sepVel = col.separationVelocity;
					toManage = col;
				}
				#if false// DEBUG Logger
				{//(!col.collideWithWorld && col.penetration > 0.0f) {
					logStr += "\n    - Collision";
					logStr += "\nref : " + col.reference + "\nhit : " + col.hitSphere + "\ncol : " + col;
				}
				#endif
			}
			if (sepVel > 0.0f)
				break;
			#if false// DEBUG Logger
			logStr += "\n    * manage";
			#endif
			if (!toManage.collideWithWorld)
				toManage.reference.MatchAwakeState (toManage.hitSphere);
			Resolve (toManage, deltaTime);
			#if false// DEBUG Logger
			logStr += "\n    * post manage";
			#endif
			foreach (var col in cols) {
				if (col.reference == toManage.reference) {
					col.penetration -= Vector3.Dot ( toManage.referenceMovement, col.collisionNormal );
				} else if (!toManage.collideWithWorld && col.reference == toManage.hitSphere) {
					col.penetration -= Vector3.Dot ( toManage.hitMovement, col.collisionNormal );
				}
				if (!col.collideWithWorld) {
					if (col.hitSphere == toManage.reference) {
						col.penetration += Vector3.Dot ( toManage.referenceMovement, col.collisionNormal );
					} else if (!toManage.collideWithWorld && col.hitSphere == toManage.hitSphere) {
						col.penetration += Vector3.Dot ( toManage.hitMovement, col.collisionNormal );
					}
				}
				#if false// DEBUG Logger
				{//(!col.collideWithWorld && col.penetration > 0.0f) {
					logStr += "\n    - Collision";
					logStr += "\nref : " + col.reference + "\nhit : " + col.hitSphere + "\ncol : " + col;
				}
				#endif
			}
			maxIter--;
		}
	}
	bool doPause = false;
	public void Simulate ( float deltaTime, float collisionDampen, List<VoxelGridForce> globalForces ) {
		logStr = "";
		lock (inSimulation)
		{
			#if true
			UpdateForces (globalForces,deltaTime);
			UpdateAccelerations ( deltaTime );
			UpdateVelocities (deltaTime);
			List<CollisionInfo> cols = new List<CollisionInfo> ();
			GlobalCollision (deltaTime, out cols);
			if (cols.Count > 0)
				SolveConstraints (cols,deltaTime);
			UpdatePositions (deltaTime);
			//UpdateAwakeState (deltaTime);
			#if false
			if ( true ) {
				Debug.Log (logStr);
				Debug.Break ();
			}
			#endif
			#else
			float remainingTime = deltaTime;
			while (true) {
				UpdateForces (globalForces,remainingTime);
				UpdateAccelerations ( remainingTime );
				EstimateTrajectory (remainingTime);
				List<CollisionInfo> cols = new List<CollisionInfo> ();
				if (!GlobalCollision (remainingTime, out cols))
					break;
				float dt = cols [0].collisionTime;
				UpdateVelocities (dt);
				if (dt > VoxelGridPhysicSimulator.GlobalDeltaTime ) {
					UpdatePositions (dt);
				}
				SolveConstraints (dt);
				remainingTime -= dt;
			}
			UpdateVelocities (remainingTime);
			UpdatePositions (remainingTime);
			#endif
		}
	}
}
