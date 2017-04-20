using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserScript : MonoBehaviour {
    public static int userId = -1;
	// Use this for initialization
	void Start () {
		
	}
    
    public static void setUser(int user)
    {
        Debug.Log(user);
        userId = user;
    }
}
