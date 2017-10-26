using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CheekyVR;

// This script is used to check colliders and either enable or disable the controller highlight.

namespace CheekyVR
{
    public class CheekyVR_ControllerHighlight : MonoBehaviour
    {
        private static GameObject leftController;
        private static GameObject rightController;

        public Renderer bodyRenderer_left;
        public Renderer bodyRenderer_right;
        private static Renderer s_bodyRenderer_left;
        private static Renderer s_bodyRenderer_right;
        public Material mat_Original;
        public Material mat_Everything_Highlighted;
        private static Material s_mat_Original;
        private static Material s_mat_Everything_Highlighted;

        private void Awake()
        {
            leftController = CheekyVR_InputManager.GetController(0);
            rightController = CheekyVR_InputManager.GetController(1);

            s_bodyRenderer_left = bodyRenderer_left;
            s_bodyRenderer_right = bodyRenderer_right;

            s_mat_Everything_Highlighted = mat_Everything_Highlighted;
            s_mat_Original = mat_Original;
        }

        public static void CheckCollider(bool enabled, Collider col)
        {
            if(col.gameObject.transform.parent != null)
            {
                if (col.gameObject.transform.parent.name == leftController.name)
                {
                    BodyHighlight(0, enabled);
                }
                if (col.gameObject.transform.parent.name == rightController.name)
                {
                    BodyHighlight(1, enabled);
                }
            }
        }

        public static void CheckCollision(bool enabled, Collision col)
        {
            if (col.gameObject.transform.parent.name == leftController.name)
            {
                BodyHighlight(0, enabled);
            }
            if (col.gameObject.transform.parent.name == rightController.name)
            {
                BodyHighlight(1, enabled);
            }
        }

        private static void BodyHighlight(int controllerIndex, bool enabled)
        {
            if(controllerIndex == 0)
            {
                if (enabled)
                {
                    s_bodyRenderer_left.material = s_mat_Everything_Highlighted;
                }
                else
                {
                    s_bodyRenderer_left.material = s_mat_Original;
                }
            }
            else if (controllerIndex == 1)
            {
                if (enabled)
                {
                    s_bodyRenderer_right.material = s_mat_Everything_Highlighted;
                }
                else
                {
                    s_bodyRenderer_right.material = s_mat_Original;
                }
            }
        }
    }
}

