using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour {

	public GameObject raceTimePanel;
	public Text raceTimeValueText, raceTimeCrashesText, raceTimeMessageText, raceTimeLiveText;

	public GameObject mainCar, ghostCar;

	// Use this for initialization
	void Start () {
		Time.timeScale = AppModel.speedup ? 3.0f : 1.0f;

		// initialise main car
		mainCar.GetComponent<CarScorer> ().enabled = true; // only enable user's car's scorer! (don't want ghost to update UI or submit score)
		if (AppModel.manualCarControls) {
			mainCar.GetComponent<KeyboardController>().controllingCar = true; // enable manual controls
		} else {
			JurassicExecute je = mainCar.GetComponent<JurassicExecute>();
			je.LoadScript(AppModel.getCurrentUserScript());
			je.controllingCar = true; // enable scripted controls
		}

		// initialise ghost car if required/possible
		if (AppModel.ghostCar) {
			var lb = AppModel.getLeaderboardManager ().GetLeaderboard (AppModel.currentLevel);
			if (lb != null) {
				Score score = lb [0]; // highest score
				// TODO: enable ghost car, and set its script to the script stored in 'score' (the best script in the leaderboard)
			}
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void stopSimulation(){		
		Application.LoadLevel(AppModel.menuSceneName);
	}
}
