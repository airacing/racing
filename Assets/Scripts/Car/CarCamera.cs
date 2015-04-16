using UnityEngine;
using System.Collections;

/*
 * Makes camera follow car.
 */

public class CarCamera : MonoBehaviour {

	public Transform car;
	private float distance = 20,
		height = 8,
		defaultFOV = 60,
		rotationDamping = 2,
		heightDamping = 2,
		zoomRatio = 5;
	public Vector3 rotationVector;

	// Use this for initialization
	void Start () {
	
	}

	void LateUpdate(){
		float wantedAngle = rotationVector.y;
		float wantedHeight = car.position.y+height;
		float myAngle = transform.eulerAngles.y;
		float myHeight = transform.position.y;

		myAngle=Mathf.LerpAngle(myAngle, wantedAngle, rotationDamping*Time.deltaTime);
		myHeight=Mathf.Lerp(myHeight, wantedHeight, heightDamping*Time.deltaTime);

		var currentRotation=Quaternion.Euler(0, myAngle, 0);

		var tmp3 = car.position;
		tmp3 -= currentRotation * Vector3.forward * distance;
		tmp3.y = myHeight;
		transform.position = tmp3;
		transform.LookAt(car);
	}

	void FixedUpdate(){
		var localVelocity=car.InverseTransformDirection(car.rigidbody.velocity);
		if(localVelocity.z<-0.5) {
			rotationVector.y=car.eulerAngles.y+180;
		}
		else {
			rotationVector.y=car.eulerAngles.y;
		}
		var acc = car.rigidbody.velocity.magnitude;
		camera.fieldOfView=defaultFOV+acc*zoomRatio*Time.deltaTime;
	}
}