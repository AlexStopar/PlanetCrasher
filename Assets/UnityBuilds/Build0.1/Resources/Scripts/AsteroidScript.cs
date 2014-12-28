using UnityEngine;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;

abstract public class Asteroid 
{
	public GameObject geom { get; set; }
	public static float ASTEROID_LARGE_SCALE = 0.3f;
	public static float ASTEROID_SMALL_SCALE = 0.28f;
	float currentScale;
	public float LARGE_CHANCE = 0.5f;
	const float GRAB_SCALE = 1.5f;
	public bool isTouched;
	public static float RADIUS = 1.0f;
	public Asteroid(float x, float y, GameObject parent)
	{
		geom = new GameObject ();
		geom.transform.parent = parent.transform;
		geom.transform.localPosition = new Vector3 (x, y, 0);
		if(Random.value < LARGE_CHANCE) currentScale = ASTEROID_LARGE_SCALE;
		else currentScale = ASTEROID_SMALL_SCALE;
		geom.transform.localScale = new Vector3 (currentScale, currentScale, currentScale);

		CircleCollider2D collider = geom.AddComponent<CircleCollider2D> ();
		collider.center = geom.transform.position;
		collider.radius = RADIUS;
		Rigidbody2D rigid = geom.AddComponent<Rigidbody2D> ();
		rigid.gravityScale = 0;
		rigid.angularDrag = 0.0f;
		rigid.interpolation = RigidbodyInterpolation2D.Extrapolate;
		rigid.fixedAngle = true;
		isTouched = false;
	}
	virtual public void SetSprite () { return;}
	public void GrabExpand() //Expands asteroid when grabbed
	{
		geom.transform.localScale = new Vector3 (GRAB_SCALE * currentScale,  
		                                         GRAB_SCALE * currentScale, GRAB_SCALE * currentScale);
	}
	public float GetScale()
	{
		return currentScale;
	}
}

public class AsteroidScript : MonoBehaviour {

	public List<Asteroid> asteroids;
	const int ASTEROID_AMOUNT = 20;
	Camera camera { get; set; }
	public float ASTEROID_SCALER = 0.97f;
	public float ASTEROID_GRAB_LIMIT = -2.2f; //Space of screen where one can grab asteroids
	const float ASTEROID_HORIZ_LIMIT = 5.0f; //Limit before asteroid is off-screen and destroyed
	const float ASTEROID_VERT_LIMIT = 6.0f;
	public float ASTEROID_SPAWN_TIME = 0.4f;
	const float ASTEROID_START_POINT = 5.0f;
	public float ASTEROID_FLING_SPEED = 0.5f;
	public float ASTEROID_DRIFT_SPEED = -0.1f;
	public float ASTEROID_DRIFT_CURVE = 0.14f;//Helps set the parabola curve (steep or wide)
	public float ASTEROID_CURVE_TIP = 4.0f; //Sets the tip of the parabola
	public float COARSE_CHANCE = 0.143f;
	public float DENSE_CHANCE = 0.143f;
	public float EXPLOSIVE_CHANCE = 0.143f;
	public float ICE_CHANCE = 0.143f;
	public float MAGMA_CHANCE = 0.143f;
	public float POISON_CHANCE = 0.143f;
	public float INFESTED_CHANCE = 0.143f;
	float spawnTimer = 0.0f;
	float oldChance;
	float chance;
	bool isGrabbing = false;
	int currentAsteroid = -1;

	void ChangeChances(float newChance)
	{
		oldChance = chance;
		chance += newChance;
	}

	void AddRandomAsteroid(float startPoint) //Adds asteroids
	{
		float rand = Random.value;
		oldChance = 0.0f;
		chance = COARSE_CHANCE;
		if(rand < chance && rand > oldChance) 
		{
			asteroids.Add(new CoarseAsteroid(startPoint, 0, gameObject)); 
			return;
		}
		else ChangeChances(DENSE_CHANCE); 
		if(rand < chance && rand > oldChance) 
		{
			asteroids.Add(new DenseAsteroid(startPoint, 0, gameObject)); 
			return;
		}
		else ChangeChances(EXPLOSIVE_CHANCE); 
		if(rand < chance && rand > oldChance) {
			asteroids.Add(new ExplosiveAsteroid(startPoint, 0, gameObject)); 
			return;
		}
		else ChangeChances(ICE_CHANCE); 
		if(rand < chance && rand > oldChance) {
			asteroids.Add(new IceAsteroid(startPoint, 0, gameObject)); 
			return;
		}
		else ChangeChances(MAGMA_CHANCE); 
		if(rand < chance && rand > oldChance) {
			asteroids.Add(new MagmaAsteroid(startPoint, 0, gameObject)); 
			return;
		}
		else ChangeChances(POISON_CHANCE); 
		if(rand < chance && rand > oldChance) {
			asteroids.Add(new PoisonAsteroid(startPoint, 0, gameObject)); 
			return;
		}
		else asteroids.Add(new InfectedAsteroid(startPoint, 0, gameObject));
	}
	Asteroid RestartAsteroid() //Reinstantiates asteroid
	{
		float rand = Random.value;
		oldChance = 0.0f;
		chance = COARSE_CHANCE;
		if(rand < chance && rand > oldChance) return new CoarseAsteroid(ASTEROID_START_POINT, 0, gameObject); 
		else ChangeChances(DENSE_CHANCE); 
		if(rand < chance && rand > oldChance) return new DenseAsteroid(ASTEROID_START_POINT, 0, gameObject); 
		else ChangeChances(EXPLOSIVE_CHANCE); 
		if(rand < chance && rand > oldChance) return new ExplosiveAsteroid(ASTEROID_START_POINT, 0, gameObject); 
		else ChangeChances(ICE_CHANCE); 
		if(rand < chance && rand > oldChance) return new IceAsteroid(ASTEROID_START_POINT, 0, gameObject); 
		else ChangeChances(MAGMA_CHANCE); 
		if(rand < chance && rand > oldChance) return new MagmaAsteroid(ASTEROID_START_POINT, 0, gameObject); 
		else ChangeChances(POISON_CHANCE); 
		if(rand < chance && rand > oldChance) return new PoisonAsteroid(ASTEROID_START_POINT, 0, gameObject); 
		else return new InfectedAsteroid(ASTEROID_START_POINT, 0, gameObject); 
	}
	
