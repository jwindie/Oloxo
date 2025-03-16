//Author: Emery Williams 

using UnityEngine;

namespace Oloxo.Cameras {

    public class SimpleCamera : MonoBehaviour {

        [Header ("Camera Joints")]
        [SerializeField] private Transform yawJoint;
        [SerializeField] private Transform tiltJoint;

        [SerializeField] private float speed;
        [SerializeField] private Vector2 zoomValues;
        [SerializeField] private Vector2 fovValues;
        [SerializeField] private Vector2 tiltvalues;
        [SerializeField] private Vector2 zoomSpeeds;
        [SerializeField] Camera renderCam;

        private float zoomTarget = 0;
        float zoomSpeed;
        public float ZoomInterpolation { get; private set; }

        public Camera RenderCam {
            get {
                return renderCam;
            }
        }

        private void Update () {

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
        }
    }
}