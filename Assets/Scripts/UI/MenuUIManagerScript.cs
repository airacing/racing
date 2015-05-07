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

	public InputField codeInputFieldObj, usernameInputFieldObj, opponentUsernameInputFieldObj;
	public Text levelNameTextObj, errorMessageTextObj;

	public Image levelImageObj;

	public GameObject scriptDocsHolderObj;
	public Text scriptDocsTextPrefab; // to create text instances for each scriptDoc


	// leaderboard
	public GameObject rankPanel, usernamePanel, raceTimePanel, userScriptButtonPanel; 
	public Text leaderboardTextPrefab;
	public GameObject leaderboardUsernamePrefab;
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

		// attempt to set the opponent score
		string otherUsername = opponentUsernameInputFieldObj.text;
		var lbDict = AppModel.getLeaderboardManager ().GetLeaderboardDict (AppModel.currentLevel);
		if (lbDict.ContainsKey (otherUsername)) {
			AppModel.otherScore = lbDict [otherUsername];
			AppModel.ghostCar = true;
		} else {
			AppModel.ghostCar = false;
		}
	}

	// refresh UI according to AppModel state
	private void Refresh(){
		// change race mode UI
		if (AppModel.currentLevel.mode == LevelInfo.OPPONENT_MODE_GHOST) {
			((Text)opponentUsernameInputFieldObj.placeholder).text = "(Ghost username...)";
		} else {
			((Text)opponentUsernameInputFieldObj.placeholder).text = "Opponent username...";
		}

		if (AppModel.otherScore != null)
			opponentUsernameInputFieldObj.text = AppModel.otherScore.username;
		else
			opponentUsernameInputFieldObj.text = "";

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

					GameObject usernameObj = (GameObject)Instantiate(leaderboardUsernamePrefab);
					//Button usernameButton = usernameObj.GetComponent<Button>();
					Text usernameText = usernameObj.GetComponent<Text>();

					Text raceTimeText = (Text)Instantiate(leaderboardTextPrefab);
					Button copyScriptButton = (Button)Instantiate(leaderboardCopyScriptPrefab);

					rankText.text = rank + ".";
					usernameText.text = score.username;
					/*if (AppModel.otherScore!=null && AppModel.otherScore.username == score.username){ // bug careful
						usernameText.fontStyle = FontStyle.Bold;
						usernameText.color = Color.blue;
					}*/

					raceTimeText.text = score.raceTime.To2dpString()+" s";
					Score capturedScore = score;
					copyScriptButton.onClick.AddListener(() => codeInputFieldObj.GetComponent<InputField> ().text = capturedScore.userScript);

					rankText.transform.SetParent (rankPanel.transform, false);
					usernameText.transform.SetParent (usernamePanel.transform, false);
					raceTimeText.transform.SetParent (raceTimePanel.transform, false);
					copyScriptButton.transform.SetParent (userScriptButtonPanel.transform, false);

					/*usernameButton.onClick.AddListener(() => {
						// if selected as opponent, deselect
						if (AppModel.otherScore!=null && AppModel.otherScore.username == capturedScore.username){
							AppModel.otherScore = null;
						} else {
							AppModel.otherScore = capturedScore;
						}
						Refresh();
					});*/

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

	// check params, and if they check out, load level scene
	// else, show error
	private void runRace(){
		// validate username
		if (!AppModel.debugging && (AppModel.currentUsername.Length < 3 || AppModel.currentUsername.Length > 12)){			
			AppModel.errorMessage = "Username must be between 3-12 characters.";
			Refresh();
			return;
		}
		// check opponent username exists
		string otherUsername = opponentUsernameInputFieldObj.text;
		if (otherUsername.Length != 0) {
			var lbDict = AppModel.getLeaderboardManager ().GetLeaderboardDict (AppModel.currentLevel);
			if (!lbDict.ContainsKey (otherUsername)) {
				AppModel.errorMessage = (AppModel.currentLevel.mode==LevelInfo.OPPONENT_MODE_GHOST ? "Ghost" : "Opponent") +" username does not exist in the leaderboard!";
				Refresh();
				return;
			}
		}
		Save ();
		Application.LoadLevel (AppModel.currentLevel.sceneName);
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
