using UnityEngine;
using System.Collections;

// Physics Utilities.
namespace CheekyVR
{
    public class CheekyVR_PhysicsUtilities
    {
        public static float tkTweakForceLinear = 1.0f;
        public static float tkTweakForceAngular = 1.0f;

        //Add time based force to object
        public static void AddForce(Vector3 forceAmount, Rigidbody inRigidBody)
        {
            inRigidBody.velocity += forceAmount * Time.fixedDeltaTime * (1.0f / inRigidBody.mass);
        }

        //Add time based force to object
        public static void AddForceAtPosition(Vector3 forceAmount, Vector3 relativePos, Rigidbody inRigidBody)
        {
            inRigidBody.velocity += forceAmount * Time.fixedDeltaTime * (1.0f / inRigidBody.mass);

            var torqueForce = Vector3.Cross(relativePos, forceAmount);
            AddTorque(torqueForce, inRigidBody);
        }

        //Add explosive style force to object
        public static void AddImpulseForce(Vector3 forceAmount, Rigidbody inRigidBody, Vector3 centerObjectPos)
        {
            var forceCalc = forceAmount * (1.0f / inRigidBody.mass);
            inRigidBody.velocity += forceCalc;
        }

        //Add torque to spin the object
        public static void AddTorque(Vector3 torque, Rigidbody inRigidBody)
        {
            var torqueToRotation = Quaternion.Inverse(inRigidBody.rotation) * torque;
            var finalAngularVelocity = inRigidBody.rotation * DivideVectors(torqueToRotation, inRigidBody.inertiaTensor);
            inRigidBody.angularVelocity += finalAngularVelocity * Time.fixedDeltaTime;
        }

        public static Vector3 DivideVectors(Vector3 v, Vector3 v2)
        {
            float xDiv = 0;
            float yDiv = 0;
            float zDiv = 0;

            if (v2.x > 0)
            {
                xDiv = v.x / v2.x;
            }

            if (v2.y > 0)
            {
                yDiv = v.y / v2.y;
            }

            if (v2.z > 0)
            {
                zDiv = v.z / v2.z;
            }

            return new Vector3(xDiv, yDiv, zDiv);
        }

        public static Vector3 CalculateForce(Rigidbody tkObject, Vector3 targetPositionWS, Vector3 forcePositionWS,
                                            float springTightness, float springDampingMin, float springDampingMax)
        {
            var mass = Mathf.Lerp(
                tkObject.mass,
                1.0f,
                1.0f);

            var damping = Mathf.Lerp(
                springDampingMin,
                springDampingMax,
                1.0f);

            // Hooke's Law
            // F = - kx - bv
            float k = springTightness;
            float b = damping;
            Vector3 x = forcePositionWS - targetPositionWS;
            Vector3 v = tkObject.GetPointVelocity(forcePositionWS);
            return (-k * x - b * v) * mass * tkTweakForceLinear;
        }

