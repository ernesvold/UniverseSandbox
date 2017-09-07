using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour {

	public float speed = 5.0f; // speed of zooming

	private float currentz; // current z position of camera
	private Camera cam;

	// Use this for initialization
	void Start () {
		float boxsize = GameObject.Find("Box").GetComponent<WallHandler>().boxsize;
		transform.position = new Vector3 (0, 0, -4 * boxsize);

		cam = GetComponent<Camera> ();
		cam.clearFlags = CameraClearFlags.SolidColor;
		cam.backgroundColor = new Color (0.0f, 0.0f, 0.0f, 0.0f);

	}
	
	// Update is called once per frame
	void Update () {

		// Let user zoom in and out
		if (Input.GetKey (KeyCode.UpArrow)) {
			currentz = transform.position.z; // store current z position
			transform.position = new Vector3(0,0,currentz + (speed * Time.deltaTime));
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			currentz = transform.position.z; // story current z position
			transform.position = new Vector3(0,0,currentz - (speed * Time.deltaTime));
		}

	}
}
