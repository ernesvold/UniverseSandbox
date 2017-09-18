using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour {

	public InputField numballsField;
	public InputField boxsizeField;

	private Button restartButton;

	// Use this for initialization
	void Start () {
		restartButton = GetComponent<Button> ();
		restartButton.onClick.AddListener(Restart);
	}

	void Restart(){
		// Read user input from text fields
		int numballs = int.Parse(numballsField.text);
		float boxsize = float.Parse(boxsizeField.text);

		// TODO: input checking

		// Restart simulation and reset camera view
		GameObject.Find ("MainGameObject").GetComponent<MainScript> ().RestartSimulation (numballs, boxsize);
		GameObject.Find ("Main Camera").GetComponent<CameraController> ().ResetCamera ();
	}
}
