using UnityEngine;
using System.Collections;
using Jurassic;
using Jurassic.Library;

public class CarDashboard : MonoBehaviour, JurassicExecute.Exposable {

	// Use this for initialization
	//Rigidbody rigidbody;

	void Awake(){		
		// set functions to be exposed to user
		// runs before user script is loaded
		GetComponent<JurassicExecute> ().AddExposable (this);
	}

	void Start () {		
		//rigidbody = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// this should probably be wheel speed, but oh well
	public float getSpeed(){
		return rigidbody.velocity.magnitude;
	}
	
	#region JURASSIC
	/* 
	 * Expose sensors to Jurassic
	 */
	public void Expose(ScriptEngine engine){
		// The generic System.Func delegate is used to define method signatures with return types;
		engine.SetGlobalFunction("getSpeed", new System.Func<double>(jsGetSpeed));
	}
	
	// type double. (float is not a supported type in Jurassic)
	public double jsGetSpeed() { return (double)getSpeed(); }

	public string GetRichTextDescription(){
		return 
			"<b>Car Dashboard</b>\n"+
			"getSpeed()";
	}
	#endregion
}
