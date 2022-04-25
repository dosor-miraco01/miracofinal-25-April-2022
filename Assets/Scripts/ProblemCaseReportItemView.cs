using RTLTMPro;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProblemCaseReportItemView : MonoBehaviour
{
    public Toggle toggle;
    public RTLTextMeshPro text;

    [ReadOnly]
    public int problemCaseIndex = 0;

    [ReadOnly]
    public ProblemItemData problem;

    [ReadOnly]
    public string logicSceneName = "";

    public FunctionsObject functionsObject;

    public void OnItemClicked()
    {
        MainController.selectedProblem = problem;
        ProblemController.selectCaseIndex = problemCaseIndex;
        if (functionsObject != null)
        {
            functionsObject.LoadScene(logicSceneName);
        }
        else
        {
            SceneManager.LoadScene(logicSceneName);
        }
    }// method
}
