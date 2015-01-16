using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoonScript : MonoBehaviour {

	public float ORBIT_SPEED = 0.05f;
	public float ORBIT_RADIUS = 2.0f;
	public float MOON_SCALER = 0.98f;

	float initX = 0.0f;
	float centerX = 0.0f;
	float zPosition = 0.0f;
	float orbitProgress = 0.0f;
	bool isBehindPlanet = false;
	bool isLeftOfCenter = true;
	Vector3 initialScale;
	void OnTriggerEnter2D (Collider2D asteroid) 
	{
		if(asteroid.gameObject == null || !asteroid.gameObject.name.Contains("Asteroid")) return;
		Destroy (asteroid.gameObject);
	}

	void Awake()
	{
		initialScale = gameObject.transform.localScale;
		initX = gameObject.transform.position.x;
		centerX = gameObject.transform.parent.position.x;
		if(initX > centerX) isLeftOfCenter = false;
		else isLeftOfCenter = true;
		orbitProgress = Mathf.Abs (centerX - initX);
		gameObject.transform.localScale = Mathf.Pow (MOON_SCALER, orbitProgress / ORBIT_SPEED) * initialScale;
	}

	void Start()
	{

	}

	void Update () 
	{
		if(Mathf.Abs (centerX - initX) > ORBIT_RADIUS)
		{
			if(initX < centerX) gameObject.transform.Translate (Vector3.right * ORBIT_SPEED);
			else gameObject.transform.Translate (Vector3.left * ORBIT_SPEED);
		}
		if(isBehindPlanet) gameObject.transform.Translate (Vector3.right * ORBIT_SPEED);
		else gameObject.transform.Translate (Vector3.left * ORBIT_SPEED);

		if(isLeftOfCenter) orbitProgress += ORBIT_SPEED;
		else orbitProgress -= ORBIT_SPEED;
		initX = gameObject.transform.position.x;
		gameObject.transform.localScale = Mathf.Pow (MOON_SCALER, orbitProgress / ORBIT_SPEED) * initialScale;

		if(initX < centerX) isLeftOfCenter = true;
		else if(initX > centerX) isLeftOfCenter = false;

		if(orbitProgress >= ORBIT_RADIUS) isBehindPlanet = true;
		else isBehindPlanet = false;

		if(isBehindPlanet) zPosition = gameObject.transform.parent.position.z + 1.0f;
		else zPosition = gameObject.transform.parent.position.z - 1.0f;
		gameObject.transform.position = new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y, zPosition); 
	}
}
