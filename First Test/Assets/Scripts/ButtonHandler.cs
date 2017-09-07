using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonHandler : MonoBehaviour {

	public UserInputHandler uih;

	public void startGameButton(string newGameLevel){

		uih.getUserInput ();

		// TODO: input checking

		SceneManager.LoadScene (newGameLevel);

	}

}
