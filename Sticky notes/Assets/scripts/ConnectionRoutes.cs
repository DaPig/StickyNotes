using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using conn;
using selectnotes;

namespace connectionRoutes
{
    public class ConnectionRoutes : MonoBehaviour
    {

        private connect dbconnection;
        private select dbselect;

        // Use this for initialization
        void Start()
        {
            dbconnection = new connect();
            dbselect = new select();
        }

        public void createUser(string user, string pw, string mail)
        {
            dbconnection.insertUser(user, pw, mail);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

