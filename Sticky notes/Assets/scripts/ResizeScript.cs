using UnityEngine;
using HoloToolkit.Unity.InputModule;

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

    private GesturesInput gesture;
    private RectTransform size;
    private BoxCollider box;

    void Start()
    {
        size = this.transform.parent.parent.GetComponent<RectTransform>();
        gesture = GameObject.Find("InputManager").GetComponent<GesturesInput>();
        box = this.transform.parent.parent.GetComponent<BoxCollider>();
    }

    void Update()
    {
        if (gesture.IsNavigating)
        {
            PerformRotation();
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