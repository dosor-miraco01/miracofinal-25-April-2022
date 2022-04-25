using RTLTMPro;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class TimeLineDataRef : MonoBehaviour
{
    public RTLTextMeshPro text;
    public Toggle toggle;
    public Image icon;

    public PlayableDirector timeline { get; set; }

    public event System.Action<TimeLineDataRef> onClicked;

    public void ToggleClicked()
    {
        if (onClicked != null)
        {
            onClicked(this);
        }
    }// method

}
