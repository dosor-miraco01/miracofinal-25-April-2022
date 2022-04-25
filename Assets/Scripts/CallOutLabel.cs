using RTLTMPro;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallOutLabel : MonoBehaviour
{
    [SerializeField]
    public string content;
    public Transform target;
    private Transform labelTr;
    public Texture2D labelBackground;
    public Texture2D labelPoint_lb;
    public Texture2D labelPoint_part;
    public Texture2D labelLine_bg;
    public GUIStyle textStyle;
    public LabelStyle labelStyle;

    public bool isOn { get; set; } = false;
    
    [ReadOnly]
    public float angle;
    // Start is called before the first frame update
    void Start()
    {
        labelTr = GetComponent<Transform>();
        
    }

    public void SetContent(string text)
    {
        FastStringBuilder output = new FastStringBuilder(text.Length);
        RTLTMPro.RTLSupport.FixRTL(text, output, false, false);
        content = output.ToString();
    }

    private void OnGUI()
    {
        if (isOn)
        {
            var lbScPos = Camera.main.WorldToScreenPoint(labelTr.position);
            var objScPos = Camera.main.WorldToScreenPoint(target.position);
            //GUI.Box(new Rect(lbScPos.x, Screen.height - lbScPos.y, 100, 100), content);
            var angleBetween = Mathf.Atan2(lbScPos.y - objScPos.y, lbScPos.x - objScPos.x) * 180 / Mathf.PI;
            angle = angleBetween;

            float aspectRatio = GUITools.AspectRatio;
            float lbWidth = aspectRatio * labelBackground.width;
            float lbHeight = aspectRatio * labelBackground.height;
             

            //Debug.Log(angleBetween);
            var pos = new Rect(lbScPos.x, Screen.height - lbScPos.y, lbWidth, lbHeight);
            if (angle>= 0 && angle <= 90)
            {
                pos = new Rect(lbScPos.x - 87, Screen.height - lbScPos.y - lbHeight + 5, lbWidth, lbHeight);
            }
            else if (angle >= 90 && angle <= 180)
            {
                pos = new Rect(lbScPos.x - lbWidth + 87, Screen.height - lbScPos.y - lbHeight + 5, lbWidth, lbHeight);
            }
            else if (angle < 0)
            {
                angle *= -1;
                if (angle >= 0 && angle <= 90)
                {
                    pos = new Rect(lbScPos.x - 87, Screen.height - lbScPos.y - 5, lbWidth, lbHeight);
                }
                else if (angle >= 90 && angle <= 180)
                {
                    pos = new Rect(lbScPos.x - lbWidth + 87, Screen.height - lbScPos.y - 5, lbWidth, lbHeight);
                }
            }
            
            GUI.DrawTexture(pos/*new Rect(lbScPos.x, Screen.height - lbScPos.y, labelBackground.width, labelBackground.height)*/, labelBackground);
            GUI.Label(pos/*new Rect(lbScPos.x, Screen.height - lbScPos.y, labelBackground.width, labelBackground.height)*/, content, textStyle);
            
            GuiHelper.DrawLine(new Vector2(lbScPos.x, Screen.height - lbScPos.y), new Vector2(objScPos.x, Screen.height - objScPos.y), labelStyle.color, labelStyle.width);

            float lbPoint_lb_Width = labelPoint_lb.width * aspectRatio;
            float lbPoint_lb_height = labelPoint_lb.height * aspectRatio;
            float lbPoint_part_width = labelPoint_part.width * aspectRatio;
            float lbPoint_part_height = labelPoint_part.height * aspectRatio;

            GUI.DrawTexture(new Rect(lbScPos.x - (lbPoint_lb_Width / 2),
                Screen.height - lbScPos.y - (lbPoint_lb_height / 2), 
                lbPoint_lb_Width, lbPoint_lb_height), labelPoint_lb);
            GUI.DrawTexture(new Rect(objScPos.x - (lbPoint_part_width / 2), 
                Screen.height - objScPos.y - (lbPoint_part_height / 2), 
                lbPoint_part_width, lbPoint_part_height), labelPoint_part);
        }
    }
}
