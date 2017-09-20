using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInputHandler : MonoBehaviour {

	public InputField numBallsField;
	public InputField boxSizeField;
	public Text errorText;

	private Button restartButton;

	// Use this for initialization
	void Start () {
		// Get button component and attach Restart method to onClick
		restartButton = GetComponent<Button> ();
		restartButton.onClick.AddListener(Restart);

		// Set character validation for text input fields
		numBallsField.characterValidation = InputField.CharacterValidation.Integer;
		boxSizeField.characterValidation = InputField.CharacterValidation.Decimal;
	}

	void Restart(){
		
		// Input checking
		if (numBallsField.text == "") {
			errorText.text = "Please enter a value for number of balls.";
			errorText.color = Color.red;
			return;
		}
		if (boxSizeField.text == "") {
			errorText.text = "Please enter a value for size of box.";
			errorText.color = Color.red;
			return;
		}

		// Read user input from text fields
		int numBalls = int.Parse(numBallsField.text);
		float boxSize = float.Parse(boxSizeField.text);

		// More input checking
		if (numBalls <= 0) {
			errorText.text = "Come on, you've gotta have at least one ball.";
			errorText.color = Color.red;
			return;
		}
		if (boxSize <= 0) {
			errorText.text = "Only positive values for box size, please.";
			errorText.color = Color.red;
			return;
		}
		if (numBalls > 2000) {
			errorText.text = "Please enter a number of balls less than 2000.";
			errorText.color = Color.blue;
			return;
		}
		if (boxSize > 100) {
			errorText.text = "Please enter a box size smaller than 100.";
			errorText.color = Color.blue;
			return;
		}


		// Restart simulation and reset camera view
		errorText.text = "Enter your custom parameters and hit Restart!";
		errorText.color = Color.black;
		GameObject.Find ("MainGameObject").GetComponent<MainScript> ().RestartSimulation (numBalls, boxSize);
		GameObject.Find ("Main Camera").GetComponent<CameraController> ().ResetCamera ();
	}
		

}
