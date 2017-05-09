﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Vector3Extensions {
	//
	public static Vector3 Reflect ( this Vector3 vector, Vector3 normal ) {
		return vector - 2 * Vector3.Dot(vector, normal) * normal;
	}
}
