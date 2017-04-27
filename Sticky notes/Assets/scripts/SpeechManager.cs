using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using HoloToolkit.Unity.SpatialMapping;
using conn;


public class SpeechManager : MonoBehaviour
{
    public GameObject text;
    private static GameObject infoText;
    private static GameObject notepad;
    private static DictationRecognizer dictationRecognizer;
    private AudioFeedback audio;

    private float current;
    private float timestamp;

    public GameObject loading;
    private GameObject currentLoading;


    private static connect dbconnection;

    // Use this string to cache the text currently displayed in the text box.
    private static StringBuilder textSoFar;
    string loginNr;

    // Using an empty string specifies the default microphone. 
    private static string deviceName = string.Empty;
    private int samplingRate;
    private const int messageLength = 60;

    // Use this to reset the UI once the Microphone is done recording after it was started.
    private static bool hasRecordingStarted;
    private bool stop = false;
    private static bool login = false;


    private float hej = 0;
    //TODO
   // private static bool completed = false;

    void Awake()
    {
        timestamp = 0;

        audio = new AudioFeedback();

        dbconnection = new connect();

        dictationRecognizer = new DictationRecognizer();

        dictationRecognizer.AutoSilenceTimeoutSeconds = 60;

        dictationRecognizer.InitialSilenceTimeoutSeconds = 60;

        dictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;

        dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;

        //dictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;

        dictationRecognizer.DictationError += DictationRecognizer_DictationError;

        // Query the maximum frequency of the default microphone. Use 'unused' to ignore the minimum frequency.
        int unused;
        Microphone.GetDeviceCaps(deviceName, out unused, out samplingRate);

        // Use this string to cache the text currently displayed in the text box.
        textSoFar = new StringBuilder();

        // Use this to reset the UI once the Microphone is done recording after it was started.
        hasRecordingStarted = false;
    }

    void Update()
    {
        /*remove "hasRecodringStarted" to run test*/
        if (hasRecordingStarted && current >= 100f && dictationRecognizer.Status == SpeechSystemStatus.Running)
        {
            // Reset the flag now that we're cleaning up the UI.
            hasRecordingStarted = false;
            
            StopRecording();
            // This acts like pressing the Stop button and sends the message to the Communicator.
            // If the microphone stops as a result of timing out, make sure to manually stop the dictation recognizer.
            // Look at the StopRecording function.
            SendMessage("RecordStop");

            timestamp = 0;
            current = 0;
            Destroy(currentLoading);
        }
        if (dictationRecognizer.Status != SpeechSystemStatus.Running)
        {
            PhraseRecognitionSystem.Restart();
        }
        if (!login)
        {
            if (infoText != null)
            {
                infoText.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.2f, 1));
                infoText.transform.LookAt(2f * infoText.transform.position - Camera.main.transform.position);
            }
            if (timestamp != 0)
            {
                current += (10 * timestamp);
                currentLoading.transform.GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = current / 100;
            }

