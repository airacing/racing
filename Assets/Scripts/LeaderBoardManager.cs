using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Provides access to the leaderboards for each level.
 * A leaderboard is just an ordered list of Scores.
 */
using System;
using System.IO;
using System.Globalization;

public class LeaderBoardManager {
	private Dictionary<int,List<Score>> leaderboards = new Dictionary<int,List<Score>> ();

	// Load the leaderboard from where-ever it is persistently stored.
	// if we are storing 
	public void LoadLeaderboards(){
		if (File.Exists("data.txt"))
		{
			string data = File.ReadAllText("data.txt");
			string[] levelSplit = data.Split(new string[] {"[level-separator]"}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string levelStr in levelSplit) {
				string[] contents = levelStr.Split(new string[] {"[line-separator]"}, StringSplitOptions.RemoveEmptyEntries);
				int level = int.Parse(contents[0], CultureInfo.InvariantCulture);
				leaderboards[level] = new Dictionary<string, Score>();
				string[] scoreSplitStr = (contents[1]).Split(new string[] {"[score-separator]"}, StringSplitOptions.RemoveEmptyEntries);
				foreach (string scoreStr in scoreSplitStr) {
					string[] finalSplit = scoreStr.Split(new string[]{"[inner-separator]"}, StringSplitOptions.None);
					string userName = finalSplit[0];
					Score score = Score.loadFromString(finalSplit[1]);
					leaderboards[level][userName] = score;
				}
			}
		}
	}

	// save leaderboards in persistence storage
	public void StoreLeaderboards(){
		string data = "";
		foreach (KeyValuePair<int, Dictionary<string, Score>> entry in leaderboards) {
			int level = entry.Key;
			Dictionary<string, Score> value = leaderboards[level];
			data += level.ToString() + "[line-separator]";
			foreach (KeyValuePair<string, Score> innerEntry in value) {
				data += innerEntry.Key + "[inner-separator]";
				data += innerEntry.Value.ToString() + "[inner-separator]";
				data += "[score-separator]";
			}
			data += "[level-separator]";
		}
		File.WriteAllText ("data.txt", data);
	}

	public List<Score> GetLeaderboard(LevelInfo level){
		if (leaderboards.ContainsKey(level.id)){
			return leaderboards[level.id];
		} else {
			return null;
		}
	}

	/*
	 * Maintains that there is only one score registered per username.
	 * Returns whether the score was improved for the username.
	 * (NOTE: would probably be better to use Dictionary for each leaderboard, except it might be harder to serialize etc.)
	 */
	public int SubmitScore(LevelInfo level, Score score){
		if (!leaderboards.ContainsKey (level.id))
			leaderboards [level.id] = new List<Score> ();
		var lb = leaderboards [level.id];

		// replace worse scores with new score
		bool alreadyExists = false;
		bool improved = false;
		for (int i=0; i<lb.Count; i++) {
			if (lb[i].username == score.username){				
				alreadyExists = true;
				if (lb[i].raceTime > score.raceTime){
					lb[i] = score;
					improved = true;
				}
				break;
			}
		}
		if (!alreadyExists)
			lb.Add (score);

		lb.Sort ( (s1,s2) => s1.raceTime.CompareTo(s2.raceTime));
		foreach (Score s in lb)
			Debug.Log (s.ToString ()); // these are not printing in correct order!? (assuming the sorting is correct)
		AppModel.saveLeaderBoard ();
		return alreadyExists ? (improved ? 1:-1) : 0;
	}
}

