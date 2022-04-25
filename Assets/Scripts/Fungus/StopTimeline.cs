// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Playables;

namespace Fungus
{
    /// <summary>
    /// Plays a once-off sound effect. Multiple sound effects can be played at the same time.
    /// </summary>
    [CommandInfo("AC",
                 "Stop Timeline",
                 "Stop Timline")]
    [AddComponentMenu("")]
    public class StopTimeline : Command
    {   
        [SerializeField] protected PlayableDirector timelineDirector;

        #region Public members

        public override void OnEnter()
        {
            if (timelineDirector == null)
            {
                Continue();
                return;
            }
            timelineDirector.Stop();
            Continue();
            
        }

        public override string GetSummary()
        {
            if (timelineDirector == null)
            {
                return "Error: No animation comp selected";
            }

            return timelineDirector.name;
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        #endregion
    }
}
