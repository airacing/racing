using UnityEngine;
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

	float startTime,endTime;

	bool raceOver = false;

	Score score = new Score();

	// Use this for initialization
	void Start () {
		startTime = Time.time;
	}

	void FixedUpdate(){
		if (score.path.Count < Score.PATH_MAX_COUNT)
			score.path.Add (transform.position);
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

				var lbm = AppModel.getLeaderboardManager ();
				int improved = lbm.SubmitScore (AppModel.currentLevel, score);

				gameUIManager.raceTimePanel.SetActive (true);
				gameUIManager.raceTimeValueText.text = score.raceTime.To2dpString () + " s";
				if (improved > 0)
					gameUIManager.raceTimeMessageText.text = "Goal! You beat your previous best!";
				else if (improved < 0)
					gameUIManager.raceTimeMessageText.text = "Worse than your previous best...";
				else
					gameUIManager.raceTimeMessageText.gameObject.SetActive(false);
				raceOver=true;
			}
		}
	}


}
