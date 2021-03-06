﻿using UnityEngine;
using System.Collections;

public class PlanetScript : MonoBehaviour {

	public bool isMoving = false;
	public float EXTERNAL_SIGHT_POINT = 6.0f;
	public float PLANETARY_SPEED = -0.1f;
	public float PLANET_PATH_CURVE = 0.09f;
	public float MAX_HP = 20.0f;
	public float CURRENT_HP;

	// Use this for initialization
	void Start () {
		CURRENT_HP = MAX_HP;
	}

	void OnTriggerEnter2D (Collider2D asteroid) 
	{
		if(asteroid.gameObject == null || !asteroid.gameObject.name.Contains("Asteroid")) return;
		AsteroidScript astroScript = asteroid.gameObject.GetComponentInParent<AsteroidScript> ();
		Asteroid damagingAsteroid = null;
		foreach(Asteroid astro in astroScript.asteroids)
		{
			if (astro.geom != null && astro.geom.name.Equals(asteroid.gameObject.name)) damagingAsteroid = astro;
		}
		CURRENT_HP -= damagingAsteroid.Damage;
		Destroy (asteroid.gameObject);
		if(CURRENT_HP < 0) Application.LoadLevel ("mainMenu");

	}
	// Update is called once per frame
	void Update () {
		if(isMoving)
		{
			Transform planetTransform = gameObject.transform;
			planetTransform.Translate(Vector3.left * PLANETARY_SPEED);
			if (planetTransform.position.x < -EXTERNAL_SIGHT_POINT) 
				planetTransform.position = new Vector3(EXTERNAL_SIGHT_POINT, planetTransform.position.y, planetTransform.position.z);
			else if(planetTransform.position.x > EXTERNAL_SIGHT_POINT)
				planetTransform.position = new Vector3(-EXTERNAL_SIGHT_POINT, planetTransform.position.y, planetTransform.position.z);

			planetTransform.position = new Vector3(planetTransform.position.x, PLANET_PATH_CURVE*
			                                      Mathf.Pow(planetTransform.position.x, 2.0f),
			                                      planetTransform.position.z);
		}
	}
}
