using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DemoScript : MonoBehaviour 
{
	//Prefabs to be assigned in Editor
	public GameObject BoltPrefab;
	public Vector2 startPos;
	public Vector2 endPos;
	public bool isOff = true;
	//For pooling
	List<GameObject> activeBoltsObj;
	List<GameObject> inactiveBoltsObj;
	int maxBolts = 40;
	
	//For handling mouse clicks
	int clicks = 0;

	
	void Start()
	{
		//Initialize lists
		activeBoltsObj = new List<GameObject>();
		inactiveBoltsObj = new List<GameObject>();
		//Grab the parent we'll be assigning to our bolt pool

		
		//For however many bolts we've specified
		for(int i = 0; i < maxBolts; i++)
		{
			//create from our prefab
			GameObject bolt = (GameObject)Instantiate(Resources.Load("Prefabs/Bolt"));
			
			//Assign parent
			bolt.transform.parent = gameObject.transform;
			bolt.transform.position = new Vector3(bolt.transform.position.x, bolt.transform.position.y, gameObject.transform.position.z);
			//Initialize our lightning with a preset number of max sexments
			bolt.GetComponent<LightningBolt>().Initialize(25);
			
			//Set inactive to start
			bolt.SetActive(false);
			
			//Store in our inactive list
			inactiveBoltsObj.Add(bolt);
		}
	}
	
	void Update()
	{
		//Declare variables for use later
		GameObject boltObj;
		LightningBolt boltComponent;
		
		//store off the count for effeciency
		int activeLineCount = activeBoltsObj.Count;
		
		//loop through active lines (backwards because we'll be removing from the list)
		for (int i = activeLineCount - 1; i >= 0; i--)
		{
			//pull GameObject
			boltObj = activeBoltsObj[i];
			
			//get the LightningBolt component
			boltComponent = boltObj.GetComponent<LightningBolt>();
			
			//if the bolt has faded out
			if(boltComponent.IsComplete)
			{
				//deactive the segments it contains
				boltComponent.DeactivateSegments();
				
				//set it inactive
				boltObj.SetActive(false);
				
				//move it to the inactive list
				activeBoltsObj.RemoveAt(i);
				inactiveBoltsObj.Add(boltObj);
			}
		}

		if(!isOff) CreatePooledBolt(startPos, endPos, Color.white, 1f);
		//update and draw active bolts
		for(int i = 0; i < activeBoltsObj.Count; i++)
		{
			activeBoltsObj[i].GetComponent<LightningBolt>().UpdateBolt();
			activeBoltsObj[i].GetComponent<LightningBolt>().Draw();
		}
	}
	
	void CreatePooledBolt(Vector2 source, Vector2 dest, Color color, float thickness)
	{
		//if there is an inactive bolt to pull from the pool
		if(inactiveBoltsObj.Count > 0)
		{
			//pull the GameObject
			GameObject boltObj = inactiveBoltsObj[inactiveBoltsObj.Count - 1];
			
			//set it active
			boltObj.SetActive(true);
			
			//move it to the active list
			activeBoltsObj.Add(boltObj);
			inactiveBoltsObj.RemoveAt(inactiveBoltsObj.Count - 1);
			
			//get the bolt component
			LightningBolt boltComponent =  boltObj.GetComponent<LightningBolt>();
			
			//activate the bolt using the given position data
			boltComponent.ActivateBolt(source, dest, color, thickness);
		}
	}
}
