using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInputHandler : MonoBehaviour {

	public float boxsize = 5.0f;
	public int numballs = 5;
	public InputField boxsizeField;
	public InputField numballsField;

	void Awake(){
		DontDestroyOnLoad (transform.gameObject);
	}

	public void getUserInput(){
		boxsize = float.Parse(boxsizeField.text);
		numballs = int.Parse (numballsField.text);
	}
		
}
