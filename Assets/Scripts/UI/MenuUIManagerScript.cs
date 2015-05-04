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
	
	//+++++++++++++++++++++++
	public InputField ghostnameInputFieldObj;
	//+++++++++++++++++++++++

	// leaderboard
	public GameObject rankPanel, usernamePanel, raceTimePanel, userScriptButtonPanel; 
	public Text leaderboardTextPrefab;
	public Button leaderboardCopyScriptPrefab;

	public Toggle speedupToggle, ghostCarToggle;

	// GUI state
	private int currentLevelIndex = 0; // current level selected in levelInfos

	// Use this for initialization
	void Start () {
		// init currentLevel
		if (AppModel.currentLevel == null)
			AppModel.currentLevel = levels [currentLevelIndex]; // set level
		else
			currentLevelIndex = levels.IndexOf (AppModel.currentLevel);

		// load leaderboards
		if (!AppModel.leaderboardsLoaded) {
			AppModel.LoadLeaderBoards ();
			AppModel.leaderboardsLoaded = true;
		}

		Time.timeScale = 1;
		Refresh ();
	}

	void OnApplicationQuit(){
		// save leaderboards		
		AppModel.StoreLeaderBoards ();
	}

	// Save state in AppModel
	private void Save(){		
		AppModel.setCurrentUserScript (codeInputFieldObj.GetComponent<InputField> ().text);	
		AppModel.currentUsername = usernameInputFieldObj.GetComponent<InputField> ().text;
		AppModel.speedup = speedupToggle.isOn;
		AppModel.ghostCar = ghostCarToggle.isOn;
		//++++++++++++++++++
		AppModel.ghostName = ghostnameInputFieldObj.GetComponent<InputField> ().text;
		//++++++++++++++++++
	}

	// refresh UI according to AppModel state
	private void Refresh(){
		// load user script
		codeInputFieldObj.GetComponent<InputField> ().text = AppModel.getCurrentUserScript ();

		// ** load docs **
		{
			// delete all current docs

			List<GameObject> children = new List<GameObject> ();
			foreach (Transform child in scriptDocsHolderObj.transform)
				children.Add (child.gameObject);
			children.ForEach (child => Destroy (child));


			// create new docs
			foreach (string doc in AppModel.currentLevel.scriptDocs) {
				Text docObj = (Text)Instantiate (scriptDocsTextPrefab);
				docObj.text = doc;
				docObj.transform.SetParent (scriptDocsHolderObj.transform, false);
			}
		}

		// ** error message **
		// display the error message, if it exists
		{
			Text errorMessageText = errorMessageTextObj.GetComponent<Text> ();
			if (AppModel.errorMessage != null) {
				errorMessageText.text = AppModel.errorMessage;
			} else {
				errorMessageText.text = "";
			}
		}

		// ** level name **
		levelNameTextObj.text = AppModel.currentLevel.levelName;

		// ** level image **
		levelImageObj.sprite = AppModel.currentLevel.sprite;

		// ** speedup + ghost***
		speedupToggle.isOn = AppModel.speedup;
		ghostCarToggle.isOn = AppModel.ghostCar;

		// ** username **
		if (AppModel.currentUsername != null)
			usernameInputFieldObj.GetComponent<InputField> ().text = AppModel.currentUsername;

		// ** highscores **
		{
			// delete all current highscore entries
			List<GameObject> lbPanels = new List<GameObject>{rankPanel, usernamePanel, raceTimePanel, userScriptButtonPanel};
			foreach (GameObject panel in lbPanels){
				List<GameObject> children = new List<GameObject>();
				foreach (Transform child in panel.transform) children.Add(child.gameObject);
				children.ForEach(child => Destroy(child));
			}

			List<Score> lb = AppModel.getLeaderboardManager ().GetLeaderboard (AppModel.currentLevel);
			if (lb!=null){
				int rank = 1;
				foreach(Score score in lb){
					Text rankText = (Text)Instantiate(leaderboardTextPrefab);
					Text usernameText = (Text)Instantiate(leaderboardTextPrefab);
					Text raceTimeText = (Text)Instantiate(leaderboardTextPrefab);
					Button copyScriptButton = (Button)Instantiate(leaderboardCopyScriptPrefab);
					rankText.text = rank + ".";
					usernameText.text = score.username;
					raceTimeText.text = score.raceTime.To2dpString()+" s";
					Score capturedScore = score;
					copyScriptButton.onClick.AddListener(() => codeInputFieldObj.GetComponent<InputField> ().text = capturedScore.userScript);

					rankText.transform.SetParent (rankPanel.transform, false);
					usernameText.transform.SetParent (usernamePanel.transform, false);
					raceTimeText.transform.SetParent (raceTimePanel.transform, false);
					copyScriptButton.transform.SetParent (userScriptButtonPanel.transform, false);

					rank++;
				}
			}
		}
	}

	// UI event functions

	public void runBtnClick(){
		Debug.Log ("Run button clicked.");	
		AppModel.manualCarControls = false;
		runRace ();
	}

	public void manualBtnClick(){
		Debug.Log ("Manual button clicked.");		
		AppModel.manualCarControls = true;
		runRace ();
	}

	private void runRace(){
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

	public void QuitButtonClick(){
		Application.Quit ();
	}
}
