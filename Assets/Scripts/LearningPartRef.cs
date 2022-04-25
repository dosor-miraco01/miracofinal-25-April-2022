using RTLTMPro;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class LearningPartRef : MonoBehaviour
{
    [MultiLineProperty, ShowIf("showWarningMessage")]
    public string warningMessage;

    public bool showWarningMessage = false;

    public LearningPart data;
    public GameObject cameraObject;
    public Transform labelPosition;

    public bool ignoreCameraNavigationPoints = false;

    public bool overrideCameraMovementSpeed = false;

    [ShowIf("overrideCameraMovementSpeed")]
    public float cameraMovementSpeed = 0;

    public bool skipTimelines = false;
    
    [ShowIf("skipTimelines")]
    public int skipTimelineFrom = 0;

    public bool allowSideImage = false;

    [ShowIf("allowSideImage")]
    public Sprite sideImage = null;

    public PlayableDirector[] timelines;

    public GameObject[] objectsToDeactivate;
    public TransparentObject[] objectsToTransparent;
    public InternalLearningPart[] internalParts;

    public TransparentObject[] transparentObjects { get; private set; }

    public event System.Action<LearningPartRef> onObjectClicked;
    private string text;
    private GUIContent content;

    public Vector3 camPosition { get; private set; }

    private bool clicked = false;
    private float clickTime = 0;
    private const float clickDelta = 0.25f;
    private void Start()
    {
        if (cameraObject != null)
        {
            camPosition = cameraObject.transform.GetChild(0).position;
            var ore = GetComponent<TransparentObject>();
            List<TransparentObject> items = new List<TransparentObject>(GetComponentsInChildren<TransparentObject>());
            if (ore != null) items.Add(ore);
            transparentObjects = items.ToArray();
        }
    }

    private void Update()
    {
        if (clicked)
        {
            if (Time.time >= (clickTime + clickDelta))
            {   
                clicked = false;
            }
        }
    }
    //private void OnMouseEnter()
    //{
    //    if (data != null)
    //    {
    //        text = data.title;
    //        if (content == null)
    //        {
    //            FastStringBuilder output = new FastStringBuilder(text.Length);
    //            RTLTMPro.RTLSupport.FixRTL(text, output, false, false);
    //            content = new GUIContent(output.ToString());
    //        }
    //    }
    //}

    private void OnMouseDown()
    {
        if (GUITools.IsPointerOverUIObject()) return;
        if (!clicked)
        {
            clicked = true;
            clickTime = Time.time;
        }
        else
        {
            if (onObjectClicked != null) onObjectClicked(this);
        }
    }

    //private void OnMouseExit()
    //{
    //    text = null;
    //}

    //private void OnGUI()
    //{
    //    if (!string.IsNullOrEmpty(text))
    //    {
    //        int oSize = GUI.skin.box.fontSize;
    //        GUI.skin.box.fontSize = 45;
    //        var pos = Input.mousePosition;
    //        var size = GUI.skin.box.CalcSize(content);
    //        GUI.Box(new Rect(pos.x - size.x - 5, Screen.height - size.y - pos.y - 5, size.x, size.y), content);
    //        GUI.skin.box.fontSize = oSize;
    //    }

    //}
}
