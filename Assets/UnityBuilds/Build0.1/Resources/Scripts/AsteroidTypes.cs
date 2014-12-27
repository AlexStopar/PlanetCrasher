using UnityEngine;
using System.Collections;

public class CoarseAsteroid : Asteroid 
{
	public CoarseAsteroid (float x, float y, GameObject parent) : base(x, y, parent)
	{
		SetSprite ();
	}
	void SetSprite()
	{
		Sprite sprite = Resources.Load<Sprite>("Textures/asteroidCoarse") as Sprite;
		if(sprite != null && geom.GetComponent<SpriteRenderer> () == null)
		{
			geom.AddComponent<SpriteRenderer> ();
			geom.GetComponent<SpriteRenderer> ().sprite = sprite;
		}
	}
}

public class DenseAsteroid : Asteroid 
{
	public DenseAsteroid (float x, float y, GameObject parent) : base(x, y, parent)
	{
		SetSprite ();
	}
	void SetSprite()
	{
		Sprite sprite = Resources.Load<Sprite>("Textures/asteroidDense") as Sprite;
		if(sprite != null && geom.GetComponent<SpriteRenderer> () == null)
		{
			geom.AddComponent<SpriteRenderer> ();
			geom.GetComponent<SpriteRenderer> ().sprite = sprite;
		}
	}
}
public class ExplosiveAsteroid : Asteroid 
{
	public ExplosiveAsteroid (float x, float y, GameObject parent) : base(x, y, parent)
	{
		SetSprite ();
	}
	void SetSprite()
	{
		Sprite sprite = Resources.Load<Sprite>("Textures/asteroidExplosive") as Sprite;
		if(sprite != null && geom.GetComponent<SpriteRenderer> () == null)
		{
			geom.AddComponent<SpriteRenderer> ();
			geom.GetComponent<SpriteRenderer> ().sprite = sprite;
		}
	}
}

public class IceAsteroid : Asteroid 
{
	public IceAsteroid (float x, float y, GameObject parent) : base(x, y, parent)
	{
		SetSprite ();
	}
	void SetSprite()
	{
		Sprite sprite = Resources.Load<Sprite>("Textures/asteroidIce") as Sprite;
		if(sprite != null && geom.GetComponent<SpriteRenderer> () == null)
		{
			geom.AddComponent<SpriteRenderer> ();
			geom.GetComponent<SpriteRenderer> ().sprite = sprite;
		}
	}
}

public class MagmaAsteroid : Asteroid 
{
	public MagmaAsteroid (float x, float y, GameObject parent) : base(x, y, parent)
	{
		SetSprite ();
	}
	void SetSprite()
	{
		Sprite sprite = Resources.Load<Sprite>("Textures/asteroidMagma") as Sprite;
		if(sprite != null && geom.GetComponent<SpriteRenderer> () == null)
		{
			geom.AddComponent<SpriteRenderer> ();
			geom.GetComponent<SpriteRenderer> ().sprite = sprite;
		}
	}
}

public class PoisonAsteroid : Asteroid 
{
	public PoisonAsteroid (float x, float y, GameObject parent) : base(x, y, parent)
	{
		SetSprite ();
	}
	void SetSprite()
	{
		Sprite sprite = Resources.Load<Sprite>("Textures/asteroidPoison") as Sprite;
		if(sprite != null && geom.GetComponent<SpriteRenderer> () == null)
		{
			geom.AddComponent<SpriteRenderer> ();
			geom.GetComponent<SpriteRenderer> ().sprite = sprite;
		}
	}
}

public class InfectedAsteroid : Asteroid 
{
	public InfectedAsteroid (float x, float y, GameObject parent) : base(x, y, parent)
	{
		SetSprite ();
	}
	void SetSprite()
	{
		Sprite sprite = Resources.Load<Sprite>("Textures/asteroidInfected") as Sprite;
		if(sprite != null && geom.GetComponent<SpriteRenderer> () == null)
		{
			geom.AddComponent<SpriteRenderer> ();
			geom.GetComponent<SpriteRenderer> ().sprite = sprite;
		}
	}
}