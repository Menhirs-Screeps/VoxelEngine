using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class VoxelGridPhysicSimulator : MonoBehaviour {
	public static float GlobalDeltaTime = 1.0f / 1000.0f;
	private static VoxelGridPhysicSimulator _instance;
	public static VoxelGridPhysicSimulator Instance
	{
		get
		{
			if (!_instance) {
			//	_instance = GameObject.FindObjectOfType<VoxelGridPhysicSimulator> ();
			//} else {
				GameObject go = new GameObject ();
				go.name = "VoxelGridPhysicSimulator";
				_instance = go.AddComponent<VoxelGridPhysicSimulator> ();
			}
			return _instance;
		}
	}
	private Thread _physicsThread;
	private bool _cancelFlag;
	[Range(1.0f,100.0f)]
	public float slowDown = 1.0f;
	[HideInInspector]
	public bool threadRunning = false;
	public float deltaTimeThreshold;
	public void StartThread () {
		if (threadRunning)
			return;
		#if true
		deltaTimeThreshold = Time.fixedDeltaTime / slowDown;
		_physicsThread = new Thread(UpdatePhysics){Name = "PhysicsSimulationThread"};
		_cancelFlag = false;
		_physicsThread.Start();
		#endif
		threadRunning = true;
	}
	public void StopThread () {
		if (!threadRunning)
			return;
		#if true
		_cancelFlag = true;
		_physicsThread.Join ();
		#endif
		threadRunning = false;
	}
	void FixedUpdate () {
		if (!threadRunning)
			return;
		#if false
		Simulate (Time.fixedDeltaTime/slowDown);
		#endif
	}
	[HideInInspector]
	public List<VoxelCollider> colliders;
	//[HideInInspector]
	public List < VoxelRigidBody > bodies = new List < VoxelRigidBody > ();
	List<VoxelGridForce> globalForces = new List<VoxelGridForce> ();
	public float collisionDampen = 0.9f;
	public void AddForce ( VoxelGridForce f ) {
		globalForces.Add (f);
	}
	void Awake () {
		colliders = new List<VoxelCollider> ();
		VoxelCollider[] colls = FindObjectsOfType(typeof(VoxelCollider)) as VoxelCollider[];
		foreach (VoxelCollider collider in colls) {
			colliders.Add (collider);
		}
	}
	/*
	void Update () {
		if (!threadRunning)
			return;
		foreach (var body in bodies) {
			if (body.animated) {
				body.UpdateMesh ();
			}
		}
	}
	*/

	internal bool Raycast ( Vector3 o, Vector3 d, out CollisionInfo hit, float dist, float speed ) {
		bool col = false;
		CollisionInfo collide = new CollisionInfo ();
		RaycastHit ray = new RaycastHit ();
		if (Physics.Raycast (o, d, out ray, dist)) {
			col = true;
			collide.collideWithWorld = true;
			collide.collisionPoint = ray.point;
			collide.collisionNormal = ray.normal;
			collide.collisionDistance = ray.distance;
			collide.collisionTime = ray.distance / speed;
			collide.worldCollisionData = ray;
		}
		hit = collide;
		return col;
	}
	internal List<CollisionInfo> CollidesWithWorld ( Particle particle ) {
		List<CollisionInfo> result = new List<CollisionInfo> ();
		foreach (VoxelCollider collider in colliders) {
			CollisionInfo local = Collider.Collides (new AABB (particle.position, Vector3.one * particle.size / 2.0f), collider.shape);
			if (local != null) {
				local.reference = particle;
				local.collideWithWorld = true;
				local.hitSphere = null;
				result.Add (local);
			}
		}
		return result;
	}
	public int AddBody ( VoxelRigidBody body ) {
		if (bodies == null)
			bodies = new List<VoxelRigidBody> ();
		/*
		VoxelGridRigidBody rb = new VoxelGridRigidBody (body);
		bodies.Add (rb);
		*/
		bodies.Add (body);
		body.physicalBodyID = bodies.Count - 1;
		return bodies.Count - 1;
	}
	public float simulationTime;
	private void Simulate ( float deltaTime ) {
		simulationTime += deltaTime;
		foreach (var body in bodies) {
			if (body.animated) {
				body.Simulate (deltaTime, collisionDampen, globalForces);
			}
		}
    }
    double totalTime = 0.0;
    int calls = 0;
    public double meanTime = 0.0;
    private void UpdatePhysics()
	{
		DateTime initial = DateTime.Now;
		DateTime lastSimulationTime = initial;
		while (_cancelFlag == false)
		{
			DateTime current = DateTime.Now;
			float elapsedMillisecs = (float) ((TimeSpan)(current - lastSimulationTime)).TotalMilliseconds;
			float deltaTime = elapsedMillisecs / 1000.0f;
			if (deltaTime >= deltaTimeThreshold)
            {
                DateTime initial1 = DateTime.Now;
                Simulate (deltaTime);
                DateTime final = DateTime.Now;
                float elapsedMillisecs1 = (float)((TimeSpan)(final - initial1)).TotalMilliseconds;
                totalTime += elapsedMillisecs1;
                calls++;
                meanTime = totalTime / calls;
                lastSimulationTime = current;
			}
		}
	}
}
