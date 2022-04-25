using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogicSceneCaller : MonoBehaviour
{
    public FunctionsObject functionsObject;

    [ShowIf("functionsObject")]
    public ControllerType controllerType = ControllerType.AnimationController;

    [ShowIf("functionsObject")]
    public string modelScene;

    [ShowIf("functionsObject")]
    public Sprite helpImage;

    // Start is called before the first frame update
    void Start()
    {
        var com = GetComponent<Button>();        
        if (com)
        {
            com.onClick.AddListener(new UnityEngine.Events.UnityAction(DoAction));
        }
    }// method

    public void DoAction()
    {
        if (functionsObject)
        {
            FunctionsObject.currentHelpImage = helpImage;
            functionsObject.LoadSceneLogic(modelScene, controllerType);
        }
    }// method
}
