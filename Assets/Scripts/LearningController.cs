using Doozy.Engine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class LearningController : IGameController
{
    private LearningPartRef[] learningObjects = null;
    private List<LearningObjectItemView> _objectItemsView = new List<LearningObjectItemView>();

    private LearningPartRef selectedObject = null;
    private int _pgIndex = 0;
    
    private bool _labelIsOn = false;
    private bool _isAutoPlayRuning = false;
    private MainController _con = null;
    private PlayableDirector _currentSelectedTimeline = null;
    public LearningPartRef SelectedItem => selectedObject;
    public bool IsItemSelected => selectedObject != null;

    public LearningController(MainController mainController)
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
        for (int i = 0; i < learningObjects.Length; ++i)
        {
            GameObject obj = GameObject.Instantiate(_con.objectItemPrefab);
            obj.GetComponent<Transform>().SetParent(_con.objectsParent);
            var item = obj.GetComponent<LearningObjectItemView>();
            item.onItemSelected += Item_onItemSelected;
            item.SetData(learningObjects[i]);
            learningObjects[i].onObjectClicked += MainController_onObjectClicked;
            _con.AddLabel(learningObjects[i].labelPosition, learningObjects[i].transform, learningObjects[i].data.title);
            if (learningObjects[i].internalParts != null)
            {
                foreach (var internalItem in learningObjects[i].internalParts)
                {
                    if (internalItem == null || internalItem.labelPosition == null || string.IsNullOrEmpty(internalItem.title)) continue;
                    _con.AddLabel(internalItem.labelPosition, internalItem.transform, internalItem.title);
                }
            }
            _objectItemsView.Add(item);
            obj.SetActive(true);
        }

        yield return null;
    }// method

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

    public void Update()
    {

    }

    public void StopAutoPlay(bool force = false)
    {
        // -Medhat
        if (force || _con.autoPlayToggle.isOn)
        {
            _con.autoPlayToggle.isOn = false    ;
            _isAutoPlayRuning = false           ;
            _con.StopCoroutine("_AutoPlay")     ;
            _con.mainAudioSource.Stop()         ;
            //needs to be tested
            //PlayCurrentPage(0);
            //PlayCurrentPage(_pgIndex - 1);
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
        for (int j = objIndex; j < learningObjects.Length; ++j)
        {
            //Debug.Log($"objectIndex: {j}", learningObjects[j]);
            _objectItemsView[j].SetToggle(true);
            SetSelectedItem(learningObjects[j]);
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < selectedObject.data.pages.Length; i++)
            {
                //Debug.Log($"pageIndex: {i} of { selectedObject.data.pages.Length}");
                PlayCurrentPage(i);
                yield return new WaitForFixedUpdate();
                yield return new WaitForSeconds(selectedObject.data.pages[i].sound.length);
            }
        }
        _isAutoPlayRuning = false;
        _con.autoPlayToggle.isOn = false;
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
        _con.objectItemsToggleGroup.allowSwitchOff = false;
        _con.StopCoroutine(WaitForSoundFinished(_con.NextPage));
        if (selectedObject != null)
        {
            _con.SetLayerAllChildren(selectedObject.transform, LayerMask.NameToLayer("Default"));
            selectedObject.cameraObject.SetActive(false);
            _SetLabelOnOrOff(false);
            _DeactivateTargets(true);
        }
        _con.StopCoroutine("CameraFocusOnObject");
        _con.SetLayerAllChildren(partRef.transform, LayerMask.NameToLayer("RenderParts"));
        partRef.cameraObject.SetActive(true);
        bool showBottomSection =_con.bottomSection.alpha == 0;
        _con.bottomSection.interactable = true;
        _con.bottomSection.alpha = 1;
        _con.bottomSection.blocksRaycasts = true;
        if (showBottomSection)
        {
            _con.bottomSection.GetComponent<UIView>().Show();
        }
        selectedObject = partRef;
        _objectItemsView[System.Array.IndexOf(learningObjects, partRef)].SetIconAlpha(1.0f);

        //titleText.text = partRef.data.title;
        _con.titleText.text = string.IsNullOrEmpty(partRef.data.longTitle) ? partRef.data.title : partRef.data.longTitle;
        _con.transparentController.SetTargets(partRef.objectsToTransparent, partRef.transparentObjects);
        _con.transparentController.SetTransparentValue(0.5f);

        _con.deactivateToggle.interactable = System.Array.IndexOf(_con.target.bodyObjects, partRef.gameObject) == -1;
        if (!_con.deactivateToggle.interactable) _con.deactivateToggle.isOn = false;// partRef.gameObject.SetActive(true);
        //SetTransparentValue(0.5f);

        //transparentSlider.value = transparent;
        _SetLabelOnOrOff(_labelIsOn);
        _DeactivateTargets(false);
        _con.StartCoroutine("CameraFocusOnObject");
        //if (!autoPlayToggle.isOn)
        {
            PlayCurrentPage(0);
        }
        //else
        //{
        //    autoPlayToggle.isOn = false;

        //}

        for (int i = 0; i < partRef.timelines.Length; ++i)
        {
            var tm = partRef.timelines[i];
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

            //tm.stopped += Timeline_stopped;
        }
        
        //partRef.timeline.playableGraph..GetGenericBinding(SetReferenceValue(new PropertyName("Audio Track"), _con.mainAudioSource);
        PlayTimeline(0);
    }// method

    private void PlayTimeline(int index)
    {
        if (_currentSelectedTimeline != null) _currentSelectedTimeline.Stop();

        PlayableDirector tm = null;
        if (index >= 0 && index < selectedObject.timelines.Length)
        {
            tm = selectedObject.timelines[index];
        }

        if (tm != null)
        {   
            tm.time = 0;
            tm.Play();
            TimelineSliderController.CurrentTimeLine = tm;
            _currentSelectedTimeline = tm;
        }
    }// method

    public void NextPage()
    {
        StopAutoPlay();
        PlayCurrentPage(_pgIndex + 1);
    }// method

    public void PrevPage()
    {
        StopAutoPlay();
        PlayCurrentPage(_pgIndex - 1);
    }// method

    private Coroutine _lastCoroutine = null;
    public void PlayCurrentPage(int pgIndex)
    {
        if (pgIndex >= 0 && pgIndex < PagesCount)
        {
            _con.CancelInvoke();
            if (_lastCoroutine != null)
            {
                _con.StopCoroutine(_lastCoroutine);
                _lastCoroutine = null;
            }
            _pgIndex = pgIndex;
            var pgData = selectedObject.data.pages[pgIndex];
            _con.mainAudioSource.clip = pgData.sound;
            _con.mainAudioSource.time = 0;
            _con.mainAudioSource.Play();
            _con.contentText.text = pgData.content;
            //_con.Invoke("NextPage", pgData.sound.length + 1);
            // Medhat Removed !
            if (_isAutoPlayRuning)
            _lastCoroutine = _con.StartCoroutine(WaitForSoundFinished(_con.NextPage));
        }
        _con.nextPageButton.interactable = _pgIndex >= 0 && _pgIndex < PagesCount - 1;
        _con.prevPageButton.interactable = _pgIndex > 0 && _pgIndex < PagesCount;
    }// method

    public IEnumerator WaitForSoundFinished(System.Action callback)
    {
        var pso = selectedObject;
        yield return new WaitForEndOfFrame();
        var currentClip = _con.mainAudioSource.clip;
        if (currentClip && _con.mainAudioSource.isPlaying)
        {
            yield return new WaitUntil(() => (!_con.mainAudioSource.isPlaying && 
            (_con.mainAudioSource.time == currentClip.length || _con.mainAudioSource.time == 0)) 
            || currentClip != _con.mainAudioSource.clip);
        }

        if (selectedObject == pso && callback != null)
        {
            callback();
        }
        yield return null;
    }// method

    public void SetLabelOnOrOff(bool isOn)
    {
        _labelIsOn = isOn;
        _SetLabelOnOrOff(isOn);
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
        if (selectedObject == null) return;
        selectedObject.labelPosition.GetComponent<CallOutLabel>().isOn = isOn;
        if (selectedObject.internalParts != null)
        {
            foreach (var internalItem in selectedObject.internalParts)
            {
                if (internalItem == null || internalItem.labelPosition == null || string.IsNullOrEmpty(internalItem.title)) continue;
                internalItem.labelPosition.GetComponent<CallOutLabel>().isOn = isOn;
            }
        }
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
