using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using CheekyVR;

// This script gets the complete button state of both controllers from SteamVR and stores it within the custom "Controller Input" data structure.

namespace CheekyVR
{
    public class ControllerInput
    {
        public bool applicationButtonDown;
        public bool applicationButtonPressed;
        public bool applicationButtonUp;

        public bool gripButtonDown;
        public bool gripButtonPressed;
        public bool gripButtonUp;

        public float triggerAxis;
        public bool triggerDown;
        public bool triggerPressed;
        public bool triggerUp;
        public bool triggerClickDown;
        public bool triggerClickUp;

        public float touchpadAxisX;
        public float touchpadAxisY;
        public bool touchpadTouchDown;
        public bool touchpadTouched;
        public bool touchpadTouchUp;
        public bool touchpadDown;
        public bool touchpadPressed;
        public bool touchpadUp;

        public Vector3 velocity;
        public Vector3 angularVelocity;

        public ControllerInput()
        {
            applicationButtonDown = false;
            applicationButtonPressed = false;
            applicationButtonUp = false;

            gripButtonDown = false;
            gripButtonPressed = false;
            gripButtonUp = false;

            triggerAxis = 0f;
            triggerDown = false;
            triggerPressed = false;
            triggerUp = false;
            triggerClickDown = false;
            triggerClickUp = false;

            touchpadAxisX = 0f;
            touchpadAxisY = 0f;
            touchpadTouched = false;
            touchpadDown = false;
            touchpadPressed = false;
            touchpadUp = false;

            velocity = Vector3.zero;
            angularVelocity = Vector3.zero;
        }
    }

    public enum HMD_TYPE
    {
        Oculus,
        Vive
    }

    public class CheekyVR_InputManager : MonoBehaviour
    {
        public HMD_TYPE currentHMDType = HMD_TYPE.Oculus;

        // Log script maintenance messages to the console.
        public bool enableScriptDebugLogs = false;
        // Log button readings to the console.
        public bool enableInputDebugLogs = false;

        public GameObject cameraRig;
        public GameObject cameraRigCollider;
        public GameObject HMD;
        public GameObject leftController;
        public GameObject rightController;
        public GameObject leftControllerModel;
        public GameObject rightControllerModel;
        private SteamVR_Controller.Device[] device = new SteamVR_Controller.Device[2];
        private SteamVR_TrackedObject[] trackedController = new SteamVR_TrackedObject[2];

        // Static copies of public variables. Can be assigned in Awake().
        private static SteamVR_Controller.Device[] s_device = new SteamVR_Controller.Device[2];
        private static GameObject s_cameraRig;
        private static GameObject s_cameraRigCollider;
        private static GameObject s_HMD;
        private static GameObject s_leftController;
        private static GameObject s_rightController;
        private static GameObject s_leftControllerModel;
        private static GameObject s_rightControllerModel;

        private uint[] controllerIndex = new uint[2];

        private static ControllerInput[] inputList = new ControllerInput[2];

        private bool leftControllerActive = false;
        private bool rightControllerActive = false;

        // How far the trigger is pressed before considered to have been "clicked". Brand new controllers have a value ~0.95f.
        public float triggerClickTolerance = 0.95f;

        void Awake()
        {
            s_device = device;

            s_cameraRig = cameraRig;
            s_cameraRigCollider = cameraRigCollider;
            s_HMD = HMD;
            s_leftController = leftController;
            s_rightController = rightController;
            s_leftControllerModel = leftControllerModel;
            s_rightControllerModel = rightControllerModel;

            if (cameraRig == null)
            {
                Debug.Log("Camera Rig has not been setup in the Input Manager.");
            }

            if(HMD == null)
            {
                Debug.Log("HMD has not been setup in the Input Manager.");
            }

            // Check if controllers have been setup in the editor. If not, assume standard SteamVR CameraRig is being used.
            if (leftController == null)
            {
                Debug.Log("Left Controller has not been setup in the Input Manager.");
            }
            else
            {
                trackedController[0] = leftController.GetComponent<SteamVR_TrackedObject>();
            }

            if (rightController == null)
            {
                Debug.Log("Right Controller has not been setup in the Input Manager.");
            }
            else
            {
                trackedController[1] = rightController.GetComponent<SteamVR_TrackedObject>();
            }

            for (int i = 0; i < 2; i++)
            {
                inputList[i] = new ControllerInput();
            }
        }

        void Update()
        {
            // Get device for controller if active.
            UpdateActiveDevices();

            // Get current controller input states.
            UpdateControllerInputs();
        }

