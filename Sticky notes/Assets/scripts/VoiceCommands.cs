using UnityEngine;
using System;
using HoloToolkit.Unity.InputModule;
using UnityEngine.UI;
using conn;
using selectnotes;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using HoloToolkit.Unity.SpatialMapping;

public class VoiceCommands : MonoBehaviour
{
    public GameObject NotepadPrefab;
    public GameObject keyboardPrefab;
    public GameObject WorkspacePrefab;
    public GameObject HeaderPrefab;

    private GameObject resize;
    private GameObject adjust;

    public static bool keyboardCreated = false;

    private connect dbconnection;
    private select dbselect;

    private SpeechManager speech;
    private AudioSource audio;

    public List<GameObject> notes;

    public bool hit;
    public RaycastHit hitInfo;

    protected SpatialMappingManager spatialMappingManager;
    public LayerMask myLayerMask;

    // Use this for initialization
    public void Start()
    {
        audio = gameObject.GetComponent<AudioSource>();
        dbconnection = new connect();
        dbselect = new select();
        speech = GetComponent < SpeechManager> ();
        notes = new List<GameObject>();

    }

    void Update()
    {
        hit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 20f, SpatialMappingManager.Instance.LayerMask);
    }

    /// <summary>
    /// Instantiates a keyboard and lets you write text to a note.
    /// </summary>
    public void editNote()  
    {
        if (GazeManager.Instance.IsGazingAtObject && !keyboardCreated)
        {
            keyboardCreated = true;
            KeyBoardOutput.createKeyboard(GazeManager.Instance.HitObject.transform.GetChild(3).gameObject);
        }
    }

    /// <summary>
    /// Clear the text on the current note you're looking at.
    /// </summary>
    public void clearNote()
    {
        if (GazeManager.Instance.IsGazingAtObject)
        {
            GazeManager.Instance.HitObject.transform.GetChild(3).GetComponentInChildren<Text>().text = "";
        }
        Debug.Log(GazeManager.Instance.HitObject.transform.GetChild(3).GetComponentInChildren<Text>().text);
        dbconnection.editNote(GazeManager.Instance.HitObject.GetComponent<NoteCommands>().noteId.ToString(), "");
        SpeechManager.clearText();
    }

    /// <summary>
    /// Instantiates a new note where you are currently looking.
    /// Adds the note to the server and gives it an ID.
    /// </summary>
    public void makeNew()
    {
        int user = UserScript.userId;
       
        {
            StartCoroutine(dbconnection.insertString((id) =>
            {
                Quaternion lockrotation = Camera.main.transform.localRotation;
                GameObject notepad = null;
                GameObject workspace = null;
                Vector3 headPosition = Camera.main.transform.position;
                Vector3 gazeDirection = Camera.main.transform.forward;
                RaycastHit hitInfoTwo;
                if (Physics.Raycast(headPosition, gazeDirection, out hitInfoTwo, 30.0f, myLayerMask))
                {
                    workspace = GazeManager.Instance.HitObject.gameObject;

                    if (workspace.tag == "Workspace")
                    {
                        notepad = Instantiate(NotepadPrefab, GazeManager.Instance.HitPosition, workspace.transform.rotation) as GameObject;
                        notepad.transform.SetParent(workspace.transform);
                    }
                }
                else if (hit)
                {
                    notepad = Instantiate(NotepadPrefab, hitInfo.point + Camera.main.transform.forward * -0.05f, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                }
                else
                {
                    notepad = Instantiate(NotepadPrefab, Camera.main.transform.position + 2f * Camera.main.transform.forward, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                }
                //notepad = Instantiate(Notepad, Camera.main.transform.position + 2f * Camera.main.transform.forward, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                notepad.GetComponent<NoteCommands>().noteId = Int32.Parse(id);
                notes.Add(notepad);
            }, "", user));
            StartScript.texts[1].GetComponentInChildren<Animator>().SetBool("DoAnimation", true);
        }
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
            dbconnection.deleteNote(GazeManager.Instance.HitObject.GetComponent<NoteCommands>().noteId.ToString());
            StartScript.texts[2].GetComponentInChildren<Animator>().SetBool("DoAnimation", true);
        }
    }

    /// <summary>
    /// Starts the speech dictation to be able to use speech-to-text.
    /// </summary>
    public void startSpeech()
    {
        if (GazeManager.Instance.IsGazingAtObject)
        {
             speech.StartRecording(GazeManager.Instance.HitObject.transform.GetChild(1).gameObject);
            StartScript.texts[0].GetComponentInChildren<Animator>().SetBool("DoAnimation", true);
        }
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
                notepad = Instantiate(NotepadPrefab, Camera.main.transform.position + Camera.main.transform.right*i*0.3f + 2f * Camera.main.transform.forward, Camera.main.transform.localRotation) as GameObject;
                notepad.transform.GetChild(3).GetComponentInChildren<Text>().text = note.Notes[i].content;
                notepad.GetComponent<NoteCommands>().noteId = i + 1;
            }
        }));
    }

    /// <summary>
    /// Removes all notes from the GUI but not from the database and instantiates the tutorial again.
    /// </summary>
    public void clearAllNotes()
    {
        foreach(GameObject game in notes)
        {
            Destroy(game);
        }
        GameObject startTexts = GameObject.Find("StartManager");
        startTexts.GetComponent<StartScript>().Clear();
        startTexts.GetComponent<StartScript>().DoSomething();
    }

    /// <summary>
    /// Activates the login scene.
    /// </summary>
    public void login()
    {
        SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
    }

    public void loginUser()
    {
        GameObject.Find("UsernameField").transform.GetChild(1).GetComponent<Text>().text = "";
        speech.StartRecordingLogin(GameObject.Find("UsernameField").transform.GetChild(0).gameObject);
    }

    /// <summary>
    /// Creates a workspace where you are looking.
    /// </summary>
    public void createWorkspace()
    {
        int user = UserScript.userId;
            /*StartCoroutine(dbconnection.insertString((id) =>
            {*/
                Quaternion lockrotation = Camera.main.transform.localRotation;
                GameObject ws;

            //Checks if we are hitting a mapped surface or not
            if (hit)
            {
                ws = Instantiate(WorkspacePrefab, hitInfo.point + Camera.main.transform.forward * -0.05f, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
            }
            else
            {
                ws = Instantiate(WorkspacePrefab, Camera.main.transform.position + 2f * Camera.main.transform.forward, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
            }
        /*}, "", user));
        StartScript.texts[1].GetComponentInChildren<Animator>().SetBool("DoAnimation", true);*/
        adjust = ws.transform.GetChild(1).gameObject;
        resize = ws.transform.GetChild(0).gameObject;
        adjust.SetActive(false);
        resize.SetActive(false);
    }

    /// <summary>
    /// Saves the workspace in the database.
    /// </summary>
    public void saveWorkspace()
    {

    }

    public void enableAdjustButtons()
    {
        adjust.SetActive(true);
        resize.SetActive(true);
    }

    public void disableAdjustButtons()
    {
        adjust.SetActive(false);
        resize.SetActive(false);
    }

    public void createHeader()
    {
        GameObject workspace;
        GameObject header;
        Vector3 headPosition = Camera.main.transform.position;
        Vector3 gazeDirection = Camera.main.transform.forward;
        RaycastHit hitInfoTwo;
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfoTwo, 30.0f, myLayerMask))
        {
            workspace = GazeManager.Instance.HitObject.gameObject;

            if (workspace.tag == "Workspace")
            {
                header = Instantiate(HeaderPrefab, GazeManager.Instance.HitPosition + Camera.main.transform.forward * -0.05f, workspace.transform.rotation) as GameObject;
                header.transform.SetParent(workspace.transform);
            }
        }
    }

    public void deleteHeader()
    {
        Debug.Log("in delete" + GazeManager.Instance.HitObject.tag);
        if(GazeManager.Instance.HitObject.tag == "Header")
        {
            Destroy(GazeManager.Instance.HitObject);
        }
    }

}
