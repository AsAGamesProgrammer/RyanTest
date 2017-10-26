using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CheekyVR;

public class CheekyVR_Teleport : MonoBehaviour 
{
    [Tooltip("Set the tolerance for what is defined as a walkable surface. The higher the value the steeper the slope allowed.")]
    [Range(0, 0.003f)]
    public float surfaceSlopeTolerance = 0.003f;

    public GameObject leftControllerDirectionOculus;
    public GameObject rightControllerDirectionOculus;
    private GameObject activeControllerDirection;
    
    private GameObject cameraRig;
    private GameObject HMD;
    private Vector3 HMD_Offset;
    private float originalYHeight = 0f;
    private GameObject activeController;
    private bool seekingTarget = false;
    private Vector3 targetLocation = Vector3.zero;
    private bool targetLocationValid = false;
    private LineRenderer lineRen;
    private Material lineRenMaterial;

    private bool allowTeleport = true;

    private GameObject[] teleportMaps;
    private LayerMask teleportLayer;

    private bool initialised = false;

    void Update() 
    {
        if(!initialised)
        {
            Initialise();
        }
    }

    void FixedUpdate () 
	{
		if(seekingTarget)
        {
            RaycastHit hit;

            if (Physics.Raycast(activeControllerDirection.transform.position, activeControllerDirection.transform.forward, out hit, Mathf.Infinity, teleportLayer))
            {
                if (!lineRen.enabled)
                {
                    lineRen.enabled = true;
                }

                // Up axis as a quaternion;
                Quaternion upAxis = Quaternion.Euler(Vector3.up);

                // Get the current rotation of the snap object.
                Quaternion targetSurfaceRotation = Quaternion.Euler(hit.normal);

                // Calculate the angle between the snap target rotation and current rotation.
                float rotationDifference = Mathf.Acos(targetSurfaceRotation.w * upAxis.w + targetSurfaceRotation.x * upAxis.x + targetSurfaceRotation.y * upAxis.y + targetSurfaceRotation.z * upAxis.z);

                // Pick the correct quaternion out of the 2 possible.
                if (rotationDifference > Mathf.PI / 2)
                {
                    rotationDifference = Mathf.PI - rotationDifference;
                }

                // No difference will return NaN. Use 0f instead to prevent errors later;
                if (float.IsNaN(rotationDifference))
                {
                    rotationDifference = 0f;
                }

                // Debug visuals.
                Debug.DrawLine(hit.point, hit.point + Vector3.up * 5f, Color.blue);
                Debug.DrawLine(hit.point, hit.point + hit.normal * 5f, Color.yellow);

                if (rotationDifference < surfaceSlopeTolerance)
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Teleport Collisions"))
                    {
                        lineRenMaterial.SetColor("_Color", Color.green);
                        targetLocationValid = true;

                        targetLocation = hit.point;
                    }
                    else
                    {
                        lineRenMaterial.SetColor("_Color", Color.red);
                        targetLocationValid = false;
                    }
                }
                else
                {
                    lineRenMaterial.SetColor("_Color", Color.red);
                    targetLocationValid = false;
                }

                lineRen.SetPosition(0, activeControllerDirection.transform.position);
                lineRen.SetPosition(1, hit.point);
            }
            else
            {
                if(!lineRen.enabled)
                {
                    lineRen.enabled = true;
                }

                lineRen.SetPosition(0, activeControllerDirection.transform.position);
                lineRen.SetPosition(1, activeControllerDirection.transform.position + activeControllerDirection.transform.forward * 1000f);

                lineRenMaterial.SetColor("_Color", Color.red);
                targetLocationValid = false;
            }
        }
    }

    public void ActivateTeleportBeam(GameObject controller)
    {
        if(!seekingTarget)
        {
            if (allowTeleport)
            {
                if (GetComponent<CheekyVR_Teleport>().enabled)
                {
                    if (!seekingTarget)
                    {
                        activeController = controller;

                        if(controller.name == "Controller (left)")
                        {
                            activeControllerDirection = leftControllerDirectionOculus;
                        }
                        else if (controller.name == "Controller (right)")
                        {
                            activeControllerDirection = rightControllerDirectionOculus;
                        }

                        for (int i = 0; i < teleportMaps.Length; i++)
                        {
                            if (teleportMaps[i].GetComponent<Renderer>() != null)
                            {
                                teleportMaps[i].GetComponent<Renderer>().enabled = true;
                            }
                        }

                        seekingTarget = true;
                    }
                }
            }
        }
    }

    public void DeactivateTeleportBeam()
    {
        if(seekingTarget)
        {
            if (allowTeleport)
            {
                if (GetComponent<CheekyVR_Teleport>().enabled)
                {
                    if (seekingTarget)
                    {
                        for (int i = 0; i < teleportMaps.Length; i++)
                        {
                            if (teleportMaps[i].GetComponent<Renderer>() != null)
                            {
                                teleportMaps[i].GetComponent<Renderer>().enabled = false;
                            }
                        }

                        lineRen.enabled = false;

                        seekingTarget = false;

                        if (targetLocationValid)
                        {
                            TeleportPlayer();
                        }
                    }
                }
            }
        }
    }

    public void EnableTeleport()
    {
        allowTeleport = true;
    }

    public void DisableTeleport()
    {
        allowTeleport = false;
    }

    private void TeleportPlayer()
    {
        HMD_Offset = HMD.transform.position - cameraRig.transform.position;
        HMD_Offset.y = 0;

        cameraRig.transform.position = targetLocation - HMD_Offset;
    }

    private void Initialise()
    {
        cameraRig = CheekyVR_InputManager.GetCameraRig();

        HMD = CheekyVR_InputManager.GetHMD();

        lineRen = GetComponent<LineRenderer>();

        lineRenMaterial = lineRen.material;

        teleportMaps = GameObject.FindGameObjectsWithTag("Teleport Map");

        teleportLayer = 1 << LayerMask.NameToLayer("Teleport Collisions");

        originalYHeight = cameraRig.transform.position.y;

        for (int i = 0; i < teleportMaps.Length; i++)
        {
            if (teleportMaps[i].GetComponent<Renderer>() != null)
            {
                teleportMaps[i].GetComponent<Renderer>().enabled = false;
            }
        }

        initialised = true;
    }
}
