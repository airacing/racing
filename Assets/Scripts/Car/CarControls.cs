using UnityEngine;
using System.Collections;
using Jurassic;
using Jurassic.Library;

/*
 * Wraps car physics up in left/right/gas/brake controls.
 */

public class CarControls : MonoBehaviour, JurassicExecute.Exposable {
	// State controlling the car
	private float 
		gas = 0f, // 0 to 1
		steering = 0f; // -1 (left) to 1 (right)
	private bool braked = false;
	
	// Internal physics workings
	// 'public' allows initing in unity editor
	// setting parameters inside the editor takes priority over in the constructor
	public WheelCollider 
		wheelFL,
		wheelFR,
		wheelRL,
		wheelRR;
	public Transform
		wheelFLTrans,
		wheelRLTrans,
		wheelRRTrans,
		wheelFRTrans;
	public float
		lowestSteerAtSpeed,
		lowSpeedSteerAngle,
		highSpeedSteerAngle,
		decelarationSpeed,
		maxTorque,
		currentSpeed,
		topSpeed,
		maxReverseSpeed,
		maxBrakeTorque,
		mySidewaysFriction,
		myForwardFriction,
		slipSidewaysFriction,
		slipForwardFriction;

	//private Vector3	originalTransformPosition;
	//private Quaternion originalTransformRotation;
	
	// accessors for game object's components
	//Rigidbody rigidbody; // don't need this for Unity 4, but will do for Unity 5

	void Awake(){		
		// set functions to be exposed to user
		// runs before user script is loaded
		GetComponent<JurassicExecute> ().AddExposable (this);
	}

	// Use this for initialization
	void Start () {
		//rigidbody = GetComponent<Rigidbody> ();
		rigidbody.centerOfMass = new Vector3 (rigidbody.centerOfMass.x, -1, 0.5f);
		myForwardFriction = wheelRR.forwardFriction.stiffness;
		mySidewaysFriction = wheelRR.sidewaysFriction.stiffness;
		slipForwardFriction=0.18f;
		slipSidewaysFriction=0.011f;

		//originalTransformPosition = transform.position;
		//originalTransformRotation = transform.rotation;
	}
	
	// each physics timestep
	void FixedUpdate(){
		Control(); // gas and steering
		HandBrake(); // braking
	}
	
	// each frame
	void Update (){		
		wheelFLTrans.Rotate(wheelFL.rpm/60*360*Time.deltaTime, 0, 0);
		wheelFRTrans.Rotate(wheelFR.rpm/60*360*Time.deltaTime, 0, 0);
		wheelRRTrans.Rotate(wheelRR.rpm/60*360*Time.deltaTime, 0, 0);
		wheelRLTrans.Rotate(wheelRL.rpm/60*360*Time.deltaTime, 0, 0);
		
		Vector3 tmp3 = wheelFLTrans.localEulerAngles;
		tmp3.y=wheelFL.steerAngle - wheelFLTrans.localEulerAngles.z;
		wheelFLTrans.localEulerAngles = tmp3;
		
		tmp3 = wheelFRTrans.localEulerAngles;
		tmp3.y = wheelFR.steerAngle - wheelFRTrans.localEulerAngles.z;
		wheelFRTrans.localEulerAngles = tmp3;

		AudioSource audio1;
		AudioSource[] aSources;
		aSources = GetComponents<AudioSource>();
		if (aSources.Length != 0) {
			audio1 = aSources [0];			
			audio1.pitch = currentSpeed / topSpeed + 1;
		}
	}
	
	void Control () {
		// wheel speed
		currentSpeed=2*Mathf.PI*wheelRL.radius*wheelRL.rpm*60/1000;
		currentSpeed=Mathf.Round(currentSpeed);
		// apply gas	
		if (  currentSpeed < topSpeed && currentSpeed > -maxReverseSpeed) {
			wheelRR.motorTorque = maxTorque * gas;
			wheelRL.motorTorque = maxTorque * gas;
		}
		else {
			wheelRR.motorTorque = 0;
			wheelRL.motorTorque = 0;
		}
		// decelerate if needed 
		if (!braked && gas == 0f) {
			wheelRR.brakeTorque = decelarationSpeed;
			wheelRL.brakeTorque = decelarationSpeed;
		}
		else {
			wheelRR.brakeTorque = 0;
			wheelRL.brakeTorque = 0;
		}
		
		// apply steering
		var speedFactor = rigidbody.velocity.magnitude/lowestSteerAtSpeed;
		var currentSteerAngle = Mathf.Lerp(lowSpeedSteerAngle, highSpeedSteerAngle, speedFactor); // init max steering angle
		currentSteerAngle *= steering;				
		wheelFL.steerAngle = currentSteerAngle;
		wheelFR.steerAngle = currentSteerAngle;
	}
	
	void HandBrake(){
		if (braked) {
			wheelFR.brakeTorque=maxBrakeTorque;
			wheelFL.brakeTorque=maxBrakeTorque;
			wheelRR.motorTorque = 0;
			wheelRL.motorTorque = 0;
			if(rigidbody.velocity.magnitude>1){
				SetSlip(slipForwardFriction, slipSidewaysFriction);
			}
			else {
				SetSlip(myForwardFriction, mySidewaysFriction);
			}
		}
		else{
			wheelFR.brakeTorque=0;
			wheelFL.brakeTorque=0;
			SetSlip(myForwardFriction, mySidewaysFriction);
		}
	}
	
	void SetSlip (float currentForwardFriction, float currentSidewaysFriction){
		WheelFrictionCurve t;
		
		t= wheelRR.sidewaysFriction;
		t.stiffness=currentSidewaysFriction;
		wheelRR.sidewaysFriction = t;
		
		t = wheelRL.sidewaysFriction;
		t.stiffness=currentSidewaysFriction;
		wheelRL.sidewaysFriction = t;
		
		t = wheelRR.forwardFriction;
		t.stiffness=currentForwardFriction;
		wheelRR.forwardFriction = t;
		
		t = wheelRL.forwardFriction;
		t.stiffness=currentForwardFriction;
		wheelRL.forwardFriction = t;
	}

	public void SetGas(float x){
		Assert.Test (x >= 0f && x <= 1f);
		gas = x;
	}
	public void SetSteering(float x){
		Assert.Test (x >= -1f && x <= 1f);
		steering = x;
	}
	public void SetBrake(bool b){
		braked = b;
	}

	#region JURASSIC
	/* 
	 * Expose controls to Jurassic
	 */
	public void Expose(ScriptEngine engine){		
		// The generic System.Action delegate is used to define method signatures with no returns;
		engine.SetGlobalFunction("setGas", new System.Action<double>(jsSetGas));
		engine.SetGlobalFunction("setSteering", new System.Action<double>(jsSetSteering));		
		engine.SetGlobalFunction("setBrake", new System.Action<bool>(jsSetBrake));
	}
	
	// type double. (float is not a supported type in Jurassic)
	public void jsSetGas(double x)
	{
		if (x < -0f || x > 1f) {
			// TODO: error this to user
			Debug.LogError ("setGas called with invalid argument. Ignoring call.");
			return;
		}
		SetGas ((float)x);
	}
	public void jsSetSteering(double x)
	{
		if (x < -1f || x > 1f) {
			Debug.LogError ("setSteering called with invalid argument. Ignoring call.");
			return;
		}
		SetSteering ((float)x);
	}
	public void jsSetBrake(bool b)
	{
		SetBrake (b);
	}

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