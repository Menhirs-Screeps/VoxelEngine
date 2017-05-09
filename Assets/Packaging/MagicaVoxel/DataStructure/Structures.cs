using System;

namespace MagicaVoxel
{
	[Serializable]
	public struct VoxelElement {
		public int x;
		public int y;
		public int z;
		public int colorIndex;
	};
	[Serializable]
	public struct ColorRGBA {
		public ColorRGBA ( byte rr, byte gg, byte bb, byte aa ) { r = rr; g = gg; b = bb; a = aa; }
		public byte r;
		public byte g;
		public byte b;
		public byte a;
		public static ColorRGBA white = new ColorRGBA ( 1, 1, 1, 1 );
	};
	[Serializable]
	public struct ModelChunck {
		public ChunckSize size;
		public ChunckXYZI xyzi;
	};
}