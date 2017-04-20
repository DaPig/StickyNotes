using UnityEngine;
using System;
using UnityEngine.UI;
using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using HoloToolkit.Unity.InputModule;

public class StartScript : MonoBehaviour {
    public static GameObject[] texts;
    public GameObject text;
    public bool hit;
    public RaycastHit hitInfo;
    private bool once = false;
    // Use this for initialization
    void Start () {
        Init();
    }

    /// <summary>
    /// Instantiates all the tutorials.
    /// </summary>
    public void Init()
    {
        StartCoroutine(DoSomething());
    }

    void Update()
    {
        hit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 20f, SpatialMappingManager.Instance.LayerMask);
        /*if(texts[0] != null)
        {
            texts[0].transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.3f, 0.2f, 1f));
            texts[0].transform.LookAt(2f * texts[0].transform.position - Camera.main.transform.position);
            texts[0].transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, Camera.main.transform.rotation.eulerAngles.z);
        }
            
        if(texts[1] != null)
        {
            texts[1].transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.2f, 1f));
            texts[1].transform.LookAt(2f * texts[1].transform.position - Camera.main.transform.position);
            texts[1].transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, Camera.main.transform.rotation.eulerAngles.z);
        }
            
        if(texts[2] != null)
        {
            texts[2].transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.7f, 0.2f, 1f));
            texts[2].transform.LookAt(2f * texts[2].transform.position - Camera.main.transform.position);
            texts[2].transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, Camera.main.transform.rotation.eulerAngles.z);
        }*/

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

    IEnumerator DoSomething()
    {
        if (!once)
        {
            yield return new WaitForSeconds(5);
            once = true;
        }
        if (hit)
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
    }
}
