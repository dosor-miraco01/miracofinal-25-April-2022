using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetData : MonoBehaviour
{
    public bool cameraMovementTimeBased = false;
    //public Renderer[] transparentObjects;
    public GameObject[] bodyObjects;
    public Transform[] preserveTransforms;
    public Collider cameraBoundaries;
    public Transform[] cameraNavigationPoints;
    public AudioSource[] audioSources;
}
