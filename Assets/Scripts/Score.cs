/* 
 * Represents an instance of a race that was finished.
 * Could be serialized to store in a file. (or JSON-ed)
 */
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
 
public class Score{
	public string username;
	public string userScript;
	public float raceTime;
	public DateTime timestamp;

	public List<Vector3> path = new List<Vector3>();
	public static int PATH_MAX_COUNT = 30000; // dont let it get too big
	public const string FMT = "O"; // Used for parsing datetime

	public override string ToString(){
		string result = "";
		result += username + "[s-separator]";
		result += userScript + "[s-separator]";
		result += raceTime + "[s-separator]";
		result += timestamp.ToString(FMT) + "[s-separator]";
		result += path.Count + "[s-separator]";
		foreach(Vector3 vec in path) {
			result += vec.x + " " + vec.y + " " + vec.z + " ";
		}
		return result;
	}

	public static Score LoadFromString(string str) {
		Score res = new Score ();
		string[] strArray = str.Split (new string[] {"[s-separator]"}, StringSplitOptions.None);
		res.username = strArray [0];
		res.userScript = strArray [1];
		res.raceTime = float.Parse (strArray [2], CultureInfo.InvariantCulture.NumberFormat);
		res.timestamp = DateTime.ParseExact (strArray [3], FMT, CultureInfo.InvariantCulture);
		int pathCount = int.Parse (strArray [4], CultureInfo.InvariantCulture);
		string[] floatStrArray = (strArray [5]).Split (new string[] {" "}, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < pathCount; i++) {
			Vector3 vec = new Vector3();
			vec.x = float.Parse(floatStrArray[i*3], CultureInfo.InvariantCulture.NumberFormat);
			vec.y = float.Parse(floatStrArray[i*3+1], CultureInfo.InvariantCulture.NumberFormat);
			vec.z = float.Parse(floatStrArray[i*3+2], CultureInfo.InvariantCulture.NumberFormat);
			res.path.Add(vec);
		}
		return res;
	}
}