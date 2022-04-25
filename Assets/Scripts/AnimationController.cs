using Doozy.Engine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Playables;

public class AnimationController : IGameController
{
    private LearningPartRef[] learningObjects = null;
    private List<LearningObjectItemView> _objectItemsView = new List<LearningObjectItemView>();

    private LearningPartRef selectedObject = null;
    private int _pgIndex = 0;

    private bool _labelIsOn = false;
    private bool _isAutoPlayRuning = false;
    private MainController _con = null;

    public LearningPartRef SelectedItem => selectedObject;
    public bool IsItemSelected => selectedObject != null;
    private TimeLineDataRef _currentSelectedTimeline = null;
    private ActivateObjects[] activateObjects;
    private Dictionary<Transform, SavedTransform> _savedTrans = new Dictionary<Transform, SavedTransform>();
    public AnimationController(MainController mainController)
    {
        _con = mainController;
    }

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
        learningObjects = _con.target.GetComponentsInChildren<LearningPartRef>();
        System.Array.Sort(learningObjects, (x, y) => x.data.orderNumber.CompareTo(y.data.orderNumber));
        activateObjects = GameObject.FindObjectsOfType<ActivateObjects>();

        foreach(var ao in activateObjects)
        {
            if (ao.items != null && ao.items.Count > 0)
            {
                ao.items.Sort((x, y) => x.orderNumber.CompareTo(y.orderNumber));
            }
        }
        //if (_con.levelType != LevelType.Disassemble)
        {
            for (int i = 0; i < learningObjects.Length; ++i)
            {
                GameObject obj = GameObject.Instantiate(_con.objectItemPrefab);
                obj.GetComponent<Transform>().SetParent(_con.objectsParent);
                var item = obj.GetComponent<LearningObjectItemView>();
                item.onItemSelected += Item_onItemSelected;
                item.SetData(learningObjects[i]);
                learningObjects[i].onObjectClicked += MainController_onObjectClicked;
                
                _objectItemsView.Add(item);
                obj.SetActive(true);
            }
        }

