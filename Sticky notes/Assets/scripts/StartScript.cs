using UnityEngine;
using System;
using UnityEngine.UI;
using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using HoloToolkit.Unity.InputModule;

public class StartScript : MonoBehaviour {
    public static GameObject[] texts;
    public GameObject text;

    public GameObject loadingBar;
    private GameObject bar;

    public GameObject welcomeTextPrefab;
    private GameObject welcomeText;

    public GameObject userIDPref;
    private GameObject userID;

    public bool hit;
    public RaycastHit hitInfo;
    private bool isLoading = false;
    private float timestamp = 0;
    private static float current = 0;

    // Use this for initialization
    void Start () {
        current = 0;
        if (UserScript.userId != -1)
        {
            timestamp = Time.deltaTime / 2;
        }
        else
        {
            timestamp = Time.deltaTime;
        }
        bar = Instantiate(loadingBar, Camera.main.transform.position, Camera.main.transform.rotation) as GameObject;
        bar.transform.GetChild(1).GetComponent<Image>().fillAmount = 0;
        userID = Instantiate(userIDPref, Camera.main.transform.position, Camera.main.transform.rotation) as GameObject;
        userID.GetComponentInChildren<Text>().text = "User: " + UserScript.userId;
        isLoading = true;
        if(UserScript.userId != -1)
        {
            SpeechManager.setLoginTrue();
            welcomeText = Instantiate(welcomeTextPrefab, Camera.main.transform.position, Camera.main.transform.rotation) as GameObject;
            welcomeText.GetComponentInChildren<Text>().text = "Welcome";
        }
        
    }

    /// <summary>
    /// Instantiates all the tutorials.
    /// </summary>

