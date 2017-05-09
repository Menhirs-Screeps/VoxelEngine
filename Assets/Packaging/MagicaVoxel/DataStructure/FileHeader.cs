using System;

namespace MagicaVoxel
{
	[Serializable]
	public class FileHeader {
		public string id;
		public int version;
		public int PopulateFromBuffer ( byte[] buffer, int index )
		{
			id = System.Text.Encoding.UTF8.GetString(buffer, index, 4);
			version = BitConverter.ToInt32(buffer, index+4);
			return index+8;
		}
	};
}
