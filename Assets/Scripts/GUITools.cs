using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public static class GUITools
{
    public const float Width = 1920;
    public const float Height = 1080;
    private static float _aspectRatio = 0;
    public static float AspectRatio
    {
        get
        {
            if (_aspectRatio == 0)
            {
                float nPercentW = 0;
                float nPercentH = 0;

                nPercentW = ((float)Screen.width / Width);
                nPercentH = ((float)Screen.height / Height);

                _aspectRatio = Mathf.Min(nPercentH, nPercentW);
            }
            return _aspectRatio;
        }
        
    }

    /// <summary>
    /// Cast a ray to test if Input.mousePosition is over any UI object in EventSystem.current. This is a replacement
    /// for IsPointerOverGameObject() which does not work on Android in 4.6.0f3
    /// </summary>
    public static bool IsPointerOverUIObject(string tag = "CameraBlocker")
    {
        if (EventSystem.current == null) return false;
        // Referencing this code for GraphicRaycaster https://gist.github.com/stramit/ead7ca1f432f3c0f181f
        // the ray cast appears to require only eventData.position.
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        //foreach (var a in results) Debug.Log(a.gameObject);
        return results.Count(x => x.gameObject.tag != "CameraBlocker" && x.gameObject.tag != tag) > 0;// Matri > 0;
    }
}