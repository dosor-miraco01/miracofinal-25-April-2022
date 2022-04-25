using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProblemCaseView : MonoBehaviour
{
    [SerializeField]
    private RTLTMPro.RTLTextMeshPro text;
    public Toggle toggle;
    public ProblemCase data { get; private set; }
    public System.Action<ProblemCaseView> onClick;

    public void SetData(ProblemCase d)
    {
        text.text = d.title;
        data = d;
    }// method

    public void OnClicked()
    {
        if (onClick != null) onClick(this);
    }// method
}
