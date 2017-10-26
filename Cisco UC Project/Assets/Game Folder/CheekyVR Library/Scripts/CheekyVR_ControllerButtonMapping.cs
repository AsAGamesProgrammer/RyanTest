using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CheekyVR;

// Use this script to map button presses to certain functions.

// Attach this script to each controller in the SteamVR CameraRig.

// Requires interactive objects to be tagged as "Interactive".

// Assumes the default SteamVR camera rig.
// Modify the awake function if different names are being used.

public class CheekyVR_ControllerButtonMapping : MonoBehaviour
{
    // REQUIRED.
    private ControllerInput[] inputList;
    // END REQUIRED.

    private List<CheekyVR_PhysicalInteractions> closestInteractiveObject = new List<CheekyVR_PhysicalInteractions>();
    private List<CheekyVR_RailingMovement> closestRailingObject = new List<CheekyVR_RailingMovement>();

    private CheekyVR_Teleport teleportScript;

    private GameObject cameraRig;
    private GameObject HMD;
    private GameObject leftController;
    private GameObject rightController;
    private CheekyVR_ControllerCollisions leftControllerCollisionScript;
    private CheekyVR_ControllerCollisions rightControllerCollisionScript;

    private bool initialised = false;

    void Update()
    {
        if (!initialised)
        {
            Initialise();
        }

        // Get the raw input from the input manager.
        inputList = CheekyVR_InputManager.GetControllerInput();

        // Attempt to grab interactive object on trigger down.
        if (inputList[0].triggerDown)
        {
            if (closestInteractiveObject[0] != null)
            {
                closestInteractiveObject[0].DoAction();
            }
        }

        // Attempt to grab interactive object on trigger down.
        if (inputList[1].triggerDown)
        {
            if (closestInteractiveObject[1] != null)
            {
                closestInteractiveObject[1].DoAction();
            }
        }

        // Attempt to drop an interactive object.
        if (inputList[0].triggerUp)
        {
            if (closestInteractiveObject[0] != null)
            {
                closestInteractiveObject[0].EndAction();
            }
        }

        // Attempt to drop an interactive object.
        if (inputList[1].triggerUp)
        {
            if (closestInteractiveObject[1] != null)
            {
                closestInteractiveObject[1].EndAction();
            }
        }

        if (inputList[0].gripButtonDown)
        {
            // Check that there is an interactive object nearby.
            if (leftControllerCollisionScript.GetClosestObject(leftControllerCollisionScript.potentialInteractiveObjects) != null)
            {
                // Get the interactive object script.
                closestInteractiveObject[0] = leftControllerCollisionScript.GetClosestObject(leftControllerCollisionScript.potentialInteractiveObjects).GetComponent<CheekyVR_PhysicalInteractions>();

                //Debug.Log(closestInteractiveObject.gameObject.name);

                if (closestInteractiveObject[0] != null)
                {
                    // If the interactive object is not currently being held.
                    if (!closestInteractiveObject[0].GetHeldStatus(0))
                    {
                        // Grab the object.
                        closestInteractiveObject[0].GrabObject(0);
                    }
                }
            }

            // Check that there is a railing object nearby.
            if (leftControllerCollisionScript.GetClosestObject(leftControllerCollisionScript.potentialRailingObjects) != null)
            {
                // Get the interactive object script.
                closestRailingObject[0] = leftControllerCollisionScript.GetClosestObject(leftControllerCollisionScript.potentialRailingObjects).GetComponent<CheekyVR_RailingMovement>();

                if (closestRailingObject[0] != null)
                {
                    closestRailingObject[0].Grab(0);
                }
            }
        }

        if (inputList[1].gripButtonDown)
        {
            // Check that there is an interactive object nearby.
            if (rightControllerCollisionScript.GetClosestObject(rightControllerCollisionScript.potentialInteractiveObjects) != null)
            {
                // Get the interactive object script.
                closestInteractiveObject[1] = rightControllerCollisionScript.GetClosestObject(rightControllerCollisionScript.potentialInteractiveObjects).GetComponent<CheekyVR_PhysicalInteractions>();

                //Debug.Log(closestInteractiveObject.gameObject.name);

                if (closestInteractiveObject[1] != null)
                {
                    // If the interactive object is not currently being held.
                    if (!closestInteractiveObject[1].GetHeldStatus(1))
                    {
                        // Grab the object.
                        closestInteractiveObject[1].GrabObject(1);
                    }
                }
            }

            // Check that there is a railing object nearby.
            if (rightControllerCollisionScript.GetClosestObject(rightControllerCollisionScript.potentialRailingObjects) != null)
            {
                // Get the interactive object script.
                closestRailingObject[1] = rightControllerCollisionScript.GetClosestObject(rightControllerCollisionScript.potentialRailingObjects).GetComponent<CheekyVR_RailingMovement>();

                if (closestRailingObject[1] != null)
                {
                    closestRailingObject[1].Grab(1);
                }
            }
        }

        if (inputList[0].gripButtonUp)
        {
            // Check that there is an interactive object cached.
            if (closestInteractiveObject[0] != null)
            {
                closestInteractiveObject[0].DropObject(0);

                // Finished with the object, set the reference back to null.
                closestInteractiveObject[0] = null;
            }

            // Check that there is a railing object cached.
            if (closestRailingObject[0] != null)
            {
                closestRailingObject[0].Release();

                // Finished with the object, set the reference back to null.
                closestRailingObject[0] = null;
            }
        }

        if (inputList[1].gripButtonUp)
        {
            // Check that there is an interactive object cached.
            if (closestInteractiveObject[1] != null)
            {
                closestInteractiveObject[1].DropObject(1);

                // Finished with the object, set the reference back to null.
                closestInteractiveObject[1] = null;
            }

            // Check that there is a railing object cached.
            if (closestRailingObject[1] != null)
            {
                closestRailingObject[1].Release();

                // Finished with the object, set the reference back to null.
                closestRailingObject[1] = null;
            }
        }

        // Activate teleport targetting.
        if (inputList[1].touchpadDown)
        {

        }

        // Deactivate teleport targetting.
        if (inputList[1].touchpadUp)
        {
            
        }

        if (inputList[0].applicationButtonDown)
        {
            teleportScript.ActivateTeleportBeam(leftController);
        }

        if (inputList[1].applicationButtonDown)
        {
            teleportScript.ActivateTeleportBeam(rightController);
        }

        if(inputList[0].applicationButtonUp)
        {
            teleportScript.DeactivateTeleportBeam();
        }

        if (inputList[1].applicationButtonUp)
        {
            teleportScript.DeactivateTeleportBeam();
        }
    }

    private void ToggleHelperModel(bool enabled)
    {
        transform.GetChild(1).gameObject.SetActive(enabled);
    }

    private void Initialise()
    {
        for(int i = 0; i < 2; i++)
        {
            closestInteractiveObject.Add(null);
            closestRailingObject.Add(null);
        }

        teleportScript = FindObjectOfType<CheekyVR_Teleport>();

        cameraRig = CheekyVR_InputManager.GetCameraRig();
        HMD = CheekyVR_InputManager.GetHMD();

        leftController = CheekyVR_InputManager.GetController(0);
        rightController = CheekyVR_InputManager.GetController(1);

        leftControllerCollisionScript = leftController.GetComponent<CheekyVR_ControllerCollisions>();
        rightControllerCollisionScript = rightController.GetComponent<CheekyVR_ControllerCollisions>();

        initialised = true;
    }
}
