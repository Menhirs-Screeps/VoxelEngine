using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MagicaVoxelFile))]
public class MagicaVoxelInspectorGUI : Editor 
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI ();
	}
}

/*
 * Stats sur Astrid
 * 
 * Voxels :
 * 		1621 sommets / triangles envoyés à la carte
 * 		triangles générés par le geometry shader : dépend du calcul de connectivité réalisé mais on obtient les stats ci dessous
 * Mesh non simplifié (pseudo cube par element de grille) :
 * 		surface globale 		: 5424 sommets / 2712 triangles
 * 		surface par matériau 	: 8984 sommets / 4492 triangles
 * 		global					: 38904 sommets / 19452 triangles
 * Mesh simplifié (suppression des sommets duppliqués) :
 * 		surface globale 		: 3281 sommets / 2712 triangles
 * 		surface par matériau 	: 5103 sommets / 4492 triangles
 * 		global					: 9432 sommets / 19452 triangles
 */

#if false
[CustomEditor(typeof(MagicaVoxelFile))]
public class MagicaVoxelInspectorGUI : Editor 
{
	enum PrefabGenerator
	{
		Voxels = 0,
		Meshes = 1,
		Particules = 2
	}
	//
	MagicaVoxelFile inspectedFile;
	string[] modelNames;
	int[] modelIDs;
	//
	int selectedModel;
	bool scaleFromX;
	bool centerMeshOnFloorPlane;
	PrefabGenerator generator;
	//Shader baseShader;
	bool ignoreInterior;
	bool discardOnConnectivity;
	//Shader baseMaterialShader;
	bool decimateOnConnectivity;
	//
	void OnEnable()
	{
		inspectedFile = (MagicaVoxelFile)target;
		modelNames = new string[ inspectedFile.content.main.models.Length + 1 ];
		modelIDs = new int[ inspectedFile.content.main.models.Length + 1 ];
		modelNames [0] = "Full model";
		modelIDs [0] = -1;
		for (int i = 0; i < inspectedFile.content.main.models.Length; i++) {
			modelNames [i + 1] = "Sub-model " + i;
			modelIDs [i + 1] = i;
		}
	}
	public override void OnInspectorGUI()
	{
		EditorGUILayout.LabelField ("File: " + inspectedFile.filename);
		EditorGUILayout.LabelField ("Version: " + inspectedFile.content.header.version);
		EditorGUILayout.Space ();
		selectedModel = EditorGUILayout.IntPopup("Model: ", selectedModel, modelNames, modelIDs);
		if (selectedModel >= 0) {
			MagicaVoxel.ModelChunck model = inspectedFile.content.main.models [selectedModel];
			EditorGUILayout.LabelField ("Size: " + model.size.x+"x"+model.size.y+"x"+model.size.z);
			scaleFromX = EditorGUILayout.Toggle("Scale model using X size", scaleFromX);
			centerMeshOnFloorPlane = EditorGUILayout.Toggle("Center mesh on floor plane", centerMeshOnFloorPlane);
			generator = (PrefabGenerator) EditorGUILayout.EnumPopup("Generate prefab as", generator);
			EditorGUILayout.Space ();
			switch (generator) {
			case PrefabGenerator.Meshes:
				EditorGUILayout.PrefixLabel("Voxel generator:");
				//baseMaterialShader = (Shader) EditorGUILayout.ObjectField(baseMaterialShader, typeof(Shader),false);
				decimateOnConnectivity = EditorGUILayout.Toggle("Connectivity simplification", decimateOnConnectivity);
				break;
			case PrefabGenerator.Voxels:
				EditorGUILayout.PrefixLabel("Mesh generator:");
				//baseShader = (Shader) EditorGUILayout.ObjectField(baseShader, typeof(Shader),false);
				ignoreInterior = EditorGUILayout.Toggle("Ignore interior elements", ignoreInterior);
				discardOnConnectivity = EditorGUILayout.Toggle("Discard faces on connectivity", discardOnConnectivity);
				break;
			case PrefabGenerator.Particules:
				EditorGUILayout.PrefixLabel("Particle generator:");
				break;
			}
			EditorGUILayout.Space ();
			if (GUILayout.Button ("Generate prefab")) {
				switch (generator) {
				case PrefabGenerator.Meshes:
					inspectedFile.SaveToVoxelMeshPrefab (selectedModel, scaleFromX, centerMeshOnFloorPlane);
					//GenerateMeshAssetForModel ( Path.GetFileNameWithoutExtension ( inspectedFile.filename ), selectedModel, model, inspectedFile.content.main.palette, inspectedFile.content.main.materials );
					break;
				case PrefabGenerator.Voxels:
					inspectedFile.SaveToVoxelGridPrefab (selectedModel, scaleFromX, centerMeshOnFloorPlane );
					//VoxelGridGenerator.GenerateAsset ( inspectedFile, selectedModel, scaleFromX, centerMeshOnFloorPlane );
					break;
				case PrefabGenerator.Particules:
					GenerateParticuleAssetForModel ( Path.GetFileNameWithoutExtension ( inspectedFile.filename ), selectedModel, model, inspectedFile.content.main.palette, inspectedFile.content.main.materials );
					break;
				}
			}
		}
	}
	bool isLitElement(MagicaVoxel.ChunckSize sz, int x, int y, int z, bool[] presence )
	{
		if ( ( x < 0 ) || ( x >= sz.x ) ) return false;
		if ( ( y < 0 ) || ( y >= sz.y ) ) return false;
		if ( ( z < 0 ) || ( z >= sz.z ) ) return false;
		int id = ( x * sz.y + y ) * sz.z + z;
		return presence[id];
	}
	bool isInteriorElement(MagicaVoxel.ChunckSize sz, int x, int y, int z, bool[] presence )
	{
		if ( ( x == 0 ) || ( x == sz.x-1 ) ) return false;
		if ( ( y == 0 ) || ( y == sz.y-1 ) ) return false;
		if ( ( z == 0 ) || ( z == sz.z-1 ) ) return false;
		//if ( ! isLitElement ( sz, x, y, z, presence ) ) return false;
		if ( ! isLitElement ( sz, x-1, y, z, presence ) ) return false;
		if ( ! isLitElement ( sz, x+1, y, z, presence ) ) return false;
		if ( ! isLitElement ( sz, x, y-1, z, presence ) ) return false;
		if ( ! isLitElement ( sz, x, y+1, z, presence ) ) return false;
		if ( ! isLitElement ( sz, x, y, z-1, presence ) ) return false;
		if ( ! isLitElement ( sz, x, y, z+1, presence ) ) return false;
		return true;
	}
	#region Voxel prefab generator
	#endregion
	#region Mesh prefab generator
	void GenerateCube ( MagicaVoxel.VoxelElement element, MagicaVoxel.ChunckSize size, bool[] presence,
		ref List<Vector3> points, ref List<int> indicies)
	{
		float globalScale = 1.0f;
		if ( scaleFromX ) globalScale /= size.x;
		float xx = ( (float) element.x )+0.5f;
		float yy = ( (float) element.y )+0.5f;
		float zz = ( (float) element.z )+0.5f;
		if ( scaleFromX )
		{
			xx *= globalScale;
			yy *= globalScale;
			zz *= globalScale;
		}
		if ( centerMeshOnFloorPlane )
		{
			xx -= ( (float) size.x ) * globalScale / 2.0f;
			yy -= ( (float) size.y ) * globalScale / 2.0f;
		}
		float halfWidth		= 0.5f*globalScale;
		float halfHeight	= 0.5f*globalScale;
		float halfDepth		= 0.5f*globalScale;
		Vector3[] boxv = {
			/* 00 */
			new Vector3 (xx	-halfWidth, yy	+halfHeight, zz	-halfDepth),
			new Vector3 (xx	+halfWidth, yy	+halfHeight, zz	-halfDepth),
			new Vector3 (xx	-halfWidth, yy	-halfHeight, zz	-halfDepth),
			new Vector3 (xx	+halfWidth, yy	-halfHeight, zz	-halfDepth),
			/* 04 */
			new Vector3 (xx	+halfWidth, yy	+halfHeight, zz	-halfDepth),
			new Vector3 (xx	+halfWidth, yy	+halfHeight, zz	+halfDepth),
			new Vector3 (xx	+halfWidth, yy	-halfHeight, zz	-halfDepth),
			new Vector3 (xx	+halfWidth, yy	-halfHeight, zz	+halfDepth),
			/* 08 */
			new Vector3 (xx	+halfWidth, yy	+halfHeight, zz	+halfDepth),
			new Vector3 (xx	-halfWidth, yy	+halfHeight, zz	+halfDepth),
			new Vector3 (xx	+halfWidth, yy	-halfHeight, zz	+halfDepth),
			new Vector3 (xx	-halfWidth, yy	-halfHeight, zz	+halfDepth),
			/* 12 */
			new Vector3 (xx	-halfWidth, yy	+halfHeight, zz	+halfDepth),
			new Vector3 (xx	-halfWidth, yy	+halfHeight, zz	-halfDepth),
			new Vector3 (xx	-halfWidth, yy	-halfHeight, zz	+halfDepth),
			new Vector3 (xx	-halfWidth, yy	-halfHeight, zz	-halfDepth),
			/* 16 */
			new Vector3 (xx	-halfWidth, yy	+halfHeight, zz	+halfDepth),
			new Vector3 (xx	+halfWidth, yy	+halfHeight, zz	+halfDepth),
			new Vector3 (xx	-halfWidth, yy	+halfHeight, zz	-halfDepth),
			new Vector3 (xx	+halfWidth, yy	+halfHeight, zz	-halfDepth),
			/* 20 */
			new Vector3 (xx	-halfWidth, yy	-halfHeight, zz	-halfDepth),
			new Vector3 (xx	+halfWidth, yy	-halfHeight, zz	-halfDepth),
			new Vector3 (xx	-halfWidth, yy	-halfHeight, zz	+halfDepth),
			new Vector3 (xx	+halfWidth, yy	-halfHeight, zz	+halfDepth)
		};
		if ( ! isLitElement ( size, element.x-1, element.y, element.z, presence ) )
		{
			int id = points.Count;
			points.Add ( boxv[12] );
			points.Add ( boxv[13] );
			points.Add ( boxv[14] );
			points.Add ( boxv[15] );
			indicies.Add ( id + 2 ); indicies.Add ( id + 1 ); indicies.Add ( id + 0 );
			indicies.Add ( id + 1 ); indicies.Add ( id + 2 ); indicies.Add ( id + 3 );
		}
		if ( ! isLitElement ( size, element.x+1, element.y, element.z, presence ) )
		{
			int id = points.Count;
			points.Add ( boxv[04] );
			points.Add ( boxv[05] );
			points.Add ( boxv[06] );
			points.Add ( boxv[07] );
			indicies.Add ( id + 2 ); indicies.Add ( id + 1 ); indicies.Add ( id + 0 );
			indicies.Add ( id + 1 ); indicies.Add ( id + 2 ); indicies.Add ( id + 3 );
		}
		if ( ! isLitElement ( size, element.x, element.y-1, element.z, presence ) )
		{
			int id = points.Count;
			points.Add ( boxv[20] );
			points.Add ( boxv[21] );
			points.Add ( boxv[22] );
			points.Add ( boxv[23] );
			indicies.Add ( id + 2 ); indicies.Add ( id + 1 ); indicies.Add ( id + 0 );
			indicies.Add ( id + 1 ); indicies.Add ( id + 2 ); indicies.Add ( id + 3 );
		}
		if ( ! isLitElement ( size, element.x, element.y+1, element.z, presence ) )
		{
			int id = points.Count;
			points.Add ( boxv[16] );
			points.Add ( boxv[17] );
			points.Add ( boxv[18] );
			points.Add ( boxv[19] );
			indicies.Add ( id + 2 ); indicies.Add ( id + 1 ); indicies.Add ( id + 0 );
			indicies.Add ( id + 1 ); indicies.Add ( id + 2 ); indicies.Add ( id + 3 );
		}
		if ( ! isLitElement ( size, element.x, element.y, element.z-1, presence ) )
		{
			int id = points.Count;
			points.Add ( boxv[00] );
			points.Add ( boxv[01] );
			points.Add ( boxv[02] );
			points.Add ( boxv[03] );
			indicies.Add ( id + 2 ); indicies.Add ( id + 1 ); indicies.Add ( id + 0 );
			indicies.Add ( id + 1 ); indicies.Add ( id + 2 ); indicies.Add ( id + 3 );
		}
		if ( ! isLitElement ( size, element.x, element.y, element.z+1, presence ) )
		{
			int id = points.Count;
			points.Add ( boxv[08] );
			points.Add ( boxv[09] );
			points.Add ( boxv[10] );
			points.Add ( boxv[11] );
			indicies.Add ( id + 2 ); indicies.Add ( id + 1 ); indicies.Add ( id + 0 );
			indicies.Add ( id + 1 ); indicies.Add ( id + 2 ); indicies.Add ( id + 3 );
		}
	}
	Mesh GenerateMesh(MagicaVoxel.ModelChunck model, MagicaVoxel.ChunckRGBA palette, ref List<Material> mats )
	{
		MagicaVoxel.ChunckSize size = model.size;
		MagicaVoxel.ChunckXYZI xyzi = model.xyzi;
		Mesh mesh = new Mesh();
		List<Vector3> points = new List<Vector3> ();
		List<Color> colors = new List<Color> ();

		bool[] presence = new bool [ size.x * size.y * size.z ];
		Dictionary<int,bool[]> presPerMat = new Dictionary<int, bool[]> ();
		Dictionary < int, List < MagicaVoxel.VoxelElement > > perMaterial = new Dictionary < int, List < MagicaVoxel.VoxelElement > > ();
		for ( int j = 0 ; j < xyzi.numVoxels ; j ++ )
		{
			MagicaVoxel.VoxelElement element = xyzi.elements[j];
			int ide = ( element.x * size.y + element.y ) * size.z + element.z;
			presence[ide] = true;
			if (!perMaterial.ContainsKey (element.colorIndex)) {
				perMaterial.Add (element.colorIndex, new List < MagicaVoxel.VoxelElement > ());
				presPerMat.Add (element.colorIndex, new bool [ size.x * size.y * size.z ]);
			}
			presPerMat [element.colorIndex] [ide] = true;
			perMaterial[element.colorIndex].Add ( element );
		}
		List<List<int>> triangles = new List<List<int>> ();
		foreach (var item in perMaterial)
		{
			List<int> mat_indicies = new List<int> ();
			int cindex = item.Key;
			foreach ( var element in item.Value )
			{
				int count = points.Count;
				GenerateCube ( element, size, presence, ref points, ref mat_indicies );
				for (int i = count; i < points.Count; i++)
					colors.Add (new Color (1, 1, 1, 1));
			}
			if (mats != null) {
				Material material = MaterialGenerator.GenerateMeshDiffuseMaterial (inspectedFile, cindex, Shader.Find ("Standard"));
				mats.Add (material);
			}
			triangles.Add (mat_indicies);
		}
		for ( int i = 0 ; i < points.Count ; i ++ )
		{
			Vector3 pt = points[i];
			float tmp = pt.y;
			pt.y = points[i].z;
			pt.z = tmp;
			points[i] = pt;
		}

		if ( decimateOnConnectivity )
		{
			Dictionary < int, int > mapper = new Dictionary < int, int > ();
			Dictionary < Vector3, int > verts = new Dictionary < Vector3, int > ();
			List < Vector3 > nvpoints = new List < Vector3 > ();
			for ( int i = 0 ; i < points.Count ; i ++ )
			{
				Vector3 pt = points[i];
				if ( ! verts.ContainsKey ( pt ) ) {
					nvpoints.Add ( pt );
					verts.Add ( pt, nvpoints.Count-1);
				}
				mapper[i] = verts[pt];
			}
			for ( int i = 0 ; i < triangles.Count ; i ++ ) {
				for ( int j = 0 ; j < triangles[i].Count ; j ++ ) {
					triangles[i][j] = mapper [ triangles[i][j] ];
				}
			}
			points = nvpoints;
		}

		mesh.vertices = points.ToArray ();
		mesh.colors = colors.ToArray ();
		mesh.subMeshCount = triangles.Count;
		for ( int i = 0 ; i < triangles.Count ; i ++ ) {
			mesh.SetIndices(triangles[i].ToArray (), MeshTopology.Triangles, i);
		}
		mesh.RecalculateNormals();
		mesh.RecalculateBounds ();
		return mesh;
	}
	void GenerateMeshAssetForModel ( string assetPathAndName, int id, MagicaVoxel.ModelChunck model, MagicaVoxel.ChunckRGBA palette, List < MagicaVoxel.ChunckMatt > materials )
	{
		string prefabFilename = "Assets/Resources/Prefabs/" + assetPathAndName + "-model"+id+".mesh.prefab";
		UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab(prefabFilename);
		List<Material> mats = new List<Material> ();
		Mesh mesh = GenerateMesh (model, palette, ref mats);
		for ( int i = 0 ; i < mats.Count ; i ++ ) {
			AssetDatabase.AddObjectToAsset(mats[i], prefabFilename );
		}
		mesh.name = "Mesh " + Path.GetFileNameWithoutExtension ( prefabFilename );
		AssetDatabase.AddObjectToAsset(mesh, prefabFilename );

		GameObject gameObject = new GameObject();
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		meshFilter.mesh = mesh;
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.materials = mats.ToArray ();
		EditorUtility.SetDirty(gameObject);
		GameObject prefabGameObject = PrefabUtility.ReplacePrefab(gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
		GameObject.DestroyImmediate(gameObject);
	}
	#endregion
	#region Particle prefab generator
	Mesh GenerateUnitCubeMesh ( float scale )
	{
		Mesh mesh = new Mesh();
		#region Vertices
		Vector3 p0 = new Vector3( -scale * .5f,	-scale * .5f, scale * .5f );
		Vector3 p1 = new Vector3( scale * .5f, 	-scale * .5f, scale * .5f );
		Vector3 p2 = new Vector3( scale * .5f, 	-scale * .5f, -scale * .5f );
		Vector3 p3 = new Vector3( -scale * .5f,	-scale * .5f, -scale * .5f );	

		Vector3 p4 = new Vector3( -scale * .5f,	scale * .5f,  scale * .5f );
		Vector3 p5 = new Vector3( scale * .5f, 	scale * .5f,  scale * .5f );
		Vector3 p6 = new Vector3( scale * .5f, 	scale * .5f,  -scale * .5f );
		Vector3 p7 = new Vector3( -scale * .5f,	scale * .5f,  -scale * .5f );

		Vector3[] vertices = new Vector3[]
		{
			// Bottom
			p0, p1, p2, p3,

			// Left
			p7, p4, p0, p3,

			// Front
			p4, p5, p1, p0,

			// Back
			p6, p7, p3, p2,

			// Right
			p5, p6, p2, p1,

			// Top
			p7, p6, p5, p4
		};
		#endregion

		#region Normales
		Vector3 up 	= Vector3.up;
		Vector3 down 	= Vector3.down;
		Vector3 front 	= Vector3.forward;
		Vector3 back 	= Vector3.back;
		Vector3 left 	= Vector3.left;
		Vector3 right 	= Vector3.right;

		Vector3[] normales = new Vector3[]
		{
			// Bottom
			down, down, down, down,

			// Left
			left, left, left, left,

			// Front
			front, front, front, front,

			// Back
			back, back, back, back,

			// Right
			right, right, right, right,

			// Top
			up, up, up, up
		};
		#endregion	

		#region UVs
		Vector2 _00 = new Vector2( 0f, 0f );
		Vector2 _10 = new Vector2( 1f, 0f );
		Vector2 _01 = new Vector2( 0f, 1f );
		Vector2 _11 = new Vector2( 1f, 1f );

		Vector2[] uvs = new Vector2[]
		{
			// Bottom
			_11, _01, _00, _10,

			// Left
			_11, _01, _00, _10,

			// Front
			_11, _01, _00, _10,

			// Back
			_11, _01, _00, _10,

			// Right
			_11, _01, _00, _10,

			// Top
			_11, _01, _00, _10,
		};
		#endregion

		#region Triangles
		int[] triangles = new int[]
		{
			// Bottom
			3, 1, 0,
			3, 2, 1,			

			// Left
			3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
			3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,

			// Front
			3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
			3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,

			// Back
			3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
			3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,

			// Right
			3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
			3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,

			// Top
			3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
			3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,

		};
		#endregion

		mesh.vertices = vertices;
		mesh.normals = normales;
		mesh.uv = uvs;
		mesh.triangles = (triangles);
		return mesh;
	}
	void GenerateParticuleAssetForModel ( string assetPathAndName, int id, MagicaVoxel.ModelChunck model, MagicaVoxel.ChunckRGBA palette, List < MagicaVoxel.ChunckMatt > materials )
	{
		string prefabFilename = "Assets/Resources/Prefabs/" + assetPathAndName + "-model"+id+".particle.prefab";
		MagicaVoxelModel subasset = ScriptableObject.CreateInstance<MagicaVoxelModel> ();
		subasset.model = model;
		subasset.palette = palette;
		subasset.materials = materials;
		string subfname = AssetDatabase.GenerateUniqueAssetPath ("Assets/Resources/Voxels/" + assetPathAndName + "-model"+id+".asset");

		MagicaVoxel.ChunckSize size = model.size;
		MagicaVoxel.ChunckXYZI xyzi = model.xyzi;
		UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab(prefabFilename);
		GameObject gameObject = new GameObject();
		ParticleSystem particule = gameObject.AddComponent < ParticleSystem > ();
		ParticleSystemRenderer prenderer = gameObject.GetComponent<ParticleSystemRenderer>();

		float globalScale = 1.0f;
		if ( scaleFromX ) globalScale /= size.x;

		var main = particule.main;
		main.startSize = globalScale;
		main.startSpeed = 0;
		main.maxParticles = xyzi.numVoxels;
		main.loop = false;
		main.playOnAwake = false;

		var shape = particule.shape;
		shape.shapeType = ParticleSystemShapeType.Box;
		shape.box = new Vector3 ( size.x, size.z, size.y ) * globalScale;
		shape.enabled = false;

		var emission = particule.emission;
		emission.enabled = false;

		Material material =  MaterialGenerator.GenerateParticleDiffuseMaterial ( inspectedFile, 0, Shader.Find("Vertex Color/Standard") );
		AssetDatabase.AddObjectToAsset(material, prefabFilename );


		prenderer.renderMode = ParticleSystemRenderMode.Mesh;
		//GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		Mesh mesh = GenerateUnitCubeMesh (1);// go.GetComponent < MeshFilter > ().mesh;
		Mesh[] meshes = { mesh };
		AssetDatabase.AddObjectToAsset(mesh, prefabFilename );
		prenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
		prenderer.receiveShadows = true;
		prenderer.material = material;
		prenderer.SetMeshes ( meshes );

		VoxelsParticuleSystemManager mgr = gameObject.AddComponent<VoxelsParticuleSystemManager> ();
		mgr.initialParticules = "Voxels/" + assetPathAndName + "-model"+id;
		EditorUtility.SetDirty(gameObject);
		GameObject prefabGameObject = PrefabUtility.ReplacePrefab(gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
		GameObject.DestroyImmediate(gameObject);

		AssetDatabase.CreateAsset (subasset, subfname);
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh();
	}
	#endregion
}
#endif