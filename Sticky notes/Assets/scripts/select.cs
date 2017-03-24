using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace selectnotes
{

    [System.Serializable]

    public class Notelist
    {
        public List<Note> Notes;
        public static Notelist CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<Notelist>(jsonString);
        }
    }


    [System.Serializable]

    public class Note
    {
        public string id;
        public string content;
        public string user;

        public static Note CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<Note>(jsonString);
        }
    }


    public class select : MonoBehaviour
    {
        public Text txt;
        string url = "http://libanaden.com/selectAll.php";

        public IEnumerator Start1(System.Action<Notelist> callback)
        {
            WWW www = new WWW(url);
            yield return www;
            if (www.error == null)
            {
                Notelist list = Notelist.CreateFromJSON(www.text);
                callback(list);
            }

            else
            {
                Debug.Log("ERROR: " + www.error);
            }
        }

    }

}