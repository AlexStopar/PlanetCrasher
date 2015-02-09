using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		if(isFirstPlayer) SetNode(node1, "Textures/slingergyNode", "Node1");
		else SetNode(node1, "Textures/slingergyNode", "Node2P1");
		base.isFling = false;
		if(isDual) 
		{
			if(isFirstPlayer)
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
			}
			else 
			{
				node1.transform.Translate (new Vector3 (-DUAL_NODE_OFFSET_X, -DUAL_NODE_OFFSET_Y, 0.0f));
				node1.transform.Rotate(new Vector3(0, 0, 180.0f - DUAL_NODE_BEND));
				node2 = new GameObject ();
				SetNode(node2, "Textures/slingergyNode", "Node2P2");
				node2.transform.Translate (new Vector3 (DUAL_NODE_OFFSET_X, -DUAL_NODE_OFFSET_Y, 0.0f));
				node2.transform.Rotate(new Vector3(0, 0, 180.0f + DUAL_NODE_BEND));
				boltHolder2 = new GameObject();
				boltHolder2.name = "BoltHolder2P2";
				boltHolder2.transform.parent = GameObject.Find ("Node2P2").transform;
			}
			boltHolder2.AddComponent<DemoScript> ();
			boltHolder2.transform.localPosition = Vector3.forward;
		}
		else 
		{
			if(isFirstPlayer) node1.transform.Translate (new Vector3 (0.0f, NODE_LOCATION, 0.0f));
			else 
			{
				node1.transform.Translate (new Vector3 (0.0f, -NODE_LOCATION, 0.0f));
				node1.transform.Rotate(new Vector3(0, 0, 180.0f));
			}
		}
		boltHolder1 = new GameObject ();
		if (isFirstPlayer) {
			boltHolder1.name = "BoltHolder1";
			boltHolder1.transform.parent = GameObject.Find ("Node1").transform;
		} else {
			boltHolder1.name = "BoltHolder2P1";
			boltHolder1.transform.parent = GameObject.Find ("Node2P1").transform;
		}
		
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
			while (i < Input.touchCount && ((touchPoint.y > grabLimit && isPlayerOne) || (touchPoint.y < grabLimit && !isPlayerOne) 
			                                || (!isDualTouch || touchPoint.x > 0) || Input.GetTouch(i).fingerId == grabID2))
			{
				i++;
				if(i < Input.touchCount) touchPoint = camera.ScreenToWorldPoint(Input.GetTouch(i).position);
			}
			if(i < Input.touchCount) 
			{
				isGrabbing1 = true;
				grabID1 = Input.GetTouch(i).fingerId;
			}
			
			if(isDualTouch)
			{
				i = 0;
				touchPoint2 = camera.ScreenToWorldPoint(Input.GetTouch(i).position);
				while (i < Input.touchCount && ((touchPoint2.y > grabLimit && isPlayerOne) 
				                                || (touchPoint2.y < grabLimit && !isPlayerOne) || touchPoint2.x < 0 || Input.GetTouch(i).fingerId == grabID1))
				{
					i++;
					if(i < Input.touchCount) touchPoint2 = camera.ScreenToWorldPoint(Input.GetTouch(i).position);
				}
				if(i < Input.touchCount) 
				{
					isGrabbing2 = true;
					grabID2 = Input.GetTouch(i).fingerId;
				}
			}
			
			if(isGrabbing1 && (touchPoint.y < grabLimit && isPlayerOne) || (touchPoint.y > grabLimit && !isPlayerOne))
			{
				float minDistance = 10.0f;
				if(currentAsteroid < 0)
				{
					foreach(Asteroid asteroid in asteroids)
					{
						if(asteroid.geom == null || (asteroid.geom.transform.position.y > grabLimit && isPlayerOne) 
						   || (asteroid.geom.transform.position.y < grabLimit && !isPlayerOne)) continue;
						float distance1 = Mathf.Abs(Vector2.Distance(new Vector2(
							asteroid.geom.transform.position.x, asteroid.geom.transform.position.y), new Vector2(touchPoint.x, touchPoint.y)));
						if(distance1 < minDistance) 
						{
							minDistance = distance1;
							i = 0;
							while (i < Input.touchCount)
							{
								if(Input.GetTouch(i).fingerId == grabID1 && 
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
					if(isPlayerOne) boltHolder1.GetComponent<DemoScript>().startPos = node1.transform.position + new Vector3(0, BOLT_NODE_OFFSET, 0);
					else boltHolder1.GetComponent<DemoScript>().startPos = node1.transform.position + new Vector3(0, -BOLT_NODE_OFFSET, 0);
					if(isDualTouch) 
					{
						boltHolder1.GetComponent<DemoScript>().startPos += new Vector2(-DUAL_BOLT_OFFSET_X, 0);
						
					}
					boltHolder1.GetComponent<DemoScript>().endPos = asteroids[currentAsteroid].geom.transform.position;
					asteroids[currentAsteroid].state = Asteroid.ASTEROID_STATE.Grabbed;
					float formerZ = asteroids[currentAsteroid].geom.transform.parent.position.z - 1.0f;
					i = 0;
					while (i < Input.touchCount)
					{
						if(i < Input.touchCount && Input.GetTouch(i).fingerId == grabID1) touchPoint = camera.ScreenToWorldPoint(Input.GetTouch(i).position);
						i++;
					}
					
					asteroids[currentAsteroid].geom.transform.position = 
						new Vector3(touchPoint.x, touchPoint.y, formerZ);
					Vector2 asteroidPosition = new Vector2(touchPoint.x, touchPoint.y);
					asteroids[currentAsteroid].GrabExpand();
					i = 0;
					while (i < Input.touchCount && Input.GetTouch(i).fingerId != grabID1)
					{
						i++;
					}
					if(i < Input.touchCount && (Input.GetTouch(i).phase == TouchPhase.Ended || 
					                            (touchPoint.y > grabLimit && isPlayerOne) || (touchPoint.y < grabLimit && !isPlayerOne))) 
					{
						isGrabbing1 = false;
						asteroids[currentAsteroid].state = Asteroid.ASTEROID_STATE.Shot;
						asteroids[currentAsteroid].geom.rigidbody2D.AddForce(shootSpeed *(
							boltHolder1.GetComponent<DemoScript>().startPos  - 
							boltHolder1.GetComponent<DemoScript>().endPos));
						boltHolder1.GetComponent<DemoScript>().isOff = true;
						currentAsteroid = -1;
						grabID1 = -1;
					}
				}
				
			}
			else 
			{
				boltHolder1.GetComponent<DemoScript>().isOff = true;
				isGrabbing1 = false;
			}
			if(isGrabbing2 && (touchPoint2.y < grabLimit && isPlayerOne) || (touchPoint2.y > grabLimit && !isPlayerOne))
			{
				float minDistance2 = 10.0f;
				
				if(currentAsteroid2 < 0)
				{
					foreach(Asteroid asteroid in asteroids)
					{
						if(asteroid.geom == null || (asteroid.geom.transform.position.y > grabLimit && isPlayerOne) || 
						   (asteroid.geom.transform.position.y < grabLimit && !isPlayerOne)) continue;
						float distance2 = Mathf.Abs(Vector2.Distance(new Vector2(
							asteroid.geom.transform.position.x, asteroid.geom.transform.position.y), new Vector2(touchPoint2.x, touchPoint2.y)));
						if(distance2 < minDistance2) 
						{
							minDistance2 = distance2;
							i = 0;
							while (i < Input.touchCount)
							{
								if(i < Input.touchCount && Input.GetTouch(i).fingerId == grabID2 && 
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
					if(isPlayerOne) boltHolder2.GetComponent<DemoScript>().startPos = 
						node2.transform.position + new Vector3(DUAL_BOLT_OFFSET_X, BOLT_NODE_OFFSET, 0);
					else boltHolder2.GetComponent<DemoScript>().startPos = 
						node2.transform.position + new Vector3(DUAL_BOLT_OFFSET_X, -BOLT_NODE_OFFSET, 0);
					boltHolder2.GetComponent<DemoScript>().endPos = asteroids[currentAsteroid2].geom.transform.position;
					asteroids[currentAsteroid2].state = Asteroid.ASTEROID_STATE.Grabbed;
					float formerZ = asteroids[currentAsteroid2].geom.transform.parent.position.z - 1.0f;
					i = 0;
					while (i < Input.touchCount)
					{
						if(i < Input.touchCount && Input.GetTouch(i).fingerId == grabID2) touchPoint2 = camera.ScreenToWorldPoint(Input.GetTouch(i).position);
						i++;
					}
					asteroids[currentAsteroid2].geom.transform.position = 
						new Vector3(touchPoint2.x, touchPoint2.y, formerZ);
					Vector2 asteroidPosition = new Vector2(touchPoint2.x, touchPoint2.y);
					asteroids[currentAsteroid2].GrabExpand();
					i = 0;
					while (i < Input.touchCount && Input.GetTouch(i).fingerId != grabID2)
					{
						i++;
					}
					if(i < Input.touchCount && (Input.GetTouch(i).phase == TouchPhase.Ended || 
					                            (touchPoint2.y > grabLimit && isPlayerOne) || (touchPoint2.y < grabLimit && !isPlayerOne))) 
					{
						asteroids[currentAsteroid2].geom.rigidbody2D.AddForce(shootSpeed *(
							boltHolder2.GetComponent<DemoScript>().startPos  - 
							boltHolder2.GetComponent<DemoScript>().endPos));
						isGrabbing2 = false;
						asteroids[currentAsteroid2].state = Asteroid.ASTEROID_STATE.Shot;
						boltHolder2.GetComponent<DemoScript>().isOff = true;
						currentAsteroid2 = -1;
						grabID2 = -2;
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

public class AlternateSlingBehavior : TouchBehavior 
{
	GameObject node1;
	GameObject node2;
	GameObject boltHolder1;
	GameObject boltHolder2;
	GameObject boltHolder3;
	float BOLT_NODE_OFFSET = 1.4f;
	float NODE_LOCATION = -3.0f;
	float DUAL_NODE_OFFSET_Y = -4.0f;
	float DUAL_BOLT_OFFSET_X = 0.8f;
	float DUAL_NODE_OFFSET_X = 1.0f;
	float DUAL_NODE_BEND = 30.0f;
	float NODE_SCALE = 0.2f;
	Vector3 touchPoint;
	Vector3 arcGrabPoint;
	Vector3 touchPoint2;
	Vector3 arcGrabPoint2;
	public AlternateSlingBehavior (float asteroidGrabLimit, float asteroidShootSpeed, 
	                               float asteroidRadius, bool isDual, bool isFirstPlayer) : base(asteroidGrabLimit, 
	                                                              asteroidShootSpeed, asteroidRadius, isDual, isFirstPlayer)
	{
		node1 = new GameObject ();
		if(isFirstPlayer) SetNode(node1, "Textures/slingergyNode", "Node1");
		else SetNode(node1, "Textures/slingergyNode", "Node2P1");
		base.isFling = false;
		if(isDual) 
		{
			if(isFirstPlayer)
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
				boltHolder3 = new GameObject();
				boltHolder3.name = "BoltHolder3";
				boltHolder3.transform.parent = GameObject.Find ("Node1").transform;
			}
			else 
			{
				node1.transform.Translate (new Vector3 (-DUAL_NODE_OFFSET_X, -DUAL_NODE_OFFSET_Y, 0.0f));
				node1.transform.Rotate(new Vector3(0, 0, 180.0f - DUAL_NODE_BEND));
				node2 = new GameObject ();
				SetNode(node2, "Textures/slingergyNode", "Node2P2");
				node2.transform.Translate (new Vector3 (DUAL_NODE_OFFSET_X, -DUAL_NODE_OFFSET_Y, 0.0f));
				node2.transform.Rotate(new Vector3(0, 0, 180.0f + DUAL_NODE_BEND));
				boltHolder2 = new GameObject();
				boltHolder2.name = "BoltHolder2P2";
				boltHolder2.transform.parent = GameObject.Find ("Node2P2").transform;
				boltHolder3 = new GameObject();
				boltHolder3.name = "BoltHolder2P3";
				boltHolder3.transform.parent = GameObject.Find ("Node2P1").transform;

			}
			boltHolder2.AddComponent<DemoScript> ();
			boltHolder2.transform.localPosition = Vector3.forward;
			boltHolder3.AddComponent<DemoScript> ();
			if(isPlayerOne) boltHolder3.GetComponent<DemoScript>().startPos = node1.transform.position + new Vector3(-0.5f, BOLT_NODE_OFFSET, 0);
			else boltHolder3.GetComponent<DemoScript>().startPos = node1.transform.position + new Vector3(-0.5f, -BOLT_NODE_OFFSET, 0);
			if(isPlayerOne) boltHolder3.GetComponent<DemoScript>().endPos = 
				node2.transform.position + new Vector3(DUAL_BOLT_OFFSET_X, BOLT_NODE_OFFSET, 0);
			else boltHolder3.GetComponent<DemoScript>().endPos = 
				node2.transform.position + new Vector3(DUAL_BOLT_OFFSET_X, -BOLT_NODE_OFFSET, 0);
			boltHolder3.GetComponent<DemoScript>().isOff = false;
		}
		else 
		{
			if(isFirstPlayer) node1.transform.Translate (new Vector3 (0.0f, NODE_LOCATION, 0.0f));
			else 
			{
				node1.transform.Translate (new Vector3 (0.0f, -NODE_LOCATION, 0.0f));
				node1.transform.Rotate(new Vector3(0, 0, 180.0f));
			}
		}
		boltHolder1 = new GameObject ();
		if (isFirstPlayer) {
			boltHolder1.name = "BoltHolder1";
			boltHolder1.transform.parent = GameObject.Find ("Node1").transform;
		} else {
			boltHolder1.name = "BoltHolder2P1";
			boltHolder1.transform.parent = GameObject.Find ("Node2P1").transform;
		}
		
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
			while (i < Input.touchCount && ((touchPoint.y > grabLimit && isPlayerOne) || (touchPoint.y < grabLimit && !isPlayerOne) 
			                                || (!isDualTouch || touchPoint.x > 0) || Input.GetTouch(i).fingerId == grabID2))
			{
				i++;
				if(i < Input.touchCount) touchPoint = camera.ScreenToWorldPoint(Input.GetTouch(i).position);
			}
			if(i < Input.touchCount) 
			{
				isGrabbing1 = true;
				grabID1 = Input.GetTouch(i).fingerId;
			}
			
			if(isDualTouch)
			{
				i = 0;
				touchPoint2 = camera.ScreenToWorldPoint(Input.GetTouch(i).position);
				while (i < Input.touchCount && ((touchPoint2.y > grabLimit && isPlayerOne) 
				                                || (touchPoint2.y < grabLimit && !isPlayerOne) || touchPoint2.x < 0 || Input.GetTouch(i).fingerId == grabID1))
				{
					i++;
					if(i < Input.touchCount) touchPoint2 = camera.ScreenToWorldPoint(Input.GetTouch(i).position);
				}
				if(i < Input.touchCount) 
				{
					isGrabbing2 = true;
					grabID2 = Input.GetTouch(i).fingerId;
				}
			}
			
			if(isGrabbing1 && (touchPoint.y < grabLimit && isPlayerOne) || (touchPoint.y > grabLimit && !isPlayerOne))
			{
				float minDistance = 10.0f;
				if(currentAsteroid < 0)
				{
					foreach(Asteroid asteroid in asteroids)
					{
						if(asteroid.geom == null || (asteroid.geom.transform.position.y > grabLimit && isPlayerOne) 
						   || (asteroid.geom.transform.position.y < grabLimit && !isPlayerOne)) continue;
						float distance1 = Mathf.Abs(Vector2.Distance(new Vector2(
							asteroid.geom.transform.position.x, asteroid.geom.transform.position.y), new Vector2(touchPoint.x, touchPoint.y)));
						if(distance1 < minDistance) 
						{
							minDistance = distance1;
							i = 0;
							while (i < Input.touchCount)
							{
								if(Input.GetTouch(i).fingerId == grabID1 && 
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
					if(isPlayerOne) boltHolder1.GetComponent<DemoScript>().startPos = node1.transform.position + new Vector3(0, BOLT_NODE_OFFSET, 0);
					else boltHolder1.GetComponent<DemoScript>().startPos = node1.transform.position + new Vector3(0, -BOLT_NODE_OFFSET, 0);

					boltHolder1.GetComponent<DemoScript>().endPos = asteroids[currentAsteroid].geom.transform.position;
					asteroids[currentAsteroid].state = Asteroid.ASTEROID_STATE.Grabbed;
					float formerZ = asteroids[currentAsteroid].geom.transform.parent.position.z - 1.0f;
					i = 0;
					while (i < Input.touchCount)
					{
						if(i < Input.touchCount && Input.GetTouch(i).fingerId == grabID1) touchPoint = camera.ScreenToWorldPoint(Input.GetTouch(i).position);
						i++;
					}
					
					asteroids[currentAsteroid].geom.transform.position = 
						new Vector3(touchPoint.x, touchPoint.y, formerZ);
					Vector2 asteroidPosition = new Vector2(touchPoint.x, touchPoint.y);
					if(isDualTouch) 
					{
						boltHolder1.GetComponent<DemoScript>().startPos += new Vector2(touchPoint.x, 0);
						
					}
					asteroids[currentAsteroid].GrabExpand();
					i = 0;
					while (i < Input.touchCount && Input.GetTouch(i).fingerId != grabID1)
					{
						i++;
					}
					if(i < Input.touchCount && (Input.GetTouch(i).phase == TouchPhase.Ended || 
					                            (touchPoint.y > grabLimit && isPlayerOne) || (touchPoint.y < grabLimit && !isPlayerOne))) 
					{
						isGrabbing1 = false;
						asteroids[currentAsteroid].state = Asteroid.ASTEROID_STATE.Shot;
						asteroids[currentAsteroid].geom.rigidbody2D.AddForce(shootSpeed *(
							boltHolder1.GetComponent<DemoScript>().startPos  - 
							boltHolder1.GetComponent<DemoScript>().endPos));
						boltHolder1.GetComponent<DemoScript>().isOff = true;
						currentAsteroid = -1;
						grabID1 = -1;
					}
				}
				
			}
			else 
			{
				boltHolder1.GetComponent<DemoScript>().isOff = true;
				isGrabbing1 = false;
			}
			if(isGrabbing2 && (touchPoint2.y < grabLimit && isPlayerOne) || (touchPoint2.y > grabLimit && !isPlayerOne))
			{
				float minDistance2 = 10.0f;
				
				if(currentAsteroid2 < 0)
				{
					foreach(Asteroid asteroid in asteroids)
					{
						if(asteroid.geom == null || (asteroid.geom.transform.position.y > grabLimit && isPlayerOne) || 
						   (asteroid.geom.transform.position.y < grabLimit && !isPlayerOne)) continue;
						float distance2 = Mathf.Abs(Vector2.Distance(new Vector2(
							asteroid.geom.transform.position.x, asteroid.geom.transform.position.y), new Vector2(touchPoint2.x, touchPoint2.y)));
						if(distance2 < minDistance2) 
						{
							minDistance2 = distance2;
							i = 0;
							while (i < Input.touchCount)
							{
								if(i < Input.touchCount && Input.GetTouch(i).fingerId == grabID2 && 
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

					boltHolder2.GetComponent<DemoScript>().endPos = asteroids[currentAsteroid2].geom.transform.position;
					asteroids[currentAsteroid2].state = Asteroid.ASTEROID_STATE.Grabbed;
					float formerZ = asteroids[currentAsteroid2].geom.transform.parent.position.z - 1.0f;
					i = 0;
					while (i < Input.touchCount)
					{
						if(i < Input.touchCount && Input.GetTouch(i).fingerId == grabID2) touchPoint2 = camera.ScreenToWorldPoint(Input.GetTouch(i).position);
						i++;
					}
					asteroids[currentAsteroid2].geom.transform.position = 
						new Vector3(touchPoint2.x, touchPoint2.y, formerZ);
					Vector2 asteroidPosition = new Vector2(touchPoint2.x, touchPoint2.y);
					if(isPlayerOne) boltHolder2.GetComponent<DemoScript>().startPos = 
						node2.transform.position + new Vector3(touchPoint2.x, BOLT_NODE_OFFSET, 0);
					else boltHolder2.GetComponent<DemoScript>().startPos = 
						node2.transform.position + new Vector3(touchPoint2.x, -BOLT_NODE_OFFSET, 0);
					asteroids[currentAsteroid2].GrabExpand();
					i = 0;
					while (i < Input.touchCount && Input.GetTouch(i).fingerId != grabID2)
					{
						i++;
					}
					if(i < Input.touchCount && (Input.GetTouch(i).phase == TouchPhase.Ended || 
					                            (touchPoint2.y > grabLimit && isPlayerOne) || (touchPoint2.y < grabLimit && !isPlayerOne))) 
					{
						asteroids[currentAsteroid2].geom.rigidbody2D.AddForce(shootSpeed *(
							boltHolder2.GetComponent<DemoScript>().startPos  - 
							boltHolder2.GetComponent<DemoScript>().endPos));
						isGrabbing2 = false;
						asteroids[currentAsteroid2].state = Asteroid.ASTEROID_STATE.Shot;
						boltHolder2.GetComponent<DemoScript>().isOff = true;
						currentAsteroid2 = -1;
						grabID2 = -2;
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

