using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheekyVR_ControllerCollisions : MonoBehaviour
{
    private float maximumInteractionRange = 1f;

    public List<GameObject> potentialInteractiveObjects;
    public List<GameObject> potentialRailingObjects;

    private void Awake ()
    {
        potentialInteractiveObjects = new List<GameObject>();
        potentialRailingObjects = new List<GameObject>();
    }

    private void LateUpdate()
    {
        // Clear all potential objects in late update to prevent the clearing of objects already found this frame.
        potentialInteractiveObjects.Clear();
        potentialRailingObjects.Clear();
    }

    private void OnTriggerStay(Collider col)
    {
        // If an object is tagged as "Interactive".
        if (col.gameObject.tag == "Interactive")
        {
            if (!potentialInteractiveObjects.Contains(col.gameObject))
            {
                if (col.gameObject.GetComponent<CheekyVR_PhysicalInteractions>() != null)
                {
                    // Add the object to the potential interactive object list.
                    potentialInteractiveObjects.Add(col.gameObject);
                }
                else
                {
                    // Add the object to the potential interactive object list.
                    potentialInteractiveObjects.Add(col.gameObject.transform.parent.gameObject);
                }
            }
        }

        // If an object is tagged as "Railing".
        if (col.gameObject.tag == "Railing")
        {
            if (!potentialRailingObjects.Contains(col.gameObject))
            {
                if (col.gameObject.GetComponent<CheekyVR_RailingMovement>() != null)
                {
                    // Add the object to the potential interactive object list.
                    potentialRailingObjects.Add(col.gameObject);
                }
            }
        }
    }

    public GameObject GetClosestObject(List<GameObject> objectList)
    {
        // Error code.
        int closestIndex = -1;

        // Loop through all objects currently within collider distance and find the closest to the controller.
        for (int i = 0; i < objectList.Count; i++)
        {
            if (Vector3.Distance(gameObject.transform.position, objectList[i].transform.position) < maximumInteractionRange)
            {
                closestIndex = i;
            }
            else if(objectList == potentialRailingObjects)
            {
                Debug.Log("Close rail");
                closestIndex = 0;
            }
        }

        // Check that an object has been found.
        if (closestIndex != -1)
        {
            return objectList[closestIndex];
        }
        else
        {
            Debug.Log("No closest object.");
            return null;
        }
    }
}
