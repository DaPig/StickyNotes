using System.Collections;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using UnityEngine;
using conn;
using HoloToolkit.Unity.SpatialMapping;
using HoloToolkit.Unity.InputModule;

public class WorkspaceScript : MonoBehaviour {

    public GameObject field;
    private GameObject thingy;
    public int id;
    private connect dbconnection;

    public static bool isWaiting = false;
	// Use this for initialization
	void Start () {
        dbconnection = new connect();
        InvokeRepeating("updateSize", 0f, 1f);
	}
	
	// Update is called once per frame
	void Update()
    {
       
    }

    /// <summary>
    /// Updates the size of the workspace "realtime"
    /// </summary>
    /*private void updateSize()
    {
        StartCoroutine(dbconnection.updateWorkspaceSize((size) => {
            Regex reg = new Regex(@"\d");
            if(reg.IsMatch(size))
            {
                string[] sizes = size.Split(',');
                this.GetComponent<RectTransform>().sizeDelta = new Vector2(float.Parse(sizes[0]), float.Parse(sizes[1]));
            }     
        }, id));
    }*/

    /*
    public void addField()
    {
        GameObject button = GazeManager.Instance.HitObject.gameObject;
        Vector3 stuff = button.transform.position;
        stuff.x += 0.3f;
        //Debug.Log(stuff);
        Vector3 pos = GazeManager.Instance.HitObject.transform.position + 0.1f * GazeManager.Instance.HitObject.transform.right;
        thingy = Instantiate(field, Camera.main.transform.position, GazeManager.Instance.HitObject.transform.rotation) as GameObject;
        thingy.transform.parent = GameObject.Find("GridWithOurElementsOrOptions").transform;
        button.transform.position = stuff;

    }*/
}
