using UnityEngine;
using System.Collections;
using System.Collections.Generic;

abstract public class TouchBehavior {

	// Use this for initialization
	public bool isFling = true;
	public float radius;
	public float shootSpeed;
	public float grabLimit;
	public bool isGrabbing1;
	public bool isGrabbing2;
	public int grabID1;
	public int grabID2;
	public enum NODE_POSITION_STATE {Left, Center, Right, Total};
	public NODE_POSITION_STATE state;
	public int currentAsteroid;
	public int currentAsteroid2;
	public bool isDualTouch;
	public bool isPlayerOne;

	public TouchBehavior(float asteroidGrabLimit, float asteroidShootSpeed, float asteroidRadius, bool isDual, bool isFirstPlayer)
	{
		grabLimit = asteroidGrabLimit;
		radius = asteroidRadius;
		shootSpeed = asteroidShootSpeed;
		state = NODE_POSITION_STATE.Right;
		isGrabbing1 = false;
		isGrabbing2 = false;
		grabID1 = -1;
		grabID2 = -2;
		currentAsteroid = -1;
		currentAsteroid2 = -1;
		isDualTouch = isDual; 
		isPlayerOne = isFirstPlayer;
	}

	public abstract List<Asteroid> ResolveTouches (List<Asteroid> asteroids, Camera camera);
	public abstract void UpdatePosition (Camera camera);
}

public class FlickBehavior : TouchBehavior
{
	public FlickBehavior (float asteroidGrabLimit, float asteroidShootSpeed, 
	                      float asteroidRadius, bool isDual, bool isFirstPlayer) : base(asteroidGrabLimit, 
	                                                              asteroidShootSpeed, asteroidRadius, isDual, isFirstPlayer)
	{
		base.isFling = true;
	}

	public override void UpdatePosition(Camera camera) {
		return;
	}

	public override List<Asteroid> ResolveTouches(List<Asteroid> asteroids, Camera camera)
	{
		if (Input.touchCount >= 1)
		{
			for(int i = 0; i < Input.touchCount; i++)
			{
				Vector3 touchPoint = camera.ScreenToWorldPoint(Input.GetTouch(i).position);
				if(((isPlayerOne && touchPoint.y < grabLimit) || (!isPlayerOne && touchPoint.y > grabLimit)))
				{
					float minDistance = 10.0f;
					
					if(!isGrabbing1 || !isGrabbing2)
					{
						foreach(Asteroid asteroid in asteroids)
						{
							if(asteroid.geom == null || (isPlayerOne && asteroid.geom.transform.position.y > grabLimit) 
							   || (!isPlayerOne && asteroid.geom.transform.position.y < grabLimit)) continue;
							float distance = Mathf.Abs(Vector2.Distance(new Vector2(
								asteroid.geom.transform.position.x, asteroid.geom.transform.position.y), new Vector2(touchPoint.x, touchPoint.y)));
							if(distance < minDistance) 
							{
								minDistance = distance;
								if(!isGrabbing1) 
								{
									currentAsteroid = asteroids.IndexOf(asteroid);
									grabID1 =  Input.GetTouch(i).fingerId;
								}
								else if(Input.GetTouch(i).fingerId != grabID1 && !isGrabbing2 && isDualTouch) 
								{
									currentAsteroid2 = asteroids.IndexOf(asteroid);
									grabID2 = Input.GetTouch(i).fingerId;
								}
							}
							asteroid.geom.transform.position = new Vector3(asteroid.geom.transform.position.x, 
							                                               asteroid.geom.transform.position.y, asteroid.geom.transform.parent.position.z);
						}
					}
					if(currentAsteroid >= 0 && Input.GetTouch(i).fingerId == grabID1 && asteroids[currentAsteroid].geom != null)
					{
						isGrabbing1 = true;
						asteroids[currentAsteroid].state = Asteroid.ASTEROID_STATE.Grabbed;
						float formerZ = asteroids[currentAsteroid].geom.transform.parent.position.z - 1.0f;
						asteroids[currentAsteroid].geom.transform.position = 
							new Vector3(touchPoint.x, touchPoint.y, formerZ);
						Vector2 asteroidPosition = new Vector2(touchPoint.x, touchPoint.y);
						asteroids[currentAsteroid].geom.rigidbody2D.AddForce(shootSpeed * 
						                                                     ( Input.GetTouch(i).deltaPosition) - asteroidPosition);
						asteroids[currentAsteroid].GrabExpand();
					}
					else if (currentAsteroid >= 0 && asteroids[currentAsteroid].geom != null) asteroids[currentAsteroid].state = Asteroid.ASTEROID_STATE.Shot;
					if(currentAsteroid2 >= 0 && Input.GetTouch(i).fingerId == grabID2 && asteroids[currentAsteroid2].geom != null && isDualTouch)
					{
						isGrabbing2 = true;
						asteroids[currentAsteroid2].state = Asteroid.ASTEROID_STATE.Grabbed;
						float formerZ = asteroids[currentAsteroid2].geom.transform.parent.position.z - 1.0f;
						asteroids[currentAsteroid2].geom.transform.position = 
							new Vector3(touchPoint.x, touchPoint.y, formerZ);
						Vector2 asteroidPosition = new Vector2(touchPoint.x, touchPoint.y);
						asteroids[currentAsteroid2].geom.rigidbody2D.AddForce(shootSpeed * 
						                                                      ( Input.GetTouch(i).deltaPosition) - asteroidPosition);
						asteroids[currentAsteroid2].GrabExpand();
					}
					else if (currentAsteroid2 >= 0 && asteroids[currentAsteroid2].geom != null) asteroids[currentAsteroid2].state = Asteroid.ASTEROID_STATE.Shot;
				}
			}
		}
		else 
		{
			isGrabbing1 = false;
			if (currentAsteroid >= 0 && asteroids[currentAsteroid].geom != null) asteroids[currentAsteroid].state = Asteroid.ASTEROID_STATE.Shot;
			isGrabbing2 = false;
			if (currentAsteroid2 >= 0 && asteroids[currentAsteroid2].geom != null) asteroids[currentAsteroid2].state = Asteroid.ASTEROID_STATE.Shot;
		}
		return asteroids;
	}
}

