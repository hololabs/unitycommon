using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

using UnityEditor;

public class ReverseWinding
{
	[MenuItem ("Utility/Reverse winding order", false, 2)]
	static void ReverseWindingOrder()
	{
		MeshFilter mf = Selection.activeTransform.GetComponent<MeshFilter>();
		Mesh mesh = mf.sharedMesh;

		int[] triangles = mesh.triangles;
		Vector3[] normals = mesh.normals;

		for(int i=0; i<triangles.Length; i+=3)
		{
			int tmp = triangles[i];
			triangles[i] = triangles[i+2];
			triangles[i+2] = tmp;
		}

		for(int i=0; i<normals.Length; i++)
		{
			normals[i] *= -1;
		}

		mesh.triangles = triangles;
		mesh.normals = normals;

	}

}
