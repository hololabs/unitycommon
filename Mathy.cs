using UnityEngine;
using System;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public static class Mathy {

	public static float Sin(float amplitude, float period, float offset)
	{
		return Mathf.Sin(Time.time * (1f / period) + offset) * amplitude;
	}

	public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
	{
		if (val.CompareTo(max) > 0) return max;
		if (val.CompareTo(min) < 0) return min;
		return val;
	}

	public static float Map(float val, float fromMin, float fromMax, float toMin, float toMax)
	{
		//normalize val to 0..1 range
		val = (val - fromMin) / (fromMax - fromMin);
		//then map to other domain
		return val * (toMax - toMin) + toMin;
	}

	public static float Map01(float val, float fromMin, float fromMax)
	{
		return (val - fromMin) / (fromMax - fromMin);
	}


	public static float CubicInterpolate(float y0, float y1, float y2, float y3, float t)
	{
		float a0, a1, a2, tsq;

		tsq = t*t;

		a0 = y3 - y2 - y0 + y1;
		a1 = y0 - y1 - a0;
		a2 = y2 - y0;

		return(a0*t*tsq+a1*tsq+a2*t+y1);
	}

	public static Vector2 CubicInterpolate(Vector2 y0, Vector2 y1, Vector2 y2, Vector2 y3, float t)
	{
		Vector2 a0, a1, a2;
		float tsq = t*t;

		a0 = y3 - y2 - y0 + y1;
		a1 = y0 - y1 - a0;
		a2 = y2 - y0;

		return(a0*t*tsq+a1*tsq+a2*t+y1);
	}

	public static Vector2 CatmullRom(Vector2 y0, Vector2 y1, Vector2 y2, Vector2 y3, float t)
	{
		Vector2 a0, a1, a2;
		float tsq = t*t;
		
		a0 = -0.5f*y0 + 1.5f*y1 - 1.5f*y2 + 0.5f*y3;
		a1 = y0 - 2.5f*y1 + 2f*y2 - 0.5f*y3;
		a2 = -0.5f*y0 + 0.5f*y2;

		return (a0*t*tsq+a1*tsq+a2*t+y1);
	}

	public static Vector2 CatmullTan(Vector2 y0, Vector2 y1, Vector2 y2, Vector2 y3, float t)
	{
		Vector2 a0, a1, a2;
		float tsq = t*t;
		
		a0 = -0.5f*y0 + 1.5f*y1 - 1.5f*y2 + 0.5f*y3;
		a1 = y0 - 2.5f*y1 + 2f*y2 - 0.5f*y3;
		a2 = -0.5f*y0 + 0.5f*y2;

		return (3*a0*tsq+2*a1*t+a2);
	}

	/*
	public static float DirectionalDeltaAngle(float x, float y) {
		while(x < 0) x += 360f;
		while(y < 0) y += 360f;
		float d = y - x;
		while (d > 360) d -= 360f;
		return d;
	}
	*/

	/*
	public static Vector3 CatmullRom(
							Vector3 y0, Vector3 y1,
							Vector3 y2, Vector3 y3,
							float t
						  )
	{
		Vector3 a0, a1, a2, a3;
		float tsq = t*t;
		
		a0 = -0.5f*y0 + 1.5f*y1 - 1.5f*y2 + 0.5f*y3;
		a1 = y0 - 2.5f*y1 + 2f*y2 - 0.5f*y3;
		a2 = -0.5f*y0 + 0.5f*y2;
		a3 = y1;

		return (a0*t*tsq+a1*tsq+a2*t+a3);
	}
	*/
	
}