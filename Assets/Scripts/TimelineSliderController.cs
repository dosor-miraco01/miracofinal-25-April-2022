using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class TimelineSliderController : MonoBehaviour
{
    public static PlayableDirector CurrentTimeLine = null;
    public bool hideIfNonPlayableDirector = false;
    //private MainController _con = null;
    //private List<PlayableDirector> _list = new List<PlayableDirector>();
    private Slider _slider = null;
    //private PlayableDirector _current = null;
    private bool _isMouseDown = false;

    // Start is called before the first frame update
    void Start()
    {
        if (hideIfNonPlayableDirector)
        {   
            //GameObject gb = null;
            for (int i = 0; i < transform.childCount; ++i)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        _slider = GetComponent<Slider>();
        _slider.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>(SetValue));
    }

    void SetValue(float v)
    {
        if (_isMouseDown)
        {
            if (CurrentTimeLine != null)
            {
                CurrentTimeLine.time = v * CurrentTimeLine.duration;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (_con == null)
        //{
        //    _con = FindObjectOfType<MainController>();
        //}

        //if (_con != null && _list.Count == 0)
        //{
        //    if (_con.isInitialized && _con.target != null)
        //    {
        //        LearningPartRef[] items = _con.target.GetComponentsInChildren<LearningPartRef>();
        //        if (items != null)
        //        {
        //            foreach (LearningPartRef it in items)
        //            {
        //                foreach (var tm in it.timelines)
        //                {
        //                    if (tm != null)
        //                    {
        //                        _list.Add(tm);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //_list.Sort((x, y) => y.state.CompareTo(x.state));
        //foreach(PlayableDirector tm in _list)
        //{
        //    if (tm.state == PlayState.Playing)
        //    {
        //        _current = tm;
        //        break;
        //    }
        //}

        if (!_isMouseDown)
        {
            if (CurrentTimeLine != null)
            {
                _slider.value = (float)(CurrentTimeLine.time / CurrentTimeLine.duration);
            }
        }

        _slider.interactable = CurrentTimeLine != null;
        
        if (hideIfNonPlayableDirector)
        {
            bool isActivated = CurrentTimeLine != null;
            GameObject gb = null;
            for (int i = 0; i < transform.childCount; ++i)
            {
                gb = transform.GetChild(i).gameObject;
                if (gb.activeSelf != isActivated) gb.gameObject.SetActive(isActivated);
            }
        }
    }// method

    public void OnPointerDown()
    {
        _isMouseDown = true;
        SetValue(_slider.value);
    }// method

    public void OnPointerUp()
    {
        _isMouseDown = false;
    }// method
}
