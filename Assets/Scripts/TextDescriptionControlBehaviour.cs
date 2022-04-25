using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class TextDescriptionControlBehaviour : PlayableBehaviour
{
    [SerializeField, Multiline]
    private string description;

    [SerializeField]
    private bool revertTolastOnPause = false;

    private MainController _mainController;
    MainController mainController
    {
        get
        {
            if (_mainController == null) _mainController = GameObject.FindObjectOfType<MainController>();
            return _mainController;
        }
    }

    private bool _isEnter = false;
    private string _lastContent = string.Empty;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (!_isEnter)
        {
            _lastContent = mainController?.contentText?.text;
            _isEnter = true;
        }
        //base.ProcessFrame(playable, info, playerData);
        if (mainController != null)
        {
            mainController.contentText.text = description;
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (_isEnter && mainController != null)
        {
            if (revertTolastOnPause)
            {
                mainController.contentText.SetText(_lastContent);
            }
            _isEnter = false;
        }
        base.OnBehaviourPause(playable, info);
    }
}