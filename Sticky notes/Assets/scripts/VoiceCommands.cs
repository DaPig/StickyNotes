using UnityEngine;
using System;
using HoloToolkit.Unity.InputModule;
using UnityEngine.UI;
using conn;
using selectnotes;

public class VoiceCommands : MonoBehaviour
{
    public GameObject Notepad;
    public GameObject keyboard;
    public static bool keyboardCreated = false;
    private connect dbconnection;
    private select dbselect;

    // Use this for initialization

    public void Start()
    {
        dbconnection = new connect();
        dbselect = new select();
        
    }

    /// <summary>
    /// Instantiates a keyboard and lets you write text to a note.
    /// </summary>
    public void editNote()  
    {
        if (GazeManager.Instance.IsGazingAtObject && !keyboardCreated)
        {
            keyboardCreated = true;
            KeyBoardOutput.createKeyboard(GazeManager.Instance.HitObject.transform.GetChild(0).GetChild(0).gameObject);
        }

    }

    /// <summary>
    /// Instantiates a new note where you are currently looking.
    /// Adds the note to the server and gives it an ID.
    /// </summary>
    public void makeNew()
    {
        StartCoroutine(dbconnection.insertString((id) =>
        {
            Quaternion lockrotation = Camera.main.transform.localRotation;
            GameObject notepad = Instantiate(Notepad, Camera.main.transform.position + 2f * Camera.main.transform.forward, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
            notepad.GetComponentInChildren<NoteCommands>().noteId = Int32.Parse(id);
        }, ""));

    }

    /// <summary>
    /// Removes the note you are currently gazing at.
    /// Will also remove the note from the server.
    /// </summary>
    public void deleteNote()
    {
        if (GazeManager.Instance.IsGazingAtObject)
        {
            Destroy(GazeManager.Instance.HitObject.gameObject);
        }
        dbconnection.deleteNote(GazeManager.Instance.HitObject.GetComponentInChildren<NoteCommands>().noteId.ToString());
    }

    /// <summary>
    /// Gets all notes from the server and display them
    /// infront of you.
    /// </summary>
    public void getNotes()
    {
        StartCoroutine(dbselect.Start1((note) => {
            GameObject notepad;
            for (int i = 0; i < note.Notes.Count; i++)
            {
                notepad = Instantiate(Notepad, Camera.main.transform.position + Camera.main.transform.right*i*0.3f + 2f * Camera.main.transform.forward, Camera.main.transform.localRotation) as GameObject;
                notepad.transform.GetChild(0).GetChild(0).GetComponentInChildren<Text>().text = note.Notes[i].content;
                notepad.GetComponentInChildren<NoteCommands>().noteId = i + 1;
            }
        }));
      

    }
}
