using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teststststs : MonoBehaviour {
    public GameObject test;
    GameObject herro;

	// Use this for initialization
	void Start () {
        herro = Instantiate(test, Camera.main.transform.position + 2f*Camera.main.transform.forward, Camera.main.transform.localRotation) as GameObject;
        
	}
	
	// Update is called once per frame
	void Update () {
        herro.transform.GetChild(2).GetComponent<Renderer>().material.color = Color.yellow;
    }
}
