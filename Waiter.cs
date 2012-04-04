using UnityEngine;
using System.Collections;

//A simplified/less versatile/less generic Unity implementation of Renaud's handy waiters

public class Waiter : MonoBehaviour {

	internal System.Func<float, bool> condition;
	internal System.Action<float> whileWaiting;
	internal System.Action whenDone;
	
	private float timeWaited;
	
	
	//internal bool useFixedUpdate;
	//Hm. There might be a smarter way to do this so we don't implement/call both Update and FixedUpdate?

	void Update () {
		//if (useFixedUpdate) return;
		
		if (condition(timeWaited))
		{
			whenDone();
			Destroy(this);
		}
		else
		{
			timeWaited += Time.deltaTime;
			whileWaiting(timeWaited);	
		}
	}
	
	/*
	void FixedUpdate () {
		if (!useFixedUpdate) return;
		
		if (condition(timeWaited))
		{
			whenDone();
			Destroy(this);
		}
		else
			timeWaited += Time.deltaTime;
			whileWaiting(timeWaited);	
			
	}
	*/
}

public static class Waiters
{

	private static GameObject globalWaiter;
	public static GameObject GlobalWaiter
	{
		get
		{
			if (!globalWaiter) globalWaiter = new GameObject("globalWaiter");
			return globalWaiter;
		}
	}

	public static Waiter Wait(float secondsToWait, System.Action whenDone)
	{
		Waiter w = GlobalWaiter.AddComponent<Waiter>();
		
		w.condition = waited => waited > secondsToWait;
		w.whileWaiting = waited => { };
		w.whenDone = whenDone;

		return w;
	}

	public static Waiter Wait(GameObject attachTo, float secondsToWait, System.Action whenDone)
	{
		Waiter w = attachTo.AddComponent<Waiter>();
		
		w.condition = waited => waited > secondsToWait;
		w.whileWaiting = waited => { };
		w.whenDone = whenDone;
		
		return w;
	}

	//interpolatorAction parameter will always go from 0 to 1 (time waited / interpolation duration)
	public static Waiter Interpolate(GameObject attachTo, float durationSeconds, System.Action<float> interpolatorAction)
	{
		return Interpolate(attachTo, durationSeconds, interpolatorAction, () => {} );
	}
	public static Waiter Interpolate(float durationSeconds, System.Action<float> interpolatorAction)
	{
		return Interpolate(durationSeconds, interpolatorAction, () => {} );
	}        
	public static Waiter Interpolate(GameObject attachTo, float durationSeconds, System.Action<float> interpolatorAction, System.Action whenDone)
	{
		Waiter w = attachTo.AddComponent<Waiter>();
		
		w.condition = waited => waited > durationSeconds;
		w.whileWaiting = waited => interpolatorAction(waited / durationSeconds);
		w.whenDone = whenDone;

		return w;
	}
	public static Waiter Interpolate(float durationSeconds, System.Action<float> interpolatorAction, System.Action whenDone)
	{
		Waiter w = GlobalWaiter.AddComponent<Waiter>();
		
		w.condition = waited => waited > durationSeconds;
		w.whileWaiting = waited => interpolatorAction(waited / durationSeconds);
		w.whenDone = whenDone;

		return w;
	}
	
}