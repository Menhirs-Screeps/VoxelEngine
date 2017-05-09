using System;

namespace MagicaVoxel
{
	[Serializable]
	public class ChunckSize : Chunck {
		public int x;
		public int y;
		public int z;
		public override int PopulateFromBuffer ( byte[] buffer, int index )
		{
			int id = base.PopulateFromBuffer ( buffer, index );
			x = BitConverter.ToInt32(contentBuffer, 0);
			y = BitConverter.ToInt32(contentBuffer, 4);
			z = BitConverter.ToInt32(contentBuffer, 8);
			return id;
		}
	};
}