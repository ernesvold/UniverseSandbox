using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	private float zoomSpeed = 5.0f; // speed of camera zoom
	private float angularSpeed = 20.0f; // speed of camera rotation
	private float minZoom = 0f; // minimum zoom distance
	private float maxZoom = 100f; // maximum zoom distance
	private float maxInclination = 60f; // maximum camera inclination
	private float minInclination = -60f; // minimum camera inclination

	void Start () {
		float boxSize = GameObject.Find ("MainGameObject").GetComponent<MainScript> ().boxSize;
		ResetCamera (boxSize);
	}

	public void ResetCamera(float boxSize){
		// Move camera back far enough to see entire box
		transform.position = new Vector3 (0, 0, -4 * boxSize);
		transform.eulerAngles = new Vector3 (0, 10, 0);

		// Set minimum and maximum zoom and zoom speed relative to box size
		minZoom = boxSize/10f; 
		maxZoom = 10f * boxSize;
		zoomSpeed = boxSize / 2;
	}

	void Update () {

		// Zoom in and out with W and S
		if (Input.GetKey (KeyCode.W)) {
			Vector3 zoom = transform.position.normalized * zoomSpeed * Time.deltaTime;
			if (transform.position.sqrMagnitude - zoom.sqrMagnitude > minZoom * minZoom) {
				transform.position -= zoom;
			}
		}

		if (Input.GetKey (KeyCode.S)) {
			Vector3 zoom = transform.position.normalized * zoomSpeed * Time.deltaTime;
			if (transform.position.sqrMagnitude + zoom.sqrMagnitude < maxZoom * maxZoom) {
				transform.position += zoom;
			}
		}

		// Rotate around y-axis with left and right arrow keys
		if (Input.GetKey (KeyCode.LeftArrow)) {
			transform.RotateAround (Vector3.zero, Vector3.up, angularSpeed*Time.deltaTime);
		}

		if (Input.GetKey (KeyCode.RightArrow)) {
			transform.RotateAround (Vector3.zero, Vector3.up, -angularSpeed*Time.deltaTime);
		}
	
		// Rotate around horizontal axis with up and down keys
		if (Input.GetKey (KeyCode.UpArrow)) {
			float inclination = Vector3.SignedAngle (Vector3.ProjectOnPlane (transform.position, Vector3.up), transform.position, transform.TransformDirection (Vector3.right));
			float angle = angularSpeed * Time.deltaTime;
			if (inclination + angle < maxInclination) {
				transform.RotateAround (Vector3.zero, transform.TransformDirection (Vector3.right), angle);
			}
		}

		if (Input.GetKey (KeyCode.DownArrow)) {
			float inclination = Vector3.SignedAngle (Vector3.ProjectOnPlane (transform.position, Vector3.up), transform.position, transform.TransformDirection (Vector3.right));
			float angle = -angularSpeed * Time.deltaTime;
			if (inclination + angle > minInclination) {
				transform.RotateAround (Vector3.zero, transform.TransformDirection (Vector3.right), angle);
			}			
		}
	}
}
