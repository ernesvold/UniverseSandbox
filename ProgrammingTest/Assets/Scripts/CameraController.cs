using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	void Start () {
		float boxsize = GameObject.Find ("MainGameObject").GetComponent<MainScript> ().boxsize;
		transform.position = new Vector3 (0, 0, -4 * boxsize);
	}
	

	void Update () {
		// TODO: User-controlled camera
	}
}
