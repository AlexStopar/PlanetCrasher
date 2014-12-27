using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Moon
{
	public GameObject geom { get; set; }
	const float RADIUS = 1.0f;
	public static float MOON_SCALE = 0.5f;
	public Moon(float x, float y, GameObject parent)
	{
		geom = new GameObject ();
		geom.transform.localScale = new Vector3 (MOON_SCALE, MOON_SCALE, -1);
		geom.transform.parent = parent.transform;
		geom.transform.localPosition = new Vector3 (x, y, 0);
		
		
		Sprite sprite = Resources.Load<Sprite>("Textures/moon") as Sprite;
		if(sprite != null && geom.GetComponent<SpriteRenderer> () == null)
		{
			geom.AddComponent<SpriteRenderer> ();
			geom.GetComponent<SpriteRenderer> ().sprite = sprite;
		}
		CircleCollider2D collider = geom.AddComponent<CircleCollider2D> ();
		collider.center = geom.transform.position;
		collider.radius = RADIUS;
		Rigidbody2D rigid = geom.AddComponent<Rigidbody2D> ();
		rigid.gravityScale = 0;
		rigid.angularDrag = 0.0f;
		rigid.interpolation = RigidbodyInterpolation2D.Extrapolate;
		rigid.fixedAngle = true;
	}
}
public class MoonScript : MonoBehaviour {
	
	public List<Moon> moons;
	// Use this for initialization
	void Start () {
		moons = new List<Moon> ();
		moons.Add (new Moon (0, 0, gameObject));
		moons[0].geom.name = "Moon" + moons.IndexOf(moons[0]).ToString();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
