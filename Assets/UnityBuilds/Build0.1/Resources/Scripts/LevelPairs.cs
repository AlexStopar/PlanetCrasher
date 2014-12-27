using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class LevelPairs {

	public static List<KeyValuePair<string, string>> levelPairs;
	// Use this for initialization
	public static void InitList () {
		levelPairs = new List<KeyValuePair<string, string>> ();
	}
	
	// Update is called once per frame
	public static void MakeList () {
		levelPairs.Add (new KeyValuePair<string, string> ("Assets/UnityBuilds/Build0.1/testLevel.unity", "JSON/test"));
		levelPairs.Add (new KeyValuePair<string, string> ("Assets/UnityBuilds/Build0.1/testLevel2.unity", "JSON/test2"));
	}
}
