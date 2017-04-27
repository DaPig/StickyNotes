
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
        string text = Regex.Replace(GameObject.Find("UsernameField").transform.GetChild(0).GetComponent<Text>().text, " ", "");
        GameObject.Find("UsernameField").transform.GetChild(0).GetComponent<Text>().text = text;

        if (GameObject.Find("UsernameField").transform.GetChild(0).GetComponent<Text>().text.Length > 4)
        {
            return false; 
        }
        return true;
    }

    public void loginUser()
    {
        if (!checkLength() || !checkLetters())
        {
            error.SetActive(true);
            error.GetComponentInChildren<Text>().text = "Pin entered was too long or contains letters!";
            return;
        }
        if(GameObject.Find("UsernameField").transform.GetChild(0).GetComponent<Text>().text.Length == 0)
        {
            error.SetActive(true);
            error.GetComponentInChildren<Text>().text = "No pin was entered!";
            return;
        }
        string user = GameObject.Find("UsernameField").transform.GetChild(0).GetComponent<Text>().text;
        StartCoroutine(dbconnection.checkUser((ifExist) =>
        {
            if(ifExist) {
                error.SetActive(false);
                UserScript.setUser(Convert.ToInt32(user));
                SceneManager.LoadScene("App", LoadSceneMode.Single);
            }
            
        }, Convert.ToInt32(user)));
        
    }

    public void createUser()
    {
        int user = UnityEngine.Random.Range(0, 10000);
        Debug.Log(user);
        StartCoroutine(dbconnection.insertUser((ifExist) =>
        {
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

    public void back()
    {
        login.SetActive(true);
        create.SetActive(false);
    }
}