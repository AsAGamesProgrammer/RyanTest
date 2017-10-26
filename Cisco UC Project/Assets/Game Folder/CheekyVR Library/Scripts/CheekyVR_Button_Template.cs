using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script gives button functionality, and can be expanded to allow specific actions to be carried out.
// Objects using this script should be tagged as "Button".

// CAUTION: Script assumes button movement on the Y axis.

[RequireComponent(typeof(Rigidbody))]
public class CheekyVR_Button_Template : MonoBehaviour
{
    private int numColliding = 0;

    private Renderer neutralRenderer;
    private Renderer displacedRenderer;

    [Tooltip("Should the button reset to it's default position when not being interacted with?")]
    public bool multiUse = false;

    public bool onlyControllerInteraction = false;

    private void Awake()
    {
        neutralRenderer = transform.GetChild(0).GetComponent<Renderer>();
        displacedRenderer = transform.GetChild(1).GetComponent<Renderer>();

        displacedRenderer.enabled = false;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (!onlyControllerInteraction)
        {
            if(col.gameObject.tag != "Button")
            {
                if (numColliding == 0)
                {
                    neutralRenderer.enabled = false;
                    displacedRenderer.enabled = true;

                    DoButtonAction();
                }

                numColliding++;
            }
        }
    }

    private void OnCollisionExit(Collision col)
    {
        if (!onlyControllerInteraction)
        {
            if (col.gameObject.tag != "Button")
            {
                numColliding--;

                if (numColliding == 0 && multiUse)
                {
                    neutralRenderer.enabled = true;
                    displacedRenderer.enabled = false;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if(onlyControllerInteraction)
        {
            if (col.gameObject.tag == "Controllers")
            {
                if (numColliding == 0)
                {
                    neutralRenderer.enabled = false;
                    displacedRenderer.enabled = true;

                    DoButtonAction();
                }

                numColliding++;
            }
        }
        else
        {
            if (numColliding == 0)
            {
                neutralRenderer.enabled = false;
                displacedRenderer.enabled = true;

                DoButtonAction();
            }

            numColliding++;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if(onlyControllerInteraction)
        {
            if (col.gameObject.tag == "Controllers")
            {
                numColliding--;

                if (numColliding == 0 && multiUse)
                {
                    neutralRenderer.enabled = true;
                    displacedRenderer.enabled = false;
                }
            }
        }
        else
        {
            numColliding--;

            if (numColliding == 0 && multiUse)
            {
                neutralRenderer.enabled = true;
                displacedRenderer.enabled = false;
            }
        }
    }

    private void DoButtonAction()
    {
        // Specific action code goes here.

        // EXAMPLE: Button changes to random colour on press.
        Color randomColour = new Color(Random.value, Random.value, Random.value);

        neutralRenderer.material.color = randomColour;
        displacedRenderer.material.color = randomColour;

        // END EXAMPLE.
    }
}
