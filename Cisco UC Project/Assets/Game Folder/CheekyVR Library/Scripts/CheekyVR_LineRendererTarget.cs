using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is used to render a line to a chosen target.

public class CheekyVR_LineRendererTarget : MonoBehaviour
{
    private LineRenderer lineRen;
    public Transform target;

	void Start ()
    {
        lineRen = GetComponent<LineRenderer>();
	}

	void Update ()
    {
        lineRen.SetPosition(0, transform.position);
        lineRen.SetPosition(1, target.position);
	}
}
