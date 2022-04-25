using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransparentController : MonoBehaviour
{
    public Slider transparentSlider; 
    public TransparentObject[] objects;
    public float transparent = 1.0f;
    private Transform _excludeTarget;
    private TransparentObject[] excludeObjects;
    private TransparentObject[] objectToTransparent;
    public Shader transparentShader;
    private void Awake()
    {
        MainController.ModelSceneLoaded += MainController_ModelSceneLoaded;
        transparentSlider.onValueChanged.AddListener((v) => SetTransparentValue(v));
    }

    private void MainController_ModelSceneLoaded()
    {
        objects = FindObjectsOfType<TransparentObject>();
    }

    public void SetTargets(TransparentObject[] objToTransparent, TransparentObject[] excludes)
    {
        excludeObjects = excludes;
        objectToTransparent = objToTransparent;
    }// method

    public void SetTransparentValue(float newValue)
    {
        //if (newValue != transparent)
        {
            transparent = newValue;
            transparentSlider.value = newValue;
            foreach (var o in objects)
            {
                o.SetValue(newValue);
            }
            if (excludeObjects != null)
            {
                foreach (var o in excludeObjects)
                    o.SetValue(1.0f);
            }
            if (objectToTransparent != null)
            {
                foreach (var o in objectToTransparent)
                    o.SetValue(newValue);
            }
        }
    }// method
}
