using System;
using System.Collections.Generic;
using UnityEngine;

public class MagicaVoxelFile : ScriptableObject {
	public MagicaVoxel.File content;
	public List<string> flags;
	public string filename;
}
