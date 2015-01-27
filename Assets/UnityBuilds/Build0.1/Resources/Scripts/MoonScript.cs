using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoonScript : MonoBehaviour {

	public float ORBIT_SPEED = 0.05f;
	public float ORBIT_RADIUS = 2.0f;
	public float MOON_SCALER = 0.98f;
	public float CURVE_TIP_OFFSET = 0.5f;
	float initX = 0.0f;
	float centerX = 0.0f;
	float yPosition = 0.0f;
	float zPosition = 0.0f;
	float curveTip = 0.0f;
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
		curveTip = gameObject.transform.position.y - CURVE_TIP_OFFSET;
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

		if (isBehindPlanet) { //Set zPosition and yPosition of moon based on parabolic curve and position behind or in front of planet
			zPosition = gameObject.transform.parent.position.z + 1.0f;
			yPosition = -(CURVE_TIP_OFFSET/Mathf.Pow(ORBIT_RADIUS, 2.0f))* 
				Mathf.Pow(gameObject.transform.position.x, 2.0f) + (curveTip + (2.0f * CURVE_TIP_OFFSET));
		} else {
			zPosition = gameObject.transform.parent.position.z - 1.0f;
			yPosition = (CURVE_TIP_OFFSET/Mathf.Pow(ORBIT_RADIUS, 2.0f))* Mathf.Pow(gameObject.transform.position.x, 2.0f) + curveTip;
		}
		gameObject.transform.position = new Vector3 (gameObject.transform.position.x, yPosition, zPosition); 
	}
}
