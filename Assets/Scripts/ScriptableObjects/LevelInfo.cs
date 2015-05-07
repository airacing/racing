using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/* 
 * Stores information describing a level.
 * Should be created in the editor.
 */
using System.Collections.Generic;
using System; 

[Serializable]
public class LevelInfo : ScriptableObject {
	public int id; // unique level id
	public string levelName; // label


	//public enum OpponentMode{shadow,race};
	//public OpponentMode mode;
	public int mode;
	public const int OPPONENT_MODE_GHOST=1, OPPONENT_MODE_PHYSCIAL=2;

	public string sceneName; // scene to be loaded that is associated with this level
	public Sprite sprite;

	// Add all the exposable modules here
	// exposable.GetRichTextDescription is called
	[SerializeField]
	public List<string> scriptDocs; // documentation for the modules (exposables) supported by this level.

	public string sampleCode;

}
