using Doozy.Engine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageController : MonoBehaviour
{
    public UIView view;
    public RTLTMPro.RTLTextMeshPro messageText;
    public RTLTMPro.RTLTextMeshPro okText;
    private System.Action _resultCallBack;

    public void Show(string msgContent, string okContent, System.Action resultCallBack)
    {
        view.Show();
        messageText.text = msgContent;
        okText.text = okContent;
        _resultCallBack = resultCallBack;
    }// method

    public void OkClicked()
    {
        view.Hide();
        _resultCallBack();
    }// method

    public void Close()
    {
        if (view.IsVisible) view.Hide();
    }
}
