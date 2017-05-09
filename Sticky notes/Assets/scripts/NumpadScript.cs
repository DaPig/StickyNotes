using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class NumpadScript : MonoBehaviour {

    GameObject login;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClick()
    {
        GameObject.Find("LoginText").GetComponent<Text>().text = "";
        Text Letter = GetComponentInChildren<Text>();
        login = GameObject.Find("LoginNr");
        string text = login.GetComponent<Text>().text;
        //typing_TimeStamp = Time.time;
        login.GetComponent<Text>().text = text + Letter.text;
    }

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
}
