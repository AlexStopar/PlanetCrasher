using UnityEngine;
using System.Collections;

public class PlanetScript : MonoBehaviour {


	// Use this for initialization
	void Start () {

	}

	void OnTriggerEnter2D (Collider2D asteroid) 
	{
		if(asteroid.gameObject == null || !asteroid.gameObject.name.Contains("Asteroid")) return;
		Destroy (asteroid.gameObject);
	}
	// Update is called once per frame
	void Update () {

	}
}
