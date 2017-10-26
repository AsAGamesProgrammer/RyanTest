using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CheekyVR;

public class CheekyVR_ControllerMovement : MonoBehaviour
{
    private static float moveSpeed = 0.1f;
    private static float rotateSpeed = 5f;
    private static GameObject s_HMD;
    private static GameObject s_cameraRig;

    private static Vector3 s_savedRight;
    private static Vector3 s_savedForward;

    private static GameObject s_HMDPivot;

    private static bool currentlyMoving = false;

    private bool initialised = false;
	
	// Update is called once per frame
	void Update ()
    {
		if(!initialised)
        {
            Initialise();
        }
	}

    private void Initialise()
    {
        s_HMD = CheekyVR_InputManager.GetHMD();
        s_cameraRig = CheekyVR_InputManager.GetCameraRig();

        s_HMDPivot = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Destroy(s_HMDPivot.GetComponent<MeshFilter>());
        Destroy(s_HMDPivot.GetComponent<BoxCollider>());
        Destroy(s_HMDPivot.GetComponent<MeshRenderer>());
        s_HMDPivot.name = "HMD Pivot Helper";

        initialised = true;
    }

    static public void Move(float x, float z)
    {
        /*if(!currentlyMoving)
        {
            s_savedRight = s_HMD.transform.right;
            s_savedForward = s_HMD.transform.forward;

            currentlyMoving = true;
        }*/

        s_savedRight = s_HMD.transform.right;
        s_savedForward = s_HMD.transform.forward;

        Vector3 rightNoY = new Vector3(s_savedRight.x, 0, s_savedRight.z);
        Vector3 forwardNoY = new Vector3(s_savedForward.x, 0, s_savedForward.z);

        s_cameraRig.transform.position += rightNoY * x * moveSpeed;
        s_cameraRig.transform.position += forwardNoY * z * moveSpeed;
    }

    static public void Rotate(float x)
    {
        s_HMDPivot.transform.position = s_HMD.transform.position;

        s_cameraRig.transform.parent = s_HMDPivot.transform;

        s_HMDPivot.transform.Rotate(Vector3.up, rotateSpeed * x);

        s_cameraRig.transform.parent = null;
    }

    static public void StopMovement()
    {
        currentlyMoving = false;
    }
}
