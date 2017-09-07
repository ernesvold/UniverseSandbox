using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoxScript : MonoBehaviour {

	public int numballs = 1;

	void Start(){
		BallList ballList = new BallList (transform, numballs);
	}
}

// A single ball
public class Ball {

	public GameObject gameobj = GameObject.CreatePrimitive (PrimitiveType.Sphere); // a sphere in unity
	public Vector3 velocity = Vector3.zero; // the 3d velocity of the ball

	// Ball constructor
	public Ball(Transform parent, Vector3 position, float diameter){
		gameobj.name = "Ball"; // name the sphere
		gameobj.transform.parent = parent.transform; // set as child
		gameobj.transform.position = position;
		gameobj.transform.localScale = Vector3.one * diameter;
		Debug.Log ("Ball created, position = ("+position.x+", "+position.y+", "+position.z+")");
	}
	
}

// A collection of balls
public class BallList {

	public GameObject gameobj = new GameObject(); // an empty game object in Unity
	public List<Ball> balls = new List<Ball>(); // the list of ball objects

	// BallList constructor
	public BallList(Transform parent, int numballs){
		gameobj.name = "BallList"; // name the empty game object
		gameobj.transform.parent = parent.transform; // set as child
		Debug.Log ("BallList created."); 
		AddBall (numballs);
	}

	// Create and add Balls to the list
	public void AddBall(int numballs){
		Vector3[] positions = new Vector3[numballs]; // ball positions
		float diameter = 1.0f; // ball radius
		float maxpos = 5.0f;
		MakeRandomPositions(numballs, maxpos, ref positions, out diameter);

		// Create and add each Ball
		for (int i = 0; i < numballs; i++){
			balls.Add (new Ball (gameobj.transform, positions [i], diameter)); // add a new ball
		}
	}

	// Generate a list of random-looking positions
	public void MakeRandomPositions(int numpositions, float maxpos, ref Vector3[] positions, out float diameter){
		int numpts1d = (int) Mathf.Ceil(Mathf.Pow(numpositions,(float)1.0/3)); // number of points in one dimension in the grid
		numpts1d = numpts1d * 3; // add extra white space 
		int numpts2d = numpts1d * numpts1d; // number of points in one 2d slice of the grid
		int numpts3d = numpts2d * numpts1d; // total number of points in the 3d grid

		int[] indices = Enumerable.Range (0, numpts3d).ToArray (); // array of indicies representing grid coordinates

		float shift = ((float)numpts1d-1)/2; // center grid at origin
		float scale = 2*maxpos/numpts1d; // scale grid to size of box
		diameter = scale; // scale up diameters to fill grid pixel

		// Shuffle array of indices (at least the first numpositions)
		PartiallyShuffleArray (numpositions, ref indices);

		// Create each coordinate, add to positions array
		int x, y, z; // x,y,z coordinate
		int index; // index corresponding to grid
		for (int i = 0; i < numpositions; i++) {
			// Select index from shuffled array
			index = indices[i];

			// Map integer to 3d coordinates
			x = numpts1d * index / numpts3d;
			y = numpts1d * (index - (x * numpts2d)) / (numpts2d);
			z = index - (x * numpts2d) - (y * numpts1d);

			// Add to positions array
			positions [i] = new Vector3 (scale*(x-shift), scale*(y-shift), scale*(z-shift));
		}

	}

	// Partially shuffle an array of integers
	public void PartiallyShuffleArray(int k, ref int[] array) {

		// Input checking
		if (k > array.Length) {
			Debug.Log ("partiallyShuffleArray: CAUTION: k is greater than length of array");
			k = array.Length;
		}

		if (k == array.Length) {
			k = k - 1; // for the Knuth shuffle, you don't need to shuffle the last element
		}

		// Knuth shuffle algorithm -- shuffles first k elements in array
		for (int i = 0; i < k; i++) {
			int tmp = array [i];
			int r = Random.Range (i, array.Length);
			array [i] = array [r];
			array [r] = tmp;
		}
	
	}

}
