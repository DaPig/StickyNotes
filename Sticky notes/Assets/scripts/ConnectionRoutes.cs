using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using conn;
using selectnotes;

namespace connectionRoutes
{


    public class ConnectionRoutes : MonoBehaviour
    {
        public bool exist;
        private connect dbconnection;
        private select dbselect;

        // Use this for initialization
        void Start()
        {
            dbconnection = new connect();
            dbselect = new select();
        }

        public void createUser(int user)
        {
            dbconnection.insertUser(user);
        }

        public void loginUser(int user)
        {
            StartCoroutine(dbconnection.checkUser((ifExist) =>
            {
                exist = ifExist;
            }, user));
        }

    }
}

