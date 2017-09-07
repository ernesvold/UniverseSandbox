using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallHandler : MonoBehaviour {

	public int numballs;

	private float ballradius = 0.5f;
	private Rigidbody rb;
	private SphereCollider sc;
	private float maxvel = 5.0f; // velocity constraint

	void Awake(){
		numballs = GameObject.Find("UserInput").GetComponent<UserInputHandler>().numballs; // fetch this value from user input first
	}

	// Use this for initialization
	void Start () {

		// Get the boxsize from the WallHandler
		float boxsize = GameObject.Find("Box").GetComponent<WallHandler>().boxsize;
		float maxpos = boxsize - ballradius; // prevent balls from starting overlapped with walls

		// Make a list of velocities
		Vector3[] velocities = new Vector3[numballs];
		MakeRandomVelocities (numballs, maxvel, ref velocities);

		// Make a list of positions
		float diameter = 1.0f;
		Vector3[] positions = new Vector3[numballs];
		makeRandomPositions (numballs, maxpos, ref positions, out diameter);

		// Build each ball
		for (int n = 0; n < numballs; n++) {
			GameObject ball = GameObject.CreatePrimitive (PrimitiveType.Sphere); // create sphere 

			ball.name = "Ball"; // name sphere
			ball.transform.parent = transform; // make sphere a child of Balls empty object
			ball.AddComponent<Rigidbody> (); // turn on physics
			ball.AddComponent<SphereCollider> (); // turn on collisions

			ball.transform.position = positions[n]; // set random position
			ball.transform.localScale = Vector3.one*diameter;
			
			rb = ball.GetComponent<Rigidbody> (); // access rigidbody
			rb.useGravity = false; // turn off gravity
			rb.velocity = velocities[n]; // set velocity

			sc = ball.GetComponent<SphereCollider> (); // access collider
			sc.material.bounciness = 1; // perfectly elastic collisions
			sc.material.bounceCombine = PhysicMaterialCombine.Maximum; // don't average "bounciness"
		}
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void makeRandomPositions(int numpositions, float maxpos, ref Vector3[] positions, out float diameter){

		int numpts1d = (int) Mathf.Ceil(Mathf.Pow(numpositions,(float)1.0/3)); // number of points in one dimension in the grid
		numpts1d = numpts1d * 3; // add extra white space 
		int numpts2d = numpts1d * numpts1d; // number of points in one 2d slice of the grid
		int numpts3d = numpts2d * numpts1d; // total number of points in the 3d grid

		int[] indices = Enumerable.Range (0, numpts3d).ToArray (); // array of indicies representing grid coordinates

		float shift = ((float)numpts1d-1)/2; // center grid at origin
		float scale = 2*maxpos/numpts1d; // scale grid to size of box
		diameter = scale; // scale up diameters to fill grid pixel

		// Shuffle array of indices (at least the first numpositions)
		partiallyShuffleArray (numpositions, ref indices);

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

	public void partiallyShuffleArray(int k, ref int[] array) {

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

	// Return a list of random velocities
	public void MakeRandomVelocities(int numvelocities, float maxvel, ref Vector3[] velocities){

		// Initialize total momentum vector
		Vector3 totalmomentum = Vector3.zero;

		// Generate some random velocities
		for (int i = 0; i < numvelocities; i++) {
			velocities[i] = new Vector3 (Random.Range (-maxvel, maxvel), Random.Range (-maxvel, maxvel), Random.Range (-maxvel, maxvel));
			totalmomentum = totalmomentum + velocities [i]; // Calculate total linear momentum (TODO: assumes each mass is 1)
		}

		// Correct velocities so total linear momentum is zero
		for (int i = 0; i < numvelocities; i++) {
			velocities [i] = velocities [i] - (totalmomentum / (float)numvelocities);
		}

	}
}
