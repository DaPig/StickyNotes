using UnityEngine;
using System;
using HoloToolkit.Unity.InputModule;
using UnityEngine.UI;
using conn;
using selectnotes;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using HoloToolkit.Unity.SpatialMapping;
using HoloToolkit.Unity.InputModule;

/// <summary>
/// GestureAction performs custom actions based on
/// which gesture is being performed.
/// </summary>
public class ResizeScript : MonoBehaviour
{
    [Tooltip("Rotation max speed controls amount of rotation.")]
    public float RotationSensitivity = 2.5f;

    private Vector3 manipulationPreviousPosition;

    private float rotationFactorX;
    private float rotationFactorY;

    private GesturesInput gesture;
    private RectTransform size;

    void Start()
    {
        size = this.transform.parent.parent.GetComponent<RectTransform>();
        gesture = GameObject.Find("InputManager").GetComponent<GesturesInput>();
    }

    void Update()
    {
        PerformRotation();
    }

    private void PerformRotation()
    {
        
        if (gesture.IsNavigating)
        {
            Debug.Log(size.rect.height);
            rotationFactorX = gesture.NavigationPosition.x * RotationSensitivity;
            rotationFactorY = gesture.NavigationPosition.y * RotationSensitivity;
            if (size.rect.width <= 50)
            {
                Debug.Log("widht 50");
                size.sizeDelta = new Vector2(size.rect.width, size.rect.height + rotationFactorY * -1);
            } else if (size.rect.height <= 50)
            {
                Debug.Log("height 50");
                size.sizeDelta = new Vector2(size.rect.width + rotationFactorX, size.rect.height);
            } else
            {
                Debug.Log("NONE");
                size.sizeDelta = new Vector2(size.rect.width + rotationFactorX, size.rect.height + rotationFactorY * -1);
            }
        }
    }
}