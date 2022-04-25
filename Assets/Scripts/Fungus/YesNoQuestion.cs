// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Calls a named method on a GameObject using the GameObject.SendMessage() system.
    /// This command is called "Call Method" because a) it's more descriptive than Send Message and we're already have
    /// a Send Message command for sending messages to trigger block execution.
    /// </summary>
    [CommandInfo("AC",
                 "Yes No Question",
                 "Ask Yes No Question ")]
    [AddComponentMenu("")]
    public class YesNoQuestion : Command
    {   
        [SerializeField] protected string question;

        [SerializeField] protected StringData resultValue;

        public override void OnStopExecuting()
        {
            var con = FindObjectOfType<YesNoQuestionViewController>();
            if (con)
            {
                con.Close();
            }
            base.OnStopExecuting();
        }

        #region Public members

        public override void OnEnter()
        {
            var con = FindObjectOfType<YesNoQuestionViewController>();
            if (con)
            {
                con.Ask(question, (result) => { resultValue.Value = result; Continue(); });
            }
        }

        public override string GetSummary()
        {
            return question;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #endregion
    }
}