using System;

namespace MagicaVoxel
{
	[Serializable]
	public class File {
		public FileHeader header;
		public ChunckMain main;
		public int PopulateFromBuffer ( byte[] buffer, int index )
		{
			header = new FileHeader ();
			main = new ChunckMain ();
			int id = index;
			id = header.PopulateFromBuffer ( buffer, id );
			id = main.PopulateFromBuffer ( buffer, id );
			return index;
		}
	}
}
