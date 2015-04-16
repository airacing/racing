using UnityEngine;
using System.Collections;

public class KeyboardController : MonoBehaviour {
	private CarControls carControls;

	void Start() {
		carControls = GetComponent<CarControls> ();
	}

	void FixedUpdate () {
		float v =  Input.GetAxis ("Vertical");
		if ( v > 0 )
			carControls.SetGas( v );
		else
			carControls.SetGas(0);

		if ( v <0 )
			carControls.SetBrake(true);
		else 
			carControls.SetBrake(false);

		carControls.SetSteering(Input.GetAxis ("Horizontal"));
	}
}
