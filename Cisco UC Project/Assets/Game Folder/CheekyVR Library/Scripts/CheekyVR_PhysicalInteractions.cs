using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CheekyVR;

namespace CheekyVR
{
    public enum InteractiveObjectType
    {
        Standard
    }
}

[RequireComponent(typeof(Rigidbody))]
public class CheekyVR_PhysicalInteractions : MonoBehaviour 
{
    // Spring strength.
    public const float kGrabSpringLinearTightness = 500f; // Strength.
    public const float kGrabSpringLinearDampingMin = 10.0f; // Damping minimum.
    public const float kGrabSpringLinearDampingMax = 20.0f; // Damping maximum
    public const float kGrabSpringAngularTightness = 500f; // Angular Strength
    public const float kGrabSpringAngularDamping = 50f; // Angular Damping

    public enum InteractionType
    {
        OneHanded, Mixed, TwoHanded
    }

    [Tooltip("What type of interaction should this object support.")]
    public InteractionType objectInteractionType = InteractionType.OneHanded;
    [Tooltip("Should the object snap to hand and align its rotation with the controller.")]
    public bool oneHandedSnapToHand = false;
    [Tooltip("Should the object use realistic physics or ignore collisions when being interacted with?")]
    public bool useInteractionPhysics = true;
    [Tooltip("Should the object provide haptic feedback for collisions?")]
    public bool hapticFeedbackEnabled = true;
    [Tooltip("Strength of the haptic feedback.")]
    [Range(0, 3999)]
    public int hapticStrength = 500;

    private bool springsEnabled = false;
    private float springCooldown = 0.3f;
    private float springCooldownRemaining = 0f;

    private bool initialised = false;

    private GameObject leftController;
    private GameObject rightController;
    private GameObject cameraRig;

    private Vector3 directionBetweenControllersNormalized = Vector3.zero;

    private bool holdingLeftHand = false;
    private Vector3 leftHandOffset = Vector3.zero;
    private bool holdingRightHand = false;
    private Vector3 rightHandOffset = Vector3.zero;
    private Vector3 averageHandOffset = Vector3.zero;

    private bool holdingActive = false;
    private bool holdingLeftPreviousFrame = false;
    private bool holdingRightPreviousFrame = false;

    private GameObject physicsTarget;

    private Collider[] objectCollider;
    private Rigidbody objectRigidbody;

    private ControllerInput[] inputList;

    public InteractiveObjectType currentObjectType = InteractiveObjectType.Standard;

    public bool enableVisualDebug = true;

    void Awake () 
	{
        initialised = false;
    }
	
	void Update () 
	{
        if(!initialised)
        {
            Initialise();

            initialised = true;
        }

        inputList = CheekyVR_InputManager.GetControllerInput();

        // Handle spring cooldown.
        if(springsEnabled)
        {
            springCooldownRemaining -= Time.deltaTime;

            if(springCooldownRemaining <= 0f)
            {
                springsEnabled = false;
            }
        }

        // If holding the object.
        if (holdingActive)
        {
            // Check the current hand combination.
            if (holdingLeftHand && holdingRightHand)
            {
                CalculateTransformTwoHands();
            }
            else if (holdingLeftHand)
            {
                // If one handed snapping is enabled, update the physics target to match.
                if(oneHandedSnapToHand)
                {
                    UpdatePhysicsLeftHand();
                }
            }
            else if (holdingRightHand)
            {
                // If one handed snapping is enabled, update the physics target to match.
                if (oneHandedSnapToHand)
                {
                    UpdatePhysicsRightHand();
                }
            }

            // If no springs, disable all rigidbody functionality. Use direct transformations.
            if (!springsEnabled)
            {
                objectRigidbody.useGravity = false;
                objectRigidbody.velocity = Vector3.zero;

                transform.position = physicsTarget.transform.position;
                transform.rotation = physicsTarget.transform.rotation;
            }
            else if(springsEnabled)
            {
                // Calculate the torque and force values using the position of the controller and current status of the rigidbody.
                var torque = CheekyVR_PhysicsUtilities.CalculateTorque(objectRigidbody, physicsTarget.transform.rotation,
                                                                  kGrabSpringAngularTightness,
                                                                  kGrabSpringAngularDamping);

                var force = CheekyVR_PhysicsUtilities.CalculateForce(objectRigidbody, physicsTarget.transform.position, objectRigidbody.transform.position,
                                                                 kGrabSpringLinearTightness,
                                                                 kGrabSpringLinearDampingMin,
                                                                 kGrabSpringLinearDampingMax);

                // Apply the calculated force.
                CheekyVR_PhysicsUtilities.AddForce(force, objectRigidbody);
                CheekyVR_PhysicsUtilities.AddTorque(torque, objectRigidbody);
            }

            // Update the holding status.
            holdingLeftPreviousFrame = holdingLeftHand;
            holdingRightPreviousFrame = holdingRightHand;
        }
    }

