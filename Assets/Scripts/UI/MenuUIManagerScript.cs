using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/* 
 * Singleton that persists between LoadScene, allowing current level, and users code for all the levels, to be remembered when coming back from levelX.
 */ 
public class MenuUIManagerScript : MonoBehaviour {
		
	// all possible selectable levels (assign this in inspector)
	[SerializeField]
	public List<LevelInfo> levels = new List<LevelInfo> ();

	public InputField codeInputFieldObj, usernameInputFieldObj;
	public Text levelNameTextObj, errorMessageTextObj;

	public Image levelImageObj;

	public GameObject scriptDocsHolderObj;
	public Text scriptDocsTextPrefab; // to create text instances for each scriptDoc

	public Text leaderboardUsernamesText, leaderboardScoresText;

	// GUI state
	private int currentLevelIndex = 0; // current level selected in levelInfos

	// Use this for initialization
	void Start () {
		if (AppModel.currentLevel == null)
			AppModel.currentLevel = levels [currentLevelIndex]; // set level
		else
			currentLevelIndex = levels.IndexOf (AppModel.currentLevel);
		AppModel.loadLeaderBoard ();
		Refresh ();
	}

	// Save user code in AppModel
	private void Save(){		
		AppModel.setCurrentUserScript (codeInputFieldObj.GetComponent<InputField> ().text);	
		AppModel.currentUsername = usernameInputFieldObj.GetComponent<InputField> ().text;
	}

	// refresh UI according to AppModel state
	private void Refresh(){
		// load user script
		codeInputFieldObj.GetComponent<InputField> ().text = AppModel.getCurrentUserScript ();

		// ** load docs **

		// delete all current docs
		List<GameObject> children = new List<GameObject>();
		foreach (Transform child in scriptDocsHolderObj.transform) children.Add(child.gameObject);
		children.ForEach(child => Destroy(child));

		// create new docs
		foreach (string doc in AppModel.currentLevel.scriptDocs) {
			Text docObj = (Text)Instantiate(scriptDocsTextPrefab);
			docObj.text = doc;
			docObj.transform.SetParent(scriptDocsHolderObj.transform,false);
		}

		// ** error message **
		// display the error message, if it exists
		Text errorMessageText = errorMessageTextObj.GetComponent<Text> ();
		if (AppModel.errorMessage != null) {
			errorMessageText.text = AppModel.errorMessage;
		} else {
			errorMessageText.text = "";
		}

		// ** level name **
		levelNameTextObj.text = AppModel.currentLevel.levelName;

		// ** level image **
		levelImageObj.sprite = AppModel.currentLevel.sprite;

		// ** username **
		if (AppModel.currentUsername != null)
			usernameInputFieldObj.GetComponent<InputField> ().text = AppModel.currentUsername;

		// ** highscores **
		string usernames = "", scores = "";
		var lb = AppModel.getLeaderboardManager ().GetLeaderboard (AppModel.currentLevel);
		if (lb != null) {
			foreach (Score s in lb) {
				usernames += s.username + "\n";
				scores += s.raceTime.To2dpString () + " s\n";
			}
			leaderboardUsernamesText.text = usernames.TrimEnd();
			leaderboardScoresText.text = scores.TrimEnd();
		} else {
			//usernames = "noleaderboard";			
			leaderboardUsernamesText.text = usernames;
			leaderboardScoresText.text = scores;
		}
	}

	public void runBtnClick(){
		Debug.Log ("Run button clicked.");	
		AppModel.manualCarControls = false;
		run ();
	}

	public void manualBtnClick(){
		Debug.Log ("Manual button clicked.");		
		AppModel.manualCarControls = true;
		run ();
	}

	private void run(){
		Save ();
		if (AppModel.debugging || AppModel.currentUsername.Length != 0){
			Application.LoadLevel (AppModel.currentLevel.sceneName);
		} else {
			AppModel.errorMessage = "Enter a username!";
			Refresh();
		}
	}

	public void prevBtnClick(){
		Debug.Log ("prev button clicked.");
		if (currentLevelIndex - 1 >= 0) {
			Save ();
			currentLevelIndex -= 1;
			AppModel.currentLevel = levels[currentLevelIndex];
			Refresh();
		}
	}

	public void nextBtnClick(){
		Debug.Log ("nxt button clicked.");
		if (currentLevelIndex + 1 < levels.Count) {
			Save ();
			currentLevelIndex += 1;
			AppModel.currentLevel = levels[currentLevelIndex];
			Refresh();
		}
	}

	void OnApplicationQuit() {
		AppModel.saveLeaderBoard ();
	}

}
