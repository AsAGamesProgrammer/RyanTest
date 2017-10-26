using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CheekyVR;

// This script is used to rotate an object over a defined period of time.

//  CAUTION: Can have unexpected results if rotating on more than 2 axes at a time due to gimbal lock.

public class CheekyVR_RotatePeriodic : MonoBehaviour
{
    public float duration = 2f;
    public float xChange = 0f;
    public float yChange = 0f;
    public float zChange = 0f;

    private float startTime = 0f;
    private bool rotationInProgress = false;
    private Vector3 startingRotation;
    private Vector3 targetRotation;

    public bool localSpace = false;

	void Update ()
    {
        // EXAMPLE: Press 0 to activate the rotation.

        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            StartRotation();
        }

        // END EXAMPLE.

        // Check if a rotation is in progress.
        if (rotationInProgress)
        {
            // Check if the rotation has ran it's full duration.
            if (Time.deltaTime - startTime < duration)
            {
                // Perform the rotation.
                if(localSpace)
                {
                    transform.localRotation = Quaternion.Euler(Vector3.Slerp(startingRotation, targetRotation, (Time.time - startTime) / duration));
                }
                else
                {
                    transform.rotation = Quaternion.Euler(Vector3.Slerp(startingRotation, targetRotation, (Time.time - startTime) / duration));
                }
            }
            else
            {
                rotationInProgress = false;
            }
        }
	}

    // Trigger the rotation.
    public void StartRotation()
    {
        // Flag the rotation as in progress.
        rotationInProgress = true;

        // Store the starting and target rotations for slerping.
        if(localSpace)
        {
            startingRotation = transform.localRotation.eulerAngles;
            targetRotation = new Vector3(transform.localRotation.eulerAngles.x + xChange, transform.localRotation.eulerAngles.y + yChange, transform.localRotation.eulerAngles.z + zChange);
        }
        else
        {
            startingRotation = transform.rotation.eulerAngles;
            targetRotation = new Vector3(transform.rotation.eulerAngles.x + xChange, transform.rotation.eulerAngles.y + yChange, transform.rotation.eulerAngles.z + zChange);
        }

        // Store the rotation start time for slerping.
        startTime = Time.time;
    }
}
