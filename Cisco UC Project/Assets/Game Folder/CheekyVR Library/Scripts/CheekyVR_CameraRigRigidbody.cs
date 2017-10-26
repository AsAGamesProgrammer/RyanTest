using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CheekyVR;

public class CheekyVR_CameraRigRigidbody : MonoBehaviour
{
    private GameObject cameraRig;
    private GameObject HMD;

    private bool initialised = false;

	void Update ()
    {
		if(!initialised)
        {
            Initialise();
        }

        transform.position = new Vector3(HMD.transform.position.x, cameraRig.transform.position.y, HMD.transform.position.z);
	}

    private void Initialise()
    {
        cameraRig = CheekyVR_InputManager.GetCameraRig();
        HMD = CheekyVR_InputManager.GetHMD();

        initialised = true;
    }
}
