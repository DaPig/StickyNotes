using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

/// <summary>
/// Basic test without using voice commands.
/// </summary>
public class InputTestStuff : MonoBehaviour, IInputClickHandler {

    public VoiceCommands voice;

	// Use this for initialization
	void Start () {
        voice = GameObject.Find("InputManager").GetComponent<VoiceCommands>();
	}
	
	// Uncomment to run tests
	public void OnInputClicked (InputEventData e) {
        //voice.startSpeech();
	}
}
