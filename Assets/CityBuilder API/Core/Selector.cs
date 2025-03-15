using System.Collections.Generic;
using UnityEngine;
using Citybuilder.ProceduralGeometry;

namespace Citybuilder.Core {
    public class Selector : Singleton<Selector> {

        private const float HIGHLIGHT_SCALE_ADDITION = .15f;
        private const float UPDATE_SELECTOR_INTERVAL = .02f;

        //max allowable sizxe for a selection
        //***for performancve resons not programatic or logic limitiations***
        private readonly Vector2 MAX_SELECTION_AREA = new Vector2 (32, 32);

        [SerializeField] [Range (0f, .1f)] private float speed;
        [SerializeField] private bool showGUI;
        [SerializeField] private Material material;

        private Vector3 targetPosition;
        private bool targetVisibility = true;
        private Vector3 velocity = Vector3.zero;
        private Vector2 velocitySize = Vector2.zero;

        private Vector2 targetSize;
        private Vector2 currentSize;
        private Transform[] corners;
        private Transform[] edges;
        private GameObject[] arrows;
        private Transform highlightArea;
        private bool isDrawingArea;

        private Vector3 areaStart;
        private Vector3 areaEnd;
        private Bounds bounds;

        private Canvas canvas;
        private TMPro.TextMeshProUGUI canvasText;

        private bool enableAreaDragging;

        private float timeSinceLastSelectorRegen;

        public Vector2Int Size {
            get {
                return new Vector2Int ((int) targetSize.x, (int) targetSize.y);
            }
        }
        public Vector2Int CoordinatePosition {
            get {
                return new Vector2Int ((int) targetPosition.x, (int) targetPosition.z);
            }
        }

        public bool AreaDraggingEnabled {
            get {
                return enableAreaDragging;
            }
            set {
                enableAreaDragging = value;
                if (value == false) {
                    ResetArea ();
                }
                else {
                    ShowArrows (true);
                }
            }
        }

        public void SetPosition (Vector3 position) {
            targetPosition = position;
        }

        public void SetVisibility (bool state) {
            targetVisibility = state;
        }

        public void ResetArea () {
            currentSize = targetSize = new Vector2 (1, 1);
            highlightArea.SetParent (transform);
            highlightArea.localPosition = Vector3.zero;
            highlightArea.localScale = Vector3.one * (1 + HIGHLIGHT_SCALE_ADDITION);
            canvas.enabled = false; //hide canvas on 1x1's
            ShowArrows (false);
        }

        public void ShowArrows (bool state) {
            for (int i = 0 ; i < arrows.Length ; i++) {
                arrows[i].gameObject.SetActive (state);
            }
        }

        public void Hide (bool state = true) {
            int layer = state ? LayerMask.NameToLayer ("Invisible") : LayerMask.NameToLayer ("Overlay");

            //sadly we have to set the layer for ALL objects individually
            gameObject.layer = layer;
            foreach (Transform t in corners) t.gameObject.layer = layer;
            foreach (Transform t in edges) t.gameObject.layer = layer;
            foreach (GameObject go in arrows) go.layer = layer;
            highlightArea.gameObject.layer = layer;
        }

        private void Awake () {
            corners = new Transform[4];
            corners[0] = transform.Find ("Corner").transform;

            arrows = new GameObject[4];
            arrows[0] = corners[0].GetChild (0).gameObject;
            arrows[0].gameObject.SetActive (false);

            for (int i = 1 ; i < 4 ; i++) {
                corners[i] = Instantiate (corners[0]).transform;
                corners[i].rotation = Quaternion.Euler (0, 90 * i, 0);
                corners[i].SetParent (transform);

                arrows[i] = corners[i].GetChild (0).gameObject;
                arrows[i].gameObject.SetActive (false);
            }

            edges = new Transform[4];
            edges[0] = transform.Find ("Edge").transform;
            for (int i = 1 ; i < 4 ; i++) {
                edges[i] = Instantiate (edges[0]).transform;
                edges[i].rotation = Quaternion.Euler (0, 90 * i, 0);
                edges[i].SetParent (transform);
            }

            highlightArea = transform.Find ("Highlight");
            highlightArea.localScale = Vector3.one * (1 + HIGHLIGHT_SCALE_ADDITION);
            highlightArea.localPosition = Vector3.zero;

            //GUI settings
            canvas = GetComponentInChildren<Canvas> ();
            canvas.transform.SetParent (null);
            canvasText = canvas.GetComponentInChildren<TMPro.TextMeshProUGUI> ();
            canvas.enabled = showGUI;
        }