            if (currentLoading != null)
            {
                currentLoading.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.7f, 0.5f, 1f));
                currentLoading.transform.LookAt(2f * currentLoading.transform.position - Camera.main.transform.position);
                currentLoading.transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, Camera.main.transform.rotation.eulerAngles.z);
            }
        }
        
        
    }

    /// <summary>
    /// Turns on the dictation recognizer and begins recording audio from the default microphone.
    /// </summary>
    /// <returns>The audio clip recorded from the microphone.</returns>
    public AudioClip StartRecording(GameObject note)
    {
        currentLoading = Instantiate(loading, Camera.main.transform.position + 1f * Camera.main.transform.forward, Camera.main.transform.localRotation);
        audio.playSound();
        /*Small text for info
        infoText = Instantiate(text, Camera.main.transform.position, Camera.main.transform.rotation);
        infoText.GetComponentInChildren<Text>().text = "Click anywhere to stop recording";*/
        TapEvent.speaking = true;
        notepad = note;
        notepad.GetComponentInParent<TapToPlace>().enabled = false;
        PhraseRecognitionSystem.Shutdown();

        dictationRecognizer.Start();

        // Set the flag that we've started recording.
        hasRecordingStarted = true;
        Debug.Log(timestamp);
        timestamp = 0.0165f;
        Debug.Log(timestamp);
        // Start recording from the microphone for 10 seconds.
        return Microphone.Start(deviceName, false, messageLength, samplingRate);
    }
     /// <summary>
     /// Speech for login, no loadingbar and sound
     /// </summary>
     /// <param name="note"></param>
     /// <returns></returns>
    public AudioClip StartRecordingLogin(GameObject note)
    {
        
        textSoFar.Length = 0;
        TapEvent.speaking = true;
        login = true;
        notepad = note;
        notepad.GetComponent<Text>().text = "";
        PhraseRecognitionSystem.Shutdown();
        dictationRecognizer.Start();
        // Set the flag that we've started recording.
        hasRecordingStarted = true;
        // Start recording from the microphone for 10 seconds.
        return Microphone.Start(deviceName, false, messageLength, samplingRate);
    }

    /// <summary>
    /// Ends the recording session.
    /// </summary>
    /// OBS! If running tests disable tap to place
    public static void StopRecording()
    {
        TapEvent.speaking = false;
        hasRecordingStarted = false;
        if (dictationRecognizer.Status == SpeechSystemStatus.Running)
        {
            dictationRecognizer.Stop();
        }
        if (!login)
        {
            dbconnection.editNote(notepad.transform.parent.GetComponent<NoteCommands>().noteId.ToString(), notepad.GetComponentInChildren<Text>().text);
            notepad.GetComponentInParent<TapToPlace>().enabled = true;
        }
        Microphone.End(deviceName);
    }

    /// <summary>
    /// Clears the text on the current note.
    /// </summary>
    public static void clearText()
    {
        textSoFar.Length = 0;
        textSoFar.Capacity = 0;
    }

    /// <summary>
    /// This event is fired while the user is talking. As the recognizer listens, it provides text of what it's heard so far.
    /// </summary>
    /// <param name="text">The currently hypothesized recognition.</param>
    private void DictationRecognizer_DictationHypothesis(string text)
    {
        if (login)
        {
            if (textSoFar.ToString().Contains("one") || textSoFar.ToString().Contains("zero"))
            {
                textSoFar.Replace("one", "1");
                textSoFar.Replace("zero", "0");
            }
            notepad.GetComponent<Text>().text = textSoFar.ToString();
        }
        else
        {
            notepad.GetComponentInChildren<Text>().text = textSoFar.ToString() + text;
        }
    }

    /// <summary>
    /// This event is fired after the user pauses, typically at the end of a sentence. The full recognized string is returned here.
    /// </summary>
    /// <param name="text">The text that was heard by the recognizer.</param>
    /// <param name="confidence">A representation of how confident (rejected, low, medium, high) the recognizer is of this recognition.</param>
    private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
    {
        if (login)
        {
            textSoFar.Append(text);
            textSoFar.Replace("one", "1");
            textSoFar.Replace("zero", "0");
            notepad.GetComponent<Text>().text = textSoFar.ToString() + " ";
        }else
        {
            textSoFar.Append(text);
            notepad.GetComponentInChildren<Text>().text = textSoFar.ToString() + " ";
        }
        
    }

    /// <summary>
    /// This event is fired when the recognizer stops, whether from Stop() being called, a timeout occurring, or some other error.
    /// Typically, this will simply return "Complete". In this case, we check to see if the recognizer timed out.
    /// </summary>
    /// <param name="cause">An enumerated reason for the session completing.</param>
    /// GOOOD BYE FOR NOW
    /*private void DictationRecognizer_DictationComplete(DictationCompletionCause cause)
    {
        // If Timeout occurs, the user has been silent for too long.
        // With dictation, the default timeout after a recognition is 20 seconds.
        // The default timeout with initial silence is 5 seconds.
        if (cause == DictationCompletionCause.TimeoutExceeded)
        {
            Microphone.End(deviceName);
            StopRecording();
            SendMessage("ResetAfterTimeout");
        }
    }*/

    //TODO
    /*private void CheckComplete(DictationCompletionCause cause)
    {
        if(cause == DictationCompletionCause.Complete)
        {
           completed = true;
        }
    }*/

    /// <summary>
    /// This event is fired when an error occurs.
    /// </summary>
    /// <param name="error">The string representation of the error reason.</param>
    /// <param name="hresult">The int representation of the hresult.</param>
    private void DictationRecognizer_DictationError(string error, int hresult)
    {
        if (login)
        {
            notepad.GetComponent<Text>().text = error + "\nHRESULT: " + hresult;
        }
        else
        {
            notepad.GetComponentInChildren<Text>().text = error + "\nHRESULT: " + hresult;
        }
        
    }

    public static string getTextSoFar()
    {
        return textSoFar.ToString();
    }

    public static void setLoginFalse()
    {
        login = false;
    }

    public static void setLoginTrue()
    {
        login = true;
    }

    public static bool getLogin()
    {
        return login;
    }
}