using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLookAt : MonoBehaviour
{
    public Transform target;
    public float duration = 1;
    public AxisConstraint constraints = AxisConstraint.None;
    public Vector3 upDirection = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartLookAt()
    {
        transform.DOLookAt(target.position, duration, constraints, upDirection).OnStart(()=> { Debug.Log("Tween started"); });
    }
}
