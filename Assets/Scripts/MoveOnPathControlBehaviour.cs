using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using DG.Tweening;
using System.Linq;
using Sirenix.OdinInspector;

[System.Serializable]
public class MoveOnPathControlBehaviour : PlayableBehaviour
{
    [SerializeField]
    private Transform pathFromChilds;

    [Button("Calc Path")]
    private void CalcPath()
    {
        if (pathFromChilds != null)
        {
            List<Vector3> points = new List<Vector3>();
            for (int i = 0; i < pathFromChilds.childCount; ++i)
            {
                if (pathFromChilds.GetChild(i) == null) continue;
                points.Add(pathFromChilds.GetChild(i).position);
            }
            path = points.ToArray();
        }
    }// method

    [SerializeField, ReadOnly]
    private Vector3[] path;
    
    private Transform target;
    private Tween _tween = null;
    private bool _isEnter = false;
    private Vector3 _orig_pos = Vector3.zero;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        target = playerData as Transform;
        if (target == null || path == null || path.Count(x => x != null) < 2) return;
        if (!_isEnter)
        {
            _orig_pos = target.position;
            _isEnter = true;
        }
        var stepSize = playable.GetDuration() / (path.Length - 1);
        int stepIndex = 0;
        for (int i = 0; i < path.Length; ++i)
        {
            if (playable.GetTime() > ((double)i) * stepSize)
            {
                stepIndex = i;
                continue;
            }
            break;
        }
        //var stepIndex = (int)(playable.GetTime() / stepSize);
        var t = (playable.GetTime() - (stepIndex * stepSize)) / stepSize; //% stepSize;
        t = Mathf.Abs((float)t);
        //var t = (float)(playable.GetTime() / playable.GetDuration());
        target.position = Vector3.Lerp(path[stepIndex], path[Mathf.Clamp(stepIndex + 1, 0, path.Length - 1)], (float)t);
        //_tween = target.DOPath(path.Select(x => x.position).ToArray(), (float)playable.GetDuration());
        //Debug.Log($"duration: {playable.GetDuration()}, time: {playable.GetTime()},stepSize: {stepSize}, stepIndex: {stepIndex}, t: {t}, to: {Mathf.Clamp(stepIndex + 1, 0, path.Length - 1)}");
        
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {   
        if (_isEnter)
        {
            if (target != null)
            {
                //target.position = _orig_pos;
                //_tween.Pause();
                _isEnter = false;
            }
        }
        base.OnBehaviourPause(playable, info);
    }
}