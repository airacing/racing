using UnityEngine;
using System.Collections;
using Jurassic;
using Jurassic.Library;

/*
 * Distance sensors based on ray casting from car.
 */

public class CarRaycastSensors : MonoBehaviour, JurassicExecute.Exposable {
	private RaycastHit
		frontLeftLight,
		frontRightLight,
		frontLeft,
		frontRight,
		left,
		right,
		rearLeft,
		rearRight,
		minLeft,
		minRight;
	public GameObject
		rightSide,
		leftSide;
	public Transform
		wheelFR,
		wheelFL;
	public float
		distanceRight,
		distanceLeft,
		distanceFrontLeft,
		distanceFrontRight;
	
	// runs before user script is loaded
	void Awake(){		
		// set functions to be exposed to user
		GetComponent<JurassicExecute> ().AddExposable (this);
	}
	
	void Start () {
		Physics.IgnoreLayerCollision(this.gameObject.layer,rightSide.layer,true);
		Physics.IgnoreLayerCollision(this.gameObject.layer,leftSide.layer,true);
	}
	
	void Update () {
		SetSensors ();
	}
	void SetSensors () {
		countLeft ();
		countRight ();
	}
	RaycastHit minDist(RaycastHit ray1, RaycastHit ray2) {
		if (ray1.distance < ray2.distance)
			return ray1;
		else
			return ray2;
	}
	private void countLeft() {
		Physics.Raycast (this.transform.position, new Vector3 (-transform.right.x, 0, -transform.right.z), out left, Mathf.Infinity);
		Physics.Raycast (this.transform.position, new Vector3 (-transform.right.x + transform.forward.x, 0, -transform.right.z + transform.forward.z), out frontLeft, Mathf.Infinity);
		Physics.Raycast (this.transform.position, new Vector3 (-transform.right.x - transform.forward.x, 0, -transform.right.z - transform.forward.z), out rearLeft, Mathf.Infinity);
		minLeft = minDist (minDist (frontLeft, left), rearLeft);
		Debug.DrawLine (this.transform.position, minLeft.point);
		distanceLeft = minLeft.distance;
		if (Physics.Raycast (wheelFL.transform.position, transform.forward, out frontLeftLight, Mathf.Infinity)) {
			distanceFrontLeft=frontLeftLight.distance;
		}
	}
	private void countRight() {
		Physics.Raycast (this.transform.position, new Vector3 (transform.right.x, 0, transform.right.z), out right, Mathf.Infinity);
		Physics.Raycast(this.transform.position, new Vector3(transform.right.x+transform.forward.x, 0, transform.right.z+transform.forward.z), out frontRight, Mathf.Infinity);
		Physics.Raycast (this.transform.position, new Vector3 (transform.right.x - transform.forward.x, 0, transform.right.z - transform.forward.z), out rearRight, Mathf.Infinity);
		minRight = minDist (minDist (frontRight, right), rearRight);
		Debug.DrawLine (this.transform.position, minRight.point);
		distanceRight = minRight.distance;
		if (Physics.Raycast (wheelFR.transform.position, transform.forward, out frontRightLight, Mathf.Infinity)) {
			distanceFrontRight=frontRightLight.distance;
		}
		
	}
	
	public float distanceFromLeft() {
		return distanceLeft;
	}
	public float distanceFromRight() {
		return distanceRight;
	}
	
	
	#region JURASSIC
	/* 
	 * Expose sensors to Jurassic
	 */
	public void Expose(ScriptEngine engine){
		// The generic System.Func delegate is used to define method signatures with return types;
		engine.SetGlobalFunction("getLeftDistance", new System.Func<double>(jsGetDistanceFromLeft));
		engine.SetGlobalFunction("getRightDistance", new System.Func<double>(jsGetDistanceFromRight));
		engine.SetGlobalFunction("getFrontLeftDistance", new System.Func<double>(jsGetDistanceFrontLeft));
		engine.SetGlobalFunction("getFrontRightDistance", new System.Func<double>(jsGetDistanceFrontRight));
	}
	
	// type double. (float is not a supported type in Jurassic)
	public double jsGetDistanceFromLeft() { return (double)distanceFromLeft(); }
	public double jsGetDistanceFromRight() { return (double)distanceFromRight(); }
	public double jsGetDistanceFrontLeft() { return (double)distanceFrontLeft; }
	public double jsGetDistanceFrontRight() { return (double)distanceFrontRight; }

	public string GetRichTextDescription(){
		return 
			"<b>Road Sensors</b>\n"+
				"getLeftDistance()\n"+
				"getRightDistance()\n"+
				"getNextTurnDistance()\n"+
				"getNextTurnAngle()\n"+
				"getPreviousTurnDistance()\n"+
				"getPreviousTurnAngle()";
	}
	#endregion
}
