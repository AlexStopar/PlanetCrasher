using UnityEngine;
using System.Collections;

public class PlanetScript : MonoBehaviour {

	public bool isMoving = false;
	public float EXTERNAL_SIGHT_POINT = 6.0f;
	public float PLANETARY_SPEED = 0.1f;
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
		if(isMoving)
		{
			gameObject.transform.Translate(Vector3.left * PLANETARY_SPEED);
			if (gameObject.transform.position.x < -EXTERNAL_SIGHT_POINT) 
				gameObject.transform.position = new Vector3(EXTERNAL_SIGHT_POINT, 
				                                            gameObject.transform.position.y, gameObject.transform.position.z);
		}
	}
}
