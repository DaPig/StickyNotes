using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class AudioFeedback : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    /// <summary>
    /// Plays a soundclip for audiofeedback.
    /// </summary>
    public void playSound()
    {
        GazeManager.Instance.HitObject.GetComponent<AudioSource>().Play();
    }
}
