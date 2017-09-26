using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainScript : MonoBehaviour {

	public int numBalls;
	public float boxSize;

	private Box box;
	private BallList ballList;

	void Start(){

		// Default initial parameters
		int initNumBalls = 6;
		float initBoxSize = 5f;

		// Begin simulation
		RestartSimulation (initNumBalls, initBoxSize);
	}

	void FixedUpdate(){

		// Run integrator
		ballList.IntegrateMotion (Time.fixedDeltaTime);

	}

	public void RestartSimulation(int newNumBalls, float newBoxSize){
		
		// Apply new user parameters
		numBalls = newNumBalls;
		boxSize = newBoxSize;

		// Destroy current box and balls, if they exist
		if (box != null) {
			Destroy (box.gameObj);
		}
		if (ballList != null){
			Destroy (ballList.gameObj);
		}

		// Create new box and balls with user parameters
		box = new Box (transform, boxSize);
		ballList = new BallList (box.gameObj.transform, boxSize, numBalls);

	}

}

public class Box{

	public GameObject gameObj = new GameObject ();

	private Wall[] walls = new Wall[6]; 
	private Edge[] edges = new Edge[12];
	private const int numWalls = 6;
	private const int numEdges = 12;

	public Box(Transform parent, float boxSize){
		gameObj.name = "Box";
		gameObj.transform.parent = parent;

		// Position and size parameters for the walls
		float width = 0.5f;
		float distance = boxSize + width / 2;
		float[] wallX = new float[numWalls]{distance,-distance,0,0,0,0};
		float[] wallY = new float[numWalls]{0,0,distance,-distance,0,0};
		float[] wallZ = new float[numWalls]{0,0,0,0,distance,-distance};	
		float[] wallSizeX = new float[numWalls]{width,width,2*boxSize,2*boxSize,2*boxSize,2*boxSize};
		float[] wallSizeY = new float[numWalls]{2*boxSize,2*boxSize,width,width,2*boxSize,2*boxSize,};
		float[] wallSizeZ = new float[numWalls]{2*boxSize,2*boxSize,2*boxSize,2*boxSize,width,width};
	
		// Make and add the walls
		for (int i = 0; i < numWalls; i++) {
			Vector3 position = new Vector3 (wallX [i], wallY [i], wallZ [i]);
			Vector3 size = new Vector3 (wallSizeX [i], wallSizeY [i], wallSizeZ [i]);
			walls [i] = new Wall (gameObj.transform, position, size);
		}

		// Position, rotation, and size parameters for the edges
		float[] edgeX = new float[numEdges]{distance,distance,distance,distance,-distance,-distance,-distance,-distance, 0, 0, 0, 0};
		float[] edgeY = new float[numEdges]{distance,-distance,0,0,distance,-distance,0,0,distance,distance,-distance,-distance};
		float[] edgeZ = new float[numEdges]{0,0,distance,-distance,0,0,distance,-distance,distance,-distance,distance,-distance};
		float[] edgeRotX = new float[numEdges]{ 90, 90, 0, 0, 90, 90, 0, 0, 0, 0, 0, 0 };
		float[] edgeRotZ = new float[numEdges]{ 0, 0, 0, 0, 0, 0, 0, 0, 90, 90, 90, 90 };
		Vector3 edgeSize = new Vector3 (width, 2*(boxSize+width), width);

		// Make and add the edges
		for (int i = 0; i < numEdges; i++) {
			Vector3 position = new Vector3 (edgeX [i], edgeY [i], edgeZ [i]);
			Vector3 rotation = new Vector3 (edgeRotX [i], 0, edgeRotZ [i]);
			edges [i] = new Edge (gameObj.transform, position, rotation, edgeSize);
		}
	}

}

// The wall of a box
public class Wall{
	
	private GameObject gameObj = GameObject.CreatePrimitive(PrimitiveType.Cube); // create cube

