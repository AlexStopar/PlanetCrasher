using UnityEngine;
using System.Collections;

public class PlanetScript : MonoBehaviour {

	public float GRAVITY_RADIUS = 3.0f;
	public float GRAVITY_FORCE_MULTIPLIER = 25.0f;
	AsteroidScript aScript;
	// Use this for initialization
	void Start () {
		int height = Screen.height;
		aScript = GameObject.Find ("AsteroidMaker").GetComponent<AsteroidScript> ();
	}

	void OnTriggerEnter2D (Collider2D asteroid) 
	{
		if(asteroid.gameObject == null || !asteroid.gameObject.name.Contains("Asteroid")) return;
		Destroy (asteroid.gameObject);
	}
	// Update is called once per frame
	void Update () {
		foreach(Asteroid asteroid in aScript.asteroids)
		{
			if(asteroid.geom == null) continue;
			Transform astroTransform = asteroid.geom.transform;
			Transform planetTransform = this.GetComponentInParent<Transform>();
			if(Vector2.Distance(new Vector2(astroTransform.position.x, astroTransform.position.y), 
			                    new Vector2(planetTransform.position.x, planetTransform.position.y)) < GRAVITY_RADIUS)
			{
				asteroid.geom.rigidbody2D.AddForce(
					new Vector2(planetTransform.position.x - astroTransform.position.x, planetTransform.position.y - astroTransform.position.y) 
					* GRAVITY_FORCE_MULTIPLIER * Time.deltaTime);
			}
		}
	}
}
