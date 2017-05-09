using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct VecMatPair
{
	public Vector3 point;
}

[RequireComponent(typeof(MeshFilter))]
public class ComputeBufferTest : MonoBehaviour {
	public Mesh theMesh;
	public Vector3[] theVertices;
	public ComputeShader shader;
	public ComputeBuffer input;
	public ComputeBuffer output;
	void RunShader()
	{
		input = new ComputeBuffer(theVertices.Length, 12);
		output = new ComputeBuffer(theVertices.Length, 12);
		VecMatPair[] inputdata = new VecMatPair[theVertices.Length];
		VecMatPair[] outputdata = new VecMatPair[theVertices.Length];
		Vector3[] theVerticesOut = new Vector3[theVertices.Length];
		for (int i = 0; i < theVertices.Length; i++) {
			inputdata [i].point = new Vector3(theVertices [i].x,theVertices[i].y,theVertices[i].z);
		}
		input.SetData (inputdata);
		output.SetData (outputdata);
		int kernel = shader.FindKernel("Multiply");
		shader.SetBuffer(kernel, "inputBuffer", input);
		shader.SetBuffer(kernel, "outputBuffer", output);
		shader.Dispatch(kernel, theVertices.Length, 1,1);
		output.GetData(outputdata);
		for (int i = 0; i < theVertices.Length; i++) {
			theVerticesOut [i] = outputdata [i].point;
		}
		theMesh.vertices = theVerticesOut;
	}
	void Start () {
		theMesh = GetComponent<MeshFilter> ().mesh;
		theVertices = theMesh.vertices;
		RunShader ();
	}
	void Update () {
	}
}
