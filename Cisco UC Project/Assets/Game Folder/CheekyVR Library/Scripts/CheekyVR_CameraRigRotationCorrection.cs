using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CheekyVR;

namespace CheekyVR
{
    public class CheekyVR_CameraRigRotationCorrection : MonoBehaviour
    {
        private static GameObject s_cameraRig;
        private static GameObject pivot;
        private static GameObject point;

        private bool initialised = false;

        // Update is called once per frame
        void Update()
        {
            if (!initialised)
            {
                Initialise();
            }
        }

        public static Vector3 CorrectRotation(Vector3 uncorrectedRotation)
        {
            pivot.transform.position = Vector3.zero;
            pivot.transform.rotation = Quaternion.identity;

            point.transform.position = uncorrectedRotation;
            point.transform.rotation = Quaternion.identity;
            point.transform.parent = pivot.transform;

            float rigRotation = s_cameraRig.transform.rotation.eulerAngles.y;

            pivot.transform.Rotate(Vector3.up, rigRotation);

            point.transform.parent = null;

            //Debug.Log("Original Velocity: " + uncorrectedRotation.x + ", " + uncorrectedRotation.y + ", " + uncorrectedRotation.z);
            //Debug.Log("New Velocity: " + returnValue.x + ", " + returnValue.y + ", " + returnValue.z);

            return point.transform.position;
        }

        private void Initialise()
        {
            s_cameraRig = CheekyVR_InputManager.GetCameraRig();

            GameObject grouping = GameObject.CreatePrimitive(PrimitiveType.Cube);
            grouping.transform.position = Vector3.zero;
            Destroy(grouping.GetComponent<MeshFilter>());
            Destroy(grouping.GetComponent<BoxCollider>());
            Destroy(grouping.GetComponent<MeshRenderer>());
            grouping.name = "Calculation Helpers";

            pivot = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(pivot.GetComponent<MeshFilter>());
            Destroy(pivot.GetComponent<BoxCollider>());
            Destroy(pivot.GetComponent<MeshRenderer>());
            pivot.transform.parent = grouping.transform;
            
            point = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(point.GetComponent<MeshFilter>());
            Destroy(point.GetComponent<BoxCollider>());
            Destroy(point.GetComponent<MeshRenderer>());
            point.transform.parent = grouping.transform;

            initialised = true;
        }
    }
}

