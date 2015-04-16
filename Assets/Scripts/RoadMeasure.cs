using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* 
 * Measures distances to left/right/center of road, from global coordinates
 * Could be extended to measure progress along the road, for example.
 * TODO: distance to next turn.
 * TODO?: angle of road segment closest to point.
 */


public class RoadMeasure : MonoBehaviour {

	// should really be set automatically...
	public RoadLines roadLines;
	
	// TODO:-ve if off of left side of road
	public float distanceFromLeft(Vector3 point){
		// get closest center line
		int segment = ClosestSegment (roadLines.center, point - transform.position);
		//Debug.Log (segment);
		return DistanceToLine (roadLines.left [segment], roadLines.left [segment + 1], point - transform.position);
	}
	
	// TODO:-ve if off of right side of road
	public float distanceFromRight(Vector3 point){
		// get closest center line
		int segment = ClosestSegment (roadLines.center, point - transform.position);
		return DistanceToLine (roadLines.right [segment], roadLines.right [segment + 1], point - transform.position);
	}

	// distance to the end of the segment of road
	/*public float straightDistanceRemaining(Vector3 point){
		int segment = ClosestSegment (roadLines.center, point - transform.position);
		return (point - roadLines.center [segment + 1]).magnitude;
	}*/

	// returns infinity if end of track
	public float nextTurnDistance(Vector3 point){
		point = point - transform.position;
		int segment = ClosestSegment (roadLines.center, point);
		if (segment + 2 < roadLines.center.Count)
			return (point - roadLines.center [segment + 1]).magnitude;
		else
			return float.PositiveInfinity;
	}

	// in degrees relative to current road segment
	public float nextTurnAngle(Vector3 point){		
		point = point - transform.position;
		int segment = ClosestSegment (roadLines.center, point);

		if (segment + 2 < roadLines.center.Count)
			return SignedAngle2 ((roadLines.center [segment + 2] - roadLines.center [segment + 1]).ToXZVector2 (),
			                      (roadLines.center [segment + 1] - roadLines.center [segment]).ToXZVector2 ());
		else
			return 0f;
	}

	// returns infinity if end of track
	public float previousTurnDistance(Vector3 point){
		point = point - transform.position;
		int segment = ClosestSegment (roadLines.center, point)-1;
		if (segment >= 0 && segment + 2 < roadLines.center.Count)
			return (point - roadLines.center [segment + 1]).magnitude;
		else
			return float.PositiveInfinity;
	}
	
	// in degrees relative to current road segment
	public float previousTurnAngle(Vector3 point){		
		point = point - transform.position;
		int segment = ClosestSegment (roadLines.center, point)-1;
		
		if (segment >= 0 && segment + 2 < roadLines.center.Count)
			return SignedAngle2 ((roadLines.center [segment + 2] - roadLines.center [segment + 1]).ToXZVector2 (),
			                      (roadLines.center [segment + 1] - roadLines.center [segment]).ToXZVector2 ());
		else
			return 0f;
	}
	
	#region math helpers	
	// returns index i in points where the closest segment is (points[i],points[i+1])
	int ClosestSegment(List<Vector3> points, Vector3 point){		
		float best = float.PositiveInfinity;
		int bestIndex = -1;
		for ( int i=0; i<points.Count-1; i++){			
			float d = DistanceToLine(points[i],points[i+1],point);
			if (d<best){
				best = d;
				bestIndex=i;
			}
		}
		return bestIndex;
	}
	
	/*float DistanceToLines(List<Vector3> points, Vector3 point){
		float best = float.PositiveInfinity;
		for ( int i=0; i<points.Count-1; i++){			
			float d = DistanceToLine(points[i],points[i+1],point);
			if (d<best)
				best = d;
		}
		return best;
	}*/
	
	float DistanceToLine(Vector3 from, Vector3 to, Vector3 point)
	{
		Vector3 direction = (to - from).normalized;
		float length = (to - from).magnitude;
		
		float distFromOrigin = Vector3.Dot (direction, point - from);
		
		if (distFromOrigin < 0) {
			return (point - from).magnitude;
		} else if (distFromOrigin > length) {
			return (point - to).magnitude;
		} else {
			return Vector3.Cross(direction, point - from).magnitude;
		}
	}	

	float SignedAngle2(Vector2 from, Vector2 to){
		int sign = Vector3.Cross(to.ToVector3(), from.ToVector3()).y < 0 ? -1 : 1;
		return sign*Vector2.Angle(from,to);		
	}
	#endregion

}
