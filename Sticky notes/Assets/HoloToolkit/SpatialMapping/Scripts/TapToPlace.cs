// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace HoloToolkit.Unity.SpatialMapping
{
    /// <summary>
    /// The TapToPlace class is a basic way to enable users to move objects 
    /// and place them on real world surfaces.
    /// Put this script on the object you want to be able to move. 
    /// Users will be able to tap objects, gaze elsewhere, and perform the
    /// tap gesture again to place.
    /// This script is used in conjunction with GazeManager, GestureManager,
    /// and SpatialMappingManager.
    /// TapToPlace also adds a WorldAnchor component to enable persistence.
    /// </summary>

    public class TapToPlace : MonoBehaviour, IInputClickHandler
    {
        [Tooltip("Supply a friendly name for the anchor as the key name for the WorldAnchorStore.")]
        public string SavedAnchorFriendlyName = "SavedAnchorFriendlyName";

        [Tooltip("Place parent on tap instead of current game object.")]
        public bool PlaceParentOnTap;

        [Tooltip("Specify the parent game object to be moved on tap, if the immediate parent is not desired.")]
        public GameObject ParentGameObjectToPlace;

        /// <summary>
        /// Keeps track of if the user is moving the object or not.
        /// Setting this to true will enable the user to move and place the object in the scene.
        /// Useful when you want to place an object immediately.
        /// </summary>
        [Tooltip("Setting this to true will enable the user to move and place the object in the scene without needing to tap on the object. Useful when you want to place an object immediately.")]
        public bool IsBeingPlaced;

        /// <summary>
        /// Manages persisted anchors.
        /// </summary>
        protected WorldAnchorManager anchorManager;

        /// <summary>
        /// Controls spatial mapping.  In this script we access spatialMappingManager
        /// to control rendering and to access the physics layer mask.
        /// </summary>
        protected SpatialMappingManager spatialMappingManager;

        public LayerMask myLayerMask;

        private RaycastHit hitInfo;

        private bool workspaceHit;

        private int count = 0;

        protected virtual void Start()
        {
            // Make sure we have all the components in the scene we need.
            anchorManager = WorldAnchorManager.Instance;
            if (anchorManager == null)
            {
                Debug.LogError("This script expects that you have a WorldAnchorManager component in your scene.");
            }

            spatialMappingManager = SpatialMappingManager.Instance;
            if (spatialMappingManager == null)
            {
                Debug.LogError("This script expects that you have a SpatialMappingManager component in your scene.");
            }

            if (anchorManager != null && spatialMappingManager != null)
            {
                //anchorManager.AttachAnchor(gameObject, SavedAnchorFriendlyName);
            }
            else
            {
                // If we don't have what we need to proceed, we may as well remove ourselves.
                Destroy(this);
            }

            if (PlaceParentOnTap)
            {
                if (ParentGameObjectToPlace != null && !gameObject.transform.IsChildOf(ParentGameObjectToPlace.transform))
                {
                    Debug.LogError("The specified parent object is not a parent of this object.");
                }

                DetermineParent();
            }
        }

        protected virtual void Update()
        {
            // If the user is in placing mode,
            // update the placement to match the user's gaze.
            if (IsBeingPlaced)
            {
                // Do a raycast into the world that will only hit the Spatial Mapping mesh.
                Vector3 headPosition = Camera.main.transform.position;
                Vector3 gazeDirection = Camera.main.transform.forward;
                RaycastHit hitInfoTwo;

                workspaceHit = Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f, myLayerMask);
                Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f, myLayerMask);
                Physics.Raycast(headPosition, gazeDirection, out hitInfoTwo, 30.0f, spatialMappingManager.LayerMask);

                // Rotate this object to face the user.
                Quaternion toQuat = Camera.main.transform.localRotation;
                toQuat.x = 0;
                toQuat.z = 0;

                //Checks which layer we are hitting and moves the note accordingly
                //No spatial mapping available, position the note ontop of a workspace
                if (hitInfo.point.z == 0)
                {
                    if (PlaceParentOnTap)
                    {
                        // Place the parent object as well but keep the focus on the current game object
                        Vector3 currentMovement = hitInfoTwo.point - gameObject.transform.position + Camera.main.transform.forward * -0.05f;
                        ParentGameObjectToPlace.transform.position += currentMovement;
                        ParentGameObjectToPlace.transform.rotation = toQuat;
                    }
                    else
                    {
                        
                        gameObject.transform.position = hitInfoTwo.point + Camera.main.transform.forward * -0.05f;
                        gameObject.transform.rotation = toQuat;
                    }
                }
                //No workspace available, position the note on the spatial mapping
                else if (hitInfoTwo.point.z == 0)
                {
                    if (PlaceParentOnTap)
                    {
                        // Place the parent object as well but keep the focus on the current game object
                        Vector3 currentMovement = hitInfo.point - gameObject.transform.position + Camera.main.transform.forward * -0.05f;
                        ParentGameObjectToPlace.transform.position += currentMovement;
                        ParentGameObjectToPlace.transform.rotation = toQuat;
                    }

                    else
                    {
                        gameObject.transform.position = hitInfo.point + Camera.main.transform.forward * -0.05f;
                        gameObject.transform.rotation = toQuat;
                    }
                }
                //The spatial mapping is closer than the workspace, move the note ontop of the spatial mapping
                else if (hitInfo.point.z <= hitInfoTwo.point.z)
                {
                    if (PlaceParentOnTap)
                    {
                        // Place the parent object as well but keep the focus on the current game object
                        Vector3 currentMovement = hitInfo.point - gameObject.transform.position + Camera.main.transform.forward * -0.05f;
                        ParentGameObjectToPlace.transform.position += currentMovement;
                        ParentGameObjectToPlace.transform.rotation = toQuat;
                    }

                    else
                    {
                        gameObject.transform.position = hitInfo.point + Camera.main.transform.forward * -0.05f;
                        gameObject.transform.rotation = toQuat;
                    }
                }
                //A workspace is closer than the spatial mapping, move the note ontop of the workspace
                else if (hitInfo.point.z > hitInfoTwo.point.z)
                {
                    if (PlaceParentOnTap)
                    {
                        // Place the parent object as well but keep the focus on the current game object
                        Vector3 currentMovement = hitInfoTwo.point - gameObject.transform.position + Camera.main.transform.forward * -0.05f;
                        ParentGameObjectToPlace.transform.position += currentMovement;
                        ParentGameObjectToPlace.transform.rotation = toQuat;
                    }

                    else
                    {
                        gameObject.transform.position = hitInfoTwo.point + Camera.main.transform.forward * -0.05f;
                        gameObject.transform.rotation = toQuat;
                    }
                }
            }
        }
        public virtual void OnInputClicked(InputEventData eventData)
        {
            // On each tap gesture, toggle whether the user is in placing mode.
            if(count == 0)
            {
                IsBeingPlaced = !IsBeingPlaced;
            }else if(count == 1)
            {
                IsBeingPlaced = !IsBeingPlaced;
            }
            IsBeingPlaced = !IsBeingPlaced;
            GameObject notepad;
            Debug.Log(IsBeingPlaced);
            // If the user is in placing mode, display the spatial mapping mesh.
            if (IsBeingPlaced)
            {
                
                spatialMappingManager.DrawVisualMeshes = true;
                Debug.Log("Whut tha hell");
                //Debug.Log(gameObject.name + " : Removing existing world anchor if any.");
                notepad = GazeManager.Instance.HitObject.transform.gameObject;
                //if the notepad gameobject actually is a PostIT, and it has a parent workspace, remove that parent
                if(notepad.tag == "PostIT")
                {
                    if (notepad.transform.parent != null)
                    {
                        if (notepad.transform.parent.tag == "Workspace")
                        {
                            notepad.transform.SetParent(null);
                        }
                    }
                }
                count++;
                //anchorManager.RemoveAnchor(gameObject);
            }
            // If the user is not in placing mode, hide the spatial mapping mesh.
            else
            {
                notepad = GazeManager.Instance.HitObject.transform.gameObject;
                //If the notepad gameobject actually is a PostIT, and we are looking at a workspace, make that workspace the parent of the note
                if (notepad.tag == "PostIT")
                {
                    if (notepad.transform.parent == null)
                    {
                        if (workspaceHit)
                        {
                            notepad.transform.SetParent(hitInfo.transform.gameObject.transform);
                            notepad.transform.localPosition = hitInfo.transform.InverseTransformPoint(hitInfo.point);
                            notepad.transform.localRotation = Quaternion.identity;
                            notepad.transform.localScale = new Vector3(10, 10, 0.1f);
                            Debug.Log(notepad.transform.parent.name);
                        }
                    }
                }
                spatialMappingManager.DrawVisualMeshes = false;
                // Add world anchor when object placement is done.
                //anchorManager.AttachAnchor(gameObject, SavedAnchorFriendlyName);
                count--;
            }
        }
        private void DetermineParent()
        {
            if (ParentGameObjectToPlace == null)
            {
                if (gameObject.transform.parent == null)
                {
                    Debug.LogError("The selected GameObject has no parent.");
                    PlaceParentOnTap = false;
                }
                else
                {
                    Debug.LogError("No parent specified. Using immediate parent instead: " + gameObject.transform.parent.gameObject.name);
                    ParentGameObjectToPlace = gameObject.transform.parent.gameObject;
                }
            }
        }
    }
}
