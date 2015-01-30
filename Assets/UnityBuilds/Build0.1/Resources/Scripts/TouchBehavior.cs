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
	public int currentAsteroid;
	public int currentAsteroid2;
	public bool isDualTouch;
	public bool isPlayerOne;

	public TouchBehavior(float asteroidGrabLimit, float asteroidShootSpeed, float asteroidRadius, bool isDual, bool isFirstPlayer)
	{
		grabLimit = asteroidGrabLimit;
		radius = asteroidRadius;
		shootSpeed = asteroidShootSpeed;
		isGrabbing1 = false;
		isGrabbing2 = false;
		currentAsteroid = -1;
		currentAsteroid2 = -1;
		isDualTouch = isDual; 
		isPlayerOne = isFirstPlayer;
	}

	public abstract List<Asteroid> ResolveTouches (List<Asteroid> asteroids, Camera camera);
}

public class FlickBehavior : TouchBehavior
{
	public FlickBehavior (float asteroidGrabLimit, float asteroidShootSpeed, 
	                      float asteroidRadius, bool isDual, bool isFirstPlayer) : base(asteroidGrabLimit, 
	                                                              asteroidShootSpeed, asteroidRadius, isDual, isFirstPlayer)
	{
		base.isFling = true;
	}

	public override List<Asteroid> ResolveTouches(List<Asteroid> asteroids, Camera camera)
	{
		if (Input.touchCount >= 1)
		{
			for(int i = 0; i < Input.touchCount; i++)
			{
				Vector3 touchPoint = camera.ScreenToWorldPoint(Input.GetTouch(i).position);
				if(((isPlayerOne && touchPoint.y < grabLimit) || (!isPlayerOne && touchPoint.y > grabLimit)) && Input.GetTouch(i).fingerId < 2)
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
								if(Input.GetTouch(i).fingerId == 0 && !isGrabbing1) currentAsteroid = asteroids.IndexOf(asteroid);
								else if(Input.GetTouch(i).fingerId == 1 && !isGrabbing2 && isDualTouch) currentAsteroid2 = asteroids.IndexOf(asteroid);
							}
							asteroid.geom.transform.position = new Vector3(asteroid.geom.transform.position.x, 
							                                               asteroid.geom.transform.position.y, asteroid.geom.transform.parent.position.z);
						}
					}
					if(currentAsteroid >= 0 && Input.GetTouch(i).fingerId == 0 && asteroids[currentAsteroid].geom != null)
					{
						isGrabbing1 = true;
						asteroids[currentAsteroid].isTouched = true;
						float formerZ = asteroids[currentAsteroid].geom.transform.parent.position.z - 1.0f;
						asteroids[currentAsteroid].geom.transform.position = 
							new Vector3(touchPoint.x, touchPoint.y, formerZ);
						Vector2 asteroidPosition = new Vector2(touchPoint.x, touchPoint.y);
						asteroids[currentAsteroid].geom.rigidbody2D.AddForce(shootSpeed * 
						                                                     ( Input.GetTouch(i).deltaPosition) - asteroidPosition);
						asteroids[currentAsteroid].GrabExpand();
					}
					if(currentAsteroid2 >= 0 && Input.GetTouch(i).fingerId == 1 && asteroids[currentAsteroid2].geom != null && isDualTouch)
					{
						isGrabbing2 = true;
						asteroids[currentAsteroid2].isTouched = true;
						float formerZ = asteroids[currentAsteroid2].geom.transform.parent.position.z - 1.0f;
						asteroids[currentAsteroid2].geom.transform.position = 
							new Vector3(touchPoint.x, touchPoint.y, formerZ);
						Vector2 asteroidPosition = new Vector2(touchPoint.x, touchPoint.y);
						asteroids[currentAsteroid2].geom.rigidbody2D.AddForce(shootSpeed * 
						                                                      ( Input.GetTouch(i).deltaPosition) - asteroidPosition);
						asteroids[currentAsteroid2].GrabExpand();
					}
				}
			}
		}
		else 
		{
			isGrabbing1 = false;
			isGrabbing2 = false;
		}
		return asteroids;
	}
}

