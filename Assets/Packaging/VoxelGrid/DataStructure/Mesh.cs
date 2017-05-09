using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelGrid {
	public class Mesh : ScriptableObject {
		public Vector3 dimensions;
		public List <Submesh > elements;
		[HideInInspector]
		public bool[] presence;
		[HideInInspector]
		public List <int> materialIndices;
		[HideInInspector]
		public List <VoxelGrid.Material> materials;
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
