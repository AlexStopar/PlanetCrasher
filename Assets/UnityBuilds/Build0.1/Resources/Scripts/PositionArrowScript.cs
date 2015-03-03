using UnityEngine;
using System.Collections;

public class PositionArrowScript : MonoBehaviour {

	public bool isPlayerTwo = false;
	public bool isRight = true;
	BoxCollider2D bounds;
	TouchBehavior touchStrategy;
	// Use this for initialization
	void Start () {
		if (!isPlayerTwo) touchStrategy = GameObject.Find ("AsteroidMaker").GetComponent<AsteroidScript> ().touchStrategy; 
		else touchStrategy = GameObject.Find ("AsteroidMaker2").GetComponent<AsteroidScript> ().touchStrategy; 
		bounds = gameObject.GetComponent<BoxCollider2D> ();
		bounds.isTrigger = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (!isPlayerTwo) touchStrategy = GameObject.Find ("AsteroidMaker").GetComponent<AsteroidScript> ().touchStrategy; 
		else touchStrategy = GameObject.Find ("AsteroidMaker2").GetComponent<AsteroidScript> ().touchStrategy; 
		if (Input.touchCount >= 1) 
		{
			if(Input.GetTouch(0).phase != TouchPhase.Began) return;
			Vector3 touchPoint = Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position);
			if(bounds == Physics2D.OverlapPoint(touchPoint)) 
			{
				int touchNumber;
				if(isRight) touchNumber = ((int)(touchStrategy.state) + 1);
				else touchNumber = ((int)(touchStrategy.state) - 1);
				if(touchNumber < (int) TouchBehavior.NODE_POSITION_STATE.Left) touchNumber = (int) TouchBehavior.NODE_POSITION_STATE.Right;
				else if (touchNumber > (int) TouchBehavior.NODE_POSITION_STATE.Right) 
					touchNumber = (int) TouchBehavior.NODE_POSITION_STATE.Left;
				touchStrategy.state = (TouchBehavior.NODE_POSITION_STATE) touchNumber;
			}
		}
	}
}
