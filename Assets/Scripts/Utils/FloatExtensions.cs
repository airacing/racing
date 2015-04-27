using UnityEngine;
using System.Collections;
using System;

public static class FloatExtensions  {
	public static string To2dpString(this float s){
		double r = Math.Truncate (s * 100);
		return (r / 100).ToString() + (((int)r)%100==0?".0":"") +(((int)r)%10==0?"0":"");
	}
}
