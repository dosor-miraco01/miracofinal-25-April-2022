using Doozy.Engine.UI;
using RTLTMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Linq;
using UnityEngine.EventSystems;
#if AC_URP
using UnityEngine.Rendering.Universal;
#endif
public class MainController : MonoBehaviour
{
    public static event System.Action ModelSceneLoaded;
    public static ProblemItemData selectedProblem;

    public LevelType levelType = LevelType.Learning;
    public ModelScene modelScene;
    public string modelSceneName;

    public TargetData target;
    public TransparentController transparentController;
    public Image backgroundImage;
    public Camera backgroundImageCamera;
    public Transform objectsParent = null;
    public GameObject objectItemPrefab = null;

    public Transform timelinesParent;
    public GameObject timelinePrefab;

    public Toggle autoPlayToggle;
    public Button repeatButton;
    public Toggle deactivateToggle;
    public Toggle labelsToggle;
    public AudioSource mainAudioSource;
    public RTLTMPro.RTLTextMeshPro contentText;
    public RTLTMPro.RTLTextMeshPro titleText;
    public Button nextPageButton;
    public Button prevPageButton;
    public ToggleGroup objectItemsToggleGroup;
    public CanvasGroup bottomSection;
 
    public Texture2D labelBackground;
    public Texture2D labelPoint_lb;
    public Texture2D labelPoint_part;
    public Texture2D labelLine_bg;
    public GUISkin labelTextStyle;
    public LabelStyle labelStyle;
    public UIView warningMessage;
    public Image sideImage;
    public GameObject cameraLock;
    public RTLTMPro.RTLTextMeshPro warningMessageContent;

    public UIView[] views;
    public float camMoveSpeed = 3.0f;

    [Header("Problem Object")]
    public ProblemsData problemsData;
    public RTLTextMeshPro problemTitleText;
    //public RTLTextMeshPro caseTitleText;
    
    public Transform problemCasesParent;
    public GameObject problemCasesPrefab;

    public Transform problemCaseStepsParent;
    public GameObject problemCaseStepPrefab;
    public FunctionsObject functionsObject;

    public Image helpImage;

    public maxCamera maxCamera { get; private set; }
    public Camera mainCamera { get; private set; }

    private IGameController _controller;

    public System.Action<Fungus.Flowchart> StepCompleted;

    public bool isInitialized { get; protected set; } = false;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        if (FunctionsObject.currentHelpImage != null && helpImage != null)
        {
            helpImage.sprite = FunctionsObject.currentHelpImage;
            FunctionsObject.currentHelpImage = null;
        }
        bottomSection.alpha = 0;
        if (!string.IsNullOrEmpty(FunctionsObject.CurrentModelScene))
        {
            modelSceneName = FunctionsObject.CurrentModelScene;
            FunctionsObject.CurrentModelScene = null;
        }
        if (levelType == LevelType.Learning) _controller = new LearningController(this);
        else if (levelType == LevelType.Problem)
        {
            _controller = new ProblemController(this);
            if (problemsData.items != null && selectedProblem != null)
            {
                modelSceneName = selectedProblem.sceneName;
                titleText.text = selectedProblem.title;
                //problemsData.items.SingleOrDefault(x => !x.isTitleOnly && x.problemId == selectedProblem.problemId && !string.IsNullOrEmpty(x.sceneName));
            }
        }
        else
        {   
            _controller = new AnimationController(this);
        }

        if (levelType != LevelType.Learning)
        {
            if (autoPlayToggle)
            {
                autoPlayToggle.interactable = false;
                var comp = autoPlayToggle.GetComponent<EventTrigger>();
                Destroy(comp);
            }
            if (levelType == LevelType.Problem && repeatButton)
            {
                repeatButton.interactable = false;
                var comp = repeatButton.GetComponent<EventTrigger>();
                Destroy(comp);
            }
            if (labelsToggle)
            {
                labelsToggle.interactable = false;
                var comp = labelsToggle.GetComponent<EventTrigger>();
                Destroy(comp);
            }
        }

        if (modelScene && modelScene.background)
        {
            backgroundImage.sprite = modelScene.background;
            backgroundImage.gameObject.SetActive(true);
            backgroundImage.transform.parent.gameObject.SetActive(true);
        }

        string mdSceneName = modelSceneName;/*modelScene ? modelScene.sceneName : modelSceneName*/;
        
