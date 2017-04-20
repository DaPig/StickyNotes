using UnityEngine;
using System;

public class DiceRotationScript : MonoBehaviour {
    public GameObject dicePrefab;
    static GameObject dice;
	// Use this for initialization
	void Start () {
        dice = Instantiate(dicePrefab, Camera.main.transform.position + 1f * Camera.main.transform.forward, dicePrefab.transform.rotation);
	}
	
    public static void rotateDice()
    {
        Debug.Log(SpeechManager.getTextSoFar().ToString());
        string text = SpeechManager.getTextSoFar();
        char last = text[text.Length - 1];
        int number = Convert.ToInt32(last);
        Debug.Log(number);
        switch (number)
        {
            case 0:
                break;
            case 1:
                dice.transform.rotation = new Quaternion(50f, -215f, 0f, 0f);
                break;
        }
    }

	// Update is called once per frame
	void Update () {
        Debug.Log(SpeechManager.getTextSoFar().ToString());
        string text = SpeechManager.getTextSoFar();
        //char last = text[text.Length - 1];
        int number = Convert.ToInt32(text);
        Debug.Log(number);
        switch (number)
        {
            case 0:
                break;
            case 1:
                float j;
                for(int i = 0; i < 5000; i++)
                {
                    j = i;
                    dice.transform.rotation = new Quaternion(0.01f*j, j*0.4f, 0f, 0f);
                }
                break;
        }
    }
}
