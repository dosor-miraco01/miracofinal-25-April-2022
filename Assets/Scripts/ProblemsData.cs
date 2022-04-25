using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/NN/ProblemsData")]
public class ProblemsData : ScriptableObject
{
    public string logicSceneName = "08 Problem";
    public ProblemItemData[] items;
}

[System.Serializable]
public class ProblemItemData
{
    public int problemId;
    public string title;
    public Sprite problemReportTitle;
    public int casesCount;
    public string sceneName;
    public bool isTitleOnly = false;
}