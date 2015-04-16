using UnityEngine;
using System.Collections;
using System;

public static class FloatExtensions  {
	public static string To2dpString(this float s){
		return (Math.Truncate(s * 100) / 100).ToString();
	}
}
