using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CheekyVR;

public class CheekyVR_RailingMovement : MonoBehaviour
{
    private bool initialised = false;

    private bool isGrabbed = false;

    private Vector3 rigOrigin = Vector3.zero;
    private Vector3 grabPosition = Vector3.zero;

    private Vector3 previousPosition = Vector3.zero;
    private Vector3 storedVelocity = Vector3.zero;

    private GameObject cameraRig;
    private GameObject cameraRigColliderObject;
    private BoxCollider cameraRigCollider;
    private Rigidbody cameraRigRigidbody;
    private GameObject leftController;
    private GameObject rightController;

    private GameObject activeController;
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if(!initialised)
        {
            Initialise();
        }

		if(isGrabbed)
        {
            Vector3 currentControllerPosition = activeController.transform.localPosition;

            Vector3 offset = currentControllerPosition - grabPosition;

            cameraRig.transform.position = rigOrigin - offset;

            storedVelocity = cameraRig.transform.position - previousPosition;

            previousPosition = cameraRig.transform.position;

            Debug.Log("Offset: " + offset);
            Debug.Log("Camera Rig: " + cameraRig.transform.position.x + ", " + cameraRig.transform.position.y + ", " + cameraRig.transform.position.z);
        }
	}
    
    private void Initialise()
    {
        cameraRig = CheekyVR_InputManager.GetCameraRig();
        leftController = CheekyVR_InputManager.GetController(0);
        rightController = CheekyVR_InputManager.GetController(1);

        cameraRigColliderObject = CheekyVR_InputManager.GetCameraRigColliderObject();
        cameraRigCollider = cameraRigColliderObject.GetComponent<BoxCollider>();
        cameraRigRigidbody = cameraRig.GetComponent<Rigidbody>();

        initialised = true;
    }

    public void Grab(float controllerID)
    {
        if(controllerID == 0)
        {
            activeController = leftController;
        }
        else if(controllerID == 1)
        {
            activeController = rightController;
        }

        rigOrigin = cameraRig.transform.position;
        grabPosition = activeController.transform.localPosition;

        isGrabbed = true;

        cameraRigCollider.enabled = false;

        Debug.Log("Grabbed rail");
    }

    public void Release()
    {
        cameraRigRigidbody.velocity = storedVelocity * 90f;

        Debug.Log("Released rail with velocity: " + cameraRigRigidbody.velocity);

        isGrabbed = false;

        cameraRigCollider.enabled = true;
    }
}
