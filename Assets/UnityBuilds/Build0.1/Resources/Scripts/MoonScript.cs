using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoonScript : MonoBehaviour {
	
	void OnTriggerEnter2D (Collider2D asteroid) 
	{
		if(asteroid.gameObject == null || !asteroid.gameObject.name.Contains("Asteroid")) return;
		Destroy (asteroid.gameObject);
	}
}
