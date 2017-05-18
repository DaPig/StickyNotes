using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace getWs
{
   
    [System.Serializable]

    public class WorkspaceList
    {
        public List<Workspace> Workspace;
        public static WorkspaceList CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<WorkspaceList>(jsonString);
        }
    }

    [System.Serializable]

    public class HeaderList
    {
        public List<Header> headerList;
        public static HeaderList CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<HeaderList>(jsonString);
        }
    }

    [System.Serializable]

    public class Workspace
    {
        public int ws_id;
        public string width;
        public string height;
        public int note_id;
        public string Note_pos;
        public string content;


        public static Workspace CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<Workspace>(jsonString);
        }
    }

    [System.Serializable]
    
    public class Header
    {
        public int ws_id;
        public int header_id;
        public string Header_pos;
        public string header_text;

        public static Header CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<Header>(jsonString);
        }
    }

    /// <summary>
    /// Gets the workspace with all its content from the database via a JSON.
    /// </summary>
    public class getWorkspace : MonoBehaviour
    {
        public Text txt;
        string url = "http://libanaden.com/getWorkspace.php";

        public IEnumerator getWS(System.Action<WorkspaceList, HeaderList> callback, int ws_id, int user_id)
        {
            WWWForm form = new WWWForm();
            form.AddField("ws_id", ws_id);
            form.AddField("user_id", user_id);
            WWW www = new WWW(url, form);
            yield return www;
            if (www.error == null)
            {
                Debug.Log(www.text);
                string[] data = www.text.Split('|');
                string headerText = data[1].Remove(data[1].Length - 2);
                WorkspaceList list = WorkspaceList.CreateFromJSON("{\"Workspace\":" + data[0] + "}");
                HeaderList headerlist = HeaderList.CreateFromJSON("{\"headerList\":" + headerText + "}");
                callback(list, headerlist);
            }

            else
            {
                Debug.Log("ERROR: " + www.error);
            }
        }

    }

}