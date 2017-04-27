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
    private bool once = false;
    private bool isLoading = false;
    private float timestamp = 0;
    private float current = 0;

    // Use this for initialization
    void Start () {

        timestamp = 0.0165f;
        bar = Instantiate(loadingBar, Camera.main.transform.position, Camera.main.transform.rotation) as GameObject;
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
            if (bar.transform.GetChild(1).GetComponent<Image>().fillAmount >= 1)
            {
                Destroy(bar);
                isLoading = false;
                SpeechManager.setLoginFalse();
                DoSomething();
            }
            current += (19 * timestamp);
            bar.transform.GetChild(1).GetComponent<Image>().fillAmount = current / 100;
            bar.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.25f, 1f));
            bar.transform.LookAt(2f * bar.transform.position - Camera.main.transform.position);
            bar.transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, Camera.main.transform.rotation.eulerAngles.z);

            if(SpeechManager.getLogin())
            {
                welcomeText.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 1f));
                welcomeText.transform.LookAt(2f * welcomeText.transform.position - Camera.main.transform.position);
                welcomeText.transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, Camera.main.transform.rotation.eulerAngles.z);
            }
        }
        
        hit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 20f, SpatialMappingManager.Instance.LayerMask);

        if (UserScript.userId != -1)
        {
            userID.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.46f, 0.57f, 1f));
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

    public void DoSomething()
    {
        Debug.Log(current);
        if (current >= 100f)
        {
            if (hit && !isLoading)
            {
                Quaternion lockrotation = Camera.main.transform.localRotation;
                texts = new GameObject[3];
                texts[0] = Instantiate(text, hitInfo.point, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                texts[1] = Instantiate(text, hitInfo.point + texts[0].transform.right * -0.2f, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                texts[2] = Instantiate(text, hitInfo.point + texts[0].transform.right * 0.2f, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                texts[0].GetComponentInChildren<Text>().text = "To edit a note\nsay \"edit note\"";
                texts[1].GetComponentInChildren<Text>().text = "To create a note\nsay \"create note\"";
                texts[2].GetComponentInChildren<Text>().text = "To remove a note\nsay \"remove note\"";
            }
            else if (!isLoading)
            {
                Quaternion lockrotation = Camera.main.transform.localRotation;
                texts = new GameObject[3];
                texts[0] = Instantiate(text, Camera.main.transform.position + 2f * Camera.main.transform.forward, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                texts[1] = Instantiate(text, Camera.main.transform.position + 2f * Camera.main.transform.forward + texts[0].transform.right * -0.2f, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                texts[2] = Instantiate(text, Camera.main.transform.position + 2f * Camera.main.transform.forward + texts[0].transform.right * 0.2f, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                texts[0].GetComponentInChildren<Text>().text = "To edit a note\nsay \"edit note\"";
                texts[1].GetComponentInChildren<Text>().text = "To create a note\nsay \"create note\"";
                texts[2].GetComponentInChildren<Text>().text = "To remove a note\nsay \"remove note\"";
            }
        }
    }
}
