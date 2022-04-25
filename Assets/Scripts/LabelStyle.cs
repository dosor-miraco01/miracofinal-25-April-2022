using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/NN/LabelStyle")]
public class LabelStyle : ScriptableObject
{
    public Color color = new Color(0.09f, 0.5686f, 0.96f);
    public int width = 5;
}
