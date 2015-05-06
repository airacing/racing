using UnityEngine;
using System.Collections;

public class CollisionScript : MonoBehaviour {

	private AudioSource audio2;
	public AudioClip crash;
	AudioSource[] aSources;

	void Awake () {
		aSources = GetComponents<AudioSource>();
		audio2 = aSources[1];

	}


	void OnCollisionEnter (Collision hit){
		audio2.PlayOneShot (crash);

	}
}
