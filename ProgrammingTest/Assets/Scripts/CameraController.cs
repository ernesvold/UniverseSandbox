using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	private float zoomspeed = 5.0f; // speed of camera zoom
	private float angularspeed = 20.0f; // speed of camera rotation

	void Start () {
		// Move camera back far enough to see entire box
		float boxsize = GameObject.Find ("MainGameObject").GetComponent<MainScript> ().boxsize;
		transform.position = new Vector3 (0, 0, -4 * boxsize);
	}
	

	void Update () {

		// Zoom in and out with W and S
		if (Input.GetKey (KeyCode.W)) {
			transform.position -= transform.position.normalized * zoomspeed * Time.deltaTime;
		}

		if (Input.GetKey (KeyCode.S)) {
			transform.position += transform.position.normalized*zoomspeed*Time.deltaTime;
		}

		// Rotate around y-axis with left and right arrow keys
		if (Input.GetKey (KeyCode.LeftArrow)) {
			transform.RotateAround (Vector3.zero, Vector3.up, angularspeed*Time.deltaTime);
		}

		if (Input.GetKey (KeyCode.RightArrow)) {
			transform.RotateAround (Vector3.zero, Vector3.up, -angularspeed*Time.deltaTime);
		}
	
		// Rotate around horizontal axis with up and down keys
		if (Input.GetKey (KeyCode.UpArrow)) {
			transform.RotateAround (Vector3.zero, transform.TransformDirection (Vector3.right), angularspeed * Time.deltaTime);
		}

		if (Input.GetKey (KeyCode.DownArrow)) {
			transform.RotateAround (Vector3.zero, transform.TransformDirection (Vector3.right), -angularspeed * Time.deltaTime);
		}
	}
}
