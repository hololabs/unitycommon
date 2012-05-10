using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

//Adapted from Renaud's handy waiters

//TODO: option to use fixed update?

public class Waiter : MonoBehaviour {

	internal struct Task
	{
		readonly Func<float, bool> condition;
		readonly Action<float> onTick;

		internal float timeWaited;
	}

	Queue<Task> taskQueue = new Queue<Task>();
	Task currentTask;

	//TODO: do first, then check condition
	void Update()
	{
		if(currentTask.condition(currentTask.timeWaited))
		{
			if(taskQueue.Count > 0)
				currentTask = taskQueue.Dequeue();
			else
				Destroy(this);
		}
		else
		{
			currentTask.timeWaited += Time.deltaTime;
			currentTask.onTick(currentTask.timeWaited);
		}

	}

	public Waiter Then(Action action)
	{
		Task t = new Task()
		{
			condition = _ => return true,
			onTick = _ => action()
		};
		
		taskQueue.Enqueue(t);

		return this;
	}

	public Waiter ThenDoUntil(Func<float, bool> cond, Action<float> action)
	{
		Task t = new Task()
		{
			condition = cond,
			onTick = action
		};
		
		taskQueue.Enqueue(t);

		return this;
	}

	public Waiter ThenWait(float seconds)
	{
		Task t = new Task()
		{
			condition = cond,
			onTick = () => { }
		};
		
		taskQueue.Enqueue(t);

		return this;
	}

	public Waiter ThenInterpolate(float durationSeconds, Action<float> action)
	{
		Task t = new Task()
		{
			condition = waited => waited > durationSeconds;
			onTick = waited => action(waited / durationSeconds);
		}

		taskQueue.Enqueue(t);

		return this;
	}
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

	public static Waiter Wait(float secondsToWait, GameObject attachTo = null)
	{
		Waiter w = (attachTo ?? GlobalWaiter).AddComponent<Waiter>();
		return w.ThenWait(secondsToWait);
	}

	public static Waiter DoUntil(Func<float, bool> condition, Action<float> action, GameObject attachTo = null)
	{
		Waiter w = (attachTo ?? GlobalWaiter).AddComponent<Waiter>();
		return w.ThenDoUntil(condition, action);
	}

	//interpolatorAction parameter will always go from 0 to 1 (time waited / interpolation duration)
	public static Waiter Interpolate(float durationSeconds, Action<float> interpolatorAction, GameObject attachTo = null)
	{
		Waiter w = (attachTo ?? GlobalWaiter).AddComponent<Waiter>();
		return w.ThenInterpolate(durationSeconds, interpolatorAction);
	}

}