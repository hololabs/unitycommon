using UnityEngine;
using System.Collections;

public class WorldSpaceLook : MonoBehaviour {

	Quaternion dir;

	void Start () {
		dir = transform.rotation;
	}
	
	void Update () {
		transform.rotation = dir;
	}
}
