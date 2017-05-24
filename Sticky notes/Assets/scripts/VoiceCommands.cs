using UnityEngine;
using System.Collections;
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
    public GameObject ErrorTextPrefab;
    public GameObject numpadWorkspacePrefab;
    public GameObject numpadWsToGroupPrefab;
    public GameObject infoTextPrefab;

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

    private GameObject numpadWorkspace;
    private GameObject numpadWsToGroup;
    private GameObject infoText;
    private GameObject ErrorText;

    public bool hit;
    public RaycastHit hitInfo;
    public static int wsId;

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
        headers = new List<GameObject>();

    }

    void Update()
    {
        hit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 20f, SpatialMappingManager.Instance.LayerMask);
        if(ErrorText != null)
        {
            ErrorText.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 1f));
            ErrorText.transform.LookAt(2f * ErrorText.transform.position - Camera.main.transform.position);
            ErrorText.transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, Camera.main.transform.rotation.eulerAngles.z);
        }
        if (infoText != null)
        {
            infoText.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 1f));
            infoText.transform.LookAt(2f * infoText.transform.position - Camera.main.transform.position);
            infoText.transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, Camera.main.transform.rotation.eulerAngles.z);
        }
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
            GazeManager.Instance.HitObject.transform.GetChild(1).GetComponentInChildren<Text>().text = "";
            dbconnection.editNote(GazeManager.Instance.HitObject.GetComponent<NoteCommands>().noteId.ToString(), "");
            SpeechManager.clearText();
        }
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
                    dbconnection.saveWs(Int32.Parse(id), workspace.GetComponent<WorkspaceScript>().id);
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
            string pos = notepad.transform.localPosition.x + "," + notepad.transform.localPosition.y;
            dbconnection.saveNotePos(Int32.Parse(id), pos);
            
        }, "", user));
        if (StartScript.texts[1].transform.GetChild(0).GetComponentInChildren<Text>().text == "To create a note\nsay \"create note\"")
            StartScript.texts[1].GetComponentInChildren<Animator>().SetBool("DoAnimation", true);
    }
 

    /// <summary>
    /// Removes the note you are currently gazing at.
    /// Will also remove the note from the server.
    /// </summary>
    public void deleteNote()
    {
        if (GazeManager.Instance.HitObject.tag == "PostIT")
        {
            Destroy(GazeManager.Instance.HitObject.gameObject);
            dbconnection.deleteNote(GazeManager.Instance.HitObject.GetComponent<NoteCommands>().noteId.ToString());
            if (StartScript.texts[2].transform.GetChild(0).GetComponentInChildren<Text>().text == "To remove a note\nsay \"remove note\"")
                StartScript.texts[2].GetComponentInChildren<Animator>().SetBool("DoAnimation", true);
        }
    }

    /// <summary>
    /// Starts the speech dictation to be able to use speech-to-text.
    /// </summary>
    public void startSpeech()
    {
        Quaternion lockrotation = Camera.main.transform.localRotation;
        if (GazeManager.Instance.HitObject.tag == "PostIT")
        {
            speech.StartRecording(GazeManager.Instance.HitObject.transform.GetChild(1).gameObject);
            if (StartScript.texts[0].transform.GetChild(0).GetComponentInChildren<Text>().text == "To edit a note\nsay \"edit note\"")
                StartScript.texts[0].GetComponentInChildren<Animator>().SetBool("DoAnimation", true);
        }else
        {
            infoText = Instantiate(infoTextPrefab, Camera.main.transform.position + 1f * Camera.main.transform.forward, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
            infoText.GetComponentInChildren<Text>().text = "You must look at a note\nto start editing";
            StartCoroutine(infoTime());
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
                if(StartScript.texts[4].transform.GetChild(0).GetComponentInChildren<Text>().text == "To create a workspace\nsay \"create workspace\"")
                   StartScript.texts[4].GetComponentInChildren<Animator>().SetBool("DoAnimation", true);
            }
            else
            { 
                ws = Instantiate(WorkspacePrefab, Camera.main.transform.position + 2f * Camera.main.transform.forward, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                if (StartScript.texts[4].transform.GetChild(0).GetComponentInChildren<Text>().text == "To create a workspace\nsay \"create workspace\"")
                    StartScript.texts[4].GetComponentInChildren<Animator>().SetBool("DoAnimation", true);
            }

            adjust = ws.transform.GetChild(1).gameObject;
            resize = ws.transform.GetChild(0).gameObject;
            adjust.SetActive(false);
            resize.SetActive(false);

            ws.GetComponent<WorkspaceScript>().id = Int32.Parse(id);
            Debug.Log(ws.transform.GetComponent<RectTransform>().rect.width);
            dbconnection.saveWorkspaceSize(Int32.Parse(id), ws.transform.GetComponent<RectTransform>().rect.width.ToString(), ws.transform.GetComponent<RectTransform>().rect.height.ToString());
            ws.transform.GetChild(2).GetComponentInChildren<Text>().text = "ID: " + id;
            if (UserScript.userId != -1)
            {
                dbconnection.addUserToWorkspace(UserScript.userId, Int32.Parse(id));
            }
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
        Quaternion lockrotation = Camera.main.transform.localRotation;
        Vector3 headPosition = Camera.main.transform.position;
        Vector3 gazeDirection = Camera.main.transform.forward;
        RaycastHit hitInfoTwo;
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfoTwo, 30.0f, myLayerMask))
        {
            adjust.SetActive(true);
            resize.SetActive(true);
            if (StartScript.texts[3].transform.GetChild(0).GetComponentInChildren<Text>().text == "To open workspace menu\nsay \"open menu\"")
                StartScript.texts[3].GetComponentInChildren<Animator>().SetBool("DoAnimation", true);
        }
        
    }

    /// <summary>
    /// Disables the menu buttons for workspace.
    /// </summary>
    public void disableAdjustButtons()
    {
        Quaternion lockrotation = Camera.main.transform.localRotation;
        Vector3 headPosition = Camera.main.transform.position;
        Vector3 gazeDirection = Camera.main.transform.forward;
        RaycastHit hitInfoTwo;
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfoTwo, 30.0f, myLayerMask))
        {
            adjust.SetActive(false);
            resize.SetActive(false);
        }
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
                
                header = Instantiate(HeaderPrefab, GazeManager.Instance.HitPosition, workspace.transform.rotation) as GameObject;
                header.transform.SetParent(workspace.transform);
                headers.Add(header);
                string pos = header.transform.localPosition.x + "," + header.transform.localPosition.y;
                StartCoroutine(dbconnection.saveHeader((id) =>
                {
                    header.GetComponent<HeaderScript>().headerId = Int32.Parse(id);
                },workspace.GetComponent<WorkspaceScript>().id, header.GetComponentInChildren<Text>().text, pos ));
                
            }
        }else
        {
            if(ErrorText == null)
            {
                ErrorText = Instantiate(ErrorTextPrefab, Camera.main.transform.position + 1f * Camera.main.transform.forward, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
                StartCoroutine(errorTime());
            }
        }
    }

    /// <summary>
    /// Removes the error message for header if the user tries to create a header outside of workspace.
    /// </summary>
    /// <returns></returns>
    public IEnumerator errorTime()
    {
        yield return new WaitForSeconds(4f);
        Destroy(ErrorText);
    }

    public IEnumerator infoTime()
    {
        yield return new WaitForSeconds(4f);
        Destroy(infoText);
    }

    /// <summary>
    /// Deletes the current header the user is gazing at.
    /// </summary>
    public void deleteHeader()
    {
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
            Destroy(GameObject.FindGameObjectWithTag("Numpad"));
            Quaternion lockrotation = Camera.main.transform.localRotation;
            GameObject workspaceObject = Instantiate(WorkspacePrefab, Camera.main.transform.position + 2f * Camera.main.transform.forward, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
            float width = float.Parse(workspace.Workspace[0].width);
            float height = float.Parse(workspace.Workspace[0].height);
            workspaceObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            workspaceObject.GetComponent<WorkspaceScript>().id = workspace.Workspace[0].ws_id;
            workspaceObject.transform.GetChild(2).GetComponentInChildren<Text>().text = "ID: " + workspaceObject.GetComponent<WorkspaceScript>().id;
            GameObject notepad;
            GameObject header;
            for (int i = 0; i < workspace.Workspace.Count; i++)
            {
                if(workspace.Workspace[i].note_id != 0)
                {
                    string[] pos = workspace.Workspace[i].Note_pos.Split(',');
                    notepad = Instantiate(NotepadPrefab, Camera.main.transform.position + Camera.main.transform.right * i * 0.3f + 2f * Camera.main.transform.forward, workspaceObject.transform.localRotation) as GameObject;
                    notepad.transform.SetParent(workspaceObject.transform);
                    float xPos = float.Parse(pos[0]);
                    float yPos = float.Parse(pos[1]);
                    Vector3 localpos = new Vector3(xPos, yPos, 0);
                    notepad.transform.localPosition = localpos;
                    notepad.transform.GetChild(1).GetComponentInChildren<Text>().text = workspace.Workspace[i].content;
                    notepad.GetComponent<NoteCommands>().noteId = workspace.Workspace[i].note_id;
                    if (!notes.Contains(notepad))
                        notes.Add(notepad);
                }
            }
            for (int i = 0; i < headerlist.headerList.Count; i++)
            {
                if(headerlist.headerList[i].header_id != 0)
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
                    if (!headers.Contains(header))
                        headers.Add(header);
                }
            }
        }, wsId, UserScript.userId));
    }


    /// <summary>
    /// Creates the numpad to get a specific workspace.
    /// </summary>
    public void getWorkspaceInput()
    {
        if(GameObject.FindGameObjectWithTag("Numpad") == null)
        {
            if (StartScript.texts[3].transform.GetChild(0).GetComponentInChildren<Text>().text == "To get a workspace\nsay \"get workspace\"")
                StartScript.texts[3].GetComponentInChildren<Animator>().SetBool("DoAnimation", true);
            Quaternion lockrotation = Camera.main.transform.localRotation;
            Instantiate(WsInputPrefab, Camera.main.transform.position + 2f * Camera.main.transform.forward, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0));
        }
    }

    public void updateWorkspace()
    {
        Vector3 headPosition = Camera.main.transform.position;
        Vector3 gazeDirection = Camera.main.transform.forward;
        RaycastHit hitInfoTwo;
        bool isWorkspace = Physics.Raycast(headPosition, gazeDirection, out hitInfoTwo, 30.0f, myLayerMask);
        int wsId = hitInfoTwo.collider.gameObject.GetComponent<WorkspaceScript>().id;
        StartCoroutine(dbGetWs.getWS((workspace, headerlist) => {
            Quaternion lockrotation = Camera.main.transform.localRotation;
            GameObject workspaceObject = null;
            if (isWorkspace)
            {
                workspaceObject = hitInfoTwo.collider.gameObject;
                float width = float.Parse(workspace.Workspace[0].width);
                float height = float.Parse(workspace.Workspace[0].height);
                workspaceObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
                GameObject notepad = null;
                GameObject header = null;
                for (int i = 0; i < workspace.Workspace.Count; i++)
                {
                    if (workspace.Workspace[i].note_id != 0)
                    {
                        string[] pos = workspace.Workspace[i].Note_pos.Split(',');
                        float xPos = float.Parse(pos[0]);
                        float yPos = float.Parse(pos[1]);
                        Vector3 localpos = new Vector3(xPos, yPos, 0);
                        if (notes[i].GetComponent<NoteCommands>().noteId == workspace.Workspace[i].note_id)
                        {
                            notes[i].transform.GetChild(1).GetComponentInChildren<Text>().text = workspace.Workspace[i].content;
                            notes[i].transform.localPosition = localpos;
                        }
                        else
                        {
                            notepad = Instantiate(NotepadPrefab, Camera.main.transform.position + Camera.main.transform.right * i * 0.3f + 2f * Camera.main.transform.forward, workspaceObject.transform.localRotation) as GameObject;
                            notepad.transform.SetParent(workspaceObject.transform);
                            notepad.transform.localPosition = localpos;
                            notepad.transform.GetChild(1).GetComponentInChildren<Text>().text = workspace.Workspace[i].content;
                            notepad.GetComponent<NoteCommands>().noteId = workspace.Workspace[i].note_id;
                            if (notes.Contains(notepad))
                                notes.Add(notepad);
                        }
                    }
                    
                }
                for (int i = 0; i < headerlist.headerList.Count; i++)
                {
                    if(headerlist.headerList[i].header_id != 0)
                    {
                        string[] pos = headerlist.headerList[i].Header_pos.Split(',');
                        float xPos = float.Parse(pos[0]);
                        float yPos = float.Parse(pos[1]);
                        Vector3 localpos = new Vector3(xPos, yPos, 0);

                        if (headers[i].GetComponent<HeaderScript>().headerId == headerlist.headerList[i].header_id)
                        {
                            headers[i].transform.localPosition = localpos;
                            headers[i].transform.GetChild(0).GetComponent<Text>().text = headerlist.headerList[i].header_text;
                        }
                        else
                        {
                            header = Instantiate(HeaderPrefab, Camera.main.transform.position + Camera.main.transform.right * i * 0.3f + 2f * Camera.main.transform.forward, workspaceObject.transform.localRotation) as GameObject;
                            header.transform.SetParent(workspaceObject.transform);
                            header.transform.localPosition = localpos;
                            header.transform.GetChild(0).GetComponent<Text>().text = headerlist.headerList[i].header_text;
                            header.GetComponent<HeaderScript>().headerId = headerlist.headerList[i].header_id;
                            if (headers.Contains(header))
                                headers.Add(header);
                        }
                    }
                }
            }
        }, wsId, UserScript.userId));
    }

    public void createGroup()
    {
        Quaternion lockrotation = Camera.main.transform.localRotation;
        if (UserScript.userId != -1)
        {
            StartCoroutine(dbconnection.createGroup(UserScript.userId));
            if (StartScript.texts[0].transform.GetChild(0).GetComponentInChildren<Text>().text == "To create a group\nsay \"create group\"")
                StartScript.texts[0].GetComponentInChildren<Animator>().SetBool("DoAnimation", true);
            infoText = Instantiate(infoTextPrefab, Camera.main.transform.position + 1f * Camera.main.transform.forward, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
            infoText.GetComponentInChildren<Text>().text = "You have created group";
            StartCoroutine(infoTime());
        }
        else
        {
            infoText = Instantiate(infoTextPrefab, Camera.main.transform.position + 1f * Camera.main.transform.forward, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
            infoText.GetComponentInChildren<Text>().text = "You may only create a group once you have logged in";
            StartCoroutine(infoTime());
        }
    }

    public void addUserToGroup()
    {
        Quaternion lockrotation = Camera.main.transform.localRotation;
        if (UserScript.userId != -1)
        {
            numpadWorkspace = Instantiate(numpadWorkspacePrefab, Camera.main.transform.position + 2f * Camera.main.transform.forward, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
            if (StartScript.texts[1].transform.GetChild(0).GetComponentInChildren<Text>().text == "To join a group\nsay \"join group\"")
                StartScript.texts[1].GetComponentInChildren<Animator>().SetBool("DoAnimation", true); 
        } else
        {
            infoText = Instantiate(infoTextPrefab, Camera.main.transform.position + 1f * Camera.main.transform.forward, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
            infoText.GetComponentInChildren<Text>().text = "You may only join a group once you have logged in";
            StartCoroutine(infoTime());
        }
        
    }

    public void addWsToGroup()
    {
        Vector3 headPosition = Camera.main.transform.position;
        Vector3 gazeDirection = Camera.main.transform.forward;
        RaycastHit hitInfo;
        Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f, myLayerMask);
        wsId = hitInfo.collider.gameObject.GetComponent<WorkspaceScript>().id;
        Quaternion lockrotation = Camera.main.transform.localRotation;
        numpadWsToGroup = Instantiate(numpadWsToGroupPrefab, Camera.main.transform.position + 2f * Camera.main.transform.forward, Quaternion.Euler(lockrotation.eulerAngles.x, lockrotation.eulerAngles.y, 0)) as GameObject;
        if (StartScript.texts[2].transform.GetChild(0).GetComponentInChildren<Text>().text == "To share a workspace\nsay \"share workspace\"")
            StartScript.texts[2].GetComponentInChildren<Animator>().SetBool("DoAnimation", true);
    }

    public void hideGuide()
    {
        int length = StartScript.texts.Length;
        for(int i = 0; i <= length; i++)
        {
            StartScript.texts[i].GetComponentInChildren<Animator>().SetBool("DoAnimation", true);
        }
    }

}
