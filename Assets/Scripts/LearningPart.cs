using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/NN/New Learning Object")]
public class LearningPart : ScriptableObject
{
    public int orderNumber;
    public string title;
    public string longTitle;
    public Sprite icon;

    public LearningPartPage[] pages;
}

[System.Serializable]
public class LearningPartPage
{
    [Multiline(4)]
    public string content;
    public AudioClip sound;
}
