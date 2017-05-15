using UnityEngine;
using System;
using UnityEngine.UI;


public class NumpadScript : MonoBehaviour {

    GameObject login;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Writes the number on the button pressed on the loginfield.
    /// </summary>
    public void OnClick()
    {
        GameObject.Find("LoginText").GetComponent<Text>().text = "";
        Text Letter = GetComponentInChildren<Text>();
        login = GameObject.Find("LoginNr");
        string text = login.GetComponent<Text>().text;
        //typing_TimeStamp = Time.time;
        login.GetComponent<Text>().text = text + Letter.text;
    }

    /// <summary>
    /// Writes the number on the button pressed on the textfield above, in app scene.
    /// </summary>
    public void OnClickWs()
    {
        GameObject WsField = GameObject.Find("WsText");
        Text Letter = GetComponentInChildren<Text>();
        string text = WsField.GetComponentInChildren<Text>().text;
        //typing_TimeStamp = Time.time;
        Debug.Log(GameObject.Find("WsText").GetComponentInChildren<Text>().text);
        WsField.GetComponentInChildren<Text>().text = text + Letter.text;
    }

    /// <summary>
    /// Removes the last given number in Login scene.
    /// </summary>
    public void backSpace()
    {
        login = GameObject.Find("LoginNr");
        if (login.GetComponent<Text>().text.Length != 0)
        {
            string text = login.GetComponent<Text>().text;
            text = text.Substring(0, text.Length - 1);
            login.GetComponent<Text>().text = text;
        }
        else
        {
            Debug.Log("Textfield Empty");
        }
    }

    /// <summary>
    /// Reemoves the last given number in app scene
    /// </summary>
    public void backSpaceWs()
    {
        GameObject WsField = GameObject.Find("WsText");
        if (WsField.GetComponentInChildren<Text>().text.Length != 0)
        {
            string text = WsField.GetComponentInChildren<Text>().text;
            text = text.Substring(0, text.Length - 1);
            WsField.GetComponentInChildren<Text>().text = text;
        }
        else
        {
            Debug.Log("Textfield Empty");
        }
    }

    public void EnterWs()
    {
        VoiceCommands input = GameObject.Find("InputManager").GetComponent<VoiceCommands>();
        string WsField = GameObject.Find("WsText").GetComponentInChildren<Text>().text;
        input.getWorkspace(Int32.Parse(WsField));
    }
}
