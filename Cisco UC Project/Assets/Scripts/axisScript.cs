using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class axisScript : MonoBehaviour {

    public GameObject axisX;
    public GameObject axisY;
    public GameObject axisZ;

    public GameObject origin;

    public GameObject testCube;

    private float minX = 0;
    private float maxX = 10;
    private float minY = 0;
    private float maxY = 10;
    private float minZ = 0;
    private float maxZ = 10;

    private float xScaleFactor;
    private float zScaleFactor;
    private float yScaleFactor;

    // Use this for initialization
    void Start ()
    {
        findScaleFactors();
        plotPointAt(7, 5, 6);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void findScaleFactors()
    {
        //Scale factor for X
        float initialScaleX = maxX - minX;
        float realScaleX = axisX.transform.localScale.y;
        xScaleFactor = realScaleX / initialScaleX;

        //Scale factor for Y
        float initialScaleY = maxY - minY;
        float realScaleY = axisY.transform.localScale.y;
        yScaleFactor = realScaleY / initialScaleY;

        //Scale factor for Z
        float initialScaleZ = maxZ - minZ;
        float realScaleZ = axisZ.transform.localScale.y;
        zScaleFactor = realScaleZ / initialScaleZ;
    }

    void plotPointAt(float x, float y, float z)
    {
        Vector3 newPosition = new Vector3();
        newPosition.x = x * xScaleFactor;
        newPosition.y = y * yScaleFactor;
        newPosition.z = z * zScaleFactor;

        newPosition += origin.transform.position;

        testCube.transform.position = newPosition;
    }
}
