using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CheekyVR;

// Use this script to display helper assists on the controllers.

// Assumes the default SteamVR camera rig.
// Modify the awake function if different names are being used.

public class CheekyVR_ControllerAssists : MonoBehaviour
{
    public Color highlightColour;

    [Header("Materials")]
    public Material mat_Original;
    public Material mat_System_Highlighted;
    public Material mat_Application_Highlighted;
    public Material mat_Grip_Highlighted;
    public Material mat_Touchpad_Highlighted;
    public Material mat_Trigger_Highlighted;

    [Header("Objects")]
    public GameObject systemButtonObject;
    public GameObject applicationButtonObject;
    public GameObject gripButtonLeftObject;
    public GameObject gripButtonRightObject;
    public GameObject touchpadObject;
    public GameObject triggerObject;

    [Header("Line Renderers")]
    public GameObject systemButtonLineRen;
    public GameObject applicationButtonLineRen;
    public GameObject gripButtonLeftLineRen;
    public GameObject gripButtonRightLineRen;
    public GameObject touchpadLineRen;
    public GameObject triggerLineRen;

    [Header("Text")]
    public GameObject systemButtonText;
    public GameObject applicationButtonText;
    public GameObject gripButtonLeftText;
    public GameObject gripButtonRightText;
    public GameObject touchpadText;
    public GameObject triggerText;

    private Renderer systemButtonRenderer;
    private Renderer applicationButtonRenderer;
    private Renderer gripButtonLeftRenderer;
    private Renderer gripButtonRightRenderer;
    private Renderer touchpadRenderer;
    private Renderer triggerRenderer;

    private bool systemButtonHighlighted = false;
    private bool applicationButtonHighlighted = false;
    private bool gripButtonsHighlighted = false;
    private bool touchpadHighlighted = false;
    private bool triggerHighlighted = false;

    public GameObject touchpadDisplay;

    private ControllerInput[] inputList;

    private int controllerIndex = -1;

    private bool enableTextHelpers = true;

    private bool initialised = false;

    // Update is called once per frame
    void Update ()
    {
        if(!initialised)
        {
            Initialise();
        }

        inputList = CheekyVR_InputManager.GetControllerInput();

        // Personal implementation of the trackpad cursor.
		if(inputList[controllerIndex].touchpadTouched)
        {
            touchpadDisplay.SetActive(true);

            // Magic numbers for correct display.
            touchpadDisplay.transform.localPosition = new Vector3(0.02f * -inputList[controllerIndex].touchpadAxisX, -0.01044881f, 0.02f * -inputList[controllerIndex].touchpadAxisY + -0.0482383f);
        }
        else
        {
            touchpadDisplay.SetActive(false);
        }

        // EXAMPLE: Press 1 through 5 to toggle highlights.

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SystemHighlight(!systemButtonHighlighted);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ApplicationHighlight(!applicationButtonHighlighted);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GripButtonHighlight(!gripButtonsHighlighted);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            TouchpadHighlight(!touchpadHighlighted);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            TriggerHighlight(!triggerHighlighted);
        }

        // END EXAMPLE.

