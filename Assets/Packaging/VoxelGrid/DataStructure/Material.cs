using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelGrid {
	[System.Serializable]
	public class Material {
		public enum Type {
			Diffuse,
			Metal,
			Glass,
			Emission
		};
		public int id;
		public Color color;
		public Type type;
		public float weight;
		public float plastic;
		public float roughness;
		public float specular;
		public float indiceOfRefraction;
		public float attenuation;
		public float power;
		public float glow;
		public float isTotalPower;
	};
}
