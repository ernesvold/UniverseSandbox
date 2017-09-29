using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInputHandler : MonoBehaviour {

	public InputField numBallsField;
	public InputField boxSizeField;
	public Text errorText;
	public Toggle radiusToggle;
	public Toggle colorToggle;
	public Button restartButton;
	public Button muteButton;
	public Button quitButton;

	private int maxNumBalls = 1000; // limit on number of balls
	private float maxBoxSize = 100; // limit on box size
	private bool isMuted = false; // flag indicating whether audio is muted

	void Start () {
		
		// Attach Restart method to onClick for restart button
		restartButton.onClick.AddListener(Restart);

		// Attach Mute method to onClick for mute button
		muteButton.onClick.AddListener(Mute);

		// Attach Quit method to onClick for quit button
		quitButton.onClick.AddListener(Quit);

		// Set character validation for text input fields
		numBallsField.characterValidation = InputField.CharacterValidation.Integer;
		boxSizeField.characterValidation = InputField.CharacterValidation.Decimal;

	}

	// Restart simulation
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

		float ballRadius = 0.5f;
		int numPts1 = (int)Mathf.Ceil (Mathf.Pow (numBalls, (float)1.0 / 3));
		float minBoxSize = ballRadius * 3;

		// More input checking
		if (numBalls <= 0) {
			
			errorText.text = "Come on, you've gotta have at least one ball.";
			errorText.color = Color.red;
			return;

		}
		if (boxSize < minBoxSize) {
			
			errorText.text = "Please enter a box size of at least "+minBoxSize.ToString()+".";
			errorText.color = Color.red;
			return;

		}
		if (numBalls > maxNumBalls) {
			
			errorText.text = "Please enter a number of balls less than "+maxNumBalls.ToString()+".";
			errorText.color = Color.blue;
			return;

		}
		if (boxSize > maxBoxSize) {
			
			errorText.text = "Please enter a box size smaller than "+maxBoxSize.ToString()+".";
			errorText.color = Color.blue;
			return;

		}
			
		if ((boxSize * 2) / (numPts1 * 3) < ballRadius * 2) {
			
			errorText.text = "That's too many balls for this box size. Try a larger box or fewer balls.";
			errorText.color = Color.red;
			return;

		}

		// Restart simulation and reset camera view
		errorText.text = "Enter your custom parameters and hit Restart!";
		errorText.color = Color.black;
		GameObject.Find ("MainGameObject").GetComponent<MainScript> ().RestartSimulation (numBalls, boxSize);
		GameObject.Find ("Main Camera").GetComponent<CameraController> ().ResetCamera (boxSize);

	}

	// Mute audio
	void Mute(){

		Text buttonText;

		if (isMuted) {
			
			AudioListener.volume = 1;
			buttonText = muteButton.GetComponentInChildren<Text>();
			buttonText.text = "Mute";

		} else {
			
			AudioListener.volume = 0;
			buttonText = muteButton.GetComponentInChildren<Text>();
			buttonText.text = "Unmute";

		}

		// Update muted flag
		isMuted = !isMuted;
	}

	// Quit simulation
	void Quit(){

		Application.Quit ();

	}

}
