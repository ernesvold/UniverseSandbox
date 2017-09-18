using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInputHandler : MonoBehaviour {

	public InputField numballsField;
	public InputField boxsizeField;
	public Text errorText;

	private Button restartButton;

	// Use this for initialization
	void Start () {
		// Get button component and attach Restart method to onClick
		restartButton = GetComponent<Button> ();
		restartButton.onClick.AddListener(Restart);

		// Set character validation for text input fields
		numballsField.characterValidation = InputField.CharacterValidation.Integer;
		boxsizeField.characterValidation = InputField.CharacterValidation.Decimal;
	}

	void Restart(){
		
		// Input checking
		if (numballsField.text == "") {
			errorText.text = "Please enter a value for number of balls.";
			errorText.color = Color.red;
			return;
		}
		if (boxsizeField.text == "") {
			errorText.text = "Please enter a value for size of box.";
			errorText.color = Color.red;
			return;
		}

		// Read user input from text fields
		int numballs = int.Parse(numballsField.text);
		float boxsize = float.Parse(boxsizeField.text);

		// More input checking
		if (numballs <= 0) {
			errorText.text = "Come on, you've gotta have at least one ball.";
			errorText.color = Color.red;
			return;
		}
		if (boxsize <= 0) {
			errorText.text = "Only positive values for box size, please.";
			errorText.color = Color.red;
			return;
		}
		if (numballs > 200) {
			errorText.text = "Please enter a number of balls less than 200.";
			errorText.color = Color.blue;
			return;
		}
		if (boxsize > 100) {
			errorText.text = "Please enter a box size smaller than 100.";
			errorText.color = Color.blue;
			return;
		}


		// Restart simulation and reset camera view
		errorText.text = "Enter your custom parameters and hit Restart!";
		errorText.color = Color.black;
		GameObject.Find ("MainGameObject").GetComponent<MainScript> ().RestartSimulation (numballs, boxsize);
		GameObject.Find ("Main Camera").GetComponent<CameraController> ().ResetCamera ();
	}
		

}
