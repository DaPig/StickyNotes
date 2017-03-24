using System.Collections;
using UnityEngine;

namespace conn
{

    [System.Serializable]

    public class connect
    {
        string deleteURL = "http://libanaden.com/deleteNote.php";
        string insertURL = "http://libanaden.com/insertData.php";
        string editURL = "http://libanaden.com/editnote.php";
        public string id;


        public IEnumerator insertString(System.Action<string> callback, string note)
        {
            WWWForm form = new WWWForm();
            //form.AddField("user", user);
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
    }

}