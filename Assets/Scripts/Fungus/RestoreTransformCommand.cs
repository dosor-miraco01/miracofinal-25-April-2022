// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Plays a once-off sound effect. Multiple sound effects can be played at the same time.
    /// </summary>
    [CommandInfo("AC",
                 "RestoreTransform",
                 "Restore Saved Transform")]
    [AddComponentMenu("")]
    public class RestoreTransformCommand : Command
    {
        [SerializeField] protected string id;
        [SerializeField] protected Transform target;

        #region Public members

        public override void OnEnter()
        {
            if (target != null)
            {
                var sv = target.GetComponent<SaveTransformBehaviour>();
                if (sv)
                {
                    sv.Restore(id);
                }
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (target == null)
            {
                return "Error: No target transform selected";
            }

            return id;
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        #endregion
    }
}