        UpdateHighlightColour();
    }

    // Activate/Deactivate the highlight for this particular button.
    public void SystemHighlight(bool enabled)
    {
        // Enable highlight.
        if(enabled)
        {
            // Update the material of the button and flag it as highlighted.
            systemButtonRenderer.material = mat_System_Highlighted;
            systemButtonHighlighted = true;

            // If text helpers are enabled, display them.
            if(enableTextHelpers)
            {
                systemButtonLineRen.SetActive(true);
                systemButtonText.SetActive(true);
            }
        }
        // Disable highlight.
        else
        {
            // Update the material of the button and flag it as not highlighted.
            systemButtonRenderer.material = mat_Original;
            systemButtonHighlighted = false;

            // If text helpers are enabled, hide them.
            if (enableTextHelpers)
            {
                systemButtonLineRen.SetActive(false);
                systemButtonText.SetActive(false);
            }
        }
    }

    public void ApplicationHighlight(bool enabled)
    {
        if (enabled)
        {
            applicationButtonRenderer.material = mat_Application_Highlighted;
            applicationButtonHighlighted = true;

            if (enableTextHelpers)
            {
                applicationButtonLineRen.SetActive(true);
                applicationButtonText.SetActive(true);
            }
        }
        else
        {
            applicationButtonRenderer.material = mat_Original;
            applicationButtonHighlighted = false;

            if (enableTextHelpers)
            {
                applicationButtonLineRen.SetActive(false);
                applicationButtonText.SetActive(false);
            }
        }
    }

    public void GripButtonHighlight(bool enabled)
    {
        if (enabled)
        {
            gripButtonLeftRenderer.material = mat_Grip_Highlighted;
            gripButtonRightRenderer.material = mat_Grip_Highlighted;
            gripButtonsHighlighted = true;

            if (enableTextHelpers)
            {
                gripButtonLeftLineRen.SetActive(true);
                gripButtonLeftText.SetActive(true);
                gripButtonRightLineRen.SetActive(true);
                gripButtonRightText.SetActive(true);
            }
        }
        else
        {
            gripButtonLeftRenderer.material = mat_Original;
            gripButtonRightRenderer.material = mat_Original;
            gripButtonsHighlighted = false;

            if (enableTextHelpers)
            {
                gripButtonLeftLineRen.SetActive(false);
                gripButtonLeftText.SetActive(false);
                gripButtonRightLineRen.SetActive(false);
                gripButtonRightText.SetActive(false);
            }
        }
    }

    public void TouchpadHighlight(bool enabled)
    {
        if (enabled)
        {
            touchpadRenderer.material = mat_Touchpad_Highlighted;
            touchpadHighlighted = true;

            if (enableTextHelpers)
            {
                touchpadLineRen.SetActive(true);
                touchpadText.SetActive(true);
            }
        }
        else
        {
            touchpadRenderer.material = mat_Original;
            touchpadHighlighted = false;

            if (enableTextHelpers)
            {
                touchpadLineRen.SetActive(false);
                touchpadText.SetActive(false);
            }
        }
    }

    public void TriggerHighlight(bool enabled)
    {
        if (enabled)
        {
            triggerRenderer.material = mat_Trigger_Highlighted;
            triggerHighlighted = true;

            if (enableTextHelpers)
            {
                triggerLineRen.SetActive(true);
                triggerText.SetActive(true);
            }
        }
        else
        {
            triggerRenderer.material = mat_Original;
            triggerHighlighted = false;

            if (enableTextHelpers)
            {
                triggerLineRen.SetActive(false);
                triggerText.SetActive(false);
            }
        }
    }

    // Change highlight colour during runtime.
    private void UpdateHighlightColour()
    {
        // Check if a button is currently highlighted.
        if(systemButtonHighlighted)
        {
            // Update the colour to match the highlight colour.
            systemButtonRenderer.material.color = highlightColour;
        }

        if (applicationButtonHighlighted)
        {
            applicationButtonRenderer.material.color = highlightColour;
        }

        if (gripButtonsHighlighted)
        {
            gripButtonLeftRenderer.material.color = highlightColour;
            gripButtonRightRenderer.material.color = highlightColour;
        }

        if (touchpadHighlighted)
        {
            touchpadRenderer.material.color = highlightColour;
        }

        if (triggerHighlighted)
        {
            triggerRenderer.material.color = highlightColour;
        }
    }

    private void Initialise()
    {
        if (transform.parent.name == CheekyVR_InputManager.GetController(0).name)
        {
            controllerIndex = 0;
        }
        if (transform.parent.name == CheekyVR_InputManager.GetController(1).name)
        {
            controllerIndex = 1;
        }

        systemButtonRenderer = systemButtonObject.GetComponent<Renderer>();
        applicationButtonRenderer = applicationButtonObject.GetComponent<Renderer>();
        gripButtonLeftRenderer = gripButtonLeftObject.GetComponent<Renderer>();
        gripButtonRightRenderer = gripButtonRightObject.GetComponent<Renderer>();
        touchpadRenderer = touchpadObject.GetComponent<Renderer>();
        triggerRenderer = triggerObject.GetComponent<Renderer>();

        initialised = true;
    }
}
