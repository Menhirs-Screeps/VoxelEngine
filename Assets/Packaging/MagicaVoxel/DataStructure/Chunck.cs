using System;

namespace MagicaVoxel
{
	[Serializable]
	public class Chunck {
		public string id;
		protected int numberOfBytesInContent;
		protected int numberOfBytesInChildrenChunck;
		protected byte[] contentBuffer;
		protected byte[] childrenBuffer;
		public virtual int PopulateFromBuffer ( byte[] buffer, int index )
		{
			id = System.Text.Encoding.UTF8.GetString(buffer, index, 4);
			numberOfBytesInContent = BitConverter.ToInt32(buffer, index+4);
			numberOfBytesInChildrenChunck = BitConverter.ToInt32(buffer, index+8);
			contentBuffer = new byte[numberOfBytesInContent];
			Array.Copy ( buffer, index + 12, contentBuffer, 0, numberOfBytesInContent );
			childrenBuffer = new byte[numberOfBytesInChildrenChunck];
			Array.Copy ( buffer, index + 12 + numberOfBytesInContent, childrenBuffer, 0, numberOfBytesInChildrenChunck );
			return index + 12 + numberOfBytesInContent + numberOfBytesInChildrenChunck;
		}
		public static string NextChunckId ( byte[] buffer, int index )
		{
			string id = System.Text.Encoding.UTF8.GetString(buffer, index, 4);
			return id;
		}
		public static Chunck ReadChunck ( byte[] buffer, ref int index )
		{
			string id = System.Text.Encoding.UTF8.GetString(buffer, index, 4);
			Chunck res = null;
			switch ( id )
			{
			case "MAIN":
				res = new ChunckMain ();
				break;
			case "PACK":
				res = new ChunckPack ();
				break;
			case "SIZE":
				res = new ChunckSize ();
				break;
			case "XYZI":
				res = new ChunckXYZI ();
				break;
			case "RGBA":
				res = new ChunckRGBA ();
				break;
			case "MATT":
				res = new ChunckMatt ();
				break;
			}
			index = res.PopulateFromBuffer ( buffer, index );
			return res;
		}
	};
}