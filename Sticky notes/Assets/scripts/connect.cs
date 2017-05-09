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
        string insertWorkspaceURL = "http://libanaden.com/insertWorkspace.php";
        string saveWorkspaceURL = "http://libanaden.com/saveWorkspace.php";
        string saveHeaderURL = "http://libanaden.com/insertHeader.php";
        public string id;

        /// <summary>
        /// Inserts a note into the database using WWW form.
        /// sends a callback with the note id.
        /// </summary>
        public IEnumerator insertString(System.Action<string> callback, string note, int user)
        {
            WWWForm form = new WWWForm();
            form.AddField("user_id", user);
            form.AddField("content", note);
            WWW www = new WWW(insertURL, form);
            yield return www;
            callback(www.text);
        }

        /// <summary>
        /// Removes a note from the database.
        /// </summary>
        public void deleteNote(string id)
        {
            WWWForm form = new WWWForm();
            form.AddField("id", id);
            WWW www = new WWW(deleteURL, form);
        }

        /// <summary>
        /// Edits the text that is written on the note.
        /// </summary>
        public void editNote(string id, string newContent)
        {
            WWWForm form = new WWWForm();
            form.AddField("id", id);
            form.AddField("content", newContent);
            WWW www = new WWW(editURL, form);
        }

        /// <summary>
        /// Insert our created user into the database.
        /// </summary>
        public IEnumerator insertUser(System.Action<bool> callback, int user)
        {
            WWWForm form = new WWWForm();
            form.AddField("usernumber", user);
            WWW www = new WWW(createUserURL, form);
            yield return www;
            if(www.text.Contains("true"))
            {
                callback(true);
            } else
            {
                callback(false);
            }
        }

        /// <summary>
        /// Checks if the user is in the database or not.
        /// </summary>
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

        public IEnumerator insertWorkspace(System.Action<string> callback)
        {
            WWWForm form = new WWWForm();
            WWW www = new WWW(insertWorkspaceURL, form);
            yield return www;
            Debug.Log(www.text);
            callback(www.text);
        }

        public IEnumerator saveWs(int note_id, int ws_id)
        {
            WWWForm form = new WWWForm();
            form.AddField("note_id", note_id);
            form.AddField("ws_id", ws_id);
            WWW www = new WWW(saveWorkspaceURL, form);
            yield return www;
            Debug.Log(www.text);

        }

        public void saveHeader(int ws_id, string header_text, string position)
        {
            WWWForm form = new WWWForm();
            form.AddField("ws_id", ws_id);
            form.AddField("header_text", header_text);
            form.AddField("position", position);
            WWW www = new WWW(saveHeaderURL, form);
        }
    }
}