using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelGrid {
	[System.Serializable]
	public class Submesh {
		public Vector3 dimensions;
		[HideInInspector]
		public bool[] presence;
		public List < Vector3 > positions;
		public List < Color > neightboors;
		public int material;
		public bool isLitElement(Vector3 pos )
		{
			if ( ( pos.x < 0 ) || ( pos.x >= dimensions.x ) ) return false;
			if ( ( pos.y < 0 ) || ( pos.y >= dimensions.y ) ) return false;
			if ( ( pos.z < 0 ) || ( pos.z >= dimensions.z ) ) return false;
			int id = (int) ( pos.y * (dimensions.x * dimensions.z) + pos.x * dimensions.z + pos.z );
			return presence[id];
		}
	};
}