	public Wall(Transform parent, Vector3 position, Vector3 size){

		gameObj.name = "Wall"; // name the cube
		gameObj.transform.parent = parent; // set as child
		gameObj.transform.position = position; // set the position
		gameObj.transform.localScale = size; // set the size

		Renderer rd;
		rd = gameObj.GetComponent<Renderer> (); // get renderer 
		rd.material = Resources.Load("FrostedGlass", typeof(Material)) as Material;

	}

}

// The edge of a box
public class Edge{
	
	private GameObject gameObj = GameObject.CreatePrimitive(PrimitiveType.Cube); // create cube

	public Edge(Transform parent, Vector3 position, Vector3 rotation, Vector3 size){

		gameObj.name = "Edge"; // name the cube
		gameObj.transform.parent = parent; // set as child
		gameObj.transform.position = position; // set the position
		gameObj.transform.Rotate(rotation); // set the rotation
		gameObj.transform.localScale = size; // set the size

		Renderer rd;
		rd = gameObj.GetComponent<Renderer> (); // get renderer
		rd.material = Resources.Load("DullMetal", typeof(Material)) as Material;
		rd.material.color = Color.grey; // make edges grey
	}


}

// A single ball
public class Ball {

	public Vector3 position; // the position of the ball
	public Vector3 velocity = Vector3.one; // the 3d velocity of the ball
	public float mass = 1f; // the mass of the ball
	public Renderer rd;
	public AudioSource ballCollideAudio;

	private GameObject gameObj = GameObject.CreatePrimitive (PrimitiveType.Sphere); // a sphere in unity
	private float radius; // the radius of the ball

	// Ball constructor
	public Ball(Transform parent, Vector3 initPosition, Vector3 initVelocity, float ballRadius, float ballMass){
		gameObj.name = "Ball"; // name the sphere
		gameObj.transform.parent = parent; // set as child
		position = initPosition; // initial position
		gameObj.transform.position = position; 
		velocity = initVelocity; // initial velocity
		radius = ballRadius; // set size
		gameObj.transform.localScale = Vector3.one * 2 * radius; 
		mass = ballMass; // set mass

		rd = gameObj.GetComponent<Renderer>();
		rd.material = Resources.Load ("ShinyMetal", typeof(Material)) as Material;

		// Add audio components
		ballCollideAudio = gameObj.AddComponent<AudioSource> ();
		AudioClip ballCollideClip = Resources.Load<AudioClip> ("ballCollide");
		ballCollideAudio.clip = ballCollideClip;
	}

	// Move the ball by its velocity
	public void MoveBall(float timestep){
		position += velocity * timestep;
		gameObj.transform.position = position;
	}

	// Detect and resolve collisions with walls
	public void CheckBoundary(float boundary){

		float radius = gameObj.transform.localScale.x/2; // get the radius (only need one dimension because it's a sphere)
		float maxpos = boundary-radius; // maximum position to keep ball inside walls

		// Check x-direction boundaries
		if (Mathf.Abs(gameObj.transform.position.x) >= maxpos){ // If the ball is outside the box
			if (gameObj.transform.position.x * velocity.x > 0) { // And heading away from the box
				velocity.x *= -1; // Turn it back in the right direction
			}
		}

		// Check y-direction boundaries
		if (Mathf.Abs(gameObj.transform.position.y) >= maxpos){
			if (gameObj.transform.position.y * velocity.y > 0) {
				velocity.y *= -1;
			}
		}

		// Check z-direction boundaries
		if (Mathf.Abs(gameObj.transform.position.z) >= maxpos){
			if (gameObj.transform.position.z * velocity.z > 0) {
				velocity.z *= -1;
			}
		}

	}