        if (!string.IsNullOrEmpty(mdSceneName))
        {
            yield return SceneManager.LoadSceneAsync(mdSceneName, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(mdSceneName));
        }
        if (modelScene && modelScene.background)
        {
#if AC_URP
            var urp = Camera.main.GetUniversalAdditionalCameraData();
            if (urp != null)
            {
                urp.renderType = CameraRenderType.Overlay;
                backgroundImageCamera.gameObject.SetActive(true);
                urp = backgroundImageCamera.GetUniversalAdditionalCameraData();
                urp.cameraStack.RemoveAt(0);
                urp.cameraStack.Add(Camera.main);
            }
#else
            backgroundImageCamera.gameObject.SetActive(true);
            Camera.main.clearFlags = CameraClearFlags.Depth;
#endif
        }

        if (MainController.ModelSceneLoaded != null) MainController.ModelSceneLoaded();
        target = GameObject.FindObjectOfType<TargetData>();
        mainCamera = Camera.main;
        maxCamera = mainCamera.GetComponent<maxCamera>();
        target.cameraBoundaries.gameObject.SetActive(true);
        target.cameraBoundaries.enabled = true;

        if (modelScene && modelScene.background)
        {
#if AC_URP
            var urp = mainCamera.GetUniversalAdditionalCameraData();
            if (urp != null)
            {
                urp.renderType = CameraRenderType.Overlay;
                backgroundImageCamera.gameObject.SetActive(true);
                urp = backgroundImageCamera.GetUniversalAdditionalCameraData();
                urp.cameraStack.RemoveAt(0);
                urp.cameraStack.Add(mainCamera);
            }
#else       
            mainCamera.clearFlags = CameraClearFlags.Depth;
#endif
        }

        yield return new WaitForFixedUpdate();
        maxCamera.bounds = target.cameraBoundaries.bounds;
        target.cameraBoundaries.gameObject.SetActive(false);
        yield return _controller.Start();

        if (FunctionsObject.currentLoadingScreen != null)
        {
            GameObject.Destroy(FunctionsObject.currentLoadingScreen);
            FunctionsObject.currentLoadingScreen = null;
        }

        isInitialized = true;
    }// method

    private void Update()
    {
        if (_controller != null) _controller.Update();
        if (cameraLock != null && maxCamera != null)
        {
            if (maxCamera.gameObject.activeSelf == cameraLock.activeSelf) cameraLock.SetActive(!maxCamera.gameObject.activeSelf);
        }

        if (target != null && target.audioSources != null)
        {
            foreach(var a in target.audioSources)
            {
                if (a != null) a.volume = mainAudioSource.volume;
            }
        }
    }
    public void AddLabel(Transform labelPosition, Transform target, string title)
    {
        var lb = labelPosition.gameObject.AddComponent<CallOutLabel>();
        lb.target = target;
        lb.labelBackground = labelBackground;
        lb.labelPoint_lb = labelPoint_lb;
        lb.labelPoint_part = labelPoint_part;
        lb.labelLine_bg = labelLine_bg;
        lb.textStyle = labelTextStyle.label;
        lb.labelStyle = labelStyle;
        lb.isOn = false;
        lb.SetContent(title);
    }
    
    public void ToggleAutoPlay()
    {
        _controller.ToggleAutoPlay();
    }

    public void LoadMainMenuScene()
    {
        if (functionsObject != null)
        {
            functionsObject.LoadMainMenu();
        }
        else
        {
            SceneManager.LoadScene("00 MainMenu");
        }
    }// method

    public void ShowDescription(string title, string content)
    {
        bool showBottomSection = this.bottomSection.alpha == 0;
        this.titleText.text = title;
        this.contentText.text = content;
        this.bottomSection.interactable = true;
        this.bottomSection.alpha = 1;
        this.bottomSection.blocksRaycasts = true;

        if (showBottomSection)
        {
            this.bottomSection.GetComponent<UIView>().Show();
        }
    }// method

    private LearningPartRef _waitingObject = null;
    public void SetSelectedItem(LearningPartRef partRef)
    {
        if (partRef.showWarningMessage)
        {
            warningMessageContent.text = partRef.warningMessage;
            warningMessage.Show();
            _waitingObject = partRef;
            return;
        }
        
        _controller.SetSelectedItem(partRef);
    }

    public void WarningMessage_Ok_Clicked()
    {
        warningMessage.Hide();
        _controller.SetSelectedItem(_waitingObject);
        _waitingObject = null;
    }// method

    public void SetLayerAllChildren(Transform root, int layer)
    {
        root.gameObject.layer = layer;
        var children = root.GetComponentsInChildren<Transform>(includeInactive: true);
        foreach (var child in children)
        {
            child.gameObject.layer = layer;
        }
    }

    public void NextPage()
    {
        _controller.NextPage();
    }// method

    public void PrevPage()
    {
        _controller.PrevPage();
    }// method

    public void PlayCurrentPage(int pgIndex)
    {
        _controller.PlayCurrentPage(pgIndex);
    }// method

    public void Log(string msg)
    {
        Debug.Log(msg);
    }

    public void SetCameraState(bool isOn)
    {
        maxCamera.isOn = isOn;
    }// method

    public void ExitApplication()
    {
        Application.Quit();
    }

    public void SetLabelOnOrOff(bool isOn)
    {
        _controller.SetLabelOnOrOff(isOn);
    }

    public void DisableTargetObjects(bool isOn)
    {
        _controller.DisableTargetObjects(isOn);
    }

    public void ToggleViews()
    {
        if (views.Length > 0)
        {
            bool isHidden = views[0].IsHidden;
            int len = views.Length;
            for (int i = 0; i < len; ++i)
            {
                if (isHidden)
                {
                    if (views[i].tag == "HiddenIfNoneItemSelected" && !_controller.IsItemSelected)
                    {
                        continue;
                    }
                    views[i].Show();
                }
                else
                {
                    views[i].Hide();
                }
            }
        }
    }// method

    public void RepeatCurrentPage()
    {
        PlayCurrentPage(0);
    }// method

    private IEnumerator _AutoPlay()
    {
        yield return _controller._AutoPlay();
    }// method

    private Vector3 FindNextWayPoint(Vector3 cp, Vector3 endPos)
    {
        float longDistance = Vector3.Distance(cp, endPos);
        float distance = longDistance;
        Vector3 nextPos = endPos;
        for (int i = 0; i < target.cameraNavigationPoints.Length; ++i)
        {
            if (cp == target.cameraNavigationPoints[i].position)
                continue;
            float d = Vector3.Distance(cp, target.cameraNavigationPoints[i].position);
            if (d < distance)
            {
                float ld = Vector3.Distance(target.cameraNavigationPoints[i].position, endPos);
                if (ld < longDistance)
                {
                    distance = d;
                    nextPos = target.cameraNavigationPoints[i].position;
                    longDistance = ld;
                }
            }
        }

        return nextPos;
    }// method