public class SlingBehavior : TouchBehavior 
{
	GameObject node1;
	GameObject node2;
	GameObject boltHolder1;
	GameObject boltHolder2;
	float BOLT_NODE_OFFSET = 1.4f;
	float NODE_LOCATION = -3.0f;
	float DUAL_NODE_OFFSET_Y = -4.0f;
	float DUAL_BOLT_OFFSET_X = 0.8f;
	float DUAL_NODE_OFFSET_X = 1.0f;
	float DUAL_NODE_BEND = 30.0f;
	float NODE_SCALE = 0.2f;
	Vector3 touchPoint;
	Vector3 touchPoint2;
	public SlingBehavior (float asteroidGrabLimit, float asteroidShootSpeed, 
	                      float asteroidRadius, bool isDual, bool isFirstPlayer) : base(asteroidGrabLimit, 
	                                                              asteroidShootSpeed, asteroidRadius, isDual, isFirstPlayer)
	{
		node1 = new GameObject ();
		SetNode(node1, "Textures/slingergyNode", "Node1");
		base.isFling = false;
		if(isDual) 
		{
			node1.transform.Translate (new Vector3 (-DUAL_NODE_OFFSET_X, DUAL_NODE_OFFSET_Y, 0.0f));
			node1.transform.Rotate(new Vector3(0, 0, DUAL_NODE_BEND));
			node2 = new GameObject ();
			SetNode(node2, "Textures/slingergyNode", "Node2");
			node2.transform.Translate (new Vector3 (DUAL_NODE_OFFSET_X, DUAL_NODE_OFFSET_Y, 0.0f));
			node2.transform.Rotate(new Vector3(0, 0, -DUAL_NODE_BEND));
			boltHolder2 = new GameObject();
			boltHolder2.name = "BoltHolder2";
			boltHolder2.transform.parent = GameObject.Find ("Node2").transform;
			boltHolder2.AddComponent<DemoScript> ();
			boltHolder2.transform.localPosition = Vector3.forward;
		}
		else node1.transform.Translate (new Vector3 (0.0f, NODE_LOCATION, 0.0f));
		boltHolder1 = new GameObject ();
		boltHolder1.name = "BoltHolder1";
		boltHolder1.transform.parent = GameObject.Find ("Node1").transform;
		boltHolder1.AddComponent<DemoScript> ();
		boltHolder1.transform.localPosition = Vector3.forward;
	}
	void SetNode(GameObject geom, string path, string name)
	{
		geom.transform.parent = GameObject.Find ("TopLayer").transform;
		Sprite sprite = Resources.Load<Sprite>(path) as Sprite;
		if(sprite != null && geom.GetComponent<SpriteRenderer> () == null)
		{
			geom.AddComponent<SpriteRenderer> ();
			geom.GetComponent<SpriteRenderer> ().sprite = sprite;
		}
		geom.transform.localScale = new Vector3(NODE_SCALE, NODE_SCALE, NODE_SCALE);
		geom.name = name;
		geom.transform.localPosition = Vector3.zero;
	}
	public override List<Asteroid> ResolveTouches(List<Asteroid> asteroids, Camera camera)
	{
		if (Input.touchCount >= 1)
		{

			int i = 0;
			touchPoint = camera.ScreenToWorldPoint(Input.GetTouch(i).position);
			while (i < Input.touchCount && Input.GetTouch(i).fingerId != 0 && touchPoint.y > grabLimit)
			{
				i++;
				if(i < Input.touchCount) touchPoint = camera.ScreenToWorldPoint(Input.GetTouch(i).position);
			}
			if(i < Input.touchCount) isGrabbing1 = true;

			if(isDualTouch)
			{
				i = 0;
				touchPoint2 = camera.ScreenToWorldPoint(Input.GetTouch(i).position);
				while (i < Input.touchCount && Input.GetTouch(i).fingerId != 1 
				       && touchPoint2.y > grabLimit)
				{
					i++;
					if(i < Input.touchCount) touchPoint2 = camera.ScreenToWorldPoint(Input.GetTouch(i).position);
				}
				if(i < Input.touchCount) isGrabbing2 = true;
			}

			if(isGrabbing1 && touchPoint.y < grabLimit)
			{
				float minDistance = 10.0f;
				if(currentAsteroid < 0)
				{
					foreach(Asteroid asteroid in asteroids)
					{
						if(asteroid.geom == null || asteroid.geom.transform.position.y > grabLimit) continue;
						float distance1 = Mathf.Abs(Vector2.Distance(new Vector2(
							asteroid.geom.transform.position.x, asteroid.geom.transform.position.y), new Vector2(touchPoint.x, touchPoint.y)));
						if(distance1 < minDistance) 
						{
							minDistance = distance1;
							i = 0;
							while (i < Input.touchCount)
							{
								if(Input.GetTouch(i).fingerId == 0 && 
								   asteroids.IndexOf(asteroid) != currentAsteroid2) currentAsteroid = asteroids.IndexOf(asteroid);
								i++;
							}
								
						}
						asteroid.geom.transform.position = new Vector3(asteroid.geom.transform.position.x, 
						                                               asteroid.geom.transform.position.y, asteroid.geom.transform.parent.position.z);
					}
				}
			
				if(currentAsteroid >= 0 && asteroids[currentAsteroid].geom != null)
				{
					boltHolder1.GetComponent<DemoScript>().isOff = false;
					boltHolder1.GetComponent<DemoScript>().startPos = node1.transform.position + new Vector3(0, BOLT_NODE_OFFSET, 0);
					if(isDualTouch) 
					{
						boltHolder1.GetComponent<DemoScript>().startPos += new Vector2(-DUAL_BOLT_OFFSET_X, 0);

					}
					boltHolder1.GetComponent<DemoScript>().endPos = asteroids[currentAsteroid].geom.transform.position;
					asteroids[currentAsteroid].isTouched = true;
					float formerZ = asteroids[currentAsteroid].geom.transform.parent.position.z - 1.0f;
					i = 0;
					while (i < Input.touchCount)
					{
						if(i < Input.touchCount && Input.GetTouch(i).fingerId == 0) touchPoint = camera.ScreenToWorldPoint(Input.GetTouch(i).position);
						i++;
					}

					asteroids[currentAsteroid].geom.transform.position = 
						new Vector3(touchPoint.x, touchPoint.y, formerZ);
					Vector2 asteroidPosition = new Vector2(touchPoint.x, touchPoint.y);
					asteroids[currentAsteroid].GrabExpand();
					i = 0;
					while (i < Input.touchCount && Input.GetTouch(i).fingerId != 0)
					{
						i++;
					}
					if(i < Input.touchCount && (Input.GetTouch(i).phase == TouchPhase.Ended || touchPoint.y > grabLimit)) 
					{
						isGrabbing1 = false;
						asteroids[currentAsteroid].geom.rigidbody2D.AddForce(shootSpeed *(
							boltHolder1.GetComponent<DemoScript>().startPos  - 
							boltHolder1.GetComponent<DemoScript>().endPos));
						boltHolder1.GetComponent<DemoScript>().isOff = true;
						currentAsteroid = -1;
					}
				}

			}
			else 
			{
				boltHolder1.GetComponent<DemoScript>().isOff = true;
				isGrabbing1 = false;
			}
			if(isGrabbing2 &&  touchPoint2.y < grabLimit)
			{
				float minDistance2 = 10.0f;

				if(currentAsteroid2 < 0)
				{
					foreach(Asteroid asteroid in asteroids)
					{
						if(asteroid.geom == null || asteroid.geom.transform.position.y > grabLimit) continue;
						float distance2 = Mathf.Abs(Vector2.Distance(new Vector2(
							asteroid.geom.transform.position.x, asteroid.geom.transform.position.y), new Vector2(touchPoint2.x, touchPoint2.y)));
						if(distance2 < minDistance2) 
						{
							minDistance2 = distance2;
							i = 0;
							while (i < Input.touchCount)
							{
								if(i < Input.touchCount && Input.GetTouch(i).fingerId == 1 && 
								   asteroids.IndexOf(asteroid) != currentAsteroid) currentAsteroid2 = asteroids.IndexOf(asteroid);
								i++;
							}
						}
						asteroid.geom.transform.position = new Vector3(asteroid.geom.transform.position.x, 
						                                               asteroid.geom.transform.position.y, asteroid.geom.transform.parent.position.z);
					}
				}

				if(currentAsteroid2 >= 0 && asteroids[currentAsteroid2].geom != null)
				{
					isGrabbing2 = true;
					boltHolder2.GetComponent<DemoScript>().isOff = false;
					boltHolder2.GetComponent<DemoScript>().startPos = 
						node2.transform.position + new Vector3(DUAL_BOLT_OFFSET_X, BOLT_NODE_OFFSET, 0);
					boltHolder2.GetComponent<DemoScript>().endPos = asteroids[currentAsteroid2].geom.transform.position;
					asteroids[currentAsteroid2].isTouched = true;
					float formerZ = asteroids[currentAsteroid2].geom.transform.parent.position.z - 1.0f;
					i = 0;
					while (i < Input.touchCount)
					{
						if(i < Input.touchCount && Input.GetTouch(i).fingerId == 1) touchPoint2 = camera.ScreenToWorldPoint(Input.GetTouch(i).position);
						i++;
					}
					asteroids[currentAsteroid2].geom.transform.position = 
						new Vector3(touchPoint2.x, touchPoint2.y, formerZ);
					Vector2 asteroidPosition = new Vector2(touchPoint2.x, touchPoint2.y);
					asteroids[currentAsteroid2].GrabExpand();
					i = 0;
					while (i < Input.touchCount && Input.GetTouch(i).fingerId != 1)
					{
						i++;
					}
					if(i < Input.touchCount && (Input.GetTouch(i).phase == TouchPhase.Ended || touchPoint2.y > grabLimit)) 
					{
						asteroids[currentAsteroid2].geom.rigidbody2D.AddForce(shootSpeed *(
							boltHolder2.GetComponent<DemoScript>().startPos  - 
							boltHolder2.GetComponent<DemoScript>().endPos));
						isGrabbing2 = false;
						boltHolder2.GetComponent<DemoScript>().isOff = true;
						currentAsteroid2 = -1;
					}
				}
			}
			else 
			{
				if(isDualTouch) boltHolder2.GetComponent<DemoScript>().isOff = true;
				isGrabbing2 = false;
			}
		}
		else 
		{
			isGrabbing1 = false;
			boltHolder1.GetComponent<DemoScript>().isOff = true;
			isGrabbing2 = false;
			if(isDualTouch) boltHolder2.GetComponent<DemoScript>().isOff = true;
		}
		return asteroids;
	}

}
