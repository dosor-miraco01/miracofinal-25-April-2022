// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using Doozy.Engine.UI;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Calls a named method on a GameObject using the GameObject.SendMessage() system.
    /// This command is called "Call Method" because a) it's more descriptive than Send Message and we're already have
    /// a Send Message command for sending messages to trigger block execution.
    /// </summary>
    [CommandInfo("AC",
                 "Display Description",
                 "Display description")]
    [AddComponentMenu("")]
    public class DisplayDescription : Command
    {
        [SerializeField] protected string content;

        
        #region Public members

        public override void OnEnter()
        {
            var con = FindObjectOfType<MainController>();
            if (con)
            {
                bool showBottomSection = con.bottomSection.alpha == 0;
                con.contentText.text = content;
                con.bottomSection.interactable = true;
                con.bottomSection.alpha = 1;
                con.bottomSection.blocksRaycasts = true;
                if (showBottomSection)
                {
                    con.bottomSection.GetComponent<UIView>().Show();
                }
            }

            Continue();
        }

        public override string GetSummary()
        {
            return content;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #endregion
    }
}