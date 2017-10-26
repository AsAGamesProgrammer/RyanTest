using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using CheekyVR;

namespace CheekyVR
{
    [Serializable]
    public class AnalyticsData
    {
        public string dateStamp;
        public string timeStamp;

        public string userName;
        public string userID;

        public float pollingRate;

        public List<Vector3> cameraRigPosition;

        public List<Vector3> HMD_Position;
        public List<Quaternion> HMD_Rotation;

        public List<Vector3> leftControllerPosition;
        public List<Quaternion> leftControllerRotation;

        public List<Vector3> rightControllerPosition;
        public List<Quaternion> rightControllerRotation;

        public AnalyticsData()
        {
            dateStamp = DateTime.Now.ToString("dd/MM/yyyy");
            timeStamp = DateTime.Now.ToString("hh:mm:ss");

            cameraRigPosition = new List<Vector3>();

            HMD_Position = new List<Vector3>();
            HMD_Rotation = new List<Quaternion>();

            leftControllerPosition = new List<Vector3>();
            leftControllerRotation = new List<Quaternion>();

            rightControllerPosition = new List<Vector3>();
            rightControllerRotation = new List<Quaternion>();
        }

        public void clearCurrentData()
        {
            dateStamp = DateTime.Now.ToString("dd/MM/yyyy");
            timeStamp = DateTime.Now.ToString("hh:mm:ss");

            cameraRigPosition.Clear();

            HMD_Position.Clear();
            HMD_Rotation.Clear();

            leftControllerPosition.Clear();
            leftControllerRotation.Clear();

            rightControllerPosition.Clear();
            rightControllerRotation.Clear();
        }
    }
}

public class CheekyVR_Analytics_Loading : MonoBehaviour
{
    private string filePath = "D:/JSONTEST/30-08-2017_03-55-20_UserTest.json";
    private AnalyticsData dataStore;

    public GameObject graphNode;

    private Color cameraRigEdgeColour = Color.yellow;
    private Color HMD_EdgeColour = Color.green;
    private Color leftControllerEdgeColour = Color.magenta;
    private Color rightControllerEdgeColour = Color.cyan;

    public void LoadJsonInput()
    {
        string jsonSource = File.ReadAllText(filePath);

        dataStore = JsonUtility.FromJson<AnalyticsData>(jsonSource);

        // Camera rig graph.
        /*for(int i = 0; i < dataStore.cameraRigPosition.Count; i++)
        {
            GameObject node = Instantiate(graphNode, dataStore.cameraRigPosition[i], Quaternion.identity);

            if(i > 0)
            {
                Vector3 origin = dataStore.cameraRigPosition[i];
                Vector3 previous = dataStore.cameraRigPosition[i - 1];

                Vector3[] vecArr = new Vector3[2];
                vecArr[0] = origin;
                vecArr[1] = previous;

                node.GetComponent<LineRenderer>().SetPositions(vecArr);
                node.GetComponent<LineRenderer>().material.color = cameraRigEdgeColour;
            }
            else
            {
                node.GetComponent<LineRenderer>().enabled = false;
            }
        }*/ 

        // HMD graph.
        for (int i = 0; i < dataStore.HMD_Position.Count; i++)
        {
            GameObject node = Instantiate(graphNode, dataStore.HMD_Position[i], Quaternion.identity);

            if (i > 0)
            {
                Vector3 origin = dataStore.HMD_Position[i];
                origin.y = dataStore.cameraRigPosition[i].y;
                Vector3 previous = dataStore.HMD_Position[i - 1];
                previous.y = origin.y = dataStore.cameraRigPosition[i].y;

                Vector3[] vecArr = new Vector3[2];
                vecArr[0] = origin;
                vecArr[1] = previous;

                node.GetComponent<LineRenderer>().SetPositions(vecArr);
                node.GetComponent<LineRenderer>().material.color = HMD_EdgeColour;
            }
            else
            {
                node.GetComponent<LineRenderer>().enabled = false;
            }
        }

        // Left controller graph.
        /*for (int i = 0; i < dataStore.leftControllerPosition.Count; i++)
        {
            GameObject node = Instantiate(graphNode, dataStore.leftControllerPosition[i], Quaternion.identity);

            if (i > 0)
            {
                Vector3 origin = dataStore.leftControllerPosition[i];
                Vector3 previous = dataStore.leftControllerPosition[i - 1];

                Vector3[] vecArr = new Vector3[2];
                vecArr[0] = origin;
                vecArr[1] = previous;

                node.GetComponent<LineRenderer>().SetPositions(vecArr);
                node.GetComponent<LineRenderer>().material.color = leftControllerEdgeColour;
            }
            else
            {
                node.GetComponent<LineRenderer>().enabled = false;
            }
        }

        // Right controller graph.
        for (int i = 0; i < dataStore.rightControllerPosition.Count; i++)
        {
            GameObject node = Instantiate(graphNode, dataStore.rightControllerPosition[i], Quaternion.identity);

            if (i > 0)
            {
                Vector3 origin = dataStore.rightControllerPosition[i];
                Vector3 previous = dataStore.rightControllerPosition[i - 1];

                Vector3[] vecArr = new Vector3[2];
                vecArr[0] = origin;
                vecArr[1] = previous;

                node.GetComponent<LineRenderer>().SetPositions(vecArr);
                node.GetComponent<LineRenderer>().material.color = rightControllerEdgeColour;
            }
            else
            {
                node.GetComponent<LineRenderer>().enabled = false;
            }
        }*/
    }
}
