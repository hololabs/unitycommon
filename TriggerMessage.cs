using UnityEngine;
using System;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class TriggerMessage : MonoBehaviour
{
	public event Action<Collider> TriggerEnterAction;
	public event Action<Collider> TriggerStayAction;
	public event Action<Collider> TriggerExitAction;

	void OnTriggerEnter(Collider other)
	{
		if(TriggerEnterAction != null)
			TriggerEnterAction(other);
	}

	void OnTriggerStay(Collider other)
	{
		if(TriggerStayAction != null)
			TriggerStayAction(other);
	}

	void OnTriggerExit(Collider other)
	{
		if(TriggerExitAction != null)
			TriggerExitAction(other);
	}

}
