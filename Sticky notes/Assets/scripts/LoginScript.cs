
using UnityEngine;
using conn;
using System;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class LoginScript : MonoBehaviour
{

    private connect dbconnection;
    private GameObject create;
    private GameObject login;
    private GameObject error;

    // Use this for initialization
    void Start()
    {
        dbconnection = new connect();
        create = GameObject.Find("UserCreationScreen");
        create.SetActive(false);
        login = GameObject.Find("LoginScreen");
        error = GameObject.Find("ErrorBar");
        error.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Checks if the user pin entered consists of any letters.
    /// </summary>
    ///<returns>True if there is only numbers false otherwise.</returns>
    public bool checkLetters()
    {
        string pin = GameObject.Find("UsernameField").transform.GetChild(0).GetComponent<Text>().text;
        if (Regex.Matches(pin, @"[a-zA-Z]").Count == 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks the length of the user pin that was entered.
    /// </summary>
    /// <returns>True if the length is less than 4 otherwise false.</returns>
    public bool checkLength()
    {
        string text = Regex.Replace(GameObject.Find("UsernameField").transform.GetChild(0).GetComponent<Text>().text, " ", "");
        GameObject.Find("UsernameField").transform.GetChild(0).GetComponent<Text>().text = text;

        if (GameObject.Find("UsernameField").transform.GetChild(0).GetComponent<Text>().text.Length > 4)
        {
            return false; 
        }
        return true;
    }

    /// <summary>
    /// Will log the user into the application.
    /// </summary>
    public void loginUser()
    {
        //If pin contains letters or is too long, display error message to user
        if (!checkLength() || !checkLetters())
        {
            error.SetActive(true);
            error.GetComponentInChildren<Text>().text = "Pin entered was too long or contains letters!";
            return;
        }

        //If no pin was entered, display error message to user
        if(GameObject.Find("UsernameField").transform.GetChild(0).GetComponent<Text>().text.Length == 0)
        {
            error.SetActive(true);
            error.GetComponentInChildren<Text>().text = "No pin was entered!";
            return;
        }
        string user = GameObject.Find("UsernameField").transform.GetChild(0).GetComponent<Text>().text;
        //Check if the user exists or not, if yes log the user in.
        StartCoroutine(dbconnection.checkUser((ifExist) =>
        {
            if(ifExist) {
                error.SetActive(false);
                UserScript.setUser(Convert.ToInt32(user));
                SceneManager.LoadScene("App", LoadSceneMode.Single);
            } else
            {
                error.GetComponentInChildren<Text>().text = "The user pin you entered does not exist";
            }
            
        }, Convert.ToInt32(user)));
        
    }

    /// <summary>
    /// Creates a user pin for the user.
    /// </summary>
    public void createUser()
    {
        //Creates a user pin using random
        int user = UnityEngine.Random.Range(0, 10000);
        Debug.Log(user);
        StartCoroutine(dbconnection.insertUser((ifExist) =>
        {
            //checks if the user pin already exists, if it does, generate a new one, otherwise display the pin to the user
            if (ifExist)
            {
                createUser();
            }
            else
            {
                login.SetActive(false);
                create.SetActive(true);
                create.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = user.ToString();
            }
        }, user));
    }

    /// <summary>
    /// Used to go back from the user creation screen.
    /// </summary>
    public void back()
    {
        login.SetActive(true);
        create.SetActive(false);
    }
}