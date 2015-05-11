using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Jurassic;
using Jurassic.Library;

/*
 * Car distance sensors based on RoadMeasure
 */

public class CarRoadSensors : MonoBehaviour, JurassicExecute.Exposable {

	public RoadMeasure roadMeasure;
	private int count = 0;

	void Awake(){		
		// set functions to be exposed to user
		// runs before user script is loaded
		GetComponent<JurassicExecute> ().AddExposable (this);
	}
	
	public float distanceFromLeft(){
		return roadMeasure.distanceFromLeft (transform.position);
	}
	
	public float distanceFromRight(){		
		return roadMeasure.distanceFromRight (transform.position);
	}

	public float nextTurnDistance(){
		return roadMeasure.nextTurnDistance (transform.position);
	}

	public float nextTurnAngle(){
		return roadMeasure.nextTurnAngle (transform.position);
	}

	public float previousTurnDistance(){
		return roadMeasure.previousTurnDistance (transform.position);
	}
	
	public float previousTurnAngle(){
		return roadMeasure.previousTurnAngle (transform.position);
	}

	public void FixedUpdate(){
		if (count > 60) {
			//distanceFromLeft ();
			//Debug.Log ("left: " + distanceFromLeft () + " right: " + distanceFromRight() + " next turn dist: " + nextTurnDistance() + " angle: " + nextTurnAngle() );
			count = 0;
		} else {
			count ++;
		}
	}


	
	public void OnGUI(){
		//GUI.Label(Rect(0,0,100,100),"left: " + distanceFromLeft());
	}

	#region JURASSIC
	/* 
	 * Expose sensors to Jurassic
	 */
	public void Expose(ScriptEngine engine){
		// The generic System.Func delegate is used to define method signatures with return types;
		engine.SetGlobalFunction("leftDist", new System.Func<double>(jsGetDistanceFromLeft));
		engine.SetGlobalFunction("rightDist", new System.Func<double>(jsGetDistanceFromRight));
		engine.SetGlobalFunction("nextTurnDist", new System.Func<double>(jsGetNextTurnDistance));
		engine.SetGlobalFunction("nextTurnAngle", new System.Func<double>(jsGetNextTurnAngle));		
		engine.SetGlobalFunction("prevTurnDist", new System.Func<double>(jsGetPreviousTurnDistance));
		engine.SetGlobalFunction("prevTurnAngle", new System.Func<double>(jsGetPreviousTurnAngle));
	}
	
	// type double. (float is not a supported type in Jurassic)
	public double jsGetDistanceFromLeft() { return (double)distanceFromLeft(); }
	public double jsGetDistanceFromRight() { return (double)distanceFromRight(); }
	public double jsGetNextTurnDistance() { return (double)nextTurnDistance(); }
	public double jsGetNextTurnAngle() { return (double)nextTurnAngle(); }
	public double jsGetPreviousTurnDistance() { return (double)previousTurnDistance(); }
	public double jsGetPreviousTurnAngle() { return (double)previousTurnAngle(); }

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

