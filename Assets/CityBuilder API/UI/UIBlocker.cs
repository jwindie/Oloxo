using Citybuilder.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Citybuilder.UI {
    /// <summary>
    /// Simple UI panel to block elements drawn underneath it
    /// </summary>
    public class UIBlocker : Singleton<UIBlocker> {

        [SerializeField] private bool lockCameraOnActivate;
        [SerializeField] private bool hideSelector;
        [SerializeField] private bool hideContextMenu;

        private Image image;

        //the number of stacked activations
        //will only allow closing if the stack is zero
        private Stack<GameObject> callerStack = new Stack<GameObject> ();

        private void Awake () {
            image = GetComponent<Image> ();
            image.enabled = false;
        }

        /// <summary>
        /// Sets the color without affecting transparency.
        /// </summary>
        /// <param name="c"></param>
        public void SetColor (Color c) {
            image.color = new Color (c.r, c.g, c.b, image.color.a);
        }

        /// <summary>
        /// Sets the transparencty of the image.
        /// </summary>
        /// <param name="f"></param>
        public void SetTransparency (float f) {
            image.color = new Color (image.color.r, image.color.g, image.color.b, f);
        }

        public void ActivateBlocker (GameObject caller) {
            image.enabled = true;
            if (lockCameraOnActivate) CameraController.Current.LockCamera ();
            if (hideContextMenu) HUDController.Current.Hide ();
            if (hideSelector) Selector.Current.Hide ();

            callerStack.Push (caller);
            transform.SetParent (caller.transform.parent, true);
            transform.SetSiblingIndex (caller.transform.GetSiblingIndex () - 1);
        }

        public void DeactivateBlocker () {
            if (callerStack.Count > 1) {
                callerStack.Pop ();
                transform.SetParent (callerStack.Peek ().transform.parent, true);
                transform.SetSiblingIndex (callerStack.Peek ().transform.GetSiblingIndex () - 1);
            }
            else {
                if (callerStack.Count == 1) {
                    callerStack.Pop ();
                }

                image.enabled = false;

                //always unlockthe camera so that if the 'lockCamera' option changes while locked, the camera won't break
                CameraController.Current.LockCamera (false);
                HUDController.Current.Hide (false);
                Selector.Current.Hide (false);
            }
        }

        /// <summary>
        /// Moves the blocker in the hierarchy to block the individual object  and all that come after it.
        /// <para>Only works if the element has the same parent as the blocker.</para>
        /// </summary>
        /// <param name="element"></param>
        public void BlockElementAndAbove (Transform element) {
            if (element.parent == transform.parent) {
                int elementChildINdex = element.GetSiblingIndex ();
                transform.SetSiblingIndex (elementChildINdex + 1);
            }
        }
    }
}