        private void UpdateActiveDevices()
        {
            for (int i = 0; i < 2; i++)
            {
                device[i] = null;

                if (trackedController[i] != null)
                {
                    controllerIndex[i] = (uint)trackedController[i].index;

                    if (enableScriptDebugLogs)
                    {
                        //Debug.Log("Controller " + i + " index: " + (int)trackedController[i].index);
                    }

                    if (controllerIndex[i] < uint.MaxValue)
                    {
                        device[i] = SteamVR_Controller.Input((int)controllerIndex[i]);

                        if (enableScriptDebugLogs)
                        {
                            //Debug.Log("Device " + i + " controller index: " + (int)controllerIndex[i]);
                        }
                    }
                }
            }

            // If controller is inactive, flag it.
            if (!leftController.activeInHierarchy)
            {
                if (leftControllerActive)
                {
                    leftControllerActive = false;

                    if (enableInputDebugLogs)
                    {
                        Debug.Log("Left controller has became inactive.");
                    }
                }
            }
            else
            {
                if (!leftControllerActive)
                {
                    leftControllerActive = true;

                    if (enableInputDebugLogs)
                    {
                        Debug.Log("Left controller has became active.");
                    }
                }
            }

            if (!rightController.activeInHierarchy)
            {
                if (rightControllerActive)
                {
                    rightControllerActive = false;

                    if (enableInputDebugLogs)
                    {
                        Debug.Log("Right controller has became inactive.");
                    }
                }
            }
            else
            {
                if (!rightControllerActive)
                {
                    rightControllerActive = true;

                    if (enableInputDebugLogs)
                    {
                        Debug.Log("Right controller has became active.");
                    }
                }
            }
        }

