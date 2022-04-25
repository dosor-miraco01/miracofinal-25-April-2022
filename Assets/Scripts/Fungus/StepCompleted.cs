// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Plays a once-off sound effect. Multiple sound effects can be played at the same time.
    /// </summary>
    [CommandInfo("AC",
                 "Step Completed",
                 "Step Completed")]
    [AddComponentMenu("")]
    public class StepCompleted : Command
    {
        
        #region Public members

        public override void OnEnter()
        {
            MainController mc = FindObjectOfType<MainController>();
            if (mc != null && mc.StepCompleted != null) mc.StepCompleted(GetFlowchart());

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        #endregion
    }
}
