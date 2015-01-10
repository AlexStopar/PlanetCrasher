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
	GameObject node3;
	GameObject boltHolder1;
	float BOLT_NODE_OFFSET = 0.7f;
	float NODE_SCALE = 0.1f;
	public SlingBehavior (float asteroidGrabLimit, float asteroidShootSpeed, 
	                      float asteroidRadius, bool isDual) : base(asteroidGrabLimit, asteroidShootSpeed, asteroidRadius, isDual)
	{
		node1 = new GameObject ();
		node2 = new GameObject ();
		Camera camera = GameObject.Find("Main Camera").camera;
		SetNode(node1, "Textures/slingergyNode", "Node1");
		SetNode(node2, "Textures/slingergyNode", "Node2");
		node1.transform.position = camera.ScreenToWorldPoint (Vector3.zero);
		node1.transform.Translate (new Vector3 (0.2f, 0.5f, 2.0f));
		node2.transform.position = new Vector3 (-node1.transform.position.x, node1.transform.position.y, node1.transform.position.z);
		base.isFling = false;
		boltHolder1 = new GameObject ();
		boltHolder1.name = "BoltHolder1";
		boltHolder1.transform.parent = GameObject.Find ("TopLayer").transform;
		boltHolder1.transform.Translate (7.0f * Vector3.back);
		boltHolder1.AddComponent<DemoScript> ();
		boltHolder1.GetComponent<DemoScript> ().startPos = new Vector2 (node1.transform.position.x, node1.transform.position.y + BOLT_NODE_OFFSET);
		boltHolder1.GetComponent<DemoScript> ().endPos = new Vector2 (node2.transform.position.x, node2.transform.position.y + BOLT_NODE_OFFSET);

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
	}
	public override List<Asteroid> ResolveTouches(List<Asteroid> asteroids, Camera camera)
	{
		return asteroids;
	}
}
