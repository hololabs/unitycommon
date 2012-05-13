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

	Queue<Task> taskQueue = new Queue<Task>();

	void Update()
	{
		Task task = taskQueue.Peek();
		task.timeWaited += Time.deltaTime;
		task.onTick(task.timeWaited);

		if(task.condition(task.timeWaited))
		{
			if(taskQueue.Count > 1)
			{
				var t = taskQueue.Dequeue();
				Waiters.taskCache.Return(t);
			}
			else
				Destroy(this);
		}

	}

	public Waiter Then(Action action)
	{
		Task t = Waiters.taskCache.Take();
		t.timeWaited = 0;
		t.condition = _ => { return true; };
		t.onTick = _ => action();
		
		taskQueue.Enqueue(t);

		return this;
	}

	public Waiter ThenDoUntil(Func<float, bool> cond, Action<float> action)
	{
		Task t = Waiters.taskCache.Take();
		t.timeWaited = 0;
		t.condition = cond;
		t.onTick = action;
		
		taskQueue.Enqueue(t);

		return this;
	}

	public Waiter ThenWait(float durationSeconds)
	{
		Task t = Waiters.taskCache.Take();
		t.timeWaited = 0;
		t.condition = waited => waited > durationSeconds;
		t.onTick = _ => { };
		
		taskQueue.Enqueue(t);

		return this;
	}

	public Waiter ThenInterpolate(float durationSeconds, Action<float> action)
	{
		Task t = Waiters.taskCache.Take();
		t.timeWaited = 0;
		t.condition = waited => waited > durationSeconds;
		t.onTick = waited => action(waited / durationSeconds);

		taskQueue.Enqueue(t);

		return this;
	}

}

public static class Waiters
{
	internal static readonly Pool<Task> taskCache = new Pool<Task>();

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

internal class Task
{
	internal Func<float, bool> condition;
	internal Action<float> onTick;

	internal float timeWaited;
}