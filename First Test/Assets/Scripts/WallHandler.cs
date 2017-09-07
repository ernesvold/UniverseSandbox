using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallHandler : MonoBehaviour {

	public float boxsize; // half the length of one wall

	private const int numWalls = 6; // number of walls
	private const int numEdges = 12; // number of edges
	private Renderer rd; 

	void Awake(){
		boxsize = GameObject.Find("UserInput").GetComponent<UserInputHandler>().boxsize; // fetch this value from user input first
	}

	// Use this for initialization
	void Start () {
		// Box wall positions and sizes
		float width = 0.5f;
		float distance = boxsize + width / 2;
		float[] xpositions = new float[numWalls]{distance,-distance,0,0,0,0};
		float[] ypositions = new float[numWalls]{0,0,distance,-distance,0,0};
		float[] zpositions = new float[numWalls]{0,0,0,0,distance,-distance};
		float[] xwidths = new float[numWalls]{width,width,2*boxsize,2*boxsize,2*boxsize,2*boxsize};
		float[] ywidths = new float[numWalls]{2*boxsize,2*boxsize,width,width,2*boxsize,2*boxsize,};
		float[] zwidths = new float[numWalls]{2*boxsize,2*boxsize,2*boxsize,2*boxsize,width,width};

		// Loop through each wall
		for (int n = 0; n < numWalls; n++) {
			GameObject wall = GameObject.CreatePrimitive (PrimitiveType.Cube); // make a face of the box
			wall.name = "Wall"; // name the wall 
			wall.transform.parent = transform; // make wall the child of the Box empty object
			wall.transform.position = new Vector3 (xpositions[n],ypositions[n],zpositions[n]); // set position of wall
			wall.transform.localScale = new Vector3 (xwidths[n],ywidths[n],zwidths[n]); // set size of wall
			rd = wall.GetComponent<Renderer> (); // get renderer
			rd.material.EnableKeyword ("_ALPHATEST_ON"); // turn on transparency
			rd.material.color = new Color(0.0f,0.0f,0.0f,0.0f); // make wall transparent
		}

		// Box edge positions and sizes
		float[] edgex = new float[numEdges]{distance,distance,distance,distance,-distance,-distance,-distance,-distance, 0, 0, 0, 0};
		float[] edgey = new float[numEdges]{distance,-distance,0,0,distance,-distance,0,0,distance,distance,-distance,-distance};
		float[] edgez = new float[numEdges]{0,0,distance,-distance,0,0,distance,-distance,distance,-distance,distance,-distance};
		float[] rotatex = new float[numEdges]{ 90, 90, 0, 0, 90, 90, 0, 0, 0, 0, 0, 0 };
		float[] rotatez = new float[numEdges]{ 0, 0, 0, 0, 0, 0, 0, 0, 90, 90, 90, 90 };

		// Loop through each edge
		for (int n = 0; n < numEdges; n++) {
			GameObject edge = GameObject.CreatePrimitive (PrimitiveType.Cube); // make an edge of the box
			edge.name = "Edge"; // name the edge
			edge.transform.parent = transform;
			edge.transform.position = new Vector3 (edgex [n], edgey [n], edgez [n]);
			edge.transform.Rotate (rotatex [n], 0, rotatez [n]); 
			edge.transform.localScale = new Vector3 (width, 2*(boxsize + width), width);
			rd = edge.GetComponent<Renderer> ();
			rd.material.color = new Color(1.0f,1.0f,1.0f,1.0f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
