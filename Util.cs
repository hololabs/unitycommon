using UnityEngine;
using System;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public static class Util
{
	public static int EnumCount<T>()
	{
		return Enum.GetValues(typeof(T)).Length;
	}
}
