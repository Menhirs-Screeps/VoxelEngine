using System;

namespace MagicaVoxel
{
	[Serializable]
	public class ChunckPack : Chunck {
		public int numberOfModels;
		public override int PopulateFromBuffer ( byte[] buffer, int index )
		{
			int id = base.PopulateFromBuffer ( buffer, index );
			numberOfModels = BitConverter.ToInt32(contentBuffer, 0);
			return id;
		}
	};
}