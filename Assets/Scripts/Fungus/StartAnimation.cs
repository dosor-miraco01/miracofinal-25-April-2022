// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Plays a once-off sound effect. Multiple sound effects can be played at the same time.
    /// </summary>
    [CommandInfo("AC",
                 "Start Animation",
                 "Plays animation")]
    [AddComponentMenu("")]
    public class StartAnimation : Command
    {
        [Tooltip("legacy animation target")]
        [SerializeField] protected Animation animationTarget;

        [Tooltip("legacy animation clip")]
        [SerializeField] protected AnimationClip animationClip;

        [SerializeField] protected bool waitUntilFinished;

        public override void OnStopExecuting()
        {
            if (animationTarget != null)
            {   
                animationTarget.Stop();
                CancelInvoke("DoWait");
            }

            base.OnStopExecuting();
        }

        protected virtual void DoWait()
        {
            Continue();
        }

        #region Public members

        public override void OnEnter()
        {
            if (animationTarget == null || animationClip == null)
            {
                Continue();
                return;
            }

            animationTarget.clip = animationClip;
            animationTarget.Play();

            if (waitUntilFinished)
            {
                Invoke("DoWait", animationClip.length);
            }
            else
            {
                Continue();
            }

        }

        public override string GetSummary()
        {
            if (animationTarget == null)
            {
                return "Error: No animation comp selected";
            }

            if (animationClip == null)
            {
                return "Error: No animation clip selected";
            }

            return animationClip.name;
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        #endregion
    }
}
