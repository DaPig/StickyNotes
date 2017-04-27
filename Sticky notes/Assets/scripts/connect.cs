using System.Collections;
using UnityEngine;
using System.Text.RegularExpressions;

namespace conn
{

    [System.Serializable]

    public class connect
    {
        string deleteURL = "http://libanaden.com/deleteNote.php";
        string insertURL = "http://libanaden.com/insertData.php";
        string editURL = "http://libanaden.com/editnote.php";
        string createUserURL = "http://libanaden.com/CreateUser.php";
        string checkUserURL = "http://libanaden.com/checkUser.php";
        public string id;


        public IEnumerator insertString(System.Action<string> callback, string note, int user)
        {
            WWWForm form = new WWWForm();
            form.AddField("user_id", user);
            form.AddField("content", note);
            WWW www = new WWW(insertURL, form);
            yield return www;
            callback(www.text);
        }

        public void deleteNote(string id)
        {
            WWWForm form = new WWWForm();
            form.AddField("id", id);
            WWW www = new WWW(deleteURL, form);
        }
		
		public void editNote(string id, string newContent)
        {
            WWWForm form = new WWWForm();
            form.AddField("id", id);
            form.AddField("content", newContent);
            WWW www = new WWW(editURL, form);
        }

        public IEnumerator insertUser(System.Action<bool> callback, int user)
        {
            WWWForm form = new WWWForm();
            form.AddField("usernumber", user);
            WWW www = new WWW(createUserURL, form);
            yield return www;
            string test;
            if(www.text.Length > 4)
            {
               test = www.text.Remove(4);
            } else
            {
                test = www.text;
            }
           
          
            if (test == "true")
            {
                callback(true);
            } else
            {
                callback(false);
            }
        }

        public IEnumerator checkUser(System.Action<bool> callback, int user)
        {
            WWWForm form = new WWWForm();
            form.AddField("usernumber", user);
            WWW www = new WWW(checkUserURL, form);       
            yield return www;
            if (www.text == "true")
            {
                callback(true);
            }else
            {
                callback(false);
            }
        }

    }

}