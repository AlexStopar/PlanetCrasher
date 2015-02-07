using UnityEngine;
using System.Collections;

public class GravityFieldScript : MonoBehaviour {

	public float GRAVITY_FORCE_MULTIPLIER = 25.0f;
	AsteroidScript aScript;
	// Use this for initialization
	void Start () {
		aScript = GameObject.Find ("AsteroidMaker").GetComponent<AsteroidScript> ();
	}

	// Update is called once per frame
	void Update () {
		foreach(Asteroid asteroid in aScript.asteroids)
		{
			if(asteroid.geom == null || asteroid.state != Asteroid.ASTEROID_STATE.Shot) continue;
			Transform astroTransform = asteroid.geom.transform;
			Transform planetTransform = this.GetComponentInParent<Transform>();
			if(Vector2.Distance(new Vector2(astroTransform.position.x, astroTransform.position.y), 
			                    new Vector2(planetTransform.position.x, planetTransform.position.y)) 
			   < renderer.bounds.extents.magnitude)
			{
				asteroid.geom.rigidbody2D.AddForce(
					new Vector2(planetTransform.position.x - astroTransform.position.x, planetTransform.position.y - astroTransform.position.y) 
					* GRAVITY_FORCE_MULTIPLIER * Time.deltaTime);
			}
		}
	}
}
