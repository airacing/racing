using UnityEngine;
using System.Collections;

public class Vector2Extensions {	
	// 90 means right, -90 means left
	public static float SignedAngle2(Vector2 to, Vector2 from){
		int sign = Vector3.Cross(to.ToVector3(), from.ToVector3()).y < 0 ? -1 : 1;
		return sign*Vector2.Angle(from,to);		
	}
}
