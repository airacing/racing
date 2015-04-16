/* 
 * Represents an instance of a race that was finished.
 * Could be serialized to store in a file. (or JSON-ed)
 */
using System;
using System.Collections.Generic;
using UnityEngine;

 
public class Score{
	public string username;
	public string userScript;
	public float raceTime;
	public DateTime timestamp;

	public List<Vector3> path = new List<Vector3>();
	public static int PATH_MAX_COUNT = 30000; // dont let it get too big

	public override string ToString(){
		return username + " " + raceTime.ToString ();
	}
}