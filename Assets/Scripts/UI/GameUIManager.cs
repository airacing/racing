using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour {

	public GameObject raceTimePanel;
	public Text raceTimeValueText, raceTimeMessageText, raceTimeLiveText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void stopSimulation(){		
		Application.LoadLevel(AppModel.menuSceneName);
	}
}
