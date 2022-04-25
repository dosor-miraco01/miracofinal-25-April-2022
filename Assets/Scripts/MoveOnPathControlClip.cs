using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class MoveOnPathControlClip : PlayableAsset, ITimelineClipAsset
{
    [SerializeField]
    private MoveOnPathControlBehaviour data = new MoveOnPathControlBehaviour();

    public ClipCaps clipCaps => ClipCaps.None;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {   
        return ScriptPlayable<MoveOnPathControlBehaviour>.Create(graph, data);
    }
}