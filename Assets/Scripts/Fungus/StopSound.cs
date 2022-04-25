// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Plays a once-off sound effect. Multiple sound effects can be played at the same time.
    /// </summary>
    [CommandInfo("AC",
                 "Stop Sound",
                 "Stop sound")]
    [AddComponentMenu("")]
    public class StopSound : Command
    {   
        protected virtual void DoWait()
        {
            Continue();
        }

        #region Public members

        public override void OnEnter()
        {
          
            var con = FindObjectOfType<MainController>();
            if (con)
            {
                con.mainAudioSource.Stop();
                Continue();
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