    void Update()
    {
        if (isLoading)
        {
            //gameObject.transform.Find("InputManager").GetComponent<KeywordManager>().enabled = false;
            if (bar.transform.GetChild(1).GetComponent<Image>().fillAmount >= 1)
            {
                Destroy(bar);
                if(welcomeText != null)
                {
                    Destroy(welcomeText);
                }
                isLoading = false;
                SpeechManager.setLoginFalse();
                DoSomething();
            }
            //Changes the fill ammount of the loading bar
            current += (20 * timestamp);
            bar.transform.GetChild(1).GetComponent<Image>().fillAmount = current / 100;
            bar.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.25f, 1f));
            bar.transform.LookAt(2f * bar.transform.position - Camera.main.transform.position);
            bar.transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, Camera.main.transform.rotation.eulerAngles.z);

            if(UserScript.userId != -1)
            {
                welcomeText.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 1f));
                welcomeText.transform.LookAt(2f * welcomeText.transform.position - Camera.main.transform.position);
                welcomeText.transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, Camera.main.transform.rotation.eulerAngles.z);
            }
            
        }
        
        hit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 20f, SpatialMappingManager.Instance.LayerMask);
        if (UserScript.userId != -1)
        {
            userID.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.88f, 0.98f, 1f));
            userID.transform.LookAt(2f * userID.transform.position - Camera.main.transform.position);
            userID.transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, Camera.main.transform.rotation.eulerAngles.z);
        }
        
    }

    /// <summary>
    /// Removes all the tutorials.
    /// </summary>
    public void Clear()
    {
        for (int i = 0; i < texts.Length; i++)
        {
            if(texts[i] != null)
            {
                Destroy(texts[i].gameObject);
                texts[i] = null;
            }
        }
    }

    /// <summary>
    /// Instantiates the guides for the application on start.
    /// </summary>
    public void DoSomething()
    {
        if (current >= 100f)
        {
            Vector3 pos = new Vector3(0f, -0.1f, 0f);
            Quaternion lockrotation = Camera.main.transform.localRotation;
            //Wait for loading to finish then instantiate tutorials depending on what we are looking at
            if (hit && !isLoading)
            {
                if(UserScript.userId != -1)
                {
                    texts = new GameObject[5];
                    texts[0] = Instantiate(text, hitInfo.point, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                    texts[1] = Instantiate(text, hitInfo.point + texts[0].transform.right * -0.3f, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                    texts[2] = Instantiate(text, hitInfo.point + texts[0].transform.right * 0.3f, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                    texts[3] = Instantiate(text, hitInfo.point + texts[0].transform.right * -0.3f + pos, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                    texts[4] = Instantiate(text, hitInfo.point + pos, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                    texts[0].GetComponentInChildren<Text>().text = "To create a group\nsay \"create group\"";
                    texts[1].GetComponentInChildren<Text>().text = "To join a group\nsay \"join group\"";
                    texts[2].GetComponentInChildren<Text>().text = "To share a workspace\nsay \"share workspace\"";
                    texts[3].GetComponentInChildren<Text>().text = "To get a workspace\nsay \"get workspace\"";
                    texts[4].GetComponentInChildren<Text>().text = "To hide the guide\nsay \"close guide\"";
                }
                else
                {
                    texts = new GameObject[7];
                    texts[0] = Instantiate(text, hitInfo.point, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                    texts[1] = Instantiate(text, hitInfo.point + texts[0].transform.right * -0.3f, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                    texts[2] = Instantiate(text, hitInfo.point + texts[0].transform.right * 0.3f, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                    texts[3] = Instantiate(text, hitInfo.point + pos, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                    texts[4] = Instantiate(text, hitInfo.point + texts[0].transform.right * -0.3f + pos, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                    texts[5] = Instantiate(text, hitInfo.point + texts[0].transform.right * 0.3f + pos, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                    texts[6] = Instantiate(text, hitInfo.point + pos + pos + texts[0].transform.right * -0.3f, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                    texts[0].GetComponentInChildren<Text>().text = "To edit a note\nsay \"edit note\"";
                    texts[1].GetComponentInChildren<Text>().text = "To create a note\nsay \"create note\"";
                    texts[2].GetComponentInChildren<Text>().text = "To remove a note\nsay \"remove note\"";
                    texts[3].GetComponentInChildren<Text>().text = "To open workspace menu\nsay \"open menu\"";
                    texts[4].GetComponentInChildren<Text>().text = "To create a workspace\nsay \"create workspace\"";
                    texts[5].GetComponentInChildren<Text>().text = "For further functions you\nhave to login, say login";
                    texts[6].GetComponentInChildren<Text>().text = "To hide the guide\nsay \"close guide\"";
                }
            }
            else if (!isLoading)
            {
                if(UserScript.userId != -1)
                {
                    texts = new GameObject[5];
                    texts[0] = Instantiate(text, Camera.main.transform.position + 2f * Camera.main.transform.forward, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                    texts[1] = Instantiate(text, Camera.main.transform.position + 2f * Camera.main.transform.forward + texts[0].transform.right * -0.3f, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                    texts[2] = Instantiate(text, Camera.main.transform.position + 2f * Camera.main.transform.forward + texts[0].transform.right * 0.3f, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                    texts[3] = Instantiate(text, Camera.main.transform.position + 2f * Camera.main.transform.forward + texts[0].transform.right * -0.3f + pos, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                    texts[4] = Instantiate(text, Camera.main.transform.position + 2f * Camera.main.transform.forward + pos, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                    texts[0].GetComponentInChildren<Text>().text = "To create a group\nsay \"create group\"";
                    texts[1].GetComponentInChildren<Text>().text = "To join a group\nsay \"join group\"";
                    texts[2].GetComponentInChildren<Text>().text = "To share a workspace\nsay \"share workspace\"";
                    texts[3].GetComponentInChildren<Text>().text = "To get a workspace\nsay \"get workspace\"";
                    texts[4].GetComponentInChildren<Text>().text = "To hide the guide\nsay \"close guide\"";
                }
                else
                {
                    texts = new GameObject[7];
                    texts[0] = Instantiate(text, Camera.main.transform.position + 2f * Camera.main.transform.forward, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                    texts[1] = Instantiate(text, Camera.main.transform.position + 2f * Camera.main.transform.forward + texts[0].transform.right * -0.3f, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                    texts[2] = Instantiate(text, Camera.main.transform.position + 2f * Camera.main.transform.forward + texts[0].transform.right * 0.3f, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                    texts[3] = Instantiate(text, Camera.main.transform.position + 2f * Camera.main.transform.forward + pos, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                    texts[4] = Instantiate(text, Camera.main.transform.position + 2f * Camera.main.transform.forward + texts[0].transform.right * -0.3f + pos, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                    texts[5] = Instantiate(text, Camera.main.transform.position + 2f * Camera.main.transform.forward + texts[0].transform.right * 0.3f + pos, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                    texts[6] = Instantiate(text, Camera.main.transform.position + 2f * Camera.main.transform.forward + pos + pos + texts[0].transform.right * -0.3f, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                    texts[0].GetComponentInChildren<Text>().text = "To edit a note\nsay \"edit note\"";
                    texts[1].GetComponentInChildren<Text>().text = "To create a note\nsay \"create note\"";
                    texts[2].GetComponentInChildren<Text>().text = "To remove a note\nsay \"remove note\"";
                    texts[3].GetComponentInChildren<Text>().text = "To open workspace menu\nsay \"open menu\"";
                    texts[4].GetComponentInChildren<Text>().text = "To create a workspace\nsay \"create workspace\"";
                    texts[5].GetComponentInChildren<Text>().text = "For further functions you\nhave to login, say login";
                    texts[6].GetComponentInChildren<Text>().text = "To hide the guide\nsay \"close guide\"";
                }
            }
        }
    }
}
