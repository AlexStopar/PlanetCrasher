using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoonScript : MonoBehaviour {

	public float ORBIT_SPEED = 0.05f;
	public float BACK_ORBIT_SLOWDOWN = 0.5f; //slows moon's orbit behind planet
	public float ORBIT_RADIUS = 2.0f;
	public float CLAMP_RADIUS = 1.0f;
	public float MOON_SCALER = 0.995f;
	public float CURVE_TIP_OFFSET = 0.5f;
	float initX = 0.0f;
	float centerX = 0.0f;
	float yPosition = 0.0f;
	float zPosition = 0.0f;
	float curveTip = 0.0f;
	float orbitProgress = 0.0f;
	bool isBehindPlanet = false;
	bool isShrinking = false;
	bool isGrowing = false;
	bool isLeftOfCenter = true;
	Vector3 currentScale;
	void OnTriggerEnter2D (Collider2D asteroid) 
	{
		if(asteroid.gameObject == null || !asteroid.gameObject.name.Contains("Asteroid")) return;
		Destroy (asteroid.gameObject);
	}

	void Awake()
	{
		currentScale = gameObject.transform.localScale;
		initX = gameObject.transform.position.x;
		curveTip = gameObject.transform.position.y - CURVE_TIP_OFFSET;
		centerX = gameObject.transform.parent.position.x;
		if(initX > centerX) isLeftOfCenter = false;
		else isLeftOfCenter = true;
		orbitProgress = Mathf.Abs (centerX - initX);
	}

	void Start()
	{

	}

	void Update () 
	{
		float currentOrbitSpeed = ORBIT_SPEED;
		if(Mathf.Abs (centerX - initX) > ORBIT_RADIUS)
		{
			if(initX < centerX) gameObject.transform.Translate (Vector3.right * ORBIT_SPEED);
			else gameObject.transform.Translate (Vector3.left * ORBIT_SPEED);
		}
		if(isBehindPlanet) 
		{
			gameObject.transform.Translate (Vector3.right * ORBIT_SPEED * BACK_ORBIT_SLOWDOWN);
			currentOrbitSpeed = ORBIT_SPEED * BACK_ORBIT_SLOWDOWN;
		}
		else gameObject.transform.Translate (Vector3.left * ORBIT_SPEED);

		if(isLeftOfCenter) orbitProgress += currentOrbitSpeed;
		else orbitProgress -= currentOrbitSpeed;
		initX = gameObject.transform.position.x;

		if(initX < centerX) isLeftOfCenter = true;
		else if(initX > centerX) isLeftOfCenter = false;

		if(orbitProgress >= ORBIT_RADIUS) isBehindPlanet = true;
		else isBehindPlanet = false;

		if(orbitProgress > CLAMP_RADIUS && orbitProgress < (2.0f * CLAMP_RADIUS))
		{
			if(isLeftOfCenter) isShrinking = true;
			else isGrowing = true;
		}
		else 
		{
			isShrinking = false;
			isGrowing = false;
		}

		if (isShrinking) currentScale = currentScale * MOON_SCALER;
		else if (isGrowing) currentScale = currentScale / MOON_SCALER;
		gameObject.transform.localScale = currentScale;

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
