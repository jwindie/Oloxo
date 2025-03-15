//Author: Jordan Williams 

using JWIndie.RuntimeDebugger;

using UnityEngine;
using UnityEngine.EventSystems;
using Citybuilder.Testing;
using System.Collections;

namespace Citybuilder.Core {
    public class CameraController : Singleton<CameraController> {
        private static CameraController instance;

        [SerializeField] private float speed;
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] Transform yaw, neck, pitch;
        [SerializeField] Camera renderCam;
        [SerializeField] Camera overlayCamera;

        [SerializeField] private Vector2 zoomValues;
        [SerializeField] private Vector2 pitchValues;
        [SerializeField] public Vector2 moveSpeedValues;

        [SerializeField] public float audioRange;
        [SerializeField] private float reach = 30;

        Vector2 dimensions;
        float zoomTarget = 0;
        float pitchTarget = 0;
        RaycastHit hit;
        bool[] inverts = new bool[3]; //zoom, pitch, rotation
        bool noClip = false;
        float _fov;
        public bool BigMapMode { get; private set; } = false;

        private bool isCameraLocked;
        public Vector3 RenderCameraPosition {
            get {
                return renderCam.transform.position;
            }
        }
        public Vector3 RenderCameraRotation {
            get {
                return renderCam.transform.rotation.eulerAngles;
            }
        }

        public Camera RenderCamera {
            get {
                return renderCam;
            }
        }
        public Vector2 MoveSpeedValues {
            get {
                return moveSpeedValues;
            }
        }
        public void LockCamera (bool state = true) {
            isCameraLocked = state;
        }
        public bool IsTileWithinAudioRange (Tile tile) {
            Vector3 tileViewportSpacePosition = renderCam.WorldToViewportPoint (tile.WorldPosition);

            if (tileViewportSpacePosition.x > -.1f && tileViewportSpacePosition.x <= 1.1f) {

                if (tileViewportSpacePosition.y > -.1f && tileViewportSpacePosition.y <= 1.1f) {

                    return Vector3.Distance (tile.WorldPosition, renderCam.transform.position) < audioRange;
                }
            }
            return false;
        }
        public bool IsPositionWithinAudioRange (Vector3 position) {
            Vector3 tileViewportSpacePosition = renderCam.WorldToViewportPoint (position);

            if (tileViewportSpacePosition.x > -.1f && tileViewportSpacePosition.x <= 1.1f) {

                if (tileViewportSpacePosition.y > -.1f && tileViewportSpacePosition.y <= 1.1f) {

                    return Vector3.Distance (position, renderCam.transform.position) < audioRange;
                }
            }
            return false;
        }
        public void SetCameraParams (float xBounds, float zBounds, float y) {
            dimensions = new Vector2 (xBounds, zBounds);
            transform.position = new Vector3 (xBounds / 2f, y, zBounds / 2f);
        }
        public void SetPitchInstant (float f) {
            pitch.localRotation = Quaternion.Euler (f, 0, 0);
            pitchTarget = f;
        }
        public void SetZoomInstant (float f) {
            neck.localPosition = new Vector3 (0, 0, -Mathf.Abs (f));
            zoomTarget = f;
        }
        public void InvertZoom (bool state) {
            inverts[0] = state;
        }
        public void InvertPitch (bool state) {
            inverts[1] = state;
        }
        public void InvertRotation (bool state) {
            inverts[2] = state;
        }
        public void SetOverlayCulling (int layer, bool state) {
            if (state) {
                overlayCamera.cullingMask |= (1) << layer;
            }
            else {
                overlayCamera.cullingMask &= ~(1 << layer);
            }
        }
        public void SetFarClip (float value) {
            renderCam.farClipPlane = overlayCamera.farClipPlane = value;
        }
        public void SetMoveSpeedValues (Vector2 values) {
            moveSpeedValues = values;
        }

