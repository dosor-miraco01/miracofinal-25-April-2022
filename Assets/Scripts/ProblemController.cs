using Doozy.Engine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Playables;
using Fungus;

public class ProblemController : IGameController
{
    public static int selectCaseIndex = -1;
    
    private ProblemCaseItemRef[] learningObjects = null;
    private List<LearningObjectItemView> _objectItemsView = new List<LearningObjectItemView>();

    private LearningPartRef selectedObject = null;
    private int _pgIndex = 0;

    //private bool _labelIsOn = false;
    //private bool _isAutoPlayRuning = false;
    private MainController _con = null;

    public LearningPartRef SelectedItem => selectedObject;
    public bool IsItemSelected => selectedObject != null;
    private int problemId = 0;
    //private ProblemCaseView _selectedCase;
    private ProblemCaseStepView _selectedStep;
    private List<ProblemCaseStepView> _stepsViews = new List<ProblemCaseStepView>();
    private Dictionary<Transform, SavedTransform> _savedTrans = new Dictionary<Transform, SavedTransform>();
    //private ProblemReportController _problemReportController;
    public ProblemController(MainController mainController)
    {
        _con = mainController;
        _con.StepCompleted = StepCompleted;
        //_problemReportController = GameObject.FindObjectOfType<ProblemReportController>();
    }

    void StepCompleted(Flowchart stepLogic)
    {
        if (_selectedStep != null)
        {
            _selectedStep.visitedObj.SetActive(true);
            var caseData = (ProblemCaseItemRef)selectedObject;
            int index  = System.Array.IndexOf(caseData.caseData.steps, stepLogic);
            foreach (var step in _stepsViews)
            {
                if (!step.visitedObj.activeSelf) return;
            }
            var caseView = _objectItemsView.SingleOrDefault(x => x.Data == selectedObject);
            caseView.visitedBackground.SetActive(true);
            ProblemReportController.AddSaveItem(new ProblemSaveItem
            {
                CaseIndex = caseData.caseData.ID,
                ProblemId = problemId,
                //StepIndex = index + 1
            });
        }
    }// method

    public int PagesCount
    {
        get
        {
            if (selectedObject == null || selectedObject.data == null || selectedObject.data.pages.Length == 0) return 0;
            return selectedObject.data.pages.Length;
        }
    }

    // Start is called before the first frame update
    public IEnumerator Start()
    {
        //learningObjects = _con.target.GetComponentsInChildren<ProblemItemRef>();
        var items = _con.target.GetComponentsInChildren<ProblemItemRef>();
        ProblemItemRef problemItemRef = null;
        foreach (var it in items)
        {
            if (MainController.selectedProblem != null && it.id == MainController.selectedProblem.problemId)
            {
                problemItemRef = it;
                break;
            }
        }

#if UNITY_EDITOR
        if (problemItemRef == null)
        {
            problemItemRef = items.FirstOrDefault();
        }
#endif

        problemId = problemItemRef.id;
        GameObject cases_obj = new GameObject("_ProblemCases");
        cases_obj.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSaveInEditor;
        _con.problemTitleText.text = problemItemRef.title;
        learningObjects = problemItemRef.cases.Select(x =>
        {
            var pcr = cases_obj.AddComponent<ProblemCaseItemRef>();
            pcr.caseData = x;
            return pcr;
        }).ToArray();
       
        for (int i = 0; i < learningObjects.Length; ++i)
        {
            learningObjects[i].caseData.ID = i + 1;
            learningObjects[i].data = LearningPart.CreateInstance<LearningPart>();
            learningObjects[i].data.title = learningObjects[i].caseData.title;
            //learningObjects[i].data.icon = learningObjects[i].icon;
            //learningObjects[i].data.orderNumber = learningObjects[i].menuOrderNum;
            GameObject obj = GameObject.Instantiate(_con.objectItemPrefab);
            obj.GetComponent<Transform>().SetParent(_con.objectsParent);
            var item = obj.GetComponent<LearningObjectItemView>();
            item.onItemSelected += Item_onItemSelected;
            item.SetData(learningObjects[i]);
            item.text.text = (i + 1).ToString();
            //learningObjects[i].onObjectClicked += MainController_onObjectClicked;
            var svd = ProblemReportController.GetProblem(problemId, i + 1);//ProblemReportController.Saved.SingleOrDefault(x => x.ProblemId == problemId && i + 1 == x.CaseIndex);
            if (svd.ProblemId == problemId)
            {
                item.visitedBackground.SetActive(true);
            }
            _objectItemsView.Add(item);
            obj.SetActive(true);
        }
        
        foreach (var tr in _con.target.preserveTransforms)
        {
            SavePPRescursive(tr);
        }

        if (selectCaseIndex != -1)
        {
            _objectItemsView[selectCaseIndex - 1].SetToggle(true);
            _objectItemsView[selectCaseIndex - 1].OnItemClicked();
            selectCaseIndex = -1;
        }

        yield return null;
    }// method