        // NOTE(Jimmy): This is func version is based on oculus feedback of latency when 
        // controlling the z-axis movement via the touchpad, we manually increase the z force 
        // to provide the player with more responsive feedback while adjusting depth.
        //private static float HACKPreviousTightness = PlayerController.kGrabSpringLinearTightness;
        public static Vector3 CalculateForceForPlayer(
            Rigidbody tkObject,
            Vector3 targetPositionWS, Vector3 forcePositionWS,
            float springTightness, float springDampingMin, float springDampingMax, Vector3 playerLook)
        {
            // HACK HACK HACK: Document this!
            //targetPositionWS += Vector3.up * PlayerController.Instance.CalculateTKObjectHangOffset(
            //	tkObject, HACKPreviousTightness);

            var mass = Mathf.Lerp(
                tkObject.mass,
                1.0f,
                1.0f);

            var damping = Mathf.Lerp(
                springDampingMin,
                springDampingMax,
                1.0f);

            // Hooke's Law
            // F = - kx - bv
            float k = springTightness;
            float b = damping;
            Vector3 delta = forcePositionWS - targetPositionWS;
            Vector3 vel = tkObject.GetPointVelocity(forcePositionWS);

            // NOTE(Jimmy): This is based on oculus feedback of latency when controlling
            // the z-axis movement via the touchpad, we manually increase the z force 
            // to provide the player with more responsive feedback while adjusting depth.
            // We split the springs into XY (which uses the old spring forces) and Z
            // which has a hardcoded high spring value.

            // Dots
            float deltaDotL = Vector3.Dot(delta, playerLook);
            float velDotL = Vector3.Dot(vel, playerLook);

            // Z Spring
            const float kZdepthLinearTightness = 550.0f;
            const float kZdepthLinearDamping = 30.0f;
            Vector3 deltaZ = playerLook * deltaDotL;
            Vector3 velZ = playerLook * velDotL;
            Vector3 forceZ = (-kZdepthLinearTightness * deltaZ - kZdepthLinearDamping * velZ) * mass * tkTweakForceLinear;
            Debug.DrawRay(tkObject.transform.position, forceZ, Color.blue);

            // XY Spring
            Vector3 deltaXY = delta - deltaZ;
            Vector3 velXY = vel - velZ;
            Vector3 forceXY = (-k * deltaXY - b * velXY) * mass * tkTweakForceLinear;
            Debug.DrawRay(tkObject.transform.position, forceZ, Color.red);

            Vector3 force = forceXY + forceZ;

            return force;
        }

        public static Vector3 CalculateForceRigidbody(Rigidbody rigidBody, Vector3 targetPosition, Vector3 worldSpaceForcePoint,
                                        float springTightness, float springDampingMin, float springDampingMax)
        {
            var mass = Mathf.Lerp(
                rigidBody.mass,
                1.0f,
                1);

            var damping = Mathf.Lerp(
                springDampingMin,
                springDampingMax,
                1);

            // Hooke's Law
            // F = - kx - bv
            float k = springTightness;
            float b = damping;
            Vector3 x = (worldSpaceForcePoint - targetPosition);
            Vector3 v = rigidBody.GetPointVelocity(worldSpaceForcePoint);
            return (-k * x - b * v) * mass;
        }

        //Calculate spring torque
        public static Vector3 CalculateTorque(
            Rigidbody tkObject, Quaternion targetRotation,
            float springTightness, float springDamping)
        {
            // Hooke's Law
            // F = - kx - bv
            float k = springTightness;
            float b = springDamping;

            Vector3 v = tkObject.angularVelocity;

            var currentForward = -tkObject.transform.forward;
            var currentUp = -tkObject.transform.up;
            var targetForward = targetRotation * Vector3.forward;
            var targetUp = targetRotation * Vector3.up;

            Vector3 x = Vector3.Cross(currentForward, targetForward);
            Vector3 y = Vector3.Cross(currentUp, targetUp);

            // TODO: We sqrt 2 times here, this could be faster...
            float rotationDifference = Mathf.Asin(Mathf.Clamp01(x.magnitude));
            float omega = Mathf.Asin(Mathf.Clamp01(y.magnitude));
            Vector3 w = x.normalized * rotationDifference + y.normalized * omega;

            Quaternion lr = tkObject.transform.rotation * tkObject.inertiaTensorRotation;
            var torque = lr * (Vector3.Scale(Quaternion.Inverse(lr) * (-k * w - b * v), tkObject.inertiaTensor) * tkTweakForceAngular);

            return torque;
        }

        public static Vector2 LineIntersectionPoint(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2)
        {
            // Get A,B,C of first line - points : ps1 to pe1
            float A1 = pe1.y - ps1.y;
            float B1 = ps1.x - pe1.x;
            float C1 = A1 * ps1.x + B1 * ps1.y;

            // Get A,B,C of second line - points : ps2 to pe2
            float A2 = pe2.y - ps2.y;
            float B2 = ps2.x - pe2.x;
            float C2 = A2 * ps2.x + B2 * ps2.y;

            // Get delta and check if the lines are parallel
            float delta = A1 * B2 - A2 * B1;

            if (delta == 0)
                return Vector2.zero;

            // now return the Vector2 intersection point
            return new Vector2((B2 * C1 - B1 * C2) / delta, (A1 * C2 - A2 * C1) / delta);
        }
    }
}