#if UNITY_EDITOR

    private List<Vector3> _lastPath = new List<Vector3>();
    void OnDrawGizmos()
    {
        if (_lastPath != null)
        {
            var origColor = Gizmos.color;
            Gizmos.color = Color.red;

            for (int i = 1; i < _lastPath.Count; i++)
            {
                Gizmos.DrawLine(_lastPath[i - 1], _lastPath[i]);
            }

            Gizmos.color = origColor;
        }
    }

#endif

#if DEVELOPMENT_BUILD || UNITY_EDITOR
    private void OnGUI()
    {
        if (Camera.main)
        {
            var boxStyle = GUI.skin.box;
            var origFontSize = boxStyle.fontSize;
            boxStyle.fontSize = 18;
            var content = new GUIContent($"ScreenSize = {Screen.width},{Screen.height}\n" +
                $"SceneSize = {Camera.main.pixelWidth},{Camera.main.pixelHeight}");
            var contentSize = boxStyle.CalcSize(content);
            GUI.Box(new Rect(((float)Screen.width - contentSize.x) / 2.0f, 10, contentSize.x, contentSize.y), content);

            boxStyle.fontSize = origFontSize;
        }
    }// method
#endif

    IEnumerator PointsNavigations()
    {
        var tr = mainCamera.transform;
        maxCamera.enabled = false;
        yield return tr.DOLookAt(_controller.SelectedItem.transform.position, 1, AxisConstraint.None/*, Vector3.zero*/).WaitForCompletion();
        var a = tr.position;
        var b = _controller.SelectedItem.camPosition;
        Vector3 nextPos = Vector3.zero;
        float camsp = camMoveSpeed;
        if (_controller.SelectedItem.overrideCameraMovementSpeed && _controller.SelectedItem.cameraMovementSpeed > 0) camsp = _controller.SelectedItem.cameraMovementSpeed;
#if UNITY_EDITOR
        _lastPath = new List<Vector3>();
        _lastPath.Add(a);
#endif
        while(true)
        {
            nextPos = FindNextWayPoint(tr.position, b);
#if UNITY_EDITOR
            _lastPath.Add(nextPos);
#endif
            yield return tr.DOMove(nextPos, camsp).SetSpeedBased(!target.cameraMovementTimeBased).OnUpdate(()=> { tr.LookAt(_controller.SelectedItem.transform); }).WaitForCompletion();
            Ray ray = new Ray(tr.position, b - transform.position);
            RaycastHit hit;
            bool isHit = Physics.Raycast(ray, out hit);
            if (!isHit)
            {
#if UNITY_EDITOR
                _lastPath.Add(b);
#endif
                yield return tr.DOMove(b, camsp).SetSpeedBased(!target.cameraMovementTimeBased).OnUpdate(() => { tr.LookAt(_controller.SelectedItem.transform); }).WaitForCompletion();
            }
            //yield return tr.DOLookAt(_controller.SelectedItem.transform.position, 1, AxisConstraint.None/*, Vector3.zero*/).WaitForCompletion();
            //tr.LookAt(_controller.SelectedItem.transform);
            if (b == tr.position) break;
        }
        
        maxCamera.SetTarget(_controller.SelectedItem.transform);
        maxCamera.Init();
        maxCamera.enabled = true;
        tr.position = b;
    }// method

    IEnumerator CameraFocusOnObject()
    {
        if (_controller.SelectedItem != null)
        {
            if (target.cameraNavigationPoints != null && target.cameraNavigationPoints.Length > 0 && !_controller.SelectedItem.ignoreCameraNavigationPoints) yield return PointsNavigations();
            else yield return DirectNavigation();
        }
    }

    IEnumerator DirectNavigation()
    {
        var tr = mainCamera.transform;
        maxCamera.enabled = false;
        var a = tr.position;
        var b = _controller.SelectedItem.camPosition;
        
        float camsp = camMoveSpeed;
        if (_controller.SelectedItem.overrideCameraMovementSpeed && _controller.SelectedItem.cameraMovementSpeed > 0) camsp = _controller.SelectedItem.cameraMovementSpeed;

        yield return tr.DOLookAt(_controller.SelectedItem.transform.position, 0.5f, AxisConstraint.None/*, Vector3.zero*/).WaitForCompletion();
        yield return tr.DOMove(b, camsp).SetSpeedBased(!target.cameraMovementTimeBased).OnUpdate(() => 
        { 
            tr.LookAt(_controller.SelectedItem.transform); 
        }).WaitForCompletion();

        maxCamera.SetTarget(_controller.SelectedItem.transform);
        maxCamera.Init();
        maxCamera.enabled = true;
        tr.position = b;
    }

    //IEnumerator DirectNavigation()
    //{
    //    var tr = mainCamera.transform;
    //    maxCamera.enabled = false;
    //    var a = tr.position;
    //    var b = _controller.SelectedItem.camPosition;
    //    //Vector3 direction = a - tr.position;
    //    //yield return tr.DOMove(b, camMoveSpeed / Vector3.Distance(a, b)).WaitForCompletion();

    //    float camsp = camMoveSpeed;
    //    if (_controller.SelectedItem.overrideCameraMovementSpeed && _controller.SelectedItem.cameraMovementSpeed > 0) camsp = _controller.SelectedItem.cameraMovementSpeed;

    //    //if (b != a)
    //    {
    //        yield return tr.DOLookAt(_controller.SelectedItem.transform.position, 1, AxisConstraint.None/*, Vector3.zero*/).WaitForCompletion();
    //        float step = (camsp / (a - b).magnitude) * Time.fixedDeltaTime;
    //        float t = 0;
    //        Vector3 direction = _controller.SelectedItem.transform.position - a;
    //        //Quaternion toRotation = Quaternion.FromToRotation(tr.forward, direction);
    //        //Quaternion fromRotation = tr.rotation;
    //        while (t <= 1.0f)
    //        {
    //            t += step; // Goes from 0 to 1, incrementing by step each time
    //            tr.position = Vector3.Lerp(a, b, t); // Move objectToMove closer to b
    //            tr.LookAt(_controller.SelectedItem.transform);
    //            //tr.rotation = Quaternion.Lerp(fromRotation, toRotation, t);
    //            yield return new WaitForFixedUpdate();         // Leave the routine and return here in the next frame
    //        }
    //    }
    //    maxCamera.SetTarget(_controller.SelectedItem.transform);
    //    maxCamera.Init();
    //    maxCamera.enabled = true;
    //    tr.position = b;
    //}
    
}

[System.Serializable]
public enum LevelType
{
    Learning,
    Assemblage,
    Disassemble,
    Electric,
    Mechanic,
    Sah7n,
    Tafregh,
    Problem
}


[System.Serializable]
public enum ControllerType
{
    LearningController,
    AnimationController,
    ProblemController
}