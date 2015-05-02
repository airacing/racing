﻿using UnityEngine;
using System.Collections;

/*
 * Scores the car's instance, based on:
 *   - (primarily) Time to get round the track.
 *   - possibly number of times crashed ( or could just reduce max speed each time crash or something)
 *   - ...
 */
using UnityEngine.UI;
using System;

public class CarScorer : MonoBehaviour {
	public GameUIManager gameUIManager; // for updating UI when race finishes

	float startTime,endTime, collisionTime;

	int numberOfCrashes;

	bool raceOver, enteredSide = false;

	Score score = new Score();

	// Use this for initialization
	void Start () {
		startTime = Time.time;
		collisionTime = Time.time;
		numberOfCrashes = 0;
	}

	void FixedUpdate(){
		if (score.path.Count < Score.PATH_MAX_COUNT)
			score.path.Add (transform.position);
	}

	void Update(){
		gameUIManager.raceTimeLiveText.text = (Time.time-startTime).To2dpString () + " s"+"\nCrashes: "+numberOfCrashes;
	}

	void OnTriggerEnter(Collider c){
		if (!raceOver) {
			// check finish line collider
			if (c.gameObject.tag == "Finish") {
				endTime = Time.time;

				score.username = AppModel.currentUsername;
				score.raceTime = endTime - startTime;
				score.timestamp = System.DateTime.Now;
				score.userScript = AppModel.getCurrentUserScript ();

				// only submit score if not manual controls
				if (!AppModel.manualCarControls){
					var lbm = AppModel.getLeaderboardManager ();
					int improved = lbm.SubmitScore (AppModel.currentLevel, score);

					gameUIManager.raceTimePanel.SetActive (true);
					gameUIManager.raceTimeValueText.text = score.raceTime.To2dpString () + " s"+"\nCrashes: "+numberOfCrashes;
					if (improved > 0)
						gameUIManager.raceTimeMessageText.text = "Goal! You beat your previous best!";
					else if (improved < 0)
						gameUIManager.raceTimeMessageText.text = "Worse than your previous best...";
					else
						gameUIManager.raceTimeMessageText.gameObject.SetActive(false);
				} else { // manual mode					
					gameUIManager.raceTimePanel.SetActive (true);
					gameUIManager.raceTimeValueText.text = score.raceTime.To2dpString () + " s"+"\nCrashes: "+numberOfCrashes;
					gameUIManager.raceTimeMessageText.text = "You realise this doesn't count, right?";
				}

				raceOver=true;
			}
			else if(c.gameObject.tag=="SidesOfRoad" && !enteredSide){
				numberOfCrashes+=1;
				enteredSide=true;
			}
		}
	}

	void OnTriggerExit(Collider c){
		if (c.gameObject.tag == "SidesOfRoad") {
			enteredSide=false;
		}
	}

	void OnCollisionEnter(Collision c){
		if (c.gameObject.tag == "Terrain" && Time.time - collisionTime > 5 && !enteredSide) {
			collisionTime = Time.time;
			numberOfCrashes += 1;
		}
	}


}
