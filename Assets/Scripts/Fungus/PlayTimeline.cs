// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Fungus
{
    /// <summary>
    /// Plays a once-off sound effect. Multiple sound effects can be played at the same time.
    /// </summary>
    [CommandInfo("AC",
                 "Play Timeline",
                 "Plays Timline")]
    [AddComponentMenu("")]
    public class PlayTimeline : Command
    {
        [Tooltip("legacy animation target")]
        [SerializeField] protected PlayableDirector timelineDirector;
        [SerializeField] protected bool waitUntilFinished;

        public override void OnStopExecuting()
        {
            if (timelineDirector != null)
            {
                timelineDirector.Stop();
                CancelInvoke("DoWait");
            }
            base.OnStopExecuting();
        }

        protected virtual void DoWait()
        {
            if ((timelineDirector.time == 0 && timelineDirector.state != PlayState.Playing) || timelineDirector.time == timelineDirector.duration)
            {
                Continue();
            }
            else
            {
                Invoke("DoWait", (float)(timelineDirector.duration - timelineDirector.time));
            }
        }

        #region Public members

        //IEnumerator _DoWait()
        //{
        //    var tm = TimelineSliderController.CurrentTimeLine;
        //    yield return new WaitForSeconds((float)(tm.duration / 2.0));
        //    var p = tm.duration * 0.05f;
        //    Debug.Log("Wait time :" + (tm.duration - p));
        //    yield return new WaitUntil(() => tm != TimelineSliderController.CurrentTimeLine || tm.time == tm.duration - p);
        //    Debug.Log("finished");
        //    Continue();
        //}

        public override void OnEnter()
        {
            if (timelineDirector == null)
            {
                Continue();
                return;
            }
            TimelineSliderController.CurrentTimeLine = timelineDirector;
            timelineDirector.Play();
            if (waitUntilFinished)
            {
                //StartCoroutine(_DoWait());
                Invoke("DoWait", (float)timelineDirector.duration);
            }
            else
            {
                Continue();
            }

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
