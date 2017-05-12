using UnityEngine;
using HoloToolkit.Unity.InputModule;
using conn;

/// <summary>
/// GestureAction performs custom actions based on
/// which gesture is being performed.
/// </summary>
public class ResizeScript : MonoBehaviour
{
    [Tooltip("Rotation max speed controls amount of rotation.")]
    public float RotationSensitivity = 0.1f;

    private Vector3 manipulationPreviousPosition;

    private float rotationFactorX;
    private float rotationFactorY;

    private InputManager gesture;
    private RectTransform size;
    private BoxCollider box;

    private connect dbconnection;

    private bool startedResizeing;
    public LayerMask myLayerMask;

    void Start()
    {
        dbconnection = new connect();
        gesture = GameObject.Find("InputManager").GetComponent<InputManager>();
        startedResizeing = false;
    }

    void Update()
    {
        if (gesture.IsNavigating)
        {
            if(!startedResizeing)
            {
                Vector3 headPosition = Camera.main.transform.position;
                Vector3 gazeDirection = Camera.main.transform.forward;
                RaycastHit hitInfo;
                Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f, myLayerMask);
                if(hitInfo.collider.gameObject.tag == "Workspace")
                {
                    size = hitInfo.collider.gameObject.GetComponent<RectTransform>();
                    box = hitInfo.collider.gameObject.GetComponent<BoxCollider>();
                } else if (hitInfo.collider.gameObject.name == "ResizeButton")
                {
                    size = hitInfo.collider.gameObject.transform.parent.parent.GetComponent<RectTransform>();
                    box = hitInfo.collider.gameObject.transform.parent.parent.GetComponent<BoxCollider>();
                }
                
                startedResizeing = true;
            }
            PerformRotation();
            dbconnection.saveWorkspaceSize(this.transform.parent.parent.GetComponent<WorkspaceScript>().id, this.transform.parent.parent.GetComponent<RectTransform>().rect.width.ToString(), this.transform.parent.parent.GetComponent<RectTransform>().rect.height.ToString());
        } else
        {
            if(startedResizeing == true)
            {
                startedResizeing = false;
            }
        }
    }

    private void PerformRotation()
    {
        Debug.Log(GazeManager.Instance.HitObject.name);
        if(GazeManager.Instance.HitObject.tag == "Workspace" || GazeManager.Instance.HitObject.tag == "ResizeButton") {
            Debug.Log(size.rect.height);
            rotationFactorX = gesture.NavigationPosition.x * RotationSensitivity;
            rotationFactorY = gesture.NavigationPosition.y * RotationSensitivity;
            if (size.rect.width <= 50)
            {
                size.sizeDelta = new Vector2(50.1f, size.rect.height + rotationFactorY * -1);
                box.size = new Vector3(50.1f, size.rect.height + rotationFactorY * -1, 0.01f);
            }
            else if (size.rect.height <= 50)
            {
                Debug.Log("height 50");
                size.sizeDelta = new Vector2(size.rect.width + rotationFactorX, 50.1f);
                box.size = new Vector3(size.rect.width + rotationFactorX, 50.1f, 0.01f);
            }
            else
            {
                size.sizeDelta = new Vector2(size.rect.width + rotationFactorX, size.rect.height + rotationFactorY * -1);
                box.size = new Vector3(size.rect.width + rotationFactorX, size.rect.height + rotationFactorY * -1, 0.01f);
            }
        }
    }
}