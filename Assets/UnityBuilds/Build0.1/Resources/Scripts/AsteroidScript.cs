using UnityEngine;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;

abstract public class Asteroid 
{
	public GameObject geom { get; set; }
	public static float ASTEROID_LARGE_SCALE = 0.3f;
	public static float ASTEROID_SMALL_SCALE = 0.24f;
	float rotationDirection;
	float currentScale;
	public float LARGE_CHANCE = 0.5f;
	public float RIGHT_ROTATION_CHANCE = 0.5f;
	const float GRAB_SCALE = 1.5f;
	public static float RADIUS = 1.0f;
	public enum ASTEROID_STATE {Free, Grabbed, Shot};
	public ASTEROID_STATE state;
	public Asteroid(float x, float y, GameObject parent)
	{
		geom = new GameObject ();
		geom.transform.parent = parent.transform;
		geom.transform.localPosition = new Vector3 (x, y, 0);
		if(Random.value < LARGE_CHANCE) currentScale = ASTEROID_LARGE_SCALE;
		else currentScale = ASTEROID_SMALL_SCALE;
		if(Random.value < RIGHT_ROTATION_CHANCE) rotationDirection = 1.0f;
		else rotationDirection = -1.0f;
		geom.transform.localScale = new Vector3 (currentScale, currentScale, currentScale);
		CircleCollider2D collider = geom.AddComponent<CircleCollider2D> ();
		collider.radius = RADIUS;
		Rigidbody2D rigid = geom.AddComponent<Rigidbody2D> ();
		rigid.gravityScale = 0;
		rigid.angularDrag = 0.0f;
		collider.center = rigid.centerOfMass;
		rigid.interpolation = RigidbodyInterpolation2D.Extrapolate;
		rigid.fixedAngle = true;
		state = ASTEROID_STATE.Free;
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

	public float GetRotationDirection()
	{
		return rotationDirection;
	}
}

public class AsteroidScript : MonoBehaviour {

	public List<Asteroid> asteroids;
	const int ASTEROID_AMOUNT = 20;
	Camera camera { get; set; }
	public bool isPlayerOne = true;
	public float ASTEROID_SCALER = 0.97f;
	public float ASTEROID_GRAB_LIMIT = -2.2f; //Space of screen where one can grab asteroids
	const float ASTEROID_HORIZ_LIMIT = 6.0f; //Limit before asteroid is off-screen and destroyed
	const float ASTEROID_VERT_LIMIT = 6.0f;
	public float ASTEROID_SPAWN_TIME = 0.4f;
	const float ASTEROID_START_POINT = 4.0f;
	public float ASTEROID_FLING_SPEED = 0.5f;
	public float ASTEROID_SLING_SPEED = 60.0f;
	public float ASTEROID_DRIFT_SPEED = -0.1f;
	public float ASTEROID_DRIFT_CURVE = 0.14f;//Helps set the parabola curve (steep or wide)
	public float COARSE_CHANCE = 0.143f;
	public float DENSE_CHANCE = 0.143f;
	public float EXPLOSIVE_CHANCE = 0.143f;
	public float ICE_CHANCE = 0.143f;
	public float MAGMA_CHANCE = 0.143f;
	public float POISON_CHANCE = 0.143f;
	public float INFESTED_CHANCE = 0.143f;
	public float ROTATION_SPEED_MIN = 1.0f;
	public float ROTATION_SPEED_MAX = 100.0f;
	float spawnTimer = 0.0f;
	float oldChance;
	float chance;
	bool isGrabbing1 = false;
	bool isGrabbing2 = false;
	int currentAsteroid = -1;
	int currentAsteroid2 = -1;
	TouchBehavior touchStrategy;

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
		camera = GameObject.Find("Main Camera").camera;
		asteroids = new List<Asteroid> ();
		AddRandomAsteroid (0);
		AddRandomAsteroid (3.0f);
		AddRandomAsteroid (-3.0f);
		asteroids[0].geom.name = "Asteroid" + asteroids.IndexOf(asteroids[0]).ToString();
		asteroids[1].geom.name = "Asteroid" + asteroids.IndexOf(asteroids[1]).ToString();
		asteroids[2].geom.name = "Asteroid" + asteroids.IndexOf(asteroids[2]).ToString();
		touchStrategy = new AlternateSlingBehavior(ASTEROID_GRAB_LIMIT, ASTEROID_SLING_SPEED, Asteroid.RADIUS, true, isPlayerOne);
	}

	// Update is called once per frame
	void Update () {
		asteroids = touchStrategy.ResolveTouches (asteroids, camera);
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
			if(asteroid.state == Asteroid.ASTEROID_STATE.Free) 
			{
				astroTransform.Translate(ASTEROID_DRIFT_SPEED * Vector3.right, Space.World);
				astroTransform.position = new Vector3(astroTransform.position.x, ASTEROID_DRIFT_CURVE*
				                                      Mathf.Pow(asteroid.geom.transform.position.x, 2.0f) + transform.position.y,
				                                      astroTransform.position.z);
				astroTransform.Rotate (0, 0, asteroid.GetRotationDirection() * 
				                       Random.Range(ROTATION_SPEED_MIN, ROTATION_SPEED_MAX) * Time.deltaTime, Space.Self);

			}
			if(Mathf.Abs(astroTransform.position.y) > ASTEROID_VERT_LIMIT || 
			   Mathf.Abs(astroTransform.position.x) > ASTEROID_HORIZ_LIMIT) Object.Destroy(asteroid.geom); 
		

			if((astroTransform.position.y > ASTEROID_GRAB_LIMIT && isPlayerOne) || 
			   (astroTransform.position.y < ASTEROID_GRAB_LIMIT && !isPlayerOne)) astroTransform.localScale
				= Mathf.Pow (ASTEROID_SCALER, (Mathf.Abs(astroTransform.position.y - ASTEROID_GRAB_LIMIT)) * 10.0f) * originalScale;
			else if (Input.touchCount <= 0) astroTransform.localScale = originalScale;

			if((astroTransform.position.y > ASTEROID_GRAB_LIMIT && isPlayerOne) || 
			   (astroTransform.position.y < ASTEROID_GRAB_LIMIT && !isPlayerOne)) asteroid.geom.collider2D.enabled = true;
			else asteroid.geom.collider2D.enabled = false;
		}
	}		
}