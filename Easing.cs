using UnityEngine;
using System.Collections;

//Shamelessly lifted from Renaud's code. Should talk to him about this...

public static class Easing
{
	// Adapted from source : http://www.robertpenner.com/easing/
	
	private const float PiOver2 = Mathf.PI / 2;

	public static float Ease(float linearStep, float acceleration, EaseType type)
	{
		float easedStep = acceleration > 0 ? EaseIn(linearStep, type) :
						  acceleration < 0 ? EaseOut(linearStep, type) :
						  linearStep;

		return Mathf.Lerp(linearStep, easedStep, Mathf.Abs(acceleration));
	}

	public static float EaseIn(float linearStep, EaseType type)
	{
		switch (type)
		{
			case EaseType.None: return 1;
			case EaseType.Linear: return linearStep;
			case EaseType.Sine: return Sine.EaseIn(linearStep);
			case EaseType.Quad: return Power.EaseIn(linearStep, 2);
			case EaseType.Cubic: return Power.EaseIn(linearStep, 3);
			case EaseType.Quart: return Power.EaseIn(linearStep, 4);
			case EaseType.Quint: return Power.EaseIn(linearStep, 5);
			case EaseType.Circ: return Circ.EaseIn(linearStep);
		}
		Debug.LogError("Um.");
		return 0;
	}

	public static float EaseOut(float linearStep, EaseType type)
	{
		switch (type)
		{
			case EaseType.None: return 1;
			case EaseType.Linear: return linearStep;
			case EaseType.Sine: return Sine.EaseOut(linearStep);
			case EaseType.Quad: return Power.EaseOut(linearStep, 2);
			case EaseType.Cubic: return Power.EaseOut(linearStep, 3);
			case EaseType.Quart: return Power.EaseOut(linearStep, 4);
			case EaseType.Quint: return Power.EaseOut(linearStep, 5);
			case EaseType.Circ: return Circ.EaseOut(linearStep);
		}
		Debug.LogError("Um.");
		return 0;
	}

	public static float EaseInOut(float linearStep, EaseType easeInType, float acceleration, EaseType easeOutType, float deceleration)
	{
		return linearStep < 0.5
				   ? Mathf.Lerp(linearStep, EaseInOut(linearStep, easeInType), acceleration)
				   : Mathf.Lerp(linearStep, EaseInOut(linearStep, easeOutType), deceleration);
	}
	public static float EaseInOut(float linearStep, EaseType easeInType, EaseType easeOutType)
	{
		return linearStep < 0.5 ? EaseInOut(linearStep, easeInType) : EaseInOut(linearStep, easeOutType);
	}
	public static float EaseInOut(float linearStep, EaseType type)
	{
		switch (type)
		{
			case EaseType.None: return 1;
			case EaseType.Linear: return linearStep;
			case EaseType.Sine: return Sine.EaseInOut(linearStep);
			case EaseType.Quad: return Power.EaseInOut(linearStep, 2);
			case EaseType.Cubic: return Power.EaseInOut(linearStep, 3);
			case EaseType.Quart: return Power.EaseInOut(linearStep, 4);
			case EaseType.Quint: return Power.EaseInOut(linearStep, 5);
			case EaseType.Circ: return Circ.EaseInOut(linearStep);
		}
		Debug.LogError("Um.");
		return 0;
	}

	static class Sine
	{
		public static float EaseIn(float s)
		{
			return Mathf.Sin(s * PiOver2 - PiOver2) + 1;
		}
		public static float EaseOut(float s)
		{
			return Mathf.Sin(s * PiOver2);
		}
		public static float EaseInOut(float s)
		{
			return (Mathf.Sin(s * Mathf.PI - PiOver2) + 1) / 2;
		}
	}

	static class Power
	{
		public static float EaseIn(float s, int power)
		{
			return Mathf.Pow(s, power);
		}
		public static float EaseOut(float s, int power)
		{
			var sign = power % 2 == 0 ? -1 : 1;
			return (sign * (Mathf.Pow(s - 1, power) + sign));
		}
		public static float EaseInOut(float s, int power)
		{
			s *= 2;
			if (s < 1) return EaseIn(s, power) / 2;
			var sign = power % 2 == 0 ? -1 : 1;
			return (sign / 2.0f * (Mathf.Pow(s - 2, power) + sign * 2));
		}
	}

	static class Circ
	{
		public static float EaseIn(float s)
		{
			return -(Mathf.Sqrt(1 - s * s) - 1);
		}
		public static float EaseOut(float s)
		{
			return Mathf.Sqrt(1 - (s - 1) * s);
		}
		public static float EaseInOut(float s)
		{
			s *= 2;
			if (s < 1) return EaseIn(s) / 2;
			return ((Mathf.Sqrt(1 - (s - 2) * s)) + 1) / 2;
		}
	}
}

public enum EaseType
{
	None,
	Linear,
	Sine,
	Quad,
	Cubic,
	Quart,
	Quint,
	Circ
}