        private void UpdateControllerInputs()
        {
            for (int i = 0; i < 2; i++)
            {
                SteamVR_Controller.Device controllerDevice = device[i];

                // Reset state of buttons.
                inputList[i].applicationButtonDown = false;
                inputList[i].applicationButtonPressed = false;
                inputList[i].applicationButtonUp = false;

                inputList[i].gripButtonDown = false;
                inputList[i].gripButtonPressed = false;
                inputList[i].gripButtonUp = false;

                inputList[i].triggerDown = false;
                inputList[i].triggerPressed = false;
                inputList[i].triggerUp = false;
                inputList[i].triggerClickDown = false;
                inputList[i].triggerClickUp = false;

                inputList[i].touchpadTouchDown = false;
                inputList[i].touchpadTouched = false;
                inputList[i].touchpadTouchUp = false;
                inputList[i].touchpadDown = false;
                inputList[i].touchpadPressed = false;
                inputList[i].touchpadUp = false;

                if (controllerDevice != null)
                {
                    if (controllerDevice.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
                    {
                        inputList[i].applicationButtonDown = true;

                        if (enableInputDebugLogs)
                        {
                            Debug.Log("Controller " + i + " - Application Button Down: " + inputList[i].applicationButtonDown);
                        }
                    }
                    if (controllerDevice.GetPress(SteamVR_Controller.ButtonMask.ApplicationMenu))
                    {
                        inputList[i].applicationButtonPressed = true;

                        if (enableInputDebugLogs)
                        {
                            Debug.Log("Controller " + i + " - Application Button Pressed: " + inputList[i].applicationButtonPressed);
                        }
                    }
                    if (controllerDevice.GetPressUp(SteamVR_Controller.ButtonMask.ApplicationMenu))
                    {
                        inputList[i].applicationButtonUp = true;

                        if (enableInputDebugLogs)
                        {
                            Debug.Log("Controller " + i + " - Application Button Up: " + inputList[i].applicationButtonUp);
                        }
                    }

                    if (controllerDevice.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
                    {
                        inputList[i].gripButtonDown = true;

                        if (enableInputDebugLogs)
                        {
                            Debug.Log("Controller " + i + " - Grip Button Down: " + inputList[i].gripButtonDown);
                        }
                    }
                    if (controllerDevice.GetPress(SteamVR_Controller.ButtonMask.Grip))
                    {
                        inputList[i].gripButtonPressed = true;

                        if (enableInputDebugLogs)
                        {
                            Debug.Log("Controller " + i + " - Grip Button Pressed: " + inputList[i].gripButtonPressed);
                        }
                    }
                    if (controllerDevice.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
                    {
                        inputList[i].gripButtonUp = true;

                        if (enableInputDebugLogs)
                        {
                            Debug.Log("Controller " + i + " - Grip Button Up: " + inputList[i].gripButtonUp);
                        }
                    }

                    float previousTriggerAxis = inputList[i].triggerAxis;

                    inputList[i].triggerAxis = controllerDevice.GetAxis(EVRButtonId.k_EButton_SteamVR_Trigger).x;

                    if (enableInputDebugLogs)
                    {
                        Debug.Log("Controller " + i + " - Trigger Axis Value: " + inputList[i].triggerAxis);
                    }

                    if (previousTriggerAxis == 0f)
                    {
                        if (inputList[i].triggerAxis > 0f)
                        {
                            inputList[i].triggerDown = true;

                            if (enableInputDebugLogs)
                            {
                                Debug.Log("Controller " + i + " - Trigger Down: " + inputList[i].triggerDown);
                            }
                        }
                    }
                    if (previousTriggerAxis > 0f)
                    {
                        if (inputList[i].triggerAxis == 0f)
                        {
                            inputList[i].triggerUp = true;

                            if (enableInputDebugLogs)
                            {
                                Debug.Log("Controller " + i + " - Trigger Up: " + inputList[i].triggerUp);
                            }
                        }
                        else
                        {
                            inputList[i].triggerPressed = true;

                            if (enableInputDebugLogs)
                            {
                                Debug.Log("Controller " + i + " - Trigger Pressed: " + inputList[i].triggerPressed);
                            }
                        }
                    }
                    if (previousTriggerAxis < triggerClickTolerance)
                    {
                        if (inputList[i].triggerAxis >= triggerClickTolerance)
                        {
                            inputList[i].triggerClickDown = true;

                            if (enableInputDebugLogs)
                            {
                                Debug.Log("Controller " + i + " - Trigger Click Down: " + inputList[i].triggerClickDown);
                            }
                        }
                    }
                    if (previousTriggerAxis >= triggerClickTolerance)
                    {
                        if (inputList[i].triggerAxis < triggerClickTolerance)
                        {
                            inputList[i].triggerClickUp = true;

                            if (enableInputDebugLogs)
                            {
                                Debug.Log("Controller " + i + " - Trigger Click Up: " + inputList[i].triggerClickUp);
                            }
                        }
                    }

                    inputList[i].touchpadAxisX = controllerDevice.GetAxis(EVRButtonId.k_EButton_Axis0).x;
                    inputList[i].touchpadAxisY = controllerDevice.GetAxis(EVRButtonId.k_EButton_Axis0).y;

                    if (enableInputDebugLogs)
                    {
                        Debug.Log("Controller " + i + " - Touchpad Axis X: " + inputList[i].touchpadAxisX);
                        Debug.Log("Controller " + i + " - Touchpad Axis Y: " + inputList[i].touchpadAxisY);
                    }

                    if (controllerDevice.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad))
                    {
                        inputList[i].touchpadTouchDown = true;

                        if (enableInputDebugLogs)
                        {
                            Debug.Log("Controller " + i + " - Touchpad Touch Down: " + inputList[i].touchpadTouchDown);
                        }
                    }
                    if (controllerDevice.GetTouch(SteamVR_Controller.ButtonMask.Touchpad))
                    {
                        inputList[i].touchpadTouched = true;

                        if (enableInputDebugLogs)
                        {
                            Debug.Log("Controller " + i + " - Touchpad Touched: " + inputList[i].touchpadTouched);
                        }
                    }
                    if (controllerDevice.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad))
                    {
                        inputList[i].touchpadTouchUp = true;

                        if (enableInputDebugLogs)
                        {
                            Debug.Log("Controller " + i + " - Touchpad Touch Up: " + inputList[i].touchpadTouchUp);
                        }
                    }
                    if (controllerDevice.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad) || controllerDevice.GetPressDown(EVRButtonId.k_EButton_A))
                    {
                        inputList[i].touchpadDown = true;

                        if (enableInputDebugLogs)
                        {
                            Debug.Log("Controller " + i + " - Touchpad Down: " + inputList[i].touchpadDown);
                        }
                    }
                    if (controllerDevice.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
                    {
                        inputList[i].touchpadPressed = true;

                        if (enableInputDebugLogs)
                        {
                            Debug.Log("Controller " + i + " - Touchpad Pressed: " + inputList[i].touchpadPressed);
                        }
                    }
                    if (controllerDevice.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
                    {
                        inputList[i].touchpadUp = true;

                        if (enableInputDebugLogs)
                        {
                            Debug.Log("Controller " + i + " - Touchpad Up: " + inputList[i].touchpadUp);
                        }
                    }

                    inputList[i].velocity = controllerDevice.velocity;
                    inputList[i].angularVelocity = controllerDevice.angularVelocity;
                }
            }
        }

        public static ControllerInput[] GetControllerInput()
        {
            return inputList;
        }

        public static GameObject GetCameraRig()
        {
            return s_cameraRig;
        }

        public static GameObject GetCameraRigColliderObject()
        {
            return s_cameraRigCollider;
        }

        public static GameObject GetHMD()
        {
            return s_HMD;
        }

        public static GameObject GetController(int controllerIndex)
        {
            if(controllerIndex == 0)
            {
                return s_leftController;
            }
            if(controllerIndex == 1)
            {
                return s_rightController;
            }
            else
            {
                return null;
            }
        }

        public static GameObject GetControllerModel(int controllerIndex)
        {
            if (controllerIndex == 0)
            {
                return s_leftControllerModel;
            }
            if (controllerIndex == 1)
            {
                return s_rightControllerModel;
            }
            else
            {
                return null;
            }
        }

        public static void TriggerHapticPulse(int controllerIndex, int duration)
        {
            s_device[controllerIndex].TriggerHapticPulse((ushort)duration, EVRButtonId.k_EButton_SteamVR_Touchpad);
        }
    }
}


