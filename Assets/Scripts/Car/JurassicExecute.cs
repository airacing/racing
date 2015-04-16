using UnityEngine;
using System.Collections;
using Jurassic;
using Jurassic.Library;
using System.Collections.Generic;

/*
 * Controls and performs execution of user's JS script.
 * User needs to define a function update(){} that will be called at each step.
 * User's initialisation code can go in the script main body.
 * 
 * The methods exposed to the user are controlled by Exposables.
 * This allows different Exposables to be added for different scenes.
 * (allowing different sensors for different environments, for example).
 * 
 * This class is inherently attached to a car instance, allowing multiple cars
 * each being controlled by different scripts.
 */
using System;

public class JurassicExecute : MonoBehaviour {
	// all controls/sensors for the particular scene should be in this list
	public List<Exposable> exposables = new List<Exposable>();
	
	private ScriptEngine engine;	
	private bool running = false;

	// Init the script stored in AppModel
	void Start(){
		if (AppModel.getCurrentUserScript() != null) {
			try {
				LoadScript (AppModel.getCurrentUserScript());
				Run ();
				AppModel.errorMessage = null;
			} catch (JavaScriptException ex){
				Debug.Log(ex.Message);
				AppModel.errorMessage = ex.Message;
				Stop ();
				Application.LoadLevel(AppModel.menuSceneName);
			}
		}
	}

	// prepare user's script
	public void LoadScript(string codeString){
		// Create an instance of the Jurassic engine then expose some stuff to it.
		engine = new ScriptEngine();
		
		// Arguments and returns of functions exposed to JavaScript must be of supported types.
		// Supported types are bool, int, double, string, Jurassic.Null, Jurassic.Undefined
		// and Jurassic.Library.ObjectInstance (or a derived type).
		// More info: http://jurassic.codeplex.com/wikipage?title=Supported%20types
		
		// Examples of exposing some static classes to JavaScript using Jurassic's "seamless .NET interop" feature.
		engine.EnableExposedClrTypes = true; // You must enable this in order to use interop feaure.
		// Then pass the names and types of the classes you want to expose to SetGlobalValue().
		engine.SetGlobalValue("Mathf", typeof(Mathf));
		engine.SetGlobalValue("Input", typeof(Input));
		engine.SetGlobalFunction("debugLog", new System.Action<string,double>(jsDebugLog));
		
		foreach (Exposable e in exposables)
			e.Expose (engine);
		engine.Evaluate (codeString);
	}

	// log output to user 
	// calls with the same delta are guaranteed to be logged on the same physics timestamp
	public void jsDebugLog(string s,double delta){
		if ((int)(Math.Ceiling(Time.fixedTime/(float)delta)) != (int)(Math.Ceiling((Time.fixedTime-Time.fixedDeltaTime)/(float)delta))){
			// TODO: replace this with in-game debug console
			Debug.Log (s);
		}
	}

	// run script
	public void Run(){
		Assert.Test (engine != null);
		running = true;
	}

	// stop running script
	// (TODO: also stop the car)
	public void Stop(){
		running = false;
	}

	// execute user update script at each physics timestep
	void FixedUpdate(){
		if (running) {
			Assert.Test(engine!=null);
			try{
				engine.CallGlobalFunction("update");
				AppModel.errorMessage = null;
			} catch (JavaScriptException ex){
				Debug.Log(ex.Message);
				AppModel.errorMessage = ex.Message;
				Stop ();
				Application.LoadLevel(AppModel.menuSceneName);
			}
		}
	}

	// classes that need to have functions exposed should implement this
	// cant get it static
	public interface Exposable{
		void Expose(ScriptEngine engine); // expose self's functions to the given engine
		string GetRichTextDescription(); // string describing self's functions
	}

	public void AddExposable(Exposable e){
		exposables.Add(e);
	}
}