        public void ToggleBigMapMode (System.Action finishTransitionFalseCallback) {
            StopAllCoroutines ();
            if (!BigMapMode) {
                BigMapMode = true;
                _fov = renderCam.fieldOfView;
                renderCam.orthographic = true;
                renderCam.orthographicSize = 4; //very important
                overlayCamera.enabled = false;
                StartCoroutine (TransitionToBigMapMode (true, null));
            }
            else {
                StartCoroutine (TransitionToBigMapMode (false, () => {
                    renderCam.orthographic = false;
                    renderCam.fieldOfView = _fov;
                    BigMapMode = false;
                    overlayCamera.enabled = true;
                    finishTransitionFalseCallback?.Invoke ();
                }));
            }
        }
        private void Awake () {

            //runtime debug commands
            RuntimeDebugController.AddCommand (
                new RuntimeDebugCommand<bool> (
                    "itilt",
                    "inverts the camera tilt",
                    "itilt <bool>",
                    (bool b) => InvertPitch (b)
                    ));

            RuntimeDebugController.AddCommand (
                new RuntimeDebugCommand<bool> (
                    "izoom",
                    "inverts the camera zoom",
                    "izoom < bool>",
                    (bool b) => InvertZoom (b)
                    ));

            RuntimeDebugController.AddCommand (
                new RuntimeDebugCommand<bool> (
                    "irot",
                    "inverts the camera rotation",
                    "irot <bool>",
                    (bool b) => InvertRotation (b)
                    ));

            RuntimeDebugController.AddCommand (
                new RuntimeDebugCommand<float> (
                    "pan",
                    "sets the camera pan speed",
                    "pan <float>",
                    (float f) => speed = Mathf.Clamp (f, 0, 500)
                    ));

            RuntimeDebugController.AddCommand (
                new RuntimeDebugCommand<float> (
                    "fclip",
                    "sets the camera far clip",
                    "fclip <float>",
                    (float f) => renderCam.farClipPlane = f
                    ));

            RuntimeDebugController.AddCommand (
                new RuntimeDebugCommand<bool> (
                    "noclip",
                    "removes boundary clip from the camera",
                    "noclip <bool>",
                    (bool v) => noClip = v
                    ));

            RuntimeDebugController.AddCommand (
                new RuntimeDebugCommand (
                    "resetcam",
                    "resets the camera to 0,0",
                    "resetcam <bool>",
                    () => transform.position = Vector3.zero
                    ));

            RuntimeDebugController.AddCommand (
                new RuntimeDebugCommand<float> (
                    "fov",
                    "sets the camera fov between 0 and 180",
                    "fov <float>",
                    (float f) => renderCam.fieldOfView = Mathf.Clamp (f, 0, 180)
                    ));

            RuntimeDebugController.AddCommand (
                new RuntimeDebugCommand<float> (
                    "minzoom",
                    "sets the camera min zoom value",
                    "minzoom <float>",
                    (float f) => zoomValues[0] = f
                    ));

            RuntimeDebugController.AddCommand (
                new RuntimeDebugCommand<float> (
                    "maxzoom",
                    "sets the camera max zoom value",
                    "maxzoom <float>",
                    (float f) => zoomValues[1] = f
                    ));
        }
        private void Start () {
            SetPitchInstant (Mathf.Lerp (pitchValues[0], pitchValues[1], .5f));
            SetZoomInstant (Mathf.Lerp (zoomValues[0], zoomValues[1], .5f));
        }
        private void Update () {

            if (isCameraLocked || BigMapMode) return;

            float zoomLerp = Mathf.InverseLerp (-zoomValues[0], -zoomValues[1], neck.localPosition.z);
            moveSpeed = Mathf.Lerp (moveSpeedValues[0], moveSpeedValues[1], zoomLerp);

            var deltaSpeed = speed * Time.unscaledDeltaTime;
            Vector2 translation = new Vector2 (
                Input.GetAxis ("Horizontal") * moveSpeed * Time.unscaledDeltaTime,
                Input.GetAxis ("Vertical") * moveSpeed * Time.unscaledDeltaTime
                );

            if (inverts[0]) {
                zoomTarget = Mathf.Clamp (zoomTarget -= Input.mouseScrollDelta.y * 2, -zoomValues[1], -zoomValues[0]);
            }
            else {
                zoomTarget = Mathf.Clamp (zoomTarget += Input.mouseScrollDelta.y * 2, -zoomValues[1], -zoomValues[0]);
            }

            if (inverts[1]) {
                pitchTarget = Mathf.Clamp (-Input.GetAxis ("Tilt") * deltaSpeed * 100 + pitchTarget, pitchValues[0], pitchValues[1]);
            }
            else {
                pitchTarget = Mathf.Clamp (Input.GetAxis ("Tilt") * deltaSpeed * 100 + pitchTarget, pitchValues[0], pitchValues[1]);
            }

            var rot = Input.GetAxis ("Rotation") * deltaSpeed * 50;
            if (inverts[2]) rot *= -1;

            //do translation based on rotation of camera base
            if (translation.x != 0 || translation.y != 0) {
                var pos = transform.position;
                var forward = yaw.forward;
                var right = yaw.right;

                pos += forward * translation.y;
                pos += right * translation.x;

                if (!noClip) {
                    transform.position = new Vector3 (
                       Mathf.Clamp (pos.x, 0, dimensions.x),
                       transform.position.y,
                       Mathf.Clamp (pos.z, 0, dimensions.y)
                       );
                }
                else {
                    transform.position = new Vector3 (
                        pos.x,
                        transform.position.y,
                        pos.z
                        );
                }
            }

            //do rotation
            if (rot != 0) {
                var currentRot = yaw.eulerAngles.y;
                var newRot = currentRot + rot;

                //makse sure rot is between 0 and 360
                if (newRot >= 360) newRot -= 360;
                else if (newRot < 0) newRot += 360;
                yaw.Rotate (Vector3.up, rot);
            }

            //do pitch
            if (pitch.localEulerAngles.x != pitchTarget) {
                if (Mathf.Abs (pitch.localEulerAngles.x - pitchTarget) <= .001f) {
                    pitch.localRotation = Quaternion.Euler (pitchTarget, 0, 0);
                }
                else {
                    pitch.localRotation = Quaternion.Lerp (
                        pitch.localRotation,
                        Quaternion.Euler (pitchTarget, 0, 0),
                        .1f
                        );
                }
            }

            //zooming 
            if (neck.localPosition.z != zoomTarget) {
                if (Mathf.Abs (neck.localPosition.z - zoomTarget) <= .001f) {
                    neck.localPosition = new Vector3 (0, 0, zoomTarget);
                }
                else {
                    neck.localPosition = new Vector3 (
                        0,
                        0,
                        Mathf.Lerp (neck.localPosition.z, zoomTarget, 0.2f)
                        );
                }
            }

            if (EventSystem.current.IsPointerOverGameObject ()) {
                //Input.ResetInputAxes ();
                return;
            }

            //cast onto the invisible plane to get a xy position we are looking at
            Ray ray = renderCam.ScreenPointToRay (Input.mousePosition);
            Selector.Current.SetVisibility (false);
            if (Physics.Raycast (ray, out RaycastHit hit, reach)) {

                WorldInteractor.HandleTouch (hit.point);
            }
        }

