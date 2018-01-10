using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prosics;
public class DrawCube: MonoScriptBase  
{

	protected override void Start()
	{
		base.Start ();
		Draw ();
	}
	void Draw()
	{
		Vector3[] vertices = {
			new Vector3 (-0.5f, 0.5f, 0.5f),
			new Vector3 (0.5f, 0.5f, 0.5f),
			new Vector3 (-0.5f, 0.5f, -0.5f),
			new Vector3 (0.5f, 0.5f, -0.5f),
			new Vector3 (-0.5f, -0.5f, 0.5f),
			new Vector3 (0.5f, -0.5f, 0.5f),
			new Vector3 (-0.5f, -0.5f, -0.5f),
			new Vector3 (0.5f, -0.5f, -0.5f)
		};
		int[] newTriangles = {
			0,1,2,0,2,4,0,4,1,
			3,2,1,3,1,7,3,7,2,
			6,2,7,6,7,4,6,4,2,
			5,7,1,5,1,4,5,4,7
		};
		/*int[] newTriangles = {
			0,2,1,0,1,4,0,4,2,
			3,1,2,3,2,7,3,7,1,
			6,2,4,6,4,7,6,7,2,
			5,1,7,5,7,4,5,4,1
		};*/
		Vector3[] normals = {
			new Vector3(-1f,1f,-1f),
			new Vector3(1f,1f,-1f),
			new Vector3(-1f,1f,1f),
			new Vector3(1f,1f,1f),
			new Vector3(-1f,-1f,-1f),
			new Vector3(1f,-1f,-1f),
			new Vector3(-1f,-1f,1f),
			new Vector3(1f,-1f,1f),
		};
		Vector2[] newUV = { 
			new Vector2(0, 0), 
			new Vector2(0, 0), 
			new Vector2(0, 0), 
			new Vector2(0, 0), 
			new Vector2(0, 0), 
			new Vector2(0, 0), 
			new Vector2(0, 0), 
			new Vector2(1, 1), 
			}
			;

		Mesh mesh = new Mesh();


		mesh.vertices = vertices;
		//mesh.uv = newUV;
		mesh.normals = normals;
		mesh.triangles = newTriangles;
		mesh.name = "testPlane";

		GetComponent<MeshFilter>().mesh = mesh;
	}
}
