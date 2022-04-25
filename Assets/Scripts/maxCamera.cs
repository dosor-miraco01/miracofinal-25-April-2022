using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Camera-Control/3dsMax Camera Style")]
public class maxCamera : MonoBehaviour
{
    public static bool isOn = false;
    [SerializeField]
    private Transform target;
    public Vector3 targetOffset;
    public float distance = 5.0f;
    public float maxDistance = 20;
    public float minDistance = .6f;
    public float xSpeed = 200.0f;
    public float ySpeed = 200.0f;
    public int yMinLimit = -80;
    public int yMaxLimit = 80;
    public int zoomRate = 40;
    public float panSpeed = 0.3f;
    public float zoomDampening = 5.0f;
    public float mouseSenstive = 0.1f;
    public Bounds bounds;
    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private Vector3 position;
    private Transform shadowTarget;
    
    void Start() { Init(); }
    void OnEnable() { Init(); }

    public void Init()
    {
        //If there is no target, create a temporary target at 'distance' from the cameras current viewpoint
        //if (!target)
        //{
        //    GameObject go = new GameObject("Cam Target");
        //    go.transform.position = transform.position + (transform.forward * distance);
        //    target = go.transform;
        //}
        if (!shadowTarget)
        {
            GameObject shadowObj = new GameObject("_ShadowTarget");
            shadowTarget = shadowObj.GetComponent<Transform>();
            shadowTarget.rotation = Quaternion.identity;
            shadowTarget.position = Vector3.zero;
            shadowTarget.localScale = Vector3.one;
        }

        if (target)
        {
            SetTarget(target);
        }
        distance = Vector3.Distance(transform.position, target.position);
        currentDistance = distance;
        desiredDistance = distance;

        //be sure to grab the current rotations as starting points.
        position = transform.position;
        rotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;

        xDeg = Vector3.Angle(Vector3.right, transform.right);
        yDeg = Vector3.Angle(Vector3.up, transform.up);
    }

    public void SetTarget(Transform tr)
    {
        target = tr;
        shadowTarget.position = tr.position;
    }

    /*
     * Camera logic on LateUpdate to only update after all character movement logic has been handled. 
     */
    void LateUpdate()
    {
        if (GUITools.IsPointerOverUIObject())
        {
            return;
        }
        // If Control and Alt and Middle button? ZOOM!
        if (Input.GetMouseButton(2))
        {
            desiredDistance -= Input.GetAxis("Mouse Y") * Time.deltaTime * zoomRate * 0.125f * Mathf.Abs(desiredDistance);
        }

        // If middle mouse and left alt are selected? ORBIT
        if (Input.GetMouseButton(1))
        {
            xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            if (Mathf.Abs(xDeg) < mouseSenstive)
            {
                xDeg = 0;
            }
            if (Mathf.Abs(yDeg) < mouseSenstive)
            {
                yDeg = 0;
            }
            
            ////////OrbitAngle

            //Clamp the vertical axis for the orbit
            yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
            // set camera rotation 
            desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
            currentRotation = transform.rotation;

            rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
            transform.rotation = rotation;
        }
        // otherwise if middle mouse is selected, we pan by way of transforming the target in screenspace
        else if (Input.GetMouseButton(2))
        {
            //grab the rotation of the camera so we can move in a psuedo local XY space
            shadowTarget.rotation = transform.rotation;
            shadowTarget.Translate(Vector3.right * -Input.GetAxis("Mouse X") * panSpeed);
            shadowTarget.Translate(transform.up * -Input.GetAxis("Mouse Y") * panSpeed, Space.World);

            ClampPosition(bounds, shadowTarget);
        }

        ////////Orbit Position

        // affect the desired Zoom distance if we roll the scrollwheel
        desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
        //clamp the zoom min/max
        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        // For smoothing of the zoom, lerp distance
        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

        // calculate position based on the new currentDistance 
        position = shadowTarget.position - (rotation * Vector3.forward * currentDistance + targetOffset);
        transform.position = position;
        //ClampPosition(bounds, transform);
    }

    static void ClampPosition(Bounds b, Transform tr)
    {
        //Debug.Log(b);
        var sp = tr.position;
        sp.x = Mathf.Clamp(sp.x, b.min.x, b.max.x);
        sp.y = Mathf.Clamp(sp.y, b.min.y, b.max.y);
        sp.z = Mathf.Clamp(sp.z, b.min.z, b.max.z);
        tr.position = sp;
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}