    private void SavePPRescursive(Transform tr)
    { 
        _savedTrans[tr] = SavedTransform.Create(tr);

        for (int i = 0; i < tr.childCount; ++i)
            SavePPRescursive(tr.GetChild(i));
    }// method


    void RestoreSavedTrans()
    {
        foreach(var nop in _savedTrans)
        {
            nop.Key.gameObject.SetActive(true);
            nop.Key.position = nop.Value.position;
            nop.Key.rotation = nop.Value.rotation;
            nop.Key.localScale = nop.Value.localScale;
        }
    }

    public void ToggleAutoPlay()
    {
        
    }// method

    public void StopAutoPlay(bool force = false)
    {
        
    }// method

    public IEnumerator _AutoPlay()
    {
        yield return null;
    }// method

    private void MainController_onObjectClicked(LearningPartRef obj)
    {
        //if (obj == selectedObject) return;
        //int index = System.Array.IndexOf(learningObjects, obj);
        //_objectItemsView[index].SetToggle(true);
        //_con.SetSelectedItem(obj);
    }

    private void Item_onItemSelected(LearningObjectItemView obj)
    {
        if (obj.Data == selectedObject) return;
        StopAutoPlay();
        _con.SetSelectedItem(obj.Data);
    }

    public void SetSelectedItem(LearningPartRef partRef)
    {
        _con.StartCoroutine(_SetSelectedItem(partRef));
    }

    public void Update()
    {
        if (selectedObject != null && selectedObject.timelines != null && selectedObject.timelines.Count(x=>x.state == UnityEngine.Playables.PlayState.Playing) != 0)
        {
            if (!_con.mainCamera.gameObject.activeSelf)
            {
                var tr = _con.mainCamera.transform;
                var camTr = selectedObject.cameraObject.transform.GetChild(0);
                tr.position = camTr.position;
                tr.rotation = camTr.rotation;
            }
        }
    }

    private IEnumerator _SetSelectedItem(LearningPartRef partRef)
    {
        _con.objectItemsToggleGroup.allowSwitchOff = false;

        //if (selectedObject != null)
        //{   
        //    RestoreSavedTrans();
        //}

        if (_selectedStep != null) _selectedStep.data.StopAllBlocks();

        _con.StopCoroutine("CameraFocusOnObject");
        
        selectedObject = partRef;
        //_objectItemsView[System.Array.IndexOf(learningObjects, partRef)].SetIconAlpha(1.0f);

        //titleText.text = partRef.data.title;
        //_con.titleText.text = string.IsNullOrEmpty(partRef.data.longTitle) ? partRef.data.title : partRef.data.longTitle;
        //_con.contentText.text = "";
        //_con.transparentController.SetTargets(partRef.objectsToTransparent, partRef.transparentObjects);
        //_con.transparentController.SetTransparentValue(0.5f);
        
        _con.deactivateToggle.interactable = System.Array.IndexOf(_con.target.bodyObjects, partRef.gameObject) == -1;
        if (!_con.deactivateToggle.interactable) _con.deactivateToggle.isOn = false;// partRef.gameObject.SetActive(true);
        //SetTransparentValue(0.5f);
        
        //transparentSlider.value = transparent;
        //_SetLabelOnOrOff(_labelIsOn);
        _DeactivateTargets(false);
        //yield return _con.StartCoroutine("CameraFocusOnObject");
        RemoveChilds(_con.problemCaseStepsParent);

        //RemoveChilds(_con.problemCaseStepsParent);
        var cas = (ProblemCaseItemRef)partRef;
        //cas.toggle.isOn = true;
        _stepsViews = new List<ProblemCaseStepView>();
        _con.ShowDescription(cas.caseData.title, "");
        for (int i = 0; i < cas.caseData.steps.Length; ++i)
        {
            var step_ob = GameObject.Instantiate(_con.problemCaseStepPrefab);
            step_ob.transform.SetParent(_con.problemCaseStepsParent);
            step_ob.transform.localScale = Vector3.one;
            var step_v = step_ob.GetComponent<ProblemCaseStepView>();
            _stepsViews.Add(step_v);
            step_v.onClick = OnCaseStepClicked;
            step_v.SetData(cas.caseData.steps[i], (i + 1).ToString());
            step_ob.SetActive(true);
            if (i == 0)
            {
                step_v.OnClicked();
            }
        }

        //RemoveChilds(_con.problemCasesParent);

        //var problem = (ProblemItemRef)partRef;
        //for (int i = 0; i < problem.cases.Length; ++i)
        //{
        //    var case_ob = GameObject.Instantiate(_con.problemCasesPrefab);
        //    case_ob.transform.SetParent(_con.problemCasesParent);
        //    var case_v = case_ob.GetComponent<ProblemCaseView>();
        //    case_v.onClick = OnCaseClicked;
        //    case_v.SetData(problem.cases[i]);
        //    case_ob.SetActive(true);
        //    if (i == 0)
        //    {   
        //        case_v.OnClicked();
        //    }
        //}

        yield return null;
    }// method

