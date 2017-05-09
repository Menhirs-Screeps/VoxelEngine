using System;

namespace MagicaVoxel
{
	[Serializable]
	public class ChunckMatt : Chunck {
		public int matID;
		public int materialType;
		public float materialWeight;
		public int propertyBits;
		public float[] propertyValues;
	};
}