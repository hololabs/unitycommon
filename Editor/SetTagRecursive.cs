using UnityEngine;
using System;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

using UnityEditor;

public class SetTagRecursive
{
	[MenuItem ("Utility/Assign Current Tag to Children")]
	public static void SetTag()
	{
		var t = Selection.activeTransform;
		SetTag(t);
	}

	static void SetTag(Transform current)
	{
		string tag = current.tag;
		foreach(Transform t in current) {
			t.tag = tag;
			if(t.childCount > 0)
				SetTag(t);
		}
	}
}
