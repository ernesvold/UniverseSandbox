using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	private float zoomspeed = 5.0f; // speed of camera zoom
	private float angularspeed = 0.5f; // speed of camera rotation

	void Start () {
		// Move camera back far enough to see entire box
		float boxsize = GameObject.Find ("MainGameObject").GetComponent<MainScript> ().boxsize;
		transform.position = new Vector3 (0, 0, -4 * boxsize);
	}
	

	void Update () {

		// Zoom in and out with W and S
		if (Input.GetKey (KeyCode.W)) {
			Vector3 zoom = transform.position.normalized*zoomspeed*Time.deltaTime;
			transform.position -= zoom;
		}

		if (Input.GetKey (KeyCode.S)) {
			Vector3 zoom = transform.position.normalized*zoomspeed*Time.deltaTime;
			transform.position += zoom;
		}

		// Rotate around y-axis with left and right arrow keys
		if (Input.GetKey (KeyCode.LeftArrow)) {
			float angle = -angularspeed*Time.deltaTime; // angle to rotate through (rad)
			float sinAngle = Mathf.Sin (angle);
			float cosAngle = Mathf.Cos (angle);
			Vector3 newpos = new Vector3 (cosAngle*transform.position.x-sinAngle*transform.position.z,0,
				sinAngle*transform.position.x+cosAngle*transform.position.z);
			transform.position = newpos;
			transform.Rotate(new Vector3(0,-angle*180/Mathf.PI,0));
		}

		if (Input.GetKey (KeyCode.RightArrow)) {
			float angle = angularspeed*Time.deltaTime;
			float sinAngle = Mathf.Sin (angle);
			float cosAngle = Mathf.Cos (angle);
			Vector3 newpos = new Vector3 (cosAngle*transform.position.x-sinAngle*transform.position.z,0,
				sinAngle*transform.position.x+cosAngle*transform.position.z);
			transform.position = newpos;
			transform.Rotate(new Vector3(0,-angle*180/Mathf.PI,0));
		}
	

	}
}
