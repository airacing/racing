using UnityEngine;
using System.Collections;

public class MinimapCamera : MonoBehaviour {

	// Update is called once per frame
	public Transform Target;
	void Update () {
		transform.position = new Vector3 (Target.position.x, transform.position.y, Target.position.z);
	}
}