	// Determine whether this ball is currently overlapping another ball
	public bool IsCollidingWith(Ball otherBall){

		// Calculate the position and velocity of other ball relative to this ball
		Vector3 relativePos = otherBall.position - position; // relative position
		Vector3 relativeVel = otherBall.velocity - velocity; // relative velocity
		float sumRadiiSquared = (radius + otherBall.radius) * (radius + otherBall.radius); // sum of ball radii, squared

		// If the balls are not overlapping, no collision
		if (relativePos.sqrMagnitude > sumRadiiSquared) {
			return false;
		}

		// If the balls are not approaching one another, no collision
		if (Vector3.Dot (relativePos, relativeVel) > 0) {
			return false;
		}

		// If none of the escape conditions have been met, the balls are colliding
		return true;
	}

	// Determine whether this ball with collide with another ball in the next timestep
	public bool WillCollideWith(Ball otherBall, float timestep){
		
		// Get the position and velocity vectors and the radii of the two balls
		Vector3 ball1pos = gameObj.transform.position; // Ball 1 position
		Vector3 ball2pos = otherBall.gameObj.transform.position; // Ball 2 position
		Vector3 ball1vel = velocity; // Ball 1 velocity
		Vector3 ball2vel = otherBall.velocity; // Ball 2 velocity
		float ball1radius = gameObj.transform.localScale.x/2; // Ball 1 radius 
		float ball2radius = otherBall.gameObj.transform.localScale.x/2; // Ball 2 radius

		// Calculate the position and velocity of Ball 2 relative to Ball 1
		Vector3 relativePos = ball2pos - ball1pos; // relative position
		Vector3 relativeVel = ball2vel - ball1vel; // relative velocity
		float edgeDistance = relativePos.magnitude - (ball1radius + ball2radius); // distance between edges of balls
		Vector3 dispVec = relativeVel * timestep; // displacement vector of Ball 2 in next timestep
		float displacement = dispVec.magnitude; // magnitude of displacement vector

		// If Ball 2 won't move the distance between the balls in the next timestep, no collision
		if (displacement < edgeDistance) {
			return false;
		}

		// If Ball 2 is not approaching Ball 1, no collision
		if (Vector3.Dot (relativePos, relativeVel) > 0) { 
			return false;
		}

		// If the distance between the balls at the moment of closest approach is greater than the sum of
		// their radii, no collision
		float centerDistance = relativePos.magnitude; // distance between centers of balls
		float closAppDisp = Vector3.Dot(dispVec.normalized, relativePos); // displacement of Ball 2 at moment of closest approach
		float closDistSquared = centerDistance * centerDistance - closAppDisp * closAppDisp; // squared distance between balls at closest approach
		float sumRadiiSquared = (ball1radius + ball2radius) * (ball1radius + ball2radius); // sum of ball radii, squared
		if (closDistSquared > sumRadiiSquared) {
			return false;
		}

		// If the displacement vector of Ball 2 is too short to reach the contact point, no collision
		float offsetSquared = sumRadiiSquared - closDistSquared; // difference between displacement at closest approach and displacement at contact
		// Check for negative number before you try to take a square root
		if (offsetSquared < 0) { 
			return false;
		}
		float contactDisp = closAppDisp - Mathf.Sqrt(offsetSquared); // displacement of Ball 2 at moment of contact
		if (contactDisp > displacement) {
			return false;
		}

		// If none of the escape conditions have been met, the balls are colliding
		return true;
			
	}
	
}

// A collection of balls
public class BallList {

	public GameObject gameObj = new GameObject(); // an empty game object in Unity

	private List<Ball> balls = new List<Ball>(); // the list of ball objects
	private Grid3D<Ball> grid;
	private float ballRadius = 0.5f;
	private float boxSize; // box size