        //GameObject pp = new GameObject("_PP");
        //pp.transform.position = Vector3.zero;
        //pp.SetActive(false);
        //int si = 0;
        foreach(var tr in _con.target.preserveTransforms)
        {
            SavePPRescursive(tr);
        }
        //_con.target.preserveTransforms
        yield return null;
    }// method

    private void SavePPRescursive(Transform tr)
    {
        //GameObject nop = new GameObject($"{index++}");
        //nop.transform.SetParent(parent);
        //nop.transform.position = tr.position;
        //nop.transform.rotation = tr.rotation;
        //nop.transform.localScale = tr.localScale;
        //_savedTrans[tr] = nop.transform;
        _savedTrans[tr] = SavedTransform.Create(tr);

        for (int i = 0; i < tr.childCount; ++i)
            SavePPRescursive(tr.GetChild(i));
    }// method


    void RestoreSavedTrans()
    {
        foreach(var nop in _savedTrans)
        {
            nop.Key.gameObject.SetActive(nop.Value.isActive);
            nop.Key.position = nop.Value.position;
            nop.Key.rotation = nop.Value.rotation;
            nop.Key.localScale = nop.Value.localScale;
        }
    }

    public void ToggleAutoPlay()
    {
        if (_con.autoPlayToggle.isOn)
        {
            _con.StartCoroutine("_AutoPlay");
        }
        else
        {
            StopAutoPlay(true);
            //mainAudioSource.Pause();
        }
    }// method

    public void StopAutoPlay(bool force = false)
    {


        if (force || _con.autoPlayToggle.isOn)
        {
            _con.autoPlayToggle.isOn = false;
            _isAutoPlayRuning = false;
            _con.StopCoroutine("_AutoPlay");
            _con.mainAudioSource.Stop();

        }
    }// method

    public IEnumerator _AutoPlay()
    {
        _isAutoPlayRuning = true;
        if (selectedObject == null)
        {
            selectedObject = learningObjects[0];
        }

        int objIndex = System.Array.IndexOf(learningObjects, selectedObject);
        //for (int j = objIndex; j < learningObjects.Length; ++j)
        //{
            _objectItemsView[objIndex].SetToggle(true);
            SetSelectedItem(learningObjects[objIndex]);
            //    yield return new WaitForEndOfFrame();
            //    for (int i = 0; i < selectedObject.data.pages.Length; i++)
            //    {
            //        PlayCurrentPage(i);
            //        yield return new WaitForFixedUpdate();
            //        yield return new WaitForSeconds(selectedObject.data.pages[i].sound.length);
            //    }
            //}
            //_isAutoPlayRuning = false;
            //_con.autoPlayToggle.isOn = false;
            yield return null;

    }// method

    private void MainController_onObjectClicked(LearningPartRef obj)
    {
        if (obj == selectedObject) return;
        int index = System.Array.IndexOf(learningObjects, obj);
        _objectItemsView[index].SetToggle(true);
        _con.SetSelectedItem(obj);
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
        if (selectedObject != null && selectedObject.timelines.Count(x=>x.state == UnityEngine.Playables.PlayState.Playing) != 0)
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

        if (selectedObject != null)
        {
            foreach (var tm in selectedObject.timelines)
            {
                tm.stopped -= Timeline_stopped;
                tm.time = tm.duration - 0.1f;
            }
            
            if (_con.timelinesParent != null)
            {
                for (int i = _con.timelinesParent.childCount-1; i >= 0; --i)
                {
                    GameObject.Destroy(_con.timelinesParent.GetChild(i).gameObject);
                }
            }

            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(0.1f);

            foreach (var tm in selectedObject.timelines)
            {
                tm.Pause();
            }

            RestoreSavedTrans();
        }

        if (activateObjects != null && activateObjects.Length > 0 && partRef != null)
        {
            int index = System.Array.IndexOf(learningObjects, partRef);
            if (index >= 0)
            {
                foreach(var ao in activateObjects)
                {
                    if (ao != null) ao.Activate(index);
                }
            }
        }

        if (partRef.allowSideImage && partRef.sideImage != null)
        {
            _con.sideImage.sprite = partRef.sideImage;
            _con.sideImage.transform.parent.gameObject.SetActive(true);
            var uiView = _con.sideImage.GetComponentInParent<UIView>();

            if (uiView != null)
            {
                int index = System.Array.IndexOf(_con.views, uiView);
                if (index == -1)
                {
                    List<UIView> uiViews = new List<UIView>(_con.views);
                    uiViews.Insert(uiViews.Count - 1, uiView);
                    _con.views = uiViews.ToArray();
                }

                uiView.Show();
            }
        }
        else
        {
            if (_con.sideImage != null)
            {
                _con.sideImage.transform.parent.gameObject.SetActive(false);
            }
        }

        _con.StopCoroutine("CameraFocusOnObject");

        bool showBottomSection = _con.bottomSection.alpha == 0;
        _con.bottomSection.interactable = true;
        _con.bottomSection.alpha = 1;
        _con.bottomSection.blocksRaycasts = true;

        selectedObject = partRef;
        _objectItemsView[System.Array.IndexOf(learningObjects, partRef)].SetIconAlpha(1.0f);

        //titleText.text = partRef.data.title;
        _con.titleText.text = string.IsNullOrEmpty(partRef.data.longTitle) ? partRef.data.title : partRef.data.longTitle;
        _con.contentText.text = "";
        _con.transparentController.SetTargets(partRef.objectsToTransparent, partRef.transparentObjects);
        _con.transparentController.SetTransparentValue(0.5f);
        
        _con.deactivateToggle.interactable = System.Array.IndexOf(_con.target.bodyObjects, partRef.gameObject) == -1;
        if (!_con.deactivateToggle.interactable) _con.deactivateToggle.isOn = false;// partRef.gameObject.SetActive(true);
        //SetTransparentValue(0.5f);
        
        //transparentSlider.value = transparent;
        _SetLabelOnOrOff(_labelIsOn);
        _DeactivateTargets(false);
        yield return _con.StartCoroutine("CameraFocusOnObject");
        if (showBottomSection)
        {
            _con.bottomSection.GetComponent<UIView>().Show();
        }
        
        int tIndex = 0;
        //foreach (var tm in partRef.timelines)
        for(int i = 0; i < partRef.timelines.Length; ++i)
        {
            var tm = partRef.timelines[i];
            var tgObj = GameObject.Instantiate(_con.timelinePrefab, _con.timelinesParent);

            var tdRef = tgObj.GetComponent<TimeLineDataRef>();
            tdRef.timeline = tm;
            tdRef.onClicked += TdRef_onClicked;
            if (!partRef.skipTimelines || (partRef.skipTimelines && partRef.skipTimelineFrom <= i))
            {
                tdRef.text.text = ((tIndex++) + 1).ToString();
                tgObj.SetActive(partRef.timelines.Length > 1);
            }
            if (i == partRef.timelines.Length-1)
            {
                tdRef.icon.gameObject.SetActive(true);
                tdRef.icon.sprite = partRef.data.icon;
                tdRef.text.gameObject.SetActive(false);
            }
            tm.time = 0;
            var timelineAsset = tm.playableAsset as UnityEngine.Timeline.TimelineAsset;

            var trackList = timelineAsset.GetOutputTracks();

            foreach (var track in trackList)
            {
                // check to see if this is the one you are looking for (by name, index etc)
                if (track is UnityEngine.Timeline.AudioTrack)
                {
                    // bind the track to our new actor instance
                    tm.SetGenericBinding(track, _con.mainAudioSource);
                }
            }

            tm.stopped += Timeline_stopped;
        }

        //partRef.timeline.playableGraph..GetGenericBinding(SetReferenceValue(new PropertyName("Audio Track"), _con.mainAudioSource);
        PlayTimeline(0);
        yield return null;
    }// method

    private void PlayTimeline(int index)
    {
        if (_con == null || _con.timelinesParent == null) return;
        if (index >= 0 && index < _con.timelinesParent.childCount)
        {
            PlayTimeline(_con.timelinesParent.GetChild(index).GetComponent<TimeLineDataRef>());
        }
    }// method

    private void PlayTimeline(TimeLineDataRef tm)
    {
        if (_currentSelectedTimeline == tm) return;
        if (_currentSelectedTimeline != null)
        {
            int index = System.Array.IndexOf(SelectedItem.timelines, tm.timeline);
            int prev = System.Array.IndexOf(SelectedItem.timelines, _currentSelectedTimeline.timeline);
            if (index > prev)
            {
                _con.StartCoroutine(this.PauseAfterChange(_currentSelectedTimeline.timeline, _currentSelectedTimeline.timeline.duration));
            }
            else
            {
                _con.StartCoroutine(this.PauseAfterChange(_currentSelectedTimeline.timeline, 0));
            }
            
        }
        if (tm != null)
        {
            tm.toggle.isOn = true;
            tm.timeline.time = 0;
            tm.timeline.Play();
            TimelineSliderController.CurrentTimeLine = tm.timeline;
            _currentSelectedTimeline = tm;
        }
    }// method

    IEnumerator PauseAfterChange(PlayableDirector tm, double seekTime)
    {
        tm.time = seekTime;
        yield return new WaitForEndOfFrame();
        tm.Pause();
        RestoreSavedTrans();
    }// method

    private void TdRef_onClicked(TimeLineDataRef obj)
    {
        PlayTimeline(obj);
    }

    private void Timeline_stopped(UnityEngine.Playables.PlayableDirector obj)
    {   
        PlayTimeline(System.Array.IndexOf(SelectedItem.timelines, obj) + 1);
    }

    public void NextPage()
    { 
    }// method

    public void PrevPage()
    {   
    }// method

    public void PlayCurrentPage(int pgIndex)
    {
        if (SelectedItem != null && _currentSelectedTimeline != null)
        {
            _currentSelectedTimeline.timeline.time = 0;
            _currentSelectedTimeline.timeline.Play();
        }
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
        //if (selectedObject == null) return;
        //selectedObject.labelPosition.GetComponent<CallOutLabel>().isOn = isOn;
        //if (selectedObject.internalParts != null)
        //{
        //    foreach (var internalItem in selectedObject.internalParts)
        //    {
        //        if (internalItem == null || internalItem.labelPosition == null || string.IsNullOrEmpty(internalItem.title)) continue;
        //        internalItem.labelPosition.GetComponent<CallOutLabel>().isOn = isOn;
        //    }
        //}
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

struct SavedTransform
{
    public bool isActive { get; set; }
    public Vector3 position { get; set; }
    public Vector3 localScale { get; set; }
    public Quaternion rotation { get; set; }
    public Transform Target { get; set; }

    public static SavedTransform Create(Transform tr)
    {
        return new SavedTransform
        {
            isActive = tr.gameObject.activeSelf,
            localScale = tr.localScale,
            position = tr.position,
            rotation = tr.rotation,
            Target = tr
        };
    }
}