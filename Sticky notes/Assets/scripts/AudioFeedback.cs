using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class AudioFeedback : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	public void playSound()
    {
        GazeManager.Instance.HitObject.GetComponent<AudioSource>().Play();
    }
}
