using UnityEngine;
using System.Collections;

public class Assert
{
	
	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	public static void Test(bool comparison, string message)
	{
		if(!comparison)
		{
			Debug.LogWarning(message);
			Debug.Break();
		}
	}

	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	public static void Test(bool comparison)
	{
		string message = "Assertion Error!!";
		if(!comparison)
		{
			Debug.LogWarning(message);
			Debug.Break();
		}
	}

	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	public static void Throw(string message)
	{
		Debug.LogWarning(message);
	}
}