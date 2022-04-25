using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProblemReportItemView : MonoBehaviour
{
    public FunctionsObject functions;
    public Image iconImage;
    public ProblemItemData problem { get; set; }
    public Transform casesParent;
    public string logicSceneName;
    public FunctionsObject functionsObject;
    public void OnItemClicked()
    {   
        if (problem != null && !problem.isTitleOnly && problem != MainController.selectedProblem)
        {
            if (functions != null) functions.RestoreCursor();
            MainController.selectedProblem = problem;
            if (functionsObject != null)
            {
                functionsObject.LoadScene(logicSceneName);
            }
            else
            {
                SceneManager.LoadScene(logicSceneName);
            }
        }
    }// method

    public void DoMouseEnter()
    {
        if (functions != null && problem != null && !problem.isTitleOnly && problem != MainController.selectedProblem)
        {
            functions.ChangeCursor();
        }
    }

    public void DoMouseExit()
    {
        if (functions != null && problem != null && !problem.isTitleOnly && problem != MainController.selectedProblem)
        {
            functions.RestoreCursor();
        }
    }// method
}
