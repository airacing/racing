
#if UNITY_EDITOR
using UnityEngine;

using UnityEditor;

public class LevelInfoAsset
{
	[MenuItem("Assets/Create/LevelInfo")]
	public static void CreateAsset ()
	{
		ScriptableObjectUtility.CreateAsset<LevelInfo> ();
	}
}

#endif