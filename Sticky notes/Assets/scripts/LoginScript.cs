
using UnityEngine;
using conn;
using System;
using UnityEngine.UI;
using connectionRoutes;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class LoginScript : MonoBehaviour
{

    private ConnectionRoutes connection;
    // Use this for initialization
    void Start()
    {
        connection = GameObject.Find("InputManager").GetComponent<ConnectionRoutes>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool checkLetters()
    {
        string pin = GameObject.Find("UsernameField").transform.GetChild(0).GetComponent<Text>().text;
        if (Regex.Matches(pin, @"[a-zA-Z]").Count == 0)
        {
            return true;
        }
        return false;
    }

    public bool checkLength()
    {
        if(GameObject.Find("UsernameField").transform.GetChild(0).GetComponent<Text>().text.Length > 4)
        {
            return false; 
        }
        return true;
    }

    public void loginUser()
    {
        if (!checkLength() || !checkLetters())
        {
            GameObject.Find("ErrorText").GetComponent<Text>().text = "Pin entered was too long or contains letters!";
            return;
        }
        string user = GameObject.Find("UsernameField").transform.GetChild(0).GetComponent<Text>().text;
        connection.loginUser(Convert.ToInt32(user));
        if (connection.exist)
        {
            SpeechManager.setLoginFalse();
            UserScript.setUser(Convert.ToInt32(user));
            SceneManager.LoadScene("App", LoadSceneMode.Single);
        }
        
    }
}