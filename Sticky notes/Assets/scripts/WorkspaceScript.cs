using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.SpatialMapping;
using HoloToolkit.Unity.InputModule;

public class WorkspaceScript : MonoBehaviour {

    public GameObject field;
    private GameObject thingy;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

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

    }
}