        private void Start () {
            currentSize = targetSize = new Vector2 (1, 1);
            canvas.enabled = false;
            canvasText.SetText ($"{targetSize.x} x {targetSize.y}");
        }

        private void StartDrawArea () {
            currentSize = targetSize = new Vector2 (1, 1);
            highlightArea.position = targetPosition;
            highlightArea.localScale = Vector3.one * (1 + HIGHLIGHT_SCALE_ADDITION);
            highlightArea.SetParent (null);

            canvas.enabled = false; //hidee canvas on 1x1's

            areaStart = targetPosition;
            isDrawingArea = true;
        }

        private void EndDrawArea () {
            //calculate the size of the area

            transform.position = areaEnd;
            currentSize = targetSize;
            RegenerateSelector ();

            transform.position = areaEnd;
            isDrawingArea = false;
            highlightArea.SetParent (transform, true);

        }

        private void Update () {

            material.SetVector ("Mouse", new Vector2 (transform.position.x, transform.position.z));
            if (enableAreaDragging) {
                if (Input.GetKeyDown (InputMap.KEY_BEGIN_DRAG_AREA)) {
                    StartDrawArea ();
                }

                if (Input.GetKeyUp (InputMap.KEY_BEGIN_DRAG_AREA)) {
                    EndDrawArea ();
                }

                if (isDrawingArea) {
                    timeSinceLastSelectorRegen += Time.unscaledTime;
                    if (timeSinceLastSelectorRegen >= UPDATE_SELECTOR_INTERVAL) {
                        timeSinceLastSelectorRegen = 0;

                        //no need to run this if the mouse has not moved since last calc
                        if (areaEnd == targetPosition) return;


                        //get intended size of area
                        //seprate distance from directions
                        var tempVectorAbs = targetPosition - areaStart;
                        var tempVectorSign = new Vector3 (tempVectorAbs.x < 0 ? -1 : 1, 0, tempVectorAbs.z < 0 ? -1 : 1);
                        tempVectorAbs.x = Mathf.Abs (tempVectorAbs.x);
                        tempVectorAbs.z = Mathf.Abs (tempVectorAbs.z);

                        //Debug.Log (tempVectorAbs);

                        //clamp the distances to the max allotted area
                        tempVectorAbs.x = Mathf.Clamp (tempVectorAbs.x, 0, MAX_SELECTION_AREA.x - 1);
                        tempVectorAbs.z = Mathf.Clamp (tempVectorAbs.z, 0, MAX_SELECTION_AREA.y - 1);

                        //scale the vector back
                        tempVectorAbs.Scale (tempVectorSign);

                        //add the start position and assign to the end pisition
                        areaEnd = areaStart + tempVectorAbs;


                        //testing
                        if (areaStart == targetPosition) {
                            targetSize = new Vector2 (1, 1);
                            canvas.enabled = false;
                            highlightArea.position = new Vector3 (areaStart.x, 0, areaStart.z);
                            highlightArea.localScale = Vector3.one * (1 + HIGHLIGHT_SCALE_ADDITION);
                        }
                        else {
                            canvas.enabled = true;
                            bounds = new Bounds (areaEnd, Vector3.zero);
                            for (int i = 0 ; i < Tile.CornerOffsets.Length ; i++) {
                                bounds.Encapsulate (areaStart + Tile.CornerOffsets[i]);
                            }
                            for (int i = 0 ; i < Tile.CornerOffsets.Length ; i++) {
                                bounds.Encapsulate (areaEnd + Tile.CornerOffsets[i]);
                            }

                            ////instead of clamping, if the bounds are greater than the max size, return
                            //if (bounds.size.x > MAX_SELECTION_AREA.x
                            //    || bounds.size.z > MAX_SELECTION_AREA.y) {
                            //    Debug.Log ("Selector bounds reached max size!");
                            //    return;
                            //}


                            //set the size clamped to the maximum allowed size
                            targetSize = new Vector2 (
                            bounds.size.x,
                            bounds.size.z
                            );

                            //position the highlight area in the center of the bound
                            //Debug.Log (bounds.center);
                            highlightArea.position = bounds.center;

                            //higlight is in a XY plane
                            highlightArea.localScale = new Vector3 (
                                Mathf.Abs (Mathf.Abs (targetSize.x)) + HIGHLIGHT_SCALE_ADDITION,
                                Mathf.Abs (Mathf.Abs (targetSize.y)) + HIGHLIGHT_SCALE_ADDITION,
                                1);


                            if (areaStart.x < areaEnd.x) targetSize.x *= -1;
                            if (areaEnd.z < areaStart.z) targetSize.y *= -1;

                        }

                        canvasText.SetText ($"{Mathf.Abs (targetSize.x)} x {Mathf.Abs (targetSize.y)}");
                        RegenerateSelector ();
                    }
                }
            }
        }

