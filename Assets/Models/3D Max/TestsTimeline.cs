using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.UI;

public class TestsTimeline : MonoBehaviour
{
    PlayableDirector director;
    TimelineAsset timelineAsset;
    public AnimationClip clip1;
    public AnimationClip clip2;
    public Slider sliderTimeline;
    // Start is called before the first frame update
    void Start()
    {
        director = GetComponent<PlayableDirector>();
        timelineAsset = (TimelineAsset)director.playableAsset;
    }


    public void AddAnimationClip()
    {
        var newTrack = (AnimationTrack)timelineAsset.GetRootTrack(0);

        director.SetGenericBinding(newTrack, gameObject);

        var timelineClip = newTrack.CreateClip(clip1);

        var timelineClip2 = newTrack.CreateClip(clip2);

        director.RebuildGraph();
    }



    public void OnChangeTimeline()
    {
        var value = sliderTimeline.value;
        director.time = value;
        director.RebuildGraph();
        director.Evaluate();

    }
}
