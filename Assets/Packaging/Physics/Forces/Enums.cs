using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ForceShapeType {
	Sphere,
	TruncatedSphere,
	Cone,
	Box,
	Cylinder
}

[System.Serializable]
public enum ForceType {
	Ponctual,
	Directional,
	ShapeNormal,
	ParticleDirection,
	Bounce
}

[System.Serializable]
public enum ForceDirection {
	Repulsive,
	Attractive
}

[System.Serializable]
public enum ForceTime {
	Impulse,
	ImpulseOnEnter,
	Pulse,
	Constant
}
