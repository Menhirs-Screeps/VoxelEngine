using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constraint
{
}
public class ParticleWorldContact : Constraint
{
}
public class ParticleParticleContact : Constraint
{
}

[System.Serializable]
public class CollisionInfo
{
	public Particle reference;
	public bool collideWithWorld;
	public Particle hitSphere;
	public float collisionTime;
	public Vector3 collisionPoint;
	public float collisionDistance;
	public Vector3 collisionNormal;
	public float penetration;
	public Vector3 referenceMovement;
	public Vector3 hitMovement;
	public float separationVelocity;

	public RaycastHit worldCollisionData;
	public float hitableDistance;
	public float hitableTime;
	// collision data
	public Vector3 relativeSpeed;
	public override string ToString () {
		return "Collision : " +
			" i=" + reference.ID +
			(collideWithWorld ? " j=world" : " j=" + hitSphere.ID ) +
			" p=" + collisionPoint.ToString("F5") +
			" n=" + collisionNormal.ToString("F5") +
			" d=" + collisionDistance +
			" t=" + collisionTime +
			" pen=" + penetration +
			" sep=" + separationVelocity;
	}
}

[System.Serializable]
public class HalfCollisionInfo
{
	public int hit;
	public float collisionTime;
	public Vector3 collisionPoint;
	public float collisionDistance;
	public Vector3 collisionNormal;
	public Vector3 relativeSpeed;
	public Vector3 relativeAcceleration;
	public float closingVelocity;
	public float totalMass;
	public float totalInverseMass;
	public override string ToString () {
		return "Collision : " +
		" p=" + collisionPoint.ToString ("F5") +
		" n=" + collisionNormal.ToString ("F5") +
		" d=" + collisionDistance.ToString ("F5") +
		" t=" + collisionTime.ToString ("F5") +
		" s=" + relativeSpeed.ToString ("F5") +
		" ms=" + relativeSpeed.magnitude.ToString ("F5");
	}
}
