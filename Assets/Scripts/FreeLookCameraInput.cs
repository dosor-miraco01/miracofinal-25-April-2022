using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CinemachineFreeLook))]
public class FreeLookCameraInput : MonoBehaviour
{
    private CinemachineFreeLook freeLookCamera;

    // Start is called before the first frame update
    void Start()
    {
        freeLookCamera = GetComponent<CinemachineFreeLook>();
        freeLookCamera.m_XAxis.m_InputAxisName = "";
        freeLookCamera.m_YAxis.m_InputAxisName = "";
    }

    // Update is called once per frame
    void Update()
    {
#if ANDROID

#else   
        if (Input.GetMouseButton(0))
        {
            freeLookCamera.m_XAxis.m_InputAxisValue = Input.GetAxis("Mouse X");
            freeLookCamera.m_YAxis.m_InputAxisValue = Input.GetAxis("Mouse Y");
        }
        else
        {
            freeLookCamera.m_XAxis.m_InputAxisValue = freeLookCamera.m_YAxis.m_InputAxisValue = 0;
        }
#endif
    }
}
