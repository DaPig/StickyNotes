using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using conn;
using System;
using UnityEngine.UI;
using connectionRoutes;

public class CreateUser : MonoBehaviour, IInputClickHandler  {

    private ConnectionRoutes connection;
	// Use this for initialization
	void Start () {
        connection = new ConnectionRoutes();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnInputClicked(InputEventData eventData)
    {
        if(GazeManager.Instance.HitObject.gameObject.GetType() == typeof(Text))
        {
            Debug.Log(GazeManager.Instance.HitObject.gameObject.name);
            GazeManager.Instance.HitObject.GetComponent<Text>().text = "";
            KeyBoardOutput.createKeyboard(GazeManager.Instance.HitObject.gameObject);
        }
    }

    public void submitUser()
    {
        string user = GameObject.Find("UsernameField").transform.GetChild(0).GetComponent<Text>().text;
        string password = GameObject.Find("PasswordField").transform.GetChild(0).GetComponent<Text>().text;
        connection.createUser(user, password, "OhShitThis THIS WAS COOL!");
        
    }
}