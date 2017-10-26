using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CheekyVR;

// This script will force any object to face the HMD.

public class CheekyVR_LookAtHMD : MonoBehaviour
{
    private Transform target;

	// Use this for initialization
	void Awake ()
    {
        target = CheekyVR_InputManager.GetHMD().transform;
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - target.position);
	}
}
