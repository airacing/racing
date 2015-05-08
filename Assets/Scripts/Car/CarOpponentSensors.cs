using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Jurassic;
using Jurassic.Library;

/* 
 * Provides the bearing and distance to the opponent.
 */

public class CarOpponentSensors : MonoBehaviour, JurassicExecute.Exposable {

	public Transform opponentCar;
	
	void Awake(){		
		// set functions to be exposed to user
		// runs before user script is loaded
		GetComponent<JurassicExecute> ().AddExposable (this);
	}

	// unsigned distance
	public float opponentDistance(){
		return (opponentCar.position - transform.position).magnitude;
	}

	// from -180.0 degrees to 180.0 degrees
	public float opponentAngle(){
		return Vector2Extensions.SignedAngle2 (transform.forward.ToXZVector2(), (opponentCar.transform.position.ToXZVector2 ()-transform.position.ToXZVector2 ()));
	}
		
	#region JURASSIC
	/* 
	 * Expose sensors to Jurassic
	 */
	public void Expose(ScriptEngine engine){
		// The generic System.Func delegate is used to define method signatures with return types;
		engine.SetGlobalFunction("getOpponentDistance", new System.Func<double>(jsGetOpponentDistance));
		engine.SetGlobalFunction("getOpponentAngle", new System.Func<double>(jsGetOpponentAngle));
	}
	
	// type double. (float is not a supported type in Jurassic)
	public double jsGetOpponentDistance() { return (double)opponentDistance(); }
	public double jsGetOpponentAngle() { return (double)opponentAngle(); }
	
	public string GetRichTextDescription(){
		return "";
	}
	
	#endregion
}

