//Author: Emery Williams 

using System;
using UnityEngine;

namespace Oloxo.Cameras {

    public class SimpleCamera : MonoBehaviour {

        [Header ("References")]
        [SerializeField] Camera renderCam;

        [Header ("Camera Joints")]
        [SerializeField] private Transform yawJoint;
        [SerializeField] private Transform tiltJoint;

        [Header ("Settings")]
        [SerializeField] private float speed;
        [SerializeField] private Vector2 zoomValues;
        [SerializeField] private Vector2 fovValues;
        [SerializeField] private Vector2 tiltvalues;
        [SerializeField] private Vector2 zoomSpeeds;

        private float zoomTarget = 0;
        float zoomSpeed;
        public float ZoomInterpolation { get; private set; }

        private bool useCameraBounds = false;
        private Vector3 cameraBoundsMin = Vector3.zero;
        private Vector3 cameraBoundsMax = Vector3.zero;

        public Camera RenderCam {
            get {
                return renderCam;
            }
        }

        public SimpleCamera SetCameraBounds (Vector3 boundsMin, Vector3 boundsMax) {
            cameraBoundsMin = boundsMin;
            cameraBoundsMax = boundsMax;
            useCameraBounds = true;
            return this;
        }
        public SimpleCamera SetCameraBounds (Vector3 boundsMin, Vector3 boundsMax, bool state) {
            cameraBoundsMin = boundsMin;
            cameraBoundsMax = boundsMax;
            useCameraBounds = state;
            return this;
        }

        public SimpleCamera UseCameraBounds (bool state) { 
            useCameraBounds = state;
            return this;
        }

        //Centers camera in the bounds only if the bounds are enabled.
        public SimpleCamera CenterInBounds () {
            if (useCameraBounds) {
                var position = (cameraBoundsMin + cameraBoundsMax) / 2f;
                position.y = 0;

                transform.position = position;
            }

            return this;
        }

        private void LateUpdate () {

            //move camera based on user inputs
            var deltaSpeed = speed * Time.unscaledDeltaTime;

            //rotate camera
            var zRoll = Input.GetAxis ("Rotation") * deltaSpeed * 4;
            var xRoll = Input.GetAxis ("Tilt") * deltaSpeed * 4;

            //zoom camera
            if (!Input.GetKey (KeyCode.LeftShift)) zoomTarget = Mathf.Clamp (zoomTarget += Input.mouseScrollDelta.y * .5f * zoomSpeed, -zoomValues[1], -zoomValues[0]);

            yawJoint.Rotate (0, zRoll, 0, Space.Self);

            //limit x rolling
            var tiltJointX = tiltJoint.localEulerAngles.x;
            var projectedTilt = tiltJointX + xRoll;
            if (projectedTilt > tiltvalues[1]) xRoll -= (projectedTilt - tiltvalues[1]);
            if (projectedTilt < tiltvalues[0]) xRoll -= (projectedTilt - tiltvalues[0]);
            if (xRoll != 0) tiltJoint.Rotate (xRoll, 0, 0, Space.Self);

            //zooming 
            if (renderCam.transform.localPosition.z != zoomTarget) {
                if (Mathf.Abs (renderCam.transform.localPosition.z - zoomTarget) <= .001f) {
                    renderCam.transform.localPosition = new Vector3 (0, 0, zoomTarget);
                }
                else {
                    renderCam.transform.localPosition = new Vector3 (
                        0,
                        0,
                        Mathf.Lerp (renderCam.transform.localPosition.z, zoomTarget, 10 * Time.deltaTime)
                        );
                }

                //lerp between zoom values based on camera z and zoom values
                ZoomInterpolation = Mathf.InverseLerp (-zoomValues[1], -zoomValues[0], renderCam.transform.localPosition.z);


                renderCam.fieldOfView = Mathf.Lerp (fovValues[1], fovValues[0], ZoomInterpolation);
                zoomSpeed = Mathf.Lerp (zoomSpeeds[1], zoomSpeeds[0], ZoomInterpolation);
            }

            //translating
            Vector3 forward = Input.GetAxis ("Vertical") * yawJoint.forward;
            Vector3 right = Input.GetAxis ("Horizontal") * yawJoint.right;

            Vector3 position = transform.position + (forward + right * speed * Time.deltaTime);

            if (useCameraBounds) {
                position.x = Mathf.Clamp (position.x, cameraBoundsMin.x, cameraBoundsMax.x);
                position.z = Mathf.Clamp (position.z, cameraBoundsMin.z, cameraBoundsMax.z);
            }

            transform.position = position;
        }

        private void OnDrawGizmosSelected () {
            Gizmos.color = Color.gray;
            if (useCameraBounds) Gizmos.color = Color.green;

            Gizmos.DrawWireCube ((cameraBoundsMin + cameraBoundsMax) / 2f, cameraBoundsMax - cameraBoundsMax);
        }
    }
}