        private IEnumerator TransitionToBigMapMode (bool state, System.Action callback) {
            float t = 0;
            float vel = 0;
            float midMap = World.Current.WorldSizeInChunks * World.Current.ChunkSize / 2f;
            Vector3 cameraPosition = renderCam.transform.position;
            Vector3 cameraPositionLocal = renderCam.transform.localPosition;
            Quaternion cameraRotation = renderCam.transform.rotation;
            float orthoSize = renderCam.orthographicSize;

            if (state) {
                while (t < 1) {
                    t = Mathf.SmoothDamp (t, 1, ref vel, .2f);
                    if (t > .999f) t = 1;

                    renderCam.transform.position = Vector3.Lerp (cameraPosition, new Vector3 (midMap, 10, midMap), t);
                    renderCam.transform.rotation = Quaternion.Slerp (cameraRotation, Quaternion.Euler (90, 0, 0), t);
                    renderCam.orthographicSize = Mathf.Lerp (orthoSize, midMap + 2, t);

                    yield return null;
                }
            }
            else {
                renderCam.orthographic = false;
                float f = renderCam.fieldOfView = 0;
                while (t < 1) {
                    t = Mathf.SmoothDamp (t, 1, ref vel, .2f);
                    if (t > .999f) t = 1;

                    renderCam.transform.localPosition = Vector3.Lerp (cameraPositionLocal, Vector3.zero, t);
                    renderCam.transform.localRotation = Quaternion.Slerp (cameraRotation, Quaternion.Euler (0, 0, 0), t);
                    renderCam.fieldOfView = Mathf.Lerp (f, _fov, t);

                    yield return null;
                }
            }

            callback?.Invoke ();
        }

#if UNITY_EDITOR

        private void OnDrawGizmosSelected () {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere (renderCam.transform.position, audioRange);
        }
#endif
    }
}