	// BallList constructor
	public BallList(Transform parent, float inputBoxSize, int numBalls){
		gameObj.name = "BallList"; // name the empty game object
		gameObj.transform.parent = parent; // set as child
		boxSize = inputBoxSize; // get box size
		AddBalls (numBalls);
		//AddBall_Test();

		// Make spatial grid
		// Calculate cell size such that the box will contain an integer number of cells, each
		//   at least twice the diameter of a ball, if possible
		float cellSize;
		int numCellsInBoxSize = Mathf.FloorToInt (boxSize / (ballRadius * 2 * 2)); //  number of cells that in boxsize
		int numCells = 8*(numCellsInBoxSize*numCellsInBoxSize*numCellsInBoxSize); // total number of cells in box
		if (numCellsInBoxSize < 1) { // if the size of a cell is larger than boxsize, 
			cellSize = 2*boxSize; // make the box one big cell
			numCells = 1;
		} else {
			cellSize = boxSize/numCellsInBoxSize; 
		}
		List<Vector3> positions = balls.Select (o => o.position).ToList (); // make a list of the ball positions
		grid = new Grid3D<Ball> (cellSize, numCells); // create the grid
		grid.Fill (positions, balls, ballRadius); // populate the grid
	}

	// Create and add Balls to the list
	private void AddBalls(int numBalls){
		Vector3[] positions = new Vector3[numBalls]; // ball positions
		Vector3[] velocities = new Vector3[numBalls]; // ball velocities
		float[] masses = Enumerable.Repeat(1f,numBalls).ToArray(); // ball masses

		// Make positions
		MakeRandomPositions(numBalls, ref positions); // generate random positions 

		// Make velocities
		float velocityCap = 20f; // limit on velocity for viewing purposes
		float collisionMaxVel = 2*ballRadius/(Mathf.Sqrt(3)*Time.fixedDeltaTime); // limit on velocity for collision calculations
		float maxVel = Mathf.Min(velocityCap, collisionMaxVel); // use the minimum of the two
		MakeRandomVelocities (numBalls, maxVel, ref masses, ref velocities); // generate random velocities

		// Does the user want to vary the radius?
		bool varyRadii = GameObject.Find ("Panel").GetComponent<UserInputHandler> ().radiusToggle.isOn;

		// Create and add each Ball
		for (int i = 0; i < numBalls; i++){
			float radius;
			if (varyRadii) {
				radius = Random.Range (ballRadius / 5, ballRadius);
			} else {
				radius = ballRadius;
			} 
			balls.Add (new Ball (gameObj.transform, positions [i], velocities[i], radius, masses[i])); // add a new ball
		}


	}

	// Create test ball scenario
	private void AddBall_Test(){
		ballRadius = 1.0f;
		balls.Add (new Ball (gameObj.transform, new Vector3 (2, 0, 0), new Vector3 (-2, 0, 0), ballRadius, 1.0f));
		balls.Add (new Ball (gameObj.transform, new Vector3 (-2, 0, 0), new Vector3 (2, 0, 0), ballRadius, 1.0f));
		balls [0].rd.material.color = Color.black;
	}

	// Generate a list of random-looking positions
	private void MakeRandomPositions(int numPositions, ref Vector3[] positions){
		int numPts1d = (int) Mathf.Ceil(Mathf.Pow(numPositions,(float)1.0/3)); // number of points in one dimension in the grid
		numPts1d = numPts1d * 3; // add extra white space 
		int numpts2d = numPts1d * numPts1d; // number of points in one 2d slice of the grid
		int numpts3d = numpts2d * numPts1d; // total number of points in the 3d grid

		int[] indices = Enumerable.Range (0, numpts3d).ToArray (); // array of indices representing grid coordinates

		float shift = ((float)numPts1d-1)/2; // center grid at origin
		float scale = boxSize*2/(numPts1d); // scale grid to size of box

		// Shuffle array of indices (at least the first numpositions)
		PartiallyShuffleArray (numPositions, ref indices);

		// Create each coordinate, add to positions array
		int x, y, z; // x,y,z coordinate
		int index; // index corresponding to grid
		for (int i = 0; i < numPositions; i++) {
			// Select index from shuffled array
			index = indices[i];

			// Map integer to 3d coordinates
			x = numPts1d * index / numpts3d;
			y = numPts1d * (index - (x * numpts2d)) / (numpts2d);
			z = index - (x * numpts2d) - (y * numPts1d);

			// Add to positions array
			positions [i] = new Vector3 (scale*(x-shift), scale*(y-shift), scale*(z-shift));
		}

	}

