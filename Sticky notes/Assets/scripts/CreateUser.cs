
using UnityEngine;
using conn;
using System;
using UnityEngine.UI;
using connectionRoutes;
using System.Linq;
using UnityEngine.SceneManagement;

public class CreateUser : MonoBehaviour{

    private ConnectionRoutes connection;
	// Use this for initialization
	void Start () {
        connection = GameObject.Find("InputManager").GetComponent<ConnectionRoutes>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void submitUser()
    {
        string user = GameObject.Find("UsernameField").transform.GetChild(0).GetComponent<Text>().text;
        connection.createUser(Convert.ToInt32(user));
        UserScript.setUser(Convert.ToInt32(user));
        SceneManager.LoadScene("App", LoadSceneMode.Single);
    }
}