using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class TapEvent : MonoBehaviour
{
    private GestureRecognizer gestureRecognizer;
    public static bool speaking = false;
    // Use this for initialization
    void Start()
    {
        gestureRecognizer = new GestureRecognizer();
        gestureRecognizer.TappedEvent += OnTap;
        gestureRecognizer.StartCapturingGestures();

    }

    private void OnTap(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        if (speaking)
        {
            SpeechManager.StopRecording();
        }
    }
}