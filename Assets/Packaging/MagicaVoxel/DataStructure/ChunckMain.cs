using System;
using System.Collections.Generic;

namespace MagicaVoxel
{
	[Serializable]
	public class ChunckMain : Chunck {
		public ChunckPack pack;
		public ModelChunck[] models;
		public ChunckRGBA palette;
		public List < ChunckMatt > materials = new List < ChunckMatt > ();
		public override int PopulateFromBuffer ( byte[] buffer, int index )
		{
			int id = base.PopulateFromBuffer ( buffer, index );
			int childrenIndex = 0;
			if ( Chunck.NextChunckId ( childrenBuffer, childrenIndex ) == "PACK" )
			{
				pack = (ChunckPack) Chunck.ReadChunck ( childrenBuffer, ref childrenIndex );
				models = new ModelChunck[pack.numberOfModels];
			}
			else models = new ModelChunck[1];
			for ( int i = 0 ; i < models.Length ; i ++ )
			{
				ModelChunck model = new ModelChunck ();
				model.size = (ChunckSize) Chunck.ReadChunck ( childrenBuffer, ref childrenIndex );
				model.xyzi = (ChunckXYZI) Chunck.ReadChunck ( childrenBuffer, ref childrenIndex );
				models[i] = model;
			}
			if ( childrenIndex < numberOfBytesInChildrenChunck ) {
				if ( Chunck.NextChunckId ( childrenBuffer, childrenIndex ) == "RGBA" )
				{
					palette = (ChunckRGBA) Chunck.ReadChunck ( childrenBuffer, ref childrenIndex );
				}
				else palette = ChunckRGBA.GetDefault ();
				while ( childrenIndex < numberOfBytesInChildrenChunck ) {
					ChunckMatt mat = (ChunckMatt) Chunck.ReadChunck ( childrenBuffer, ref childrenIndex );
					materials.Add ( mat );
				}
			}
			return id;
		}
	};
}