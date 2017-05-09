using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollisionShape
{
	public Vector3 center;
	public Vector3 GetAxis (int i) {
		if (i == 0)
			return Vector3.right;
		else if (i == 1)
			return Vector3.up;
		else if (i == 2)
			return Vector3.forward;
		else
			return center;
	}
}

[System.Serializable]
public class Sphere : CollisionShape
{
	public float radius;
	public Sphere () { center = Vector3.zero; radius = 0.5f; }
	public Sphere (Vector3 c, float r) { center = c; radius = r; }
}

[System.Serializable]
public class AABB : CollisionShape
{
	public Vector3 halfSize;
	public AABB () { center = Vector3.zero; halfSize = 0.5f * Vector3.one; }
	public AABB (Vector3 c, Vector3 h) { center = c; halfSize = h; }
	public static AABB operator* ( AABB box, float scale ) {
		return new AABB (box.center,scale*box.halfSize);
	}
	public static AABB operator+ ( AABB box, Vector3 d ) {
		return new AABB (box.center+d,box.halfSize);
	}
}

public static class Collider {
	public static CollisionInfo Collides ( Sphere sph1, Sphere sph2 ) {
		// Cache the sphere positions
		Vector3 positionOne = sph1.center;
		Vector3 positionTwo = sph2.center;

		// Find the vector between the objects
		Vector3 midline = positionOne - positionTwo;
		float sz = midline.magnitude;

		// See if it is large enough.
		if (sz < 0.0f)
			return null;
		if (sz > (sph1.radius+sph2.radius))
			return null;
		// We manually create the normal, because we have the
		// size to hand.
		Vector3 normal = midline.normalized;

		CollisionInfo local = new CollisionInfo ();
		local.collisionNormal = normal;
		local.collisionPoint = positionOne + midline * 0.5f;
		local.collisionDistance = sz;
		local.penetration = (sph1.radius+sph2.radius) - sz;
		local.collideWithWorld = false;
		return local;
	}
	static float transformToAxis(AABB box,Vector3 axis )
	{
		return
			box.halfSize.x * Mathf.Abs(Vector3.Dot(axis, box.GetAxis(0))) +
			box.halfSize.y * Mathf.Abs(Vector3.Dot(axis, box.GetAxis(1))) +
			box.halfSize.z * Mathf.Abs(Vector3.Dot(axis, box.GetAxis(2)));
	}
	static float penetrationOnAxis(AABB one, AABB two,Vector3 axis,Vector3 toCentre)
	{
		float oneProject = transformToAxis(one, axis);
		float twoProject = transformToAxis(two, axis);
		float distance = Mathf.Abs(Vector3.Dot(toCentre, axis));
		return oneProject + twoProject - distance;
	}
	static bool tryAxis(AABB one,AABB two,Vector3 axis,Vector3 toCentre,int index,ref float smallestPenetration,ref int smallestCase)
	{
		if (axis.sqrMagnitude < 0.0001) return true;
		axis.Normalize ();

		float penetration = penetrationOnAxis(one, two, axis, toCentre);

		if (penetration < 0) return false;
		if (penetration < smallestPenetration) {
			smallestPenetration = penetration;
			smallestCase = index;
		}
		return true;
	}