        private void LateUpdate () {

            canvasText.transform.localRotation = Quaternion.Euler (0, 0, -CameraController.Current.RenderCameraRotation.y);

            if (isDrawingArea) {
                transform.position = Vector3.SmoothDamp (transform.position, areaStart, ref velocity, speed);
                currentSize = Vector2.SmoothDamp (currentSize, -targetSize, ref velocitySize, speed);
                canvas.transform.position = areaEnd;

            }
            else {
                transform.position = Vector3.SmoothDamp (transform.position, targetPosition, ref velocity, speed);
                canvas.transform.position = transform.position;
            }


            gameObject.layer = targetVisibility ? 0 : LayerMask.NameToLayer ("Invisible");
            RegenerateSelector ();
        }

        private void RegenerateSelector () {
            Vector2 sizeAbs = new Vector2 (Mathf.Abs (currentSize.x), Mathf.Abs (currentSize.y));

            corners[0].localPosition = new Vector3 (sizeAbs.x - 1, 0, 0);
            corners[1].localPosition = new Vector3 (sizeAbs.x - 1, 0, -sizeAbs.y + 1);
            corners[2].localPosition = new Vector3 (0, 0, -sizeAbs.y + 1);

            edges[0].localPosition = new Vector3 (sizeAbs.x - 1, 0, 0.5f);
            edges[0].localScale = new Vector3 (1, 1, sizeAbs.y * 2);

            edges[1].localPosition = new Vector3 (sizeAbs.x - .5f, 0, -sizeAbs.y + 1);
            edges[1].localScale = new Vector3 (1, 1, sizeAbs.x * 2);

            edges[2].localPosition = new Vector3 (0, 0, -sizeAbs.y + .5f);
            edges[2].localScale = new Vector3 (1, 1, sizeAbs.y * 2);

            edges[3].localPosition = new Vector3 (-.5f, 0, -0);
            edges[3].localScale = new Vector3 (1, 1, sizeAbs.x * 2);

            Vector3 _scale = Vector3.one;
            if (currentSize.x < 0) _scale.x = -1;
            if (currentSize.y < 1) _scale.z = -1;

            transform.localScale = _scale;
        }

        private void OnDrawGizmos () {
            Gizmos.color = Color.green;
            Gizmos.DrawCube (areaStart, new Vector3 (1, 0, 1));

            Gizmos.color = Color.red;
            Gizmos.DrawCube (areaEnd, new Vector3 (1, 0, 1));

            Gizmos.color = new Color (1, .5f, 0);
            Gizmos.DrawSphere (bounds.center + Vector3.up * 1, .25f);
            Gizmos.DrawWireCube (bounds.center, bounds.size + new Vector3 (0, 2, 0));
        }

        private void OnValidate () {
            if (canvas) canvas.enabled = showGUI;
        }
    }
}
