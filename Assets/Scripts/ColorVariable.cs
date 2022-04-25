using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/NN/ColorVariable")]
public class ColorVariable : ScriptableObject
{
    [SerializeField]
    private Color value = Color.white;

    public Color Value => value;
}
