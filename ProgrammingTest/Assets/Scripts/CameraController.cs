using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	private float zoomspeed = 5.0f; // speed of camera zoom
	private float angularspeed = 20.0f; // speed of camera rotation
	private float minzoom = 0f; // minimum zoom distance
	private float maxzoom = 100f; // maximum zoom distance
	private float maxinclination = 60f; // maximum camera inclination
	private float mininclination = -60f; // minimum camera inclination

	void Start () {
		// Move camera back far enough to see entire box
		float boxsize = GameObject.Find ("MainGameObject").GetComponent<MainScript> ().boxsize;
		transform.position = new Vector3 (0, 0, -4 * boxsize);

		// Set minimum and maximum zoom relative to box size
		minzoom = boxsize/10f; 
		maxzoom = 10f * boxsize;
	}
	

	void Update () {

		// Zoom in and out with W and S
		if (Input.GetKey (KeyCode.W)) {
			Vector3 zoom = transform.position.normalized * zoomspeed * Time.deltaTime;
			Debug.Log ("Dist to center = "+transform.position.sqrMagnitude);
			if (transform.position.sqrMagnitude - zoom.sqrMagnitude > minzoom * minzoom) {
				transform.position -= zoom;
			}
		}

		if (Input.GetKey (KeyCode.S)) {
			Vector3 zoom = transform.position.normalized * zoomspeed * Time.deltaTime;
			Debug.Log ("Dist to center = "+transform.position.sqrMagnitude);
			if (transform.position.sqrMagnitude + zoom.sqrMagnitude < maxzoom * maxzoom) {
				transform.position += zoom;
			}
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
			float inclination = Vector3.SignedAngle (Vector3.ProjectOnPlane (transform.position, Vector3.up), transform.position, transform.TransformDirection (Vector3.right));
			float angle = angularspeed * Time.deltaTime;
			if (inclination + angle < maxinclination) {
				transform.RotateAround (Vector3.zero, transform.TransformDirection (Vector3.right), angle);
			}
		}

		if (Input.GetKey (KeyCode.DownArrow)) {
			float inclination = Vector3.SignedAngle (Vector3.ProjectOnPlane (transform.position, Vector3.up), transform.position, transform.TransformDirection (Vector3.right));
			float angle = -angularspeed * Time.deltaTime;
			if (inclination + angle > mininclination) {
				transform.RotateAround (Vector3.zero, transform.TransformDirection (Vector3.right), angle);
			}			
		}
	}
}
