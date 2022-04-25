using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProblemCaseStepView : MonoBehaviour
{
    [SerializeField]
    private RTLTMPro.RTLTextMeshPro text;
    public GameObject visitedObj;
    public Toggle toggle;
    public Flowchart data { get; private set; }
    public System.Action<ProblemCaseStepView> onClick;

    public void SetData(Flowchart d, string stepTitle)
    {
        text.text = stepTitle;
        data = d;
    }// method

    public void OnClicked()
    {
        if (onClick != null) onClick(this);
    }// method
}
