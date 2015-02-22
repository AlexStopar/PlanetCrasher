using UnityEngine;
using UnityEditor;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;

public class LevelScript : MonoBehaviour {

	string name;
	string planet;
	string levelName;
	int asteroidNumber;

	// Use this for initialization
	void Start () {
		LevelPairs.InitList ();
		LevelPairs.MakeList ();
		foreach(KeyValuePair<string, string> pair in LevelPairs.levelPairs)
		{
			if(LevelPairs.levelPairs.IndexOf(pair).Equals(Application.loadedLevel)) levelName = pair.Value;
		}
//		SimpleJSON.JSONNode node = SimpleJSON.JSONNode.Parse (Resources.Load<TextAsset>
		              //            (levelName).text);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.Q)) Application.LoadLevel ("testLevel2");
	}
}
