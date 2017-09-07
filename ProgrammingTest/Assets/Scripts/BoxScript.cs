using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxScript : MonoBehaviour {
	void Start(){
		BallList ballList = new BallList ();
		ballList.gameobj.transform.parent = transform;
	}
}

// A single ball
public class Ball {

	public GameObject gameobj = GameObject.CreatePrimitive (PrimitiveType.Sphere); // a sphere in unity
	public Vector3 velocity = Vector3.zero; // the 3d velocity of the ball

	// Ball constructor
	public Ball(GameObject parent){
		gameobj.name = "Ball"; // name the sphere
		gameobj.transform.parent = parent.transform; // set as child
		Debug.Log ("Ball created.");
	}
	
}

// A collection of balls
public class BallList {

	public GameObject gameobj = new GameObject(); // an empty game object in Unity
	public List<Ball> balls = new List<Ball>(); // the list of ball objects

	// BallList constructor
	public BallList(){
		gameobj.name = "BallList"; // name the empty game object
		Debug.Log ("BallList created."); 
		balls.Add (new Ball(gameobj)); // add a new ball
	}


}
