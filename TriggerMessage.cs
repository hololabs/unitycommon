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
	public event Action<Collider> TriggerAction;

	void OnTriggerStay(Collider other)
	{
		TriggerAction(other);
	}

}
