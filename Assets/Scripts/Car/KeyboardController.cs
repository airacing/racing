using UnityEngine;
using System.Collections;

public class KeyboardController : MonoBehaviour {
	private CarControls carControls;
	public bool controllingCar = false; // only takes control of the car when this is true

	void Start() {
		carControls = GetComponent<CarControls> ();
	}

	void FixedUpdate () {
		if (controllingCar) {
			float v = Input.GetAxis ("Vertical");
			if (v > 0)
				carControls.SetGas (v);
			else
				carControls.SetGas (0);

			if (v < 0)
				carControls.SetBrake (true);
			else 
				carControls.SetBrake (false);

			carControls.SetSteering (Input.GetAxis ("Horizontal"));
		}
	}
}
