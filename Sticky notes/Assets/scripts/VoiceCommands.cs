using UnityEngine;
using System;
using HoloToolkit.Unity.InputModule;
using UnityEngine.UI;
using conn;
using selectnotes;
using getWs;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using HoloToolkit.Unity.SpatialMapping;

public class VoiceCommands : MonoBehaviour
{
    public GameObject NotepadPrefab;
    public GameObject keyboardPrefab;
    public GameObject WorkspacePrefab;
    public GameObject HeaderPrefab;
    public GameObject WsInputPrefab;

    private GameObject resize;
    private GameObject adjust;

    public static bool keyboardCreated = false;

    private connect dbconnection;
    private select dbselect;
    private getWorkspace dbGetWs;

    private SpeechManager speech;
    private AudioSource audio;

    public List<GameObject> notes;
    public List<GameObject> headers;

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
        dbGetWs = new getWorkspace();
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
        StartCoroutine(dbconnection.insertNote((id) =>
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
            if(workspace != null)
            {
                if (notepad.transform.parent.tag == "Workspace")
                {
                    string pos = notepad.transform.localPosition.x + "," + notepad.transform.localPosition.y;
                    dbconnection.saveNotePos(Int32.Parse(id), pos);
                }
            }
            
            
        }, "", user));
        StartScript.texts[1].GetComponentInChildren<Animator>().SetBool("DoAnimation", true);
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
                notepad.transform.GetChild(1).GetComponentInChildren<Text>().text = note.Notes[i].content;
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

    /// <summary>
    /// Activation of voice dictation on login
    /// CURRENTLY NOT USED!!!
    /// </summary>
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
        GameObject ws;
        StartCoroutine(dbconnection.insertWorkspace((id) =>
        {

            Quaternion lockrotation = Camera.main.transform.localRotation;
            

            //Checks if we are hitting a mapped surface or not
            if (hit)
            {
                ws = Instantiate(WorkspacePrefab, hitInfo.point + Camera.main.transform.forward * -0.05f, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
            }
            else
            { 
                ws = Instantiate(WorkspacePrefab, Camera.main.transform.position + 2f * Camera.main.transform.forward, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
            }

            adjust = ws.transform.GetChild(1).gameObject;
            resize = ws.transform.GetChild(0).gameObject;
            adjust.SetActive(false);
            resize.SetActive(false);

            ws.GetComponent<WorkspaceScript>().id = Int32.Parse(id);
            Debug.Log(ws.transform.GetComponent<RectTransform>().rect.width);
            dbconnection.saveWorkspaceSize(Int32.Parse(id), ws.transform.GetComponent<RectTransform>().rect.width.ToString(), ws.transform.GetComponent<RectTransform>().rect.height.ToString());
            ws.transform.GetChild(2).GetComponentInChildren<Text>().text = id;
        }));        
    }

    /// <summary>
    /// Saves the workspace in the database.
    /// </summary>
    public void saveWorkspace()
    {
        GameObject ws = null;
        Vector3 headPosition = Camera.main.transform.position;
        Vector3 gazeDirection = Camera.main.transform.forward;
        RaycastHit hitInfoTwo;
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfoTwo, 30.0f, myLayerMask))
        {
            ws = hitInfoTwo.transform.gameObject;
        }
        Debug.Log(ws.tag);
        if(ws.tag == "Workspace")
        {
            foreach (GameObject note in notes)
            {
                if (ws.GetComponent<WorkspaceScript>().id == note.transform.parent.GetComponent<WorkspaceScript>().id)
                {
                    dbconnection.saveWs(note.GetComponent<NoteCommands>().noteId, note.transform.parent.GetComponent<WorkspaceScript>().id);
                }
            }

            foreach (GameObject header in headers)
            {
                if (header.transform.parent.GetComponent<WorkspaceScript>().id == ws.GetComponent<WorkspaceScript>().id)
                {
                    string pos = header.transform.localPosition.x + "," + header.transform.localPosition.y;

                }
            }
        }
        
    }

    /// <summary>
    /// Removes the workspace which is gazed at.
    /// </summary>
    public void removeWorkspace()
    {
        GameObject ws = null;
        Vector3 headPosition = Camera.main.transform.position;
        Vector3 gazeDirection = Camera.main.transform.forward;
        RaycastHit hitInfoTwo;
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfoTwo, 30.0f, myLayerMask))
        {
            ws = hitInfoTwo.transform.gameObject;
        }
        if(ws.tag == "Workspace")
        {
            Destroy(ws);
        }
    }

    /// <summary>
    /// Enables the menu buttons for workspace.
    /// </summary>
    public void enableAdjustButtons()
    {
        adjust.SetActive(true);
        resize.SetActive(true);
    }

    /// <summary>
    /// Disables the menu buttons for workspace.
    /// </summary>
    public void disableAdjustButtons()
    {
        adjust.SetActive(false);
        resize.SetActive(false);
    }

    /// <summary>
    /// Creates a header on the workspace, if the user is gazing at a workspace.
    /// </summary>
    public void createHeader()
    {
        GameObject workspace;
        GameObject header;
        Quaternion lockrotation = Camera.main.transform.localRotation;
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
                headers.Add(header);
                string pos = header.transform.localPosition.x + "," + header.transform.localPosition.y;
                StartCoroutine(dbconnection.saveHeader((id) =>
                {
                    header.GetComponent<HeaderScript>().headerId = Int32.Parse(id);
                },workspace.GetComponent<WorkspaceScript>().id, header.GetComponentInChildren<Text>().text, pos ));
                
            }
        }
    }

    /// <summary>
    /// Deletes the current header the user is gazing at.
    /// </summary>
    public void deleteHeader()
    {
        Debug.Log("in delete" + GazeManager.Instance.HitObject.tag);
        if(GazeManager.Instance.HitObject.tag == "Header")
        {
            Destroy(GazeManager.Instance.HitObject);
        }
    }

    /// <summary>
    /// Enables the editation of the current header which the user is gazing at.
    /// </summary>
    public void editHeader()
    {
        if (GazeManager.Instance.HitObject.tag == "Header")
        {
            speech.StartRecordingHeader(GazeManager.Instance.HitObject.transform.GetChild(0).gameObject);

        }
    }

    /// <summary>
    /// Gets a specific workspace from the database with all the notes and headers in it.
    /// </summary>
    public void getWorkspace(int wsId)
    {
        //StartCoroutine(dbconnection.getWorkspace(68));
       StartCoroutine(dbGetWs.getWS((workspace, headerlist) => {
           Quaternion lockrotation = Camera.main.transform.localRotation;
           GameObject workspaceObject = Instantiate(WorkspacePrefab, Camera.main.transform.position + 2f * Camera.main.transform.forward, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
           float width = float.Parse(workspace.Workspace[0].width);
           float height = float.Parse(workspace.Workspace[0].height);
           workspaceObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
           workspaceObject.GetComponent<WorkspaceScript>().id = workspace.Workspace[0].ws_id;
           workspaceObject.transform.GetChild(2).GetComponentInChildren<Text>().text = "ID:" + workspaceObject.GetComponent<WorkspaceScript>().id;
           GameObject notepad;
           GameObject header;
           for (int i = 0; i < workspace.Workspace.Count; i++)
           {
               string[] pos = workspace.Workspace[i].Note_pos.Split(',');
               notepad = Instantiate(NotepadPrefab, Camera.main.transform.position + Camera.main.transform.right * i * 0.3f + 2f * Camera.main.transform.forward, workspaceObject.transform.localRotation) as GameObject;
               notepad.transform.SetParent(workspaceObject.transform);
               float xPos = float.Parse(pos[0]);
               float yPos = float.Parse(pos[1]);
               Vector3 localpos = new Vector3(xPos, yPos , 0);
               notepad.transform.localPosition = localpos;
               notepad.transform.GetChild(1).GetComponentInChildren<Text>().text = workspace.Workspace[i].content;
               notepad.GetComponent<NoteCommands>().noteId = workspace.Workspace[i].note_id;
           }
           Debug.Log(headerlist.headerList);
           for (int i = 0; i < headerlist.headerList.Count; i++)
           {
               string[] pos = headerlist.headerList[i].Header_pos.Split(',');
               header = Instantiate(HeaderPrefab, Camera.main.transform.position + Camera.main.transform.right * i * 0.3f + 2f * Camera.main.transform.forward, workspaceObject.transform.localRotation) as GameObject;
               header.transform.SetParent(workspaceObject.transform);
               float xPos = float.Parse(pos[0]);
               float yPos = float.Parse(pos[1]);
               Vector3 localpos = new Vector3(xPos, yPos, 0);
               header.transform.localPosition = localpos;
               header.transform.GetChild(0).GetComponent<Text>().text = headerlist.headerList[i].header_text;
               header.GetComponent<HeaderScript>().headerId = headerlist.headerList[i].header_id;
           }
           Destroy(GameObject.Find("Numpad"));
       }, wsId));

    }

    public void getWorkspaceInput()
    {
        Quaternion lockrotation = Camera.main.transform.localRotation;
        Instantiate(WsInputPrefab, Camera.main.transform.position + 2f * Camera.main.transform.forward, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0));
    }

}
