// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Plays a once-off sound effect. Multiple sound effects can be played at the same time.
    /// </summary>
    [CommandInfo("AC",
                 "Play Sound",
                 "Plays a once-off sound.")]
    [AddComponentMenu("")]
    public class PlaySound2 : Command
    {
        [Tooltip("Sound clip to play")]
        [SerializeField] protected AudioClip soundClip;


        [Tooltip("Wait until the sound has finished playing before continuing execution.")]
        [SerializeField] protected bool waitUntilFinished;

        protected virtual void DoWait()
        {
            Continue();
        }

        public override void OnStopExecuting()
        {
            var con = FindObjectOfType<MainController>();
            if (con)
            {  
                con.mainAudioSource.clip = null;
                con.mainAudioSource.Stop();
                CancelInvoke("DoWait");
            }
            base.OnStopExecuting();
        }

        #region Public members

        public override void OnEnter()
        {
            if (soundClip == null)
            {
                Continue();
                return;
            }

            var con = FindObjectOfType<MainController>();
            if (con)
            {
                con.mainAudioSource.clip = soundClip;
                con.mainAudioSource.Play();
                if (waitUntilFinished)
                {
                    Invoke("DoWait", soundClip.length);
                }
                else
                {
                    Continue();
                }
            }
            else
            {
                Continue();
            }
        }

        public override string GetSummary()
        {
            if (soundClip == null)
            {
                return "Error: No sound clip selected";
            }

            return soundClip.name;
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        #endregion
    }
}
