using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PausePlayTimelineButton : MonoBehaviour
{
    public bool hideIfNonPlayableDirector = false;

    private Image _img = null;
    
    private void Start()
    {
        _img = GetComponent<Image>();
    }

    private void Update()
    {
        if (hideIfNonPlayableDirector)
        {
            bool isEnabled = TimelineSliderController.CurrentTimeLine != null;
            if (_img.enabled != isEnabled)
            {
                _img.enabled = isEnabled;
            }
        }
    }
   
}
