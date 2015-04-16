using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Provides access to the leaderboards for each level.
 * A leaderboard is just an ordered list of Scores.
 */
using System;

public class LeaderBoardManager {
	private Dictionary<int,List<Score>> leaderboards = new Dictionary<int,List<Score>> ();

	// Load the leaderboard from where-ever it is persistently stored.
	// if we are storing 
	public void LoadLeaderboards(){
	}

	// save leaderboards in persistence storage
	public void StoreLeaderboards(){
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

		return alreadyExists ? (improved ? 1:-1) : 0;
	}
}

