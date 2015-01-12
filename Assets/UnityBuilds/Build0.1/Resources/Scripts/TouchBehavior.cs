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

	public TouchBehavior(float asteroidGrabLimit, float asteroidShootSpeed, float asteroidRadius, bool isDual)
	{
		grabLimit = asteroidGrabLimit;
		radius = asteroidRadius;
		shootSpeed = asteroidShootSpeed;
		isGrabbing1 = false;
		isGrabbing2 = false;
		currentAsteroid = -1;
		currentAsteroid2 = -1;
		isDualTouch = isDual;
	}

	public abstract List<Asteroid> ResolveTouches (List<Asteroid> asteroids, Camera camera);
}

public class FlickBehavior : TouchBehavior
{
	public FlickBehavior (float asteroidGrabLimit, float asteroidShootSpeed, 
	                      float asteroidRadius, bool isDual) : base(asteroidGrabLimit, asteroidShootSpeed, asteroidRadius, isDual)
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
				if(touchPoint.y < grabLimit && Input.GetTouch(i).fingerId < 2)
				{
					float minDistance = 10.0f;
					
					if(!isGrabbing1 || !isGrabbing2)
					{
						foreach(Asteroid asteroid in asteroids)
						{
							if(asteroid.geom == null || asteroid.geom.transform.position.y > grabLimit) continue;
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
							new Vector3(touchPoint.x, touchPoint.y + Asteroid.RADIUS, formerZ);
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
							new Vector3(touchPoint.x, touchPoint.y + Asteroid.RADIUS, formerZ);
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
	float BOLT_NODE_OFFSET = 1.4f;
	float NODE_LOCATION = -3.0f;
	float DUAL_NODE_OFFSET_Y = -4.0f;
	float DUAL_NODE_OFFSET_X = 1.0f;
	float DUAL_NODE_BEND = 30.0f;
	float NODE_SCALE = 0.2f;
	int firstFingerTouch = -1;
	Vector3 touchPoint;
	public SlingBehavior (float asteroidGrabLimit, float asteroidShootSpeed, 
	                      float asteroidRadius, bool isDual) : base(asteroidGrabLimit, asteroidShootSpeed, asteroidRadius, isDual)
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
			if(firstFingerTouch < 0)
			{
				firstFingerTouch = 0;
				touchPoint = camera.ScreenToWorldPoint(Input.GetTouch(firstFingerTouch).position);
				while (firstFingerTouch < Input.touchCount && Input.GetTouch(firstFingerTouch).fingerId != 0 && touchPoint.y > grabLimit)
				{
					firstFingerTouch++;
					touchPoint = camera.ScreenToWorldPoint(Input.GetTouch(firstFingerTouch).position);
				}
			}


			if(firstFingerTouch < Input.touchCount &&  touchPoint.y < grabLimit)
			{
				float minDistance = 10.0f;

				if(!isGrabbing1 || !isGrabbing2)
				{
					foreach(Asteroid asteroid in asteroids)
					{
						if(asteroid.geom == null || asteroid.geom.transform.position.y > grabLimit) continue;
						float distance = Mathf.Abs(Vector2.Distance(new Vector2(
							asteroid.geom.transform.position.x, asteroid.geom.transform.position.y), new Vector2(touchPoint.x, touchPoint.y)));
						if(distance < minDistance) 
						{
							minDistance = distance;
							if(Input.GetTouch(firstFingerTouch).fingerId == 0 && !isGrabbing1) currentAsteroid = asteroids.IndexOf(asteroid);
							else if(Input.GetTouch(firstFingerTouch).fingerId == 1 && !isGrabbing2 && isDualTouch) currentAsteroid2 = asteroids.IndexOf(asteroid);
						}
						asteroid.geom.transform.position = new Vector3(asteroid.geom.transform.position.x, 
						                                               asteroid.geom.transform.position.y, asteroid.geom.transform.parent.position.z);
					}
				}
				if(currentAsteroid >= 0 && Input.GetTouch(firstFingerTouch).fingerId == 0 && asteroids[currentAsteroid].geom != null)
				{
					isGrabbing1 = true;
					boltHolder1.GetComponent<DemoScript>().isOff = false;
					boltHolder1.GetComponent<DemoScript>().startPos = node1.transform.position + new Vector3(0, BOLT_NODE_OFFSET, 0);
					boltHolder1.GetComponent<DemoScript>().endPos = asteroids[currentAsteroid].geom.transform.position;
					asteroids[currentAsteroid].isTouched = true;
					float formerZ = asteroids[currentAsteroid].geom.transform.parent.position.z - 1.0f;
					touchPoint = camera.ScreenToWorldPoint(Input.GetTouch(firstFingerTouch).position);
					asteroids[currentAsteroid].geom.transform.position = 
						new Vector3(touchPoint.x, touchPoint.y + Asteroid.RADIUS, formerZ);
					Vector2 asteroidPosition = new Vector2(touchPoint.x, touchPoint.y);
					asteroids[currentAsteroid].GrabExpand();
					if(Input.GetTouch(firstFingerTouch).phase == TouchPhase.Ended || touchPoint.y > grabLimit) 
					{
						asteroids[currentAsteroid].geom.rigidbody2D.AddForce(shootSpeed *(
							boltHolder1.GetComponent<DemoScript>().startPos  - 
							boltHolder1.GetComponent<DemoScript>().endPos));
						isGrabbing1 = false;
						boltHolder1.GetComponent<DemoScript>().isOff = true;
						firstFingerTouch = -1;
					}
				}
			}
			else firstFingerTouch = -1;
		}
		else 
		{
			isGrabbing1 = false;
			isGrabbing2 = false;
		}
		return asteroids;
	}

}