	// Partially shuffle an array of integers
	private void PartiallyShuffleArray(int k, ref int[] array) {

		// Input checking
		if (k > array.Length) {
			Debug.Log ("partiallyShuffleArray: CAUTION: k is greater than length of array");
			k = array.Length;
		}

		// For the Knuth shuffle, you don't need to shuffle the last element
		if (k == array.Length) {
			k = k - 1; 
		}

		// Knuth shuffle algorithm -- shuffles first k elements in array
		for (int i = 0; i < k; i++) {
			int tmp = array [i];
			int r = Random.Range (i, array.Length);
			array [i] = array [r];
			array [r] = tmp;
		}
	
	}

	// Generate a list of random velocities with a total linear momentum of zero
	private void MakeRandomVelocities(int numVelocities, float maxVel, ref float[] masses, ref Vector3[] velocities){
	
		// Initialize total momentum vector
		Vector3 totalMomentum = Vector3.zero;

		// Generate some random velocities
		for (int i = 0; i < numVelocities; i++) {
			velocities[i] = new Vector3 (Random.Range (-maxVel, maxVel), Random.Range (-maxVel, maxVel), Random.Range (-maxVel, maxVel));
			totalMomentum = totalMomentum + masses[i]*velocities [i]; // Calculate total linear momentum 
		}

		// If there's more than one ball, correct velocities so total linear momentum is zero
		if (numVelocities > 1) {
			for (int i = 0; i < numVelocities; i++) {
				velocities [i] = (masses[i]*velocities [i] - (totalMomentum / (float)numVelocities))/masses[i];
			}
		}

	}

	// Integrate the motion of the balls, including collision detection and resolution
	public void IntegrateMotion(float timeStep){

		CheckCollisions_Grid (timeStep);

		foreach (Ball ball in balls){

			ball.CheckBoundary (boxSize);

			ball.MoveBall (timeStep);
		}

		// Update grid
		grid.Empty ();
		List<Vector3> positions = balls.Select (o => o.position).ToList (); // make a list of the ball positions
		grid.Fill (positions, balls, ballRadius);

	}

	// A laughably inefficient collision detection algorithm
	private void CheckCollisions_Direct (float timeStep){

		// For every ball in the list
		for (int i = 0; i < balls.Count - 1; i++) {

			// Check whether it's colliding with another ball
			for (int j = i + 1; j < balls.Count; j++) {
				
				if (balls[i].IsCollidingWith(balls[j])){

					CollisionResolve (balls [i], balls [j]); // Trigger collision resolution

				}

			}

		}

	}

	private void CheckCollisions_Grid (float timeStep){

		List<int> keys = grid.GetKeys();

		// For each cell in the grid
		foreach (int key in keys) {
			
			// Get a list of balls in that cell
			List<Ball> ballsInCell = grid.GetObjectsInCell(key);

			// If there is more than one ball,
			if (ballsInCell.Count > 1) {
				
				// For each ball in the cell
				for (int i = 0; i < ballsInCell.Count - 1; i++) {
					
					// Check whether it's colliding with another ball
					for (int j = i + 1; j < ballsInCell.Count; j++) {

						if (ballsInCell[i].IsCollidingWith(ballsInCell[j])){

							CollisionResolve (ballsInCell [i], ballsInCell [j]); // Trigger collision resolution

						}

					}
				}
			}
		}

	}

