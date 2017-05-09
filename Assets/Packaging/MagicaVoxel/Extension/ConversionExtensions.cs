using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class ConversionExtensions
{
	public static Color ToColor ( this MagicaVoxel.ColorRGBA me ) {
		Color res = new Color (me.r / 255.0f, me.g / 255.0f, me.b / 255.0f, me.a / 255.0f);
		return res;
	}
}