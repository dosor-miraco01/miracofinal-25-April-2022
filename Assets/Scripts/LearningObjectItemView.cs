using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class LearningObjectItemView : MonoBehaviour
{
    public Image iconImage;
    public RTLTMPro.RTLTextMeshPro labelText;
    public GameObject visitedBackground;
    public TextMeshProUGUI text;
    public event System.Action<LearningObjectItemView> onItemSelected;
    [SerializeField]
    private LearningPartRef data;
    public LearningPartRef Data => data;
    private Toggle _toggleComp;
    private void Awake()
    {
        _toggleComp = GetComponent<Toggle>();
        SetIconAlpha(0.5f);
        //_toggleComp.onValueChanged.AddListener((isOk)=> { if (isOk && onItemSelected != null) onItemSelected(this); }) ;
    }

    public void SetData(LearningPartRef newData)
    {
        if(newData != data)
        {
            iconImage.sprite = newData.data.icon;
            labelText.text = newData.data.title;
            data = newData;
        }
    }

    public void OnItemClicked()
    {
        if (onItemSelected != null) onItemSelected(this);
    }

    public void SetToggle(bool isOn)
    {
        _toggleComp.isOn = isOn;
        GetComponentInParent<GridAspectRatio>().CheckIfSelectedItemNeedToExpand(GetComponent<RectTransform>());
    }

    public void SetIconAlpha(float v)
    {
        var c = iconImage.color;
        c.a = v;
        iconImage.color = c;
    }
}