    private Vector3 getControllerAverageDirection()
    {
        // Left hand forward direction.
        Vector3 leftForwardDir = leftController.transform.forward;

        // Right hand forward direction.
        Vector3 rightForwardDir = rightController.transform.forward;

        // Mid point between both hands.
        Vector3 midPoint = (leftController.transform.position + rightController.transform.position) / 2f;

        // Average hand forward direction;
        Vector3 averageDirection = (leftForwardDir + rightForwardDir) / 2.0f;

        if(enableVisualDebug)
        {
            Debug.DrawRay(midPoint, averageDirection * 1f, Color.blue);
        }

        return averageDirection;
    }

    private void UpdatePhysicsLeftHand()
    {
        physicsTarget.transform.position = leftController.transform.position;
        physicsTarget.transform.rotation = leftController.transform.rotation;
    }

    private void UpdatePhysicsRightHand()
    {
        physicsTarget.transform.position = rightController.transform.position;
        physicsTarget.transform.rotation = rightController.transform.rotation;
    }

    private void CalculateTransformTwoHands()
    {
        // Direction from the left controller to the right controller.
        directionBetweenControllersNormalized = (rightController.transform.position - leftController.transform.position).normalized;

        // Average direction of controllers.
        Vector3 controllerAverage = getControllerAverageDirection();

        // Object up direction.
        Vector3 firstCross = Vector3.Cross(controllerAverage.normalized, directionBetweenControllersNormalized);

        // Object forward direction.
        Vector3 secondCross = Vector3.Cross(firstCross, -directionBetweenControllersNormalized);

        // Save the rotation.
        physicsTarget.transform.rotation = Quaternion.LookRotation(secondCross, firstCross);

        // Calculate the mid point between the controllers.
        Vector3 midPoint = (leftController.transform.position + rightController.transform.position) / 2f;

        // Save the position.
        physicsTarget.transform.position = midPoint + averageHandOffset;


        if(enableVisualDebug)
        {
            Debug.DrawRay(midPoint, firstCross.normalized, Color.red);
            Debug.DrawRay(midPoint, secondCross.normalized, Color.yellow);
        }
    }

