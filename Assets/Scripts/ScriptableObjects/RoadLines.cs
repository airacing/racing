using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class RoadLines : ScriptableObject {
		
	// should really have getters
	[SerializeField]
	public List<Vector3> left = new List<Vector3>();

	[SerializeField]
	public List<Vector3> right = new List<Vector3>();

	[SerializeField]
	public List<Vector3> center = new List<Vector3>();
	
}
