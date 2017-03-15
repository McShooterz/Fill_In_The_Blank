//this controls the ball/player object in the "game"
/*
the ball will move towards a destination position set by the network manager,
once the ball has reached it's destination it will set atLocation to true telling the manger to request a new destination.
the manger will then send the request to the server and retrieve a vector from it.
*/
using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {
	private float speed;//how fast will the ball move per update? 
	
	private Vector3 position;
	
	//this will be set by the network manager,
	public Vector3 destination;
	//this will tell the network manager that the ball has reached the destination and needs a new destination vector.
	public bool atLocation;

	void Start ()
	{
		
		speed = .3f;
	}

	void FixedUpdate ()
	{
		//get the difference in the destination and position
		Vector3 velocity = destination-position;
		
		//if the difference magnitude is less than our speed than we just set the position to the destination, 
		//because if we moved at our regular pace we would pass it.
		if (velocity.magnitude <speed){
			
			position = destination;
			//set atLocation to true telling the manager we need a new dest.
			atLocation = true;
		}
		else{
			//normalize the velocity and then multiply it by the speed so we only move .3 units in any direction per update. then add this velocity to the postion
			velocity.Normalize();
			position += velocity*speed;
		}
		transform.position = position;//set the position.
	}
}