    public void GrabObject(int controllerID)
    {
        // If 
        if(oneHandedSnapToHand)
        {
            CheekyVR_InputManager.GetControllerModel(controllerID).SetActive(false);
        }

        objectRigidbody.useGravity = false;

        #region Left Hand
        // Left controller.
        if(controllerID == 0)
        {
            // If not already holding with the left hand.
            if(!holdingLeftHand)
            {
                // Flag the left hand as holding. Calculate the offset between the object and the hand position.
                holdingLeftHand = true;
                leftHandOffset = transform.position - leftController.transform.position;


                // Switch based on the interaction type of the object.
                switch (objectInteractionType)
                {
                    case InteractionType.OneHanded:
                        {
                            // Object is already being interacted with by the other hand, undo the flag change.
                            if(holdingRightHand)
                            {
                                holdingLeftHand = false;
                            }

                            // If snapping is disabled, update the physics target to be the controller.
                            else if (!oneHandedSnapToHand)
                            {
                                physicsTarget.transform.position = transform.position;
                                physicsTarget.transform.rotation = transform.rotation;
                                physicsTarget.transform.parent = leftController.transform;
                            }

                            // Flag to show the object is being interacted with.
                            holdingActive = true;
                            break;
                        }
                    case InteractionType.Mixed:
                        {
                            // Object is already being interacted with by the other hand. Change from 1h to 2h. Set appropriate variables required for 2h.
                            if (holdingRightHand)
                            {
                                rightHandOffset = transform.position - rightController.transform.position;
                                averageHandOffset = (leftHandOffset + rightHandOffset) / 2f;
                            }

                            // Flag to show the object is being interacted with.
                            holdingActive = true;
                            break;
                        }
                    case InteractionType.TwoHanded:
                        {
                            // Object is already being interacted with by the other hand. Change from 1h to 2h. Set appropriate variables required for 2h.
                            if (holdingRightHand)
                            {
                                averageHandOffset = (leftHandOffset + rightHandOffset) / 2f;
                                holdingActive = true;
                            }

                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
        }
        #endregion

        #region Right Hand
        // Right controller.
        if (controllerID == 1)
        {
            if (!holdingRightHand)
            {
                holdingRightHand = true;
                rightHandOffset = transform.position - rightController.transform.position;

                switch (objectInteractionType)
                {
                    case InteractionType.OneHanded:
                        {
                            if (holdingLeftHand)
                            {
                                holdingRightHand = false;
                            }

                            else if(!oneHandedSnapToHand)
                            {
                                physicsTarget.transform.position = transform.position;
                                physicsTarget.transform.rotation = transform.rotation;
                                physicsTarget.transform.parent = rightController.transform;
                            }
                            
                            holdingActive = true;
                            break;
                        }
                    case InteractionType.Mixed:
                        {
                            if(holdingLeftHand)
                            {
                                leftHandOffset = transform.position - leftController.transform.position;
                                averageHandOffset = (leftHandOffset + rightHandOffset) / 2f;
                            }

                            holdingActive = true;
                            break;
                        }
                    case InteractionType.TwoHanded:
                        {
                            if (holdingLeftHand)
                            {
                                averageHandOffset = (leftHandOffset + rightHandOffset) / 2f;
                                holdingActive = true;
                            }

                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
        }
        #endregion

        if(!useInteractionPhysics)
        {
            TogglePhysics(false);
        }
    }

    public void DropObject(int controllerID)
    {
        if (oneHandedSnapToHand)
        {
            CheekyVR_InputManager.GetControllerModel(controllerID).SetActive(true);
        }

        objectRigidbody.useGravity = true;

        #region Left Hand
        // Left controller.
        if (controllerID == 0)
        {
            // Check if the object is currently being held by the left hand.
            if (holdingLeftHand)
            {
                // Change the holding flag.
                holdingLeftHand = false;

                // Switch based on the interaction type of the object.
                switch (objectInteractionType)
                {
                    case InteractionType.OneHanded:
                        {
                            // If snapping is disabled, the physics target can be restored to its original parent.
                            if(!oneHandedSnapToHand)
                            {
                                physicsTarget.transform.parent = transform;
                            }

                            // If one handed there will only ever be one hand interacting. The object is now not being interacted with.
                            holdingActive = false;
                            break;
                        }
                    case InteractionType.Mixed:
                        {
                            // If not being held by the other hand, the object is now not being interacted with.
                            if(!holdingRightHand)
                            {
                                holdingActive = false;
                            }
                            // If held by the other hand, the object is now 1h interaction with that hand.
                            else
                            {
                                rightHandOffset = transform.position - rightController.transform.position;
                            }

                            break;
                        }
                    case InteractionType.TwoHanded:
                        {
                            // Cannot continue interaction with 1h. Force the other hand to drop also.
                            holdingRightHand = false;
                            holdingActive = false;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
        }
        #endregion

        #region Right Hand
        // Right controller.
        if (controllerID == 1)
        {
            if (holdingRightHand)
            {
                holdingRightHand = false;

                switch (objectInteractionType)
                {
                    case InteractionType.OneHanded:
                        {
                            if(!oneHandedSnapToHand)
                            {
                                physicsTarget.transform.parent = transform;
                            }

                            holdingActive = false;
                            break;
                        }
                    case InteractionType.Mixed:
                        {
                            if (!holdingLeftHand)
                            {
                                holdingActive = false;
                            }
                            else
                            {
                                leftHandOffset = transform.position - leftController.transform.position;
                            }

                            break;
                        }
                    case InteractionType.TwoHanded:
                        {
                            holdingLeftHand = false;
                            holdingActive = false;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
        }
        #endregion

        if (!useInteractionPhysics)
        {
            TogglePhysics(true);
        }

        ApplyReleaseVelocity();
    }

    public bool GetHeldStatus(int controllerID)
    {
        if(controllerID == 0)
        {
            return holdingLeftHand;
        }
        else if (controllerID == 1)
        {
            return holdingRightHand;
        }
        else
        {
            return false;
        }
    }

    private void OnCollisionStay(Collision col)
    {
        if(holdingActive)
        {
            if(hapticFeedbackEnabled)
            {
                // Basic haptic feedback on collision.
                if (col.gameObject.tag != "Controllers")
                {
                    // If springs are being used.
                    if (useInteractionPhysics)
                    {
                        // Activate the spring if not already active.
                        if (!springsEnabled)
                        {
                            springsEnabled = true;
                        }

                        // Two hands.
                        if (holdingLeftHand && holdingRightHand)
                        {
                            CheekyVR_InputManager.TriggerHapticPulse(0, hapticStrength);
                            CheekyVR_InputManager.TriggerHapticPulse(1, hapticStrength);
                        }
                        // Left hand.
                        else if (holdingLeftHand)
                        {
                            CheekyVR_InputManager.TriggerHapticPulse(0, hapticStrength);
                        }
                        // Right hand.
                        else if (holdingRightHand)
                        {
                            CheekyVR_InputManager.TriggerHapticPulse(1, hapticStrength);
                        }

                        // Activate the spring cooldown period.
                        springCooldownRemaining = springCooldown;
                    }
                }
            }
        }
    }

    private void OnTriggerStay(Collider col)
    {
        //CheekyVR_ControllerHighlight.CheckCollider(true, col);
    }

    private void OnTriggerExit(Collider col)
    {
        //CheekyVR_ControllerHighlight.CheckCollider(false, col);
    }

    private void ApplyReleaseVelocity()
    {
        if(!holdingActive)
        {
            objectRigidbody.useGravity = true;

            Vector3 leftControllerVelocity = inputList[0].velocity;
            Vector3 rightControllerVelocity = inputList[1].velocity;

            leftControllerVelocity = CheekyVR_CameraRigRotationCorrection.CorrectRotation(leftControllerVelocity);
            rightControllerVelocity = CheekyVR_CameraRigRotationCorrection.CorrectRotation(rightControllerVelocity);

            Vector3 leftControllerAngularVelocity = inputList[0].angularVelocity;
            Vector3 rightControllerAngularVelocity = inputList[1].angularVelocity;

            leftControllerAngularVelocity = CheekyVR_CameraRigRotationCorrection.CorrectRotation(leftControllerAngularVelocity);
            rightControllerAngularVelocity = CheekyVR_CameraRigRotationCorrection.CorrectRotation(rightControllerAngularVelocity);

            if (holdingLeftPreviousFrame && holdingRightPreviousFrame)
            {
                Vector3 averageControllerVelocity = (leftControllerVelocity + rightControllerVelocity) / 2f;
                Vector3 averageControllerAngularVelocity = (leftControllerAngularVelocity + rightControllerAngularVelocity) / 2f;

                objectRigidbody.velocity = averageControllerVelocity;
                objectRigidbody.angularVelocity = averageControllerAngularVelocity;

                holdingLeftPreviousFrame = false;
                holdingRightPreviousFrame = false;
            }
            else if (holdingLeftPreviousFrame)
            {
                objectRigidbody.velocity = leftControllerVelocity;
                objectRigidbody.angularVelocity = leftControllerAngularVelocity;
                holdingLeftPreviousFrame = false;
            }
            else if (holdingRightPreviousFrame)
            {
                objectRigidbody.velocity = rightControllerVelocity;
                objectRigidbody.angularVelocity = rightControllerAngularVelocity;
                holdingRightPreviousFrame = false;
            }
        }
    }

    private void TogglePhysics(bool enabled)
    {
        for(int i = 0; i < objectCollider.Length; i++)
        {
            objectCollider[i].isTrigger = !enabled;
        }

        objectRigidbody.isKinematic = !enabled;
    }

    private void Initialise()
    {
        leftController = CheekyVR_InputManager.GetController(0);
        rightController = CheekyVR_InputManager.GetController(1);
        cameraRig = CheekyVR_InputManager.GetCameraRig();

        physicsTarget = new GameObject(transform.name + " Physics Target");
        physicsTarget.transform.parent = transform;

        objectCollider = GetComponentsInChildren<Collider>();
        objectRigidbody = GetComponent<Rigidbody>();
    }

    public void DoAction()
    {
        switch(currentObjectType)
        {
            

            default:
                {
                    break;
                }
        }
    }

    public void EndAction()
    {
        switch (currentObjectType)
        {
            default:
                {
                    break;
                }
        }
    }

    public InteractiveObjectType GetObjectType()
    {
        return currentObjectType;
    }
}
