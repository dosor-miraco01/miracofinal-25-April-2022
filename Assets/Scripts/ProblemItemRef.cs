using Fungus;
using RTLTMPro;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ProblemItemRef : LearningPartRef
{
    public int id;
    public string title;
    public Sprite icon;
    //public int menuOrderNum;

    public ProblemCase[] cases;
}// method

[System.Serializable]
public class ProblemCase
{
    public string title;
    public int ID { get; set; }
    public Flowchart[] steps;
}