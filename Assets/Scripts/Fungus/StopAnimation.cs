// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Plays a once-off sound effect. Multiple sound effects can be played at the same time.
    /// </summary>
    [CommandInfo("AC",
                 "Stop Animation",
                 "Stop animation")]
    [AddComponentMenu("")]
    public class StopAnimation : Command
    {
        [Tooltip("legacy animation target")]
        [SerializeField] protected Animation animationTarget;

        protected virtual void DoWait()
        {
            Continue();
        }

        #region Public members

        public override void OnEnter()
        {
            if (animationTarget == null)
            {
                Continue();
                return;
            }

            animationTarget.Stop();
            Continue();
        }

        public override string GetSummary()
        {
            if (animationTarget == null)
            {
                return "Error: No animation comp selected";
            }

            return animationTarget.name;
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        #endregion
    }
}