	void Start () {
		transform.position = new Vector3 (transform.position.x, -3.3f, transform.position.z);
		camera = GameObject.Find("Main Camera").camera;
		asteroids = new List<Asteroid> ();
		AddRandomAsteroid (0);
		AddRandomAsteroid (3.0f);
		AddRandomAsteroid (-3.0f);
		asteroids[0].geom.name = "Asteroid" + asteroids.IndexOf(asteroids[0]).ToString();
		asteroids[1].geom.name = "Asteroid" + asteroids.IndexOf(asteroids[1]).ToString();
		asteroids[2].geom.name = "Asteroid" + asteroids.IndexOf(asteroids[2]).ToString();
	}

	// Update is called once per frame
	void Update () {
		if (Input.touchCount >= 1)
		{
				Vector3 touchPoint = camera.ScreenToWorldPoint(Input.GetTouch(0).position);
				if(touchPoint.y < ASTEROID_GRAB_LIMIT)
				{
					float minDistance = 10.0f;
					
					if(!isGrabbing)
					{
						foreach(Asteroid asteroid in asteroids)
						{
					  	    if(asteroid.geom == null || asteroid.geom.transform.position.y > ASTEROID_GRAB_LIMIT) continue;
							float distance = Mathf.Abs(Vector2.Distance(new Vector2(
							asteroid.geom.transform.position.x, asteroid.geom.transform.position.y), new Vector2(touchPoint.x, touchPoint.y)));
							if(distance < minDistance) 
							{
								minDistance = distance;
								currentAsteroid = asteroids.IndexOf(asteroid);
							}
							asteroid.geom.transform.position = new Vector3(asteroid.geom.transform.position.x, 
						                                               asteroid.geom.transform.position.y, asteroid.geom.transform.parent.position.z);
						}
					}
					if(currentAsteroid >= 0)
					{
						isGrabbing = true;
						asteroids[currentAsteroid].isTouched = true;
						float formerZ = asteroids[currentAsteroid].geom.transform.parent.position.z - 1.0f;
						asteroids[currentAsteroid].geom.transform.position = 
						new Vector3(touchPoint.x, touchPoint.y + Asteroid.RADIUS, formerZ);
					    Vector2 asteroidPosition = new Vector2(touchPoint.x, touchPoint.y);
						asteroids[currentAsteroid].geom.rigidbody2D.AddForce(ASTEROID_FLING_SPEED * 
					                                                     ( Input.GetTouch(0).deltaPosition) - asteroidPosition);
						asteroids[currentAsteroid].GrabExpand();
					}

				}
		}
		else isGrabbing = false;
		spawnTimer += Time.deltaTime;
		if(spawnTimer > ASTEROID_SPAWN_TIME)
		{
			if(asteroids.Count < ASTEROID_AMOUNT) 
			{
				AddRandomAsteroid (ASTEROID_START_POINT);
				asteroids[asteroids.Count-1].geom.name = "Asteroid" + (asteroids.Count-1).ToString();
			}
			else
			{
				for(int i = 0; i < ASTEROID_AMOUNT; i++)
				{
					if(asteroids[i].geom == null) 
					{
						asteroids[i] = RestartAsteroid();
						asteroids[i].geom.name = "Asteroid" + i.ToString();
						i = ASTEROID_AMOUNT;
					}
				}
			}
			spawnTimer = 0;
		}
		foreach(Asteroid asteroid in asteroids)
		{
			if(asteroid.geom == null) continue;
			Transform astroTransform = asteroid.geom.transform;
			Vector3 originalScale = new Vector3 (asteroid.GetScale(),  
			                                     asteroid.GetScale(), asteroid.GetScale());
			if(!asteroid.isTouched) 
			{
				astroTransform.Translate(ASTEROID_DRIFT_SPEED * Vector3.right, Space.World);
				astroTransform.position = new Vector3(astroTransform.position.x, ASTEROID_DRIFT_CURVE*
				                                      Mathf.Pow(asteroid.geom.transform.position.x, 2.0f) - ASTEROID_CURVE_TIP,
				                                      astroTransform.position.z);
				astroTransform.Rotate (0, 0, 100 * Time.deltaTime, Space.Self);
			}
			if(Mathf.Abs(astroTransform.position.y) > ASTEROID_VERT_LIMIT || 
			   Mathf.Abs(astroTransform.position.x) > ASTEROID_HORIZ_LIMIT) Object.Destroy(asteroid.geom); 
		

			if(astroTransform.position.y > ASTEROID_GRAB_LIMIT) astroTransform.localScale
				= Mathf.Pow (ASTEROID_SCALER, (astroTransform.position.y - ASTEROID_GRAB_LIMIT) * 10.0f) * originalScale;
			else if (Input.touchCount <= 0) astroTransform.localScale = originalScale;
		}
	}		
}