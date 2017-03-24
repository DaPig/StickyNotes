using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using conn;
//using System.Timers;

public class KeyBoardOutput : MonoBehaviour {
    private static GameObject keyboardText;
    private static GameObject notepad;
    private static GameObject keyboard;
    private static string cursor = "|";

    private connect dbconnection;

    //private float blink_TimeStamp;
    //static float typing_TimeStamp;
    //private bool cursor_set = true;
    //public static bool typing = false;

    private bool symbol = false;
    
    public void Start()
    {
        dbconnection = new connect();
    }

    public static void createKeyboard(GameObject notepadGaze)
    {
        keyboard = Instantiate(Resources.Load("KeyBoard"), Camera.main.transform.position + 1f * Camera.main.transform.forward, Quaternion.identity) as GameObject;
        keyboardText = GameObject.Find("KeyboardText");
       
        notepad = notepadGaze;
        keyboardText.GetComponentInChildren<Text>().text = notepad.GetComponentInChildren<Text>().text + cursor;
    }
    
    //Used to register the cicks of letters, symbols and space.
    //Gets the text in the text field and adds the text of the button to it.
    public void OnClick () {
        Text Letter = GetComponentInChildren<Text>();
        keyboardText = GameObject.Find("KeyboardText");
        string text = keyboardText.GetComponentInChildren<Text>().text;
        //typing_TimeStamp = Time.time;
        if (text.Length > 0)
        {
            text = text.Substring(0, text.Length - 1);
        }
        if (Letter.text.Equals("Space")) {
            keyboardText.GetComponentInChildren<Text>().text = text + " " + cursor;
        } else {
            keyboardText.GetComponentInChildren<Text>().text = text + Letter.text + cursor;
        }
	}

    //Registers the click of the Enter button, moves the text from the textfield to any text field you supply
    //then simply removes the keyboard from the view.
    public void Enter() {
        keyboardText = GameObject.Find("KeyboardText");
        Debug.Log(notepad.gameObject.name);
        VoiceCommands.keyboardCreated = false;
        string text = keyboardText.GetComponentInChildren<Text>().text;
        text = text.Substring(0, text.Length - 1);
        notepad.GetComponentInChildren<Text>().text = text;
        dbconnection.editNote(notepad.transform.parent.GetComponent<NoteCommands>().noteId.ToString(), text);
        Destroy(keyboard.gameObject);

    }

    //Changes the case of letters, from upper to lower and vice versa.
    public void changeCase() {
        //GameObject keyboard = GameObject.Find("KeyboardCanvas");
        Text[] hello = this.transform.parent.gameObject.GetComponentsInChildren<Text>();
        for(int i = 0; i < hello.Length; i++) {
            if (Regex.Matches(hello[i].text, @"[a-zåäö]").Count == 1) {
                hello[i].text = hello[i].text.ToUpper();
            } else {
                if(hello[i].text.Length == 1)
                    hello[i].text = hello[i].text.ToLower();
            }
        }
    }

    //A simple backspace function, removes the last entered character.
    public void backSpace()
    {
        keyboardText = GameObject.Find("KeyboardText");
        if (keyboardText.GetComponentInChildren<Text>().text.Length != 1)
        {
            string text = keyboardText.GetComponentInChildren<Text>().text;
            text = text.Substring(0, text.Length - 2);
            keyboardText.GetComponentInChildren<Text>().text = text + cursor;
        } else {
            Debug.Log("Textfield Empty");
        }
    }

    //Creates a new row.
    public void newRow()
    {
        keyboardText = GameObject.Find("KeyboardText");
        string text = keyboardText.GetComponentInChildren<Text>().text;
        if (text.Length > 0)
        {
            text = text.Substring(0, text.Length - 1);
        }
        keyboardText.GetComponentInChildren<Text>().text = text + "\n" + cursor; 
    }

    //Changes the letters to symbols and back.
    public void symbols()
    {
        string[] symbols = { "+", "*", "/", "=", "%", "_", "€", "£", "$", "[", "]", "#", "¤", "&", "(", ")", "{", "}",
                            "^", "¨", "~", "\"", "|", "´", "°", "<", ">", ";","½"};
        string[] letters = { "q", "w", "e", "r", "t", "y", "u", "i", "o", "p", "å", "a", "s", "d", "f", "g", "h", "j",
                            "k", "l", "ö", "ä", "z", "x", "c", "v", "b", "n", "m"};
        Text[] hello = this.transform.parent.gameObject.GetComponentsInChildren<Text>();
        int j = 0;
        for (int i = 0; i < hello.Length; i++)
        {
            if (hello[i].transform.parent.tag == "Letter" && symbol != true)
            {
                if(symbols[j].Equals("½"))
                {
                    symbol = true;
                }
                hello[i].text = symbols[j];
                j++;
            } else if (hello[i].transform.parent.tag == "Letter")
            {
                if(letters[j].Equals("m"))
                {
                    symbol = false;
                }
                hello[i].text = letters[j];
                j++;
            }
        }
    } 

    //Will be used to navigate the text string so you can edit the text more freely.
    //Needs to be implemented.
    public void navigate()
    {
        keyboardText = GameObject.Find("KeyboardText");
        string text = keyboardText.GetComponentInChildren<Text>().text;
        text = text.Substring(0, text.Length - 1);
    }

  //Used to make the Cursor blink to indicate where you are typing
  /*  public void CursorBlink()
    {
        StringBuilder sb = new StringBuilder();
        if(typing_TimeStamp > 0)
        {
            typing_TimeStamp--;
        }
        if (Time.time - blink_TimeStamp >= 0.5 && Time.time - typing_TimeStamp >= 5)
        {
            blink_TimeStamp = Time.time;
            keyboardText = GameObject.Find("KeyboardText");
            sb.Append(keyboardText.GetComponentInChildren<Text>().text);
            if (sb.Length > 0 && cursor_set == true)
            {
                cursor_set = false;
                keyboardText.GetComponentInChildren<Text>().text = keyboardText.GetComponentInChildren<Text>().text.Replace("|", "");
            }
            else
            {
                cursor_set = true;
                if (!keyboardText.GetComponentInChildren<Text>().text.Contains("|"))
                {
                    keyboardText.GetComponentInChildren<Text>().text = sb.Insert(sb.Length, cursor).ToString();
                }
            }
        }
        else
        {
            return;
        }
     }*/

    public void Update()
    {
        //CursorBlink();
       
    }

   
}

