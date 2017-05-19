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
        string saveHeaderPosURL = "http://libanaden.com/insertHeaderPos.php";
        string saveHeaderTextURL = "http://libanaden.com/insertHeaderText.php";
        string saveNotePosURL = "http://libanaden.com/insertNotePos.php";
        string removeNoteRelationURL = "http://libanaden.com/removeConsistsOf.php";
        string getWorkspaceURL = "http://libanaden.com/getWorkspace.php";
        string setWorkspaceSizeURL = "http://libanaden.com/insertWorkspaceSize.php";
        string checkWorkspaceSizeURL = "http://libanaden.com/checkWorkspaceSize.php";
        string addUserToGroupURL = "http://libanaden.com/addUserToGroup.php";
        string addUserToWorkspaceURL = "http://libanaden.com/addUserToWorkspace.php";
        string addWsToGroupURL = "http://libanaden.com/addWsToGroup.php";
        string createGroupURL = "http://libanaden.com/createGroup.php";

        public string id;

        public static bool sizeUpdate = false;

        /// <summary>
        /// Inserts a note into the database using WWW form.
        /// sends a callback with the note id.
        /// </summary>
        public IEnumerator insertNote(System.Action<string> callback, string note, int user)
        {
            WWWForm form = new WWWForm();
            form.AddField("user_id", user);
            form.AddField("content", note);
            WWW www = new WWW(insertURL, form);
            yield return www;
            callback(www.text);
        }

        /// <summary>
        /// Saves the notes current position in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="position"></param>
        public void saveNotePos(int id, string position)
        {
            WWWForm form = new WWWForm();
            form.AddField("id", id);
            form.AddField("position", position);
            WWW www = new WWW(saveNotePosURL, form);
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

        /// <summary>
        /// Inserts the created workspace on the database.
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IEnumerator insertWorkspace(System.Action<string> callback)
        {
            WWWForm form = new WWWForm();
            WWW www = new WWW(insertWorkspaceURL, form);
            yield return www;
            callback(www.text);
        }

        /// <summary>
        /// Saves the current workspace and updates the values on the database.
        /// </summary>
        /// <param name="note_id"></param>
        /// <param name="ws_id"></param>
        public void saveWs(int note_id, int ws_id)
        {
            WWWForm form = new WWWForm();
            form.AddField("note_id", note_id);
            form.AddField("ws_id", ws_id);
            WWW www = new WWW(saveWorkspaceURL, form);
        }

        /// <summary>
        /// Saves the header that is attached to a specific workspace on the database.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="ws_id"></param>
        /// <param name="header_text"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public IEnumerator saveHeader(System.Action<string> callback, int ws_id, string header_text, string position)
        {
            WWWForm form = new WWWForm();
            form.AddField("ws_id", ws_id);
            form.AddField("header_text", header_text);
            form.AddField("position", position);
            WWW www = new WWW(saveHeaderURL, form);
            yield return www;
            callback(www.text);
        }

        /// <summary>
        /// Updates the content of header in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="header_text"></param>
        public void saveHeaderText(int id, string header_text)
        {
            WWWForm form = new WWWForm();
            form.AddField("id", id);
            form.AddField("header_text", header_text);
            WWW www = new WWW(saveHeaderTextURL, form);

        }

        /// <summary>
        /// Updates the headers position on the database.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public IEnumerator saveHeaderPos(int id, string position)
        {
            WWWForm form = new WWWForm();
            form.AddField("id", id);
            form.AddField("position", position);
            WWW www = new WWW(saveHeaderPosURL, form);
            yield return www;
        }

        /// <summary>
        /// Removes the relation between a note and a workspace.
        /// </summary>
        /// <param name="note_id"></param>
        /// <returns></returns>
        public IEnumerator removeNoteRelation(int note_id)
        {
            WWWForm form = new WWWForm();
            form.AddField("note_id", note_id);
            WWW www = new WWW(removeNoteRelationURL, form);
            yield return www;
        }

        /// <summary>
        /// Save the workspace width and height.
        /// </summary>
        /// <param name="ws_id"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void saveWorkspaceSize(int ws_id, string width, string height)
        {
            WWWForm form = new WWWForm();
            form.AddField("ws_id", ws_id);
            form.AddField("width", width);
            form.AddField("height", height);
            WWW www = new WWW(setWorkspaceSizeURL, form);
        }

        public void addUserToGroup(int user_id, int group_id)
        {
            WWWForm form = new WWWForm();
            form.AddField("user_id", user_id);
            form.AddField("group_id", group_id);
            WWW www = new WWW(addUserToGroupURL, form);
        }

        public void addUserToWorkspace(int ws_id, int user_id)
        {
            WWWForm form = new WWWForm();
            form.AddField("ws_id", ws_id);
            form.AddField("user_id", user_id);
            WWW www = new WWW(addUserToWorkspaceURL, form);
        }

        public void addWsToGroup(int ws_id, int group_id)
        {
            WWWForm form = new WWWForm();
            form.AddField("ws_id", ws_id);
            form.AddField("group_id", group_id);
            WWW www = new WWW(addWsToGroupURL, form);
        }

        public IEnumerator createGroup(int user_id)
        {
            WWWForm form = new WWWForm();
            form.AddField("user_id", user_id);
            WWW www = new WWW(createGroupURL, form);
            yield return www;
        }

    }
}