	static CollisionInfo fillPointFaceBoxBox(AABB one,AABB two,Vector3 toCentre,int best,float pen)
	{
		// This method is called when we know that a vertex from
		// box two is in contact with box one.

		CollisionInfo contact = new CollisionInfo ();

		// We know which axis the collision is on (i.e. best),
		// but we need to work out which of the two faces on
		// this axis.
		Vector3 normal = one.GetAxis(best);
		if (Vector3.Dot(one.GetAxis(best), toCentre) > 0)
		{
			normal = normal * -1.0f;
		}

		// Work out which vertex of box two we're colliding with.
		// Using toCentre doesn't work!
		Vector3 vertex = two.halfSize;
		if (Vector3.Dot(two.GetAxis(0), normal) < 0) vertex.x = -vertex.x;
		if (Vector3.Dot(two.GetAxis(1), normal) < 0) vertex.y = -vertex.y;
		if (Vector3.Dot(two.GetAxis(2), normal) < 0) vertex.z = -vertex.z;

		// Create the contact data
		contact.collisionNormal = normal;
		contact.penetration = pen;
		contact.collisionPoint = two.center + vertex;/* to modify */ // two.getTransform() * vertex;
		return contact;
	}
	static Vector3 contactPoint(
		Vector3 pOne,
		Vector3 dOne,
		float oneSize,
		Vector3 pTwo,
		Vector3 dTwo,
		float twoSize,

		// If this is true, and the contact point is outside
		// the edge (in the case of an edge-face contact) then
		// we use one's midpoint, otherwise we use two's.
		bool useOne)
	{
		Vector3 toSt, cOne, cTwo;
		float dpStaOne, dpStaTwo, dpOneTwo, smOne, smTwo;
		float denom, mua, mub;

		smOne = dOne.sqrMagnitude;
		smTwo = dTwo.sqrMagnitude;
		dpOneTwo = Vector3.Dot(dTwo, dOne);

		toSt = pOne - pTwo;
		dpStaOne = Vector3.Dot(dOne, toSt);
		dpStaTwo = Vector3.Dot(dTwo, toSt);

		denom = smOne * smTwo - dpOneTwo * dpOneTwo;

		// Zero denominator indicates parrallel lines
		if (Mathf.Abs(denom) < 0.0001f) {
			return useOne?pOne:pTwo;
		}

		mua = (dpOneTwo * dpStaTwo - smTwo * dpStaOne) / denom;
		mub = (smOne * dpStaTwo - dpOneTwo * dpStaOne) / denom;

		// If either of the edges has the nearest point out
		// of bounds, then the edges aren't crossed, we have
		// an edge-face contact. Our point is on the edge, which
		// we know from the useOne parameter.
		if (mua > oneSize ||
			mua < -oneSize ||
			mub > twoSize ||
			mub < -twoSize)
		{
			return useOne?pOne:pTwo;
		}
		else
		{
			cOne = pOne + dOne * mua;
			cTwo = pTwo + dTwo * mub;

			return cOne * 0.5f + cTwo * 0.5f;
		}
	}
	public static CollisionInfo Collides ( AABB one, AABB two ) {
		// Find the vector between the two centres
		Vector3 toCentre = two.GetAxis(3) - one.GetAxis(3);

		// We start assuming there is no contact
		float pen = Mathf.Infinity;
		int best = 0xffffff;

		// Now we check each axes, returning if it gives us
		// a separating axis, and keeping track of the axis with
		// the smallest penetration otherwise.
		if (!tryAxis(one, two, one.GetAxis(0), toCentre, 0, ref pen, ref best)) return null;
		if (!tryAxis(one, two, one.GetAxis(1), toCentre, 1, ref pen, ref best)) return null;
		if (!tryAxis(one, two, one.GetAxis(2), toCentre, 2, ref pen, ref best)) return null;

		if (!tryAxis(one, two, two.GetAxis(0), toCentre, 3, ref pen, ref best)) return null;
		if (!tryAxis(one, two, two.GetAxis(1), toCentre, 4, ref pen, ref best)) return null;
		if (!tryAxis(one, two, two.GetAxis(2), toCentre, 5, ref pen, ref best)) return null;

		// Store the best axis-major, in case we run into almost
		// parallel edge collisions later
		int bestSingleAxis = best;

		if (!tryAxis(one, two, Vector3.Cross ( one.GetAxis(0), two.GetAxis(0) ), toCentre, 6, ref pen, ref best)) return null;
		if (!tryAxis(one, two, Vector3.Cross ( one.GetAxis(0), two.GetAxis(1) ), toCentre, 7, ref pen, ref best)) return null;
		if (!tryAxis(one, two, Vector3.Cross ( one.GetAxis(0), two.GetAxis(2) ), toCentre, 8, ref pen, ref best)) return null;
		if (!tryAxis(one, two, Vector3.Cross ( one.GetAxis(1), two.GetAxis(0) ), toCentre, 9, ref pen, ref best)) return null;
		if (!tryAxis(one, two, Vector3.Cross ( one.GetAxis(1), two.GetAxis(1) ), toCentre, 10, ref pen, ref best)) return null;
		if (!tryAxis(one, two, Vector3.Cross ( one.GetAxis(1), two.GetAxis(2) ), toCentre, 11, ref pen, ref best)) return null;
		if (!tryAxis(one, two, Vector3.Cross ( one.GetAxis(2), two.GetAxis(0) ), toCentre, 12, ref pen, ref best)) return null;
		if (!tryAxis(one, two, Vector3.Cross ( one.GetAxis(2), two.GetAxis(1) ), toCentre, 13, ref pen, ref best)) return null;
		if (!tryAxis(one, two, Vector3.Cross ( one.GetAxis(2), two.GetAxis(2) ), toCentre, 14, ref pen, ref best)) return null;

		// We now know there's a collision, and we know which
		// of the axes gave the smallest penetration. We now
		// can deal with it in different ways depending on
		// the case.
		if (best < 3)
		{
			// We've got a vertex of box two on a face of box one.
			CollisionInfo local;
			local = fillPointFaceBoxBox(one, two, toCentre, best, pen);
			return local;
		}
		else if (best < 6)
		{
			// We've got a vertex of box one on a face of box two.
			// We use the same algorithm as above, but swap around
			// one and two (and therefore also the vector between their
			// centres).
			CollisionInfo local;
			local = fillPointFaceBoxBox(two, one, toCentre*-1.0f, best-3, pen);
			return local;
		}
		else
		{
			// We've got an edge-edge contact. Find out which axes
			best -= 6;
			int oneAxisIndex = best / 3;
			int twoAxisIndex = best % 3;
			Vector3 oneAxis = one.GetAxis(oneAxisIndex);
			Vector3 twoAxis = two.GetAxis(twoAxisIndex);
			Vector3 axis = Vector3.Cross (oneAxis, twoAxis);
			axis.Normalize ();

			// The axis should point from box one to box two.
			if (Vector3.Dot(axis, toCentre) > 0) axis = axis * -1.0f;

			// We have the axes, but not the edges: each axis has 4 edges parallel
			// to it, we need to find which of the 4 for each object. We do
			// that by finding the point in the centre of the edge. We know
			// its component in the direction of the box's collision axis is zero
			// (its a mid-point) and we determine which of the extremes in each
			// of the other axes is closest.
			Vector3 ptOnOneEdge = one.halfSize;
			Vector3 ptOnTwoEdge = two.halfSize;
			for (int i = 0; i < 3; i++)
			{
				if (i == oneAxisIndex) ptOnOneEdge[i] = 0;
				else if (Vector3.Dot(one.GetAxis(i), axis) > 0) ptOnOneEdge[i] = -ptOnOneEdge[i];

				if (i == twoAxisIndex) ptOnTwoEdge[i] = 0;
				else if (Vector3.Dot(two.GetAxis(i), axis) < 0) ptOnTwoEdge[i] = -ptOnTwoEdge[i];
			}

			// Move them into world coordinates (they are already oriented
			// correctly, since they have been derived from the axes).
			ptOnOneEdge = one.center + ptOnOneEdge; /* to modify */ // one.transform * ptOnOneEdge;
			ptOnTwoEdge = two.center + ptOnTwoEdge; /* to modify */ // two.transform * ptOnTwoEdge;

			// So we have a point and a direction for the colliding edges.
			// We need to find out point of closest approach of the two
			// line-segments.
			Vector3 vertex = contactPoint(
				ptOnOneEdge, oneAxis, one.halfSize[oneAxisIndex],
				ptOnTwoEdge, twoAxis, two.halfSize[twoAxisIndex],
				bestSingleAxis > 2
			);

			// We can fill the contact.
			CollisionInfo contact = new CollisionInfo ();
			contact.penetration = pen;
			contact.collisionNormal = axis;
			contact.collisionPoint = vertex;
			return contact;
		}
		return null;
	}
	/*
	public static bool Collides ( AABB box, Sphere sph ) {
		float X = Mathf.Max ( box.min.x, Mathf.Min ( sph.center.x, box.max.x ) );
		float Y = Mathf.Max ( box.min.y, Mathf.Min ( sph.center.y, box.max.y ) );
		float Z = Mathf.Max ( box.min.z, Mathf.Min ( sph.center.z, box.max.z ) );
		var distance = Mathf.Sqrt(X*X+Y*Y+Z*Z);
		return distance < sph.radius;
	}
	public static bool Collides ( Sphere sph, AABB box ) {
		float X = Mathf.Max ( box.min.x, Mathf.Min ( sph.center.x, box.max.x ) );
		float Y = Mathf.Max ( box.min.y, Mathf.Min ( sph.center.y, box.max.y ) );
		float Z = Mathf.Max ( box.min.z, Mathf.Min ( sph.center.z, box.max.z ) );
		var distance = Mathf.Sqrt(X*X+Y*Y+Z*Z);
		return distance < sph.radius;
	}
	*/
}
