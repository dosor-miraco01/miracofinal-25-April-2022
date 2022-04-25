using Doozy.Engine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YesNoQuestionViewController : MonoBehaviour
{
    public UIView view;
    public RTLTMPro.RTLTextMeshPro questionText;
    private System.Action<string> _resultCallBack;

    public void Ask(string questionContent, System.Action<string> resultCallBack)
    {
        view.Show();
        questionText.text = questionContent;
        _resultCallBack = resultCallBack;
    }// method

    public void YesClicked()
    {
        view.Hide();
        _resultCallBack("Yes");
    }// method

    public void NoClicked()
    {
        view.Hide();
        _resultCallBack("No");
    }// method

    public void Close()
    {
        if (view.IsVisible) view.Hide();
    }
}
