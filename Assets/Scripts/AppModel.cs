using UnityEngine;
using System.Collections;

/*
 * Data that needs to be shared between levelMenu and levelX should be stored here.
 * This enables, for example, the user's code to be retained when returning from levelX back to levelMenu.
 * 
 * TODO: Users code for each differnt level stored here (hashmap)
 */
using System.Collections.Generic;


public class AppModel {	
	// set this to false for the real thing
	public static bool debugging = false;

	// Game state
	public static LevelInfo currentLevel { get; set; } // current level user is working on
	public static string errorMessage { get; set; }
	public static string currentUsername;
	public static bool manualCarControls;
	public static bool leaderboardsLoaded = false;
	public static bool speedup = false, ghostCar = false;

	public static Score otherScore = null; // score of opponent or ghost

	private static Dictionary<string,string> userScripts = new Dictionary<string,string>(); // users code for each level
	private static LeaderBoardManager leaderBoardManager = new LeaderBoardManager();

	public static string menuSceneName = "levelMenu";

	public static void LoadLeaderBoards() {
		leaderBoardManager.LoadLeaderboards ();
	}

	public static void StoreLeaderBoards() {
		leaderBoardManager.StoreLeaderboards ();
	}
	// gets script for current level
	// returns sample script if user hasnt written one yet
	public static string getCurrentUserScript(){
		Assert.Test (currentLevel != null);
		// if user has code stored, load it
		if (userScripts.ContainsKey (currentLevel.sceneName)) {
			return userScripts[currentLevel.sceneName];
		} else { // else load sample code
			return currentLevel.sampleCode;
		}
	}

	public static LeaderBoardManager getLeaderboardManager(){
		return leaderBoardManager;
	}

	// store user script
	public static void setCurrentUserScript(string userScript){
		Assert.Test (currentLevel != null,"no scene selected..!!");
		userScripts[currentLevel.sceneName] = userScript;
	}
}
