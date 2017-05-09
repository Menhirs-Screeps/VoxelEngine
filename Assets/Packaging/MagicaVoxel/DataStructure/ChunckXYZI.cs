using System;

namespace MagicaVoxel
{
	[Serializable]
	public class ChunckXYZI : Chunck {
		public int numVoxels;
		public VoxelElement[] elements;
		public override int PopulateFromBuffer ( byte[] buffer, int index )
		{
			int id = base.PopulateFromBuffer ( buffer, index );
			numVoxels = BitConverter.ToInt32(contentBuffer, 0);
			elements = new VoxelElement[numVoxels];
			for ( int i = 0 ; i < numVoxels ; i ++ )
			{
				elements[i] = new VoxelElement ();
				elements[i].x = contentBuffer [ 4 + 4*i + 0 ];
				elements[i].y = contentBuffer [ 4 + 4*i + 1 ];
				elements[i].z = contentBuffer [ 4 + 4*i + 2 ];
				elements[i].colorIndex = contentBuffer [ 4 + 4*i + 3 ];
			}
			return id;
		}
	};
}