	// Resolve collision between two balls
	private void CollisionResolve(Ball ball1, Ball ball2){

		// Calculate normal vector of collision point
		Vector3 normal = ball2.position - ball1.position; 
		normal.Normalize ();

		// Calculate components of velocity vector along normal
		float n1 = Vector3.Dot (ball1.velocity, normal);
		float n2 = Vector3.Dot (ball2.velocity, normal);

		// Calculate new velocities to converse momentum and kinetic energy
		float deltaP = (2.0f * (n1 - n2)) / (ball1.mass + ball2.mass);
		Vector3 newVelocity1 = ball1.velocity - deltaP * ball2.mass * normal;
		Vector3 newVelocity2 = ball2.velocity + deltaP * ball1.mass * normal;
		ball1.velocity = newVelocity1;
		ball2.velocity = newVelocity2;

		// Play collision audio
		if (ball1.ballCollideAudio != null) {
			ball1.ballCollideAudio.Play ();
		} else if (ball2.ballCollideAudio != null) {
			ball2.ballCollideAudio.Play ();
		}
	}

}

// A 3D spatial hash for better collision detection
public class Grid3D<T>{

	private float cellSize; // size of each cell in the grid
	private int numCells; // total number of possible cells
	private Dictionary<int, List<T>> dict = new Dictionary<int, List<T>>(); // the spatial hash
	private int p1 = 73856093; // "Large primes" for hash function
	private int p2 = 19349669; 
	private int p3 = 83492791;       

	// Grid3D constructor
	public Grid3D(float size, int num){
		cellSize = size;
		numCells = num;
	}

	// Add (each vertex of) an object to the dict 
	public void AddObject(Vector3 position, T obj, float size){
		AddVertex (new Vector3(position.x+size, position.y+size, position.z+size), obj);
		AddVertex (new Vector3(position.x+size, position.y+size, position.z-size), obj);
		AddVertex (new Vector3(position.x+size, position.y-size, position.z-size), obj);
		AddVertex (new Vector3(position.x+size, position.y-size, position.z+size), obj);
		AddVertex (new Vector3(position.x-size, position.y+size, position.z+size), obj);
		AddVertex (new Vector3(position.x-size, position.y+size, position.z-size), obj);
		AddVertex (new Vector3(position.x-size, position.y-size, position.z-size), obj);
		AddVertex (new Vector3(position.x-size, position.y-size, position.z+size), obj);
	}

	// Add a vertex to the dict
	private void AddVertex(Vector3 vertex, T obj){

		// Convert position to key
		int key = MakeKey(vertex);

		List<T> cell;
		// If cell exists, pick that cell's list
		if (dict.ContainsKey (key)) {
			cell = dict [key];
		} 
		// If cell doesn't exist, add it to dictionary
		else {
			cell = new List<T>();
			dict.Add(key, cell);
		}
			
		// If that cell doesn't already contain the object, add it
		if (!cell.Contains (obj)) { 
			cell.Add (obj);
		} 
	}

	// Convert a position to an integer key
	private int MakeKey(Vector3 position){
		int x = Mathf.FloorToInt(position.x / cellSize);
		int y = Mathf.FloorToInt(position.y / cellSize);
		int z = Mathf.FloorToInt(position.z / cellSize);
		return (x * p1 ^ y * p2 ^ z * p3) % numCells; 
	}

	// Add a list of objects to the dict
	public void Fill(List<Vector3> posList, List<T> objList, float size){
		
		int numObj = 0;

		// Error checking - the posList and the objList must be the same length
		if (posList.Count != objList.Count) {
			Debug.Log ("WARNING: Grid3D.Add: number of positions does not equal number of objects!");
			numObj = Mathf.Min (posList.Count, objList.Count);
		} else {
			numObj = objList.Count;
		}

		// Add each object to the dict
		for (int i = 0; i<numObj; i++){
			AddObject(posList[i], objList[i], size);
		}

	}

	// Get the list of keys
	public List<int> GetKeys(){
		return dict.Keys.ToList();
	}


	// Get the list of objects corresponding to a key
	public List<T> GetObjectsInCell(int key){
		if (dict.ContainsKey (key)) {
			return dict [key];
		}
		return new List<T>();
	}

	// Empty the grid
	public void Empty(){
		dict.Clear ();
	}

}




