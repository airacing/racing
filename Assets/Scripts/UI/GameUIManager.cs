using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour {

	public GameObject raceTimePanel;
	public Text raceTimeValueText, raceTimeCrashesText, raceTimeMessageText, raceTimeLiveText;

	public GameObject mainCar, otherCar;

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

		// initialise other car if required/possible
		if (AppModel.ghostCar) {
			var lbDict = AppModel.getLeaderboardManager ().GetLeaderboardDict (AppModel.currentLevel);
			if (lbDict != null && lbDict.ContainsKey(AppModel.otherScore.username)) {
				//Score score = lb [0]; // highest score
				// TODO: enable ghost car, and set its script to the script stored in 'score' (the best script in the leaderboard)
				//++++++++++++++++++++++++++++++++
				// gonna change it so the ghost car doesn't have to use the "best" script, cuz they might
				// not be stable in each run (and we can't get rid of it without having a new highscore)
				otherCar.SetActive(true);
				
				Score score = lbDict[AppModel.otherScore.username];
				string ghostScript = score.userScript;
				
				JurassicExecute je = otherCar.GetComponent<JurassicExecute>();
				je.LoadScript(ghostScript);
				je.controllingCar = true; // enable scripted controls
				//++++++++++++++++++++++++++++++++
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
