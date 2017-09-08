using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainScript : MonoBehaviour {

	public int numballs;
	public float boxsize;
	private Box box;
	private BallList ballList;

	void Start(){
		box = new Box (transform, boxsize);
		ballList = new BallList (box.gameobj.transform, boxsize, numballs);
	}

	void FixedUpdate(){
		ballList.IntegrateMotion (Time.fixedTime);
	}
}

public class Box{

	public GameObject gameobj = new GameObject ();
	public Wall[] walls = new Wall[6]; 
	public Edge[] edges = new Edge[12];
	public const int numWalls = 6;
	public const int numEdges = 12;

	public Box(Transform parent, float boxsize){
		gameobj.name = "Box";
		gameobj.transform.parent = parent;

		// Position and size parameters for the walls
		float width = 0.5f;
		float distance = boxsize + width / 2;
		float[] wallx = new float[numWalls]{distance,-distance,0,0,0,0};
		float[] wally = new float[numWalls]{0,0,distance,-distance,0,0};
		float[] wallz = new float[numWalls]{0,0,0,0,distance,-distance};	
		float[] wallsizex = new float[numWalls]{width,width,2*boxsize,2*boxsize,2*boxsize,2*boxsize};
		float[] wallsizey = new float[numWalls]{2*boxsize,2*boxsize,width,width,2*boxsize,2*boxsize,};
		float[] wallsizez = new float[numWalls]{2*boxsize,2*boxsize,2*boxsize,2*boxsize,width,width};
	
		// Make and add the walls
		for (int i = 0; i < numWalls; i++) {
			Vector3 position = new Vector3 (wallx [i], wally [i], wallz [i]);
			Vector3 size = new Vector3 (wallsizex [i], wallsizey [i], wallsizez [i]);
			walls [i] = new Wall (gameobj.transform, position, size);
		}

		// Position, rotation, and size parameters for the edges
		float[] edgex = new float[numEdges]{distance,distance,distance,distance,-distance,-distance,-distance,-distance, 0, 0, 0, 0};
		float[] edgey = new float[numEdges]{distance,-distance,0,0,distance,-distance,0,0,distance,distance,-distance,-distance};
		float[] edgez = new float[numEdges]{0,0,distance,-distance,0,0,distance,-distance,distance,-distance,distance,-distance};
		float[] edgerotx = new float[numEdges]{ 90, 90, 0, 0, 90, 90, 0, 0, 0, 0, 0, 0 };
		float[] edgerotz = new float[numEdges]{ 0, 0, 0, 0, 0, 0, 0, 0, 90, 90, 90, 90 };
		Vector3 edgesize = new Vector3 (width, 2*(boxsize+width), width);

		// Make and add the edges
		for (int i = 0; i < numEdges; i++) {
			Vector3 position = new Vector3 (edgex [i], edgey [i], edgez [i]);
			Vector3 rotation = new Vector3 (edgerotx [i], 0, edgerotz [i]);
			edges [i] = new Edge (gameobj.transform, position, rotation, edgesize);
		}
	}

}

public class Wall{
	public GameObject gameobj = GameObject.CreatePrimitive(PrimitiveType.Cube); // create cube

	public Wall(Transform parent, Vector3 position, Vector3 size){
		Renderer rd;

		gameobj.name = "Wall"; // name the cube
		gameobj.transform.parent = parent; // set as child
		gameobj.transform.position = position; // set the position
		gameobj.transform.localScale = size; // set the size

		rd = gameobj.GetComponent<Renderer> (); // get renderer
		rd.material.EnableKeyword("_ALPHATEST_ON"); // turn on transparency
		rd.material.color = new Color(0f ,0f, 0f, 0f); // make wall transparent
	}

}

public class Edge{
	public GameObject gameobj = GameObject.CreatePrimitive(PrimitiveType.Cube); // create cube

	public Edge(Transform parent, Vector3 position, Vector3 rotation, Vector3 size){
		gameobj.name = "Edge"; // name the cube
		gameobj.transform.parent = parent; // set as child
		gameobj.transform.position = position; // set the position
		gameobj.transform.Rotate(rotation); // set the rotation
		gameobj.transform.localScale = size; // set the size
	}


}

// A single ball
public class Ball {

	public GameObject gameobj = GameObject.CreatePrimitive (PrimitiveType.Sphere); // a sphere in unity
	public Vector3 velocity = Vector3.one; // the 3d velocity of the ball

	// Ball constructor
	public Ball(Transform parent, Vector3 initposition, Vector3 initvelocity, float diameter){
		gameobj.name = "Ball"; // name the sphere
		gameobj.transform.parent = parent; // set as child
		gameobj.transform.position = initposition; // initial position
		velocity = initvelocity; // initial velocity
		gameobj.transform.localScale = Vector3.one * diameter; // set diameter
	}

	public void MoveBall(float timestep){
		gameobj.transform.position += velocity * timestep;
	}
	
}

// A collection of balls
public class BallList {

	public GameObject gameobj = new GameObject(); // an empty game object in Unity
	public List<Ball> balls = new List<Ball>(); // the list of ball objects
	public float boxsize = 5.0f; // default box size

	// BallList constructor
	public BallList(Transform parent, float inputboxsize, int numballs){
		gameobj.name = "BallList"; // name the empty game object
		gameobj.transform.parent = parent; // set as child
		boxsize = inputboxsize; // get boxsize
		AddBall (numballs);
	}

	// Create and add Balls to the list
	public void AddBall(int numballs){
		Vector3[] positions = new Vector3[numballs]; // ball positions
		Vector3[] velocities = new Vector3[numballs]; // ball velocities
		float diameter = 1.0f; // ball radius
		float maxpos = boxsize - diameter/2; // maximum initial position of balls (can't overlap wall 
		float maxvel = 1.0f;
		MakeRandomPositions(numballs, maxpos, ref positions, out diameter);
		MakeRandomVelocities (numballs, maxvel, ref velocities);

		// Create and add each Ball
		for (int i = 0; i < numballs; i++){
			balls.Add (new Ball (gameobj.transform, positions [i], velocities[i], diameter)); // add a new ball
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

	public void MakeRandomVelocities(int numvelocities, float maxvel, ref Vector3[] velocities){
	
		// Initialize total momentum vector
		Vector3 totalmomentum = Vector3.zero;

		// Generate some random velocities
		for (int i = 0; i < numvelocities; i++) {
			velocities[i] = new Vector3 (Random.Range (-maxvel, maxvel), Random.Range (-maxvel, maxvel), Random.Range (-maxvel, maxvel));
			totalmomentum = totalmomentum + velocities [i]; // Calculate total linear momentum (TODO: assumes each mass is 1)
		}

		// If there's more than one ball, correct velocities so total linear momentum is zero
		if (numvelocities > 1) {
			for (int i = 0; i < numvelocities; i++) {
				velocities [i] = velocities [i] - (totalmomentum / (float)numvelocities);
			}
		}

	}

	public void IntegrateMotion(float timestep){

		for (int i = 0; i < balls.Count; i++) {

			balls [i].MoveBall (timestep);
		}

	}

}