    //void OnCaseClicked(ProblemCaseView cas)
    //{
    //    _selectedCase = cas;
    //    RemoveChilds(_con.problemCaseStepsParent);
    //    if (cas != null)
    //    {
    //        cas.toggle.isOn = true;
    //        for (int i = 0; i < cas.data.steps.Length; ++i)
    //        {
    //            var step_ob = GameObject.Instantiate(_con.problemCaseStepPrefab);
    //            step_ob.transform.SetParent(_con.problemCaseStepsParent);
    //            var step_v = step_ob.GetComponent<ProblemCaseStepView>();
    //            step_v.onClick = OnCaseStepClicked;
    //            step_v.SetData(cas.data.steps[i], (i + 1).ToString());
    //            step_ob.SetActive(true);
    //            if (i == 0)
    //            {
    //                step_v.OnClicked();
    //            }
    //        }
    //    }
    //}// method

    void OnCaseStepClicked(ProblemCaseStepView chart)
    {
        if (_selectedStep != null) _selectedStep.data.StopAllBlocks();
        var ynq = GameObject.FindObjectOfType<YesNoQuestionViewController>();
        if (ynq != null) ynq.Close();
        var okm = GameObject.FindObjectOfType<MessageController>();
        if (okm != null) okm.Close();

        RestoreSavedTrans();
        _selectedStep = chart;
        if (chart == null) return;
        chart.toggle.isOn = true;
        chart.data.ExecuteIfHasBlock("Start");
    }// method

    public static void RemoveChilds(Transform tra)
    {
        for (int i = tra.childCount - 1; i >= 0; --i)
        {
            GameObject.Destroy(tra.GetChild(i).gameObject);
        }
    }

    public void NextPage()
    { 
        if (selectedObject != null && selectedObject != null)
        {
            var cs = (ProblemCaseItemRef)selectedObject;
            int i = System.Array.IndexOf(cs.caseData.steps, _selectedStep.data);
            if (i >= 0 && i < cs.caseData.steps.Length - 1) _con.problemCaseStepsParent.GetChild(i + 1).GetComponent<ProblemCaseStepView>().OnClicked();
        }
    }// method

    public void PrevPage()
    {   
    }// method

    public void PlayCurrentPage(int pgIndex)
    {
        
    }// method

    public void SetLabelOnOrOff(bool isOn)
    {
        //_labelIsOn = isOn;
        //_SetLabelOnOrOff(isOn);
    }

    public void DisableTargetObjects(bool isOn)
    {
        if (_con.target != null)
        {
            foreach (var t in _con.target.bodyObjects)
            {
                t.SetActive(!isOn);
            }
        }
    }

    private void _SetLabelOnOrOff(bool isOn)
    {
       
    }// method

    public void RepeatCurrentPage()
    {
        PlayCurrentPage(0);
    }// method

    void _DeactivateTargets(bool isOn)
    {
        if (selectedObject == null || selectedObject.objectsToDeactivate == null) return;
        foreach (GameObject gb in selectedObject.objectsToDeactivate)
        {
            if (gb == null) continue;
            gb.SetActive(isOn);
        }
    }// method
}

public struct ProblemSaveItem
{
    public int ProblemId { get; set; }
    public int CaseIndex { get; set; }
    //public int StepIndex { get; set; }
}