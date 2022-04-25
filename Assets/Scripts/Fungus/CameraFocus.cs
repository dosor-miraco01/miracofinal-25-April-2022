// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Plays a once-off sound effect. Multiple sound effects can be played at the same time.
    /// </summary>
    [CommandInfo("AC",
                 "Camera Foucs",
                 "Camera Focus")]
    [AddComponentMenu("")]
    public class CameraFocus : Command
    {
        [SerializeField] protected Transform focusTarget;
        [SerializeField] protected Transform cameraPosition;
        [SerializeField] protected float camMoveSpeed = 5;
        [SerializeField] protected bool movementSpeedBased = true;

        private maxCamera maxCamera;
        private Camera mainCamera;

        protected virtual void DoWait()
        {
            Continue();
        }

        #region Public members

        public override void OnStopExecuting()
        {
            StopCoroutine("DirectNavigation");
            maxCamera.SetTarget(focusTarget);
            maxCamera.Init();
            maxCamera.enabled = true;
            
            base.OnStopExecuting();
        }

        IEnumerator DirectNavigation()
        {
            var tr = mainCamera.transform;
            maxCamera.enabled = false;
            var a = tr.position;
            var b = cameraPosition.position;

            float camsp = camMoveSpeed;
            
            yield return tr.DOLookAt(focusTarget.position, 0.5f, AxisConstraint.None/*, Vector3.zero*/).WaitForCompletion();
            yield return tr.DOMove(b, camsp).SetSpeedBased(movementSpeedBased).OnUpdate(() =>
            {
                tr.LookAt(focusTarget);
            }).WaitForCompletion();

            maxCamera.SetTarget(focusTarget);
            maxCamera.Init();
            maxCamera.enabled = true;
            tr.position = b;

            Continue();
        }

        public override void OnEnter()
        {
            if (focusTarget == null || cameraPosition == null)
            {
                Continue();
                return;
            }
            var con = FindObjectOfType<MainController>();
            if (con)
            {
                mainCamera = con.mainCamera;
                maxCamera = con.maxCamera;
                StartCoroutine("DirectNavigation");
            }
            else
            {
                Continue();
            }
        }

        public override string GetSummary()
        {
            return "";
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        #endregion
    }
}
