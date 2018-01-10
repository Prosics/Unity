using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prosics;

public class DrawPlane : MonoScriptBase 
{
	protected override void Start()
	{
		base.Start ();
		Draw ();
	}
	void Draw()
	{
		Vector3[] newVertices = { 
			new Vector3(0, 0, 0), 
			new Vector3(0, 1, 0), 
			new Vector3(1, 1, 0), 
			new Vector3(1, 0, 0)};
		Vector2[] newUV = { 
			new Vector2(0, 0), 
			new Vector2(0, 1), 
			new Vector2(1, 1), 
			new Vector2(1, 0)};
		int[] newTriangles = {0,2,1,0,3,2};

		Mesh mesh = new Mesh();


		mesh.vertices = newVertices;
		mesh.uv = newUV;
		mesh.triangles = newTriangles;
		mesh.name = "testPlane";

		GetComponent<MeshFilter>().mesh = mesh;
	}
}
