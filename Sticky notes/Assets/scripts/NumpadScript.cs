using UnityEngine;
using System;
using UnityEngine.UI;
using conn;
using System.Collections;

public class NumpadScript : MonoBehaviour {

    GameObject login;

    private connect dbconnection;

    public GameObject infoTextPrefab;
    private GameObject infoText;

    public LayerMask myLayerMask;
    // Use this for initialization
    void Start () {
        dbconnection = new connect();
	}
	
	// Update is called once per frame
	void Update () {
        if (infoText != null)
        {
            infoText.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 1f));
            infoText.transform.LookAt(2f * infoText.transform.position - Camera.main.transform.position);
            infoText.transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, Camera.main.transform.rotation.eulerAngles.z);
        }
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

    public void enterGroupId()
    {
        if(infoText == null)
        {
            Quaternion lockrotation = Camera.main.transform.localRotation;
            string WsField = GameObject.Find("WsText").GetComponentInChildren<Text>().text;
            dbconnection.addUserToGroup(UserScript.userId, Int32.Parse(WsField));
            GameObject.FindGameObjectWithTag("Numpad").GetComponent<Renderer>().enabled = false;
            infoText = Instantiate(infoTextPrefab, Camera.main.transform.position + 1f * Camera.main.transform.forward, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
            infoText.GetComponentInChildren<Text>().text = "Welcome to group " + WsField;
            StartCoroutine(infoErrorTime());
        }
    }

    public void enterSharedWsId()
    {
        if (infoText == null)
        {
            Quaternion lockrotation = Camera.main.transform.localRotation;
            string WsField = GameObject.Find("WsText").GetComponentInChildren<Text>().text;
            dbconnection.addWsToGroup(VoiceCommands.wsId, Int32.Parse(WsField));
            GameObject.FindGameObjectWithTag("Numpad").GetComponent<Renderer>().enabled = false;
            infoText = Instantiate(infoTextPrefab, Camera.main.transform.position + 1f * Camera.main.transform.forward, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
            infoText.GetComponentInChildren<Text>().text = "Your workspace has now been added to group " + WsField;
            StartCoroutine(infoErrorTime());
        }
    }

    public IEnumerator infoErrorTime()
    {
        yield return new WaitForSeconds(2f);
        Destroy(infoText);
        Destroy(GameObject.FindGameObjectWithTag("Numpad"));
    }
}
