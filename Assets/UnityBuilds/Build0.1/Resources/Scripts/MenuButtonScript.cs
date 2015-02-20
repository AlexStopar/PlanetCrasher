using UnityEngine;
using UnityEditor;
using System.Collections;

public class MenuButtonScript : MonoBehaviour {

	BoxCollider2D StartButtonBounds;
	BoxCollider2D TwoPlayerBounds;
	Camera camera;
	// Use this for initialization
	void Start () {
		StartButtonBounds = GameObject.Find ("StartButton").GetComponent<BoxCollider2D> ();
		TwoPlayerBounds = GameObject.Find ("2PlayerButton").GetComponent<BoxCollider2D> ();
		camera = GameObject.Find ("Main Camera").camera;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.touchCount >= 1) 
		{
				Vector3 touchPoint = camera.ScreenToWorldPoint (Input.GetTouch (0).position);
				if(StartButtonBounds == Physics2D.OverlapPoint(touchPoint)) Application.LoadLevel ("testLevel");
				else if(TwoPlayerBounds == Physics2D.OverlapPoint(touchPoint)) Application.LoadLevel("2PLevel");	   
		}
	}
}
