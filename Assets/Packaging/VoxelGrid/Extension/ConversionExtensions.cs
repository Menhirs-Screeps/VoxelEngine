using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VoxelGeneratorStrategy {
	CompleteGrid,
	GlobalSurface,
	PerMaterialSurface
}

public static partial class ConversionExtensions {
	//
	public static VoxelGrid.Material ToVoxelMaterial ( this MagicaVoxelFile inspectedFile, int selectedMaterial ) {
		VoxelGrid.Material res = new VoxelGrid.Material ();
		res.id = selectedMaterial;
		res.color = inspectedFile.content.main.palette.colors [selectedMaterial-1].ToColor ();
		return res;
	}
	public static VoxelGrid.Mesh ToVoxelGrid ( this MagicaVoxelFile inspectedFile, int selectedModel )
	{
		MagicaVoxel.ModelChunck model = inspectedFile.content.main.models [selectedModel];
		Vector3 dims = new Vector3 (model.size.x, model.size.z, model.size.y);
		VoxelGrid.Mesh mesh = new VoxelGrid.Mesh ();
		mesh.dimensions = dims;
		mesh.presence = new bool [ (int) ( dims.x * dims.z * dims.y ) ];
		mesh.elements = new List<VoxelGrid.Submesh> ();
		mesh.materialIndices = new List<int> ();
		mesh.materials = new List<VoxelGrid.Material> ();
		for (int i = 0; i < model.xyzi.numVoxels; i++) {
			MagicaVoxel.VoxelElement element = model.xyzi.elements [i];
			Vector3 pos = new Vector3 (element.x, element.z, element.y);
			if (!mesh.materialIndices.Contains (element.colorIndex)) {
				VoxelGrid.Submesh sub = new VoxelGrid.Submesh ();
				sub.dimensions = dims;
				sub.presence = new bool [ (int)(dims.x * dims.z * dims.y) ];
				sub.positions = new List < Vector3 > ();
				sub.neightboors = new List < Color > ();
				sub.material = element.colorIndex;
				mesh.elements.Add (sub);
				mesh.materialIndices.Add (element.colorIndex);
				mesh.materials.Add (inspectedFile.ToVoxelMaterial (element.colorIndex));
			}
			VoxelGrid.Submesh submesh = mesh.elements [mesh.materialIndices.FindIndex (v => v == element.colorIndex)];
			int id = (int) ( pos.y * (dims.x * dims.z) + pos.x * dims.z + pos.z );
			mesh.presence [id] = true;
			submesh.presence [id] = true;
			submesh.positions.Add (pos);
		}
		foreach ( var submesh in mesh.elements ) {
			foreach (var pos in submesh.positions) {
				Color neightboors = new Color (0, 0, 0, 0);

				neightboors.r += !submesh.isLitElement (pos + Vector3.forward)	? 0.32f : 0.0f;
				neightboors.r += !submesh.isLitElement (pos + Vector3.back)		? 0.16f : 0.0f;
				neightboors.r += !submesh.isLitElement (pos + Vector3.right)	? 0.08f : 0.0f;
				neightboors.r += !submesh.isLitElement (pos + Vector3.left)		? 0.04f : 0.0f;
				neightboors.r += !submesh.isLitElement (pos + Vector3.up)		? 0.02f : 0.0f;
				neightboors.r += !submesh.isLitElement (pos + Vector3.down)		? 0.01f : 0.0f;

				neightboors.g += !mesh.isLitElement (pos + Vector3.forward)		? 0.32f : 0.0f;
				neightboors.g += !mesh.isLitElement (pos + Vector3.back)		? 0.16f : 0.0f;
				neightboors.g += !mesh.isLitElement (pos + Vector3.right)		? 0.08f : 0.0f;
				neightboors.g += !mesh.isLitElement (pos + Vector3.left)		? 0.04f : 0.0f;
				neightboors.g += !mesh.isLitElement (pos + Vector3.up)			? 0.02f : 0.0f;
				neightboors.g += !mesh.isLitElement (pos + Vector3.down)		? 0.01f : 0.0f;

				neightboors.b = 0.01f;

				submesh.neightboors.Add (neightboors);
			}
		}
		return mesh;
	}
	//
	public static Material ToMaterial(this VoxelGrid.Material initial, Shader shader )
	{
		Material material = new Material ( shader );
		material.hideFlags = HideFlags.HideInInspector;
		if (initial.type == VoxelGrid.Material.Type.Diffuse) {
			material.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
			material.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
			material.SetInt ("_ZWrite", 1);
			material.DisableKeyword ("_ALPHATEST_ON");
			material.DisableKeyword ("_ALPHABLEND_ON");
			material.DisableKeyword ("_ALPHAPREMULTIPLY_ON");
			material.DisableKeyword ("_NORMALMAP");
			material.DisableKeyword ("_METALLICGLOSSMAP");
			material.DisableKeyword ("_PARALLAXMAP");
			material.DisableKeyword ("_DETAIL_MULX2");
			material.DisableKeyword ("_EMISSION");
			material.DisableKeyword ("_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A");
			material.EnableKeyword ("_SPECULARHIGHLIGHTS_OFF");
			material.EnableKeyword ("_GLOSSYREFLECTIONS_OFF ");
			material.SetFloat ("_SpecularHighlights", 0.0f);
			material.SetFloat ("_GlossyReflections", 0.0f);
			material.SetFloat ("_Mode", 0.0f);
		}
		material.color = initial.color;
		return material;
	}
	public static Mesh ToMesh(this VoxelGrid.Mesh initial, float scale, Vector3 center, VoxelGeneratorStrategy strategy )
	{
		Debug.Log (center + " " + scale);
		List<Vector3> points = new List<Vector3> ();
		List<Color> colors = new List<Color> ();
		List<Vector3> normals = new List<Vector3> ();
		List<List<int>> indices = new List<List<int>> ();
		Mesh mesh = new Mesh ();
		for ( int i = 0 ; i < initial.elements.Count ; i ++ ) {
			int matId = initial.materialIndices[i];
			VoxelGrid.Submesh submesh = initial.elements[i];
			List<int> index = new List<int> ();
			for (int j = 0; j < submesh.positions.Count; j++) {
				Vector3 pos = submesh.positions [j];
				Vector3 neightboors = new Vector3 ( submesh.neightboors [j].r, submesh.neightboors [j].g, submesh.neightboors [j].b );
				pos = (pos + (new Vector3 (0.5f, 0.5f, 0.5f)) - center) * scale;
				if ((strategy == VoxelGeneratorStrategy.CompleteGrid)
				    || ((strategy == VoxelGeneratorStrategy.GlobalSurface) && (neightboors.y > 0.0f))
				    || ((strategy == VoxelGeneratorStrategy.PerMaterialSurface) && (neightboors.x > 0.0f))) {
					points.Add (pos);
					normals.Add (neightboors);
					colors.Add (initial.materials [initial.materialIndices.FindIndex (v => v == submesh.material)].color);
					index.Add (points.Count - 1);
				}
			}
			indices.Add (index);
		}
		mesh.vertices = points.ToArray ();
		mesh.colors = colors.ToArray ();
		mesh.normals = normals.ToArray ();
		mesh.subMeshCount = indices.Count;
		for (int i = 0; i < indices.Count; i++) {
			mesh.SetIndices (indices [i].ToArray (), MeshTopology.Points, i);
		}
		return mesh;
	}
}
