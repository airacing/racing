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
	// score = leaderboards[level_id][username]
	private Dictionary<int,Dictionary<string,Score>> leaderboards = new Dictionary<int,Dictionary<string,Score>> ();

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
					Score score = Score.LoadFromString(finalSplit[1]);
					leaderboards[level][userName] = score;
				}
			}
		}
	}

	// save leaderboards in persistence storage
	public void StoreLeaderboards(){
		System.Text.StringBuilder data = new System.Text.StringBuilder (30000 * 100);
		foreach (KeyValuePair<int, Dictionary<string, Score>> entry in leaderboards) {
			int level = entry.Key;
			Dictionary<string, Score> value = leaderboards[level];
			data.Append(level.ToString() + "[line-separator]");
			foreach (KeyValuePair<string, Score> innerEntry in value) {
				data.Append(innerEntry.Key + "[inner-separator]");
				data.Append(innerEntry.Value.ToString() + "[inner-separator]");
				data.Append("[score-separator]");
			}
			data.Append("[level-separator]");
		}
		File.WriteAllText ("data.txt", data.ToString());
	}

	// returns sorted 
	public List<Score> GetLeaderboard(LevelInfo level){
		if (leaderboards.ContainsKey(level.id)){
			var l = new List<Score>(leaderboards[level.id].Values);
			l.Sort ( (s1,s2) => (s1.raceTime.To2dp()!=s2.raceTime.To2dp())?s1.raceTime.CompareTo(s2.raceTime):s1.timestamp.CompareTo(s2.timestamp));
			return l;
		} else {
			return null;
		}
	}

	
	//++++++++++++++++++++++
	// returns unsorted 
	// dictionary form
	public Dictionary<string, Score> GetLeaderboardDict(LevelInfo level){
		return leaderboards [level.id];
	}
	//++++++++++++++++++++++


	/*
	 * Maintains that there is only one score registered per username.
	 * Returns whether the score was improved for the username.
	 * (NOTE: would probably be better to use Dictionary for each leaderboard, except it might be harder to serialize etc.)
	 */
	public int SubmitScore(LevelInfo level, Score score){
		//create new leaderboard for level, if one doesn't already exist
		if (!leaderboards.ContainsKey (level.id))
			leaderboards [level.id] = new Dictionary<string, Score>();
		var lb = leaderboards [level.id];

		// replace worse scores with new score
		int res; 
		if (!lb.ContainsKey (score.username)) {
			lb [score.username] = score;
			res = 0;
		} else if (lb [score.username].raceTime > score.raceTime) {
			lb [score.username] = score;
			res = 1;
		} else {
			res = -1;
		}
		return res;
	}
}

