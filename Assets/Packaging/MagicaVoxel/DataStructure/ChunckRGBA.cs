using System;

namespace MagicaVoxel
{
	[Serializable]
	public class ChunckRGBA : Chunck {
		public ColorRGBA[] colors;
		public override int PopulateFromBuffer ( byte[] buffer, int index )
		{
			int id = base.PopulateFromBuffer ( buffer, index );
			colors = new ColorRGBA [ 256 ];
			for ( int i = 0 ; i < 256 ; i ++ )
			{
				colors[i] = new ColorRGBA ();
				colors[i].r = contentBuffer [ 4 * i + 0 ];
				colors[i].g = contentBuffer [ 4 * i + 1 ];
				colors[i].b = contentBuffer [ 4 * i + 2 ];
				colors[i].a = contentBuffer [ 4 * i + 3 ];
			}
			return id;
		}
		public static ChunckRGBA GetDefault () {
			ChunckRGBA palette = new ChunckRGBA ();
			palette.colors = new ColorRGBA [ 255 ];
			return palette;
		}
	};
}