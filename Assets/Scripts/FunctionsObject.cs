using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Playables;

[CreateAssetMenu(menuName = "Assets/Functions")]
public class FunctionsObject : ScriptableObject
{
    public const string ProblemsReportSceneName = "_ProblemsReport";
    public const string MainMenuSceneName = "00 MainMenu";
    public const string DefaultLearningLogicSceneName = "01 Learning";
    public const string DefaultAnimationLogicSceneName = "02 Animation";
    public const string DefaultProblemLogicSceneName = "03 Problem";
    public static string CurrentModelScene = null;
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    public GameObject loadingScreen;
    public static GameObject currentLoadingScreen = null;
    public static Sprite currentHelpImage = null;

    private void OnEnable()
    {   
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }// method

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name.Contains("MainMenu") || arg0.name == "_ProblemsReport")
        {
            MainController.selectedProblem = null;
            if (currentLoadingScreen != null)
            {
                GameObject.Destroy(currentLoadingScreen);
                currentLoadingScreen = null;
            }
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }// method

    public void LoadScene(string sceneName)
    {
        if (currentLoadingScreen == null)
        {
            currentLoadingScreen = GameObject.Instantiate(loadingScreen);
            GameObject.DontDestroyOnLoad(currentLoadingScreen);
        }
        SceneManager.LoadScene(sceneName);
    }// method

    public void LoadProblemsReport()
    {
        LoadScene(ProblemsReportSceneName);
    }// method

    public void LoadMainMenu()
    {
        LoadScene(MainMenuSceneName);
    }

    public void LoadSceneLogic(string modelSceneName, ControllerType type)
    {
        if (type == ControllerType.AnimationController)
        {
            CurrentModelScene = modelSceneName;
            LoadScene(DefaultAnimationLogicSceneName);
        }
        else if (type == ControllerType.LearningController)
        {
            CurrentModelScene = modelSceneName;
            LoadScene(DefaultLearningLogicSceneName);
        }
    }// method

    public void Exit()
    {
        Application.Quit();
    }

    public void StopScene()
    {
        var colliders = GameObject.FindObjectsOfType<HighlightPlus.HighlightTrigger>().Select(x => x.GetComponent<Collider>());
        foreach (var col in colliders) col.enabled = false;
        var dirs = GameObject.FindObjectsOfType<PlayableDirector>();
        foreach(var d in dirs)
        {
            if (d.state == PlayState.Playing) d.Pause();
        }
        PauseMainAudio();
    }// method

    private List<PlayableDirector> _savedPausedDirs = null;
    public void PauseAllRunningPlayableDirectors()
    {
        var dirs = GameObject.FindObjectsOfType<PlayableDirector>();
        _savedPausedDirs = new List<PlayableDirector>();
        foreach (var d in dirs)
        {
            if (d.state == PlayState.Playing)
            {
                _savedPausedDirs.Add(d);
                d.Pause();
            }
        }
    }// method

    public void ResumeAllPausedPlayableDirectors()
    {
        if (_savedPausedDirs != null)
        {
            foreach(var d in _savedPausedDirs)
            {
                if (d.state == PlayState.Paused) d.Resume();
            }
            _savedPausedDirs = null;
        }
        //var dirs = GameObject.FindObjectsOfType<PlayableDirector>();
        //foreach (var d in dirs)
        //{
        //    if (d.state == PlayState.Paused) d.Resume();
        //}
    }// method

    public void RaiseEventTimelineReachedEnd()
    {
        TimelineSliderController.CurrentTimeLine = null;
    }//

    public void ToggleAllPlayableDirectors()
    {
        if (TimelineSliderController.CurrentTimeLine != null)
        {
            var tm = TimelineSliderController.CurrentTimeLine;
            if (tm.state == PlayState.Paused)
            {
                tm.Resume();
            }
            else
            {
                tm.Pause();
            }
        }
        //var dirs = GameObject.FindObjectsOfType<PlayableDirector>();

        //int playing = 0;
        //foreach (var d in dirs)
        //{
        //    if (d.state == PlayState.Playing) ++playing;
        //}

        //if (playing > 0)
        //{
        //    PauseAllRunningPlayableDirectors();
        //}
        //else
        //{
        //    ResumeAllPausedPlayableDirectors();
        //}
    }// method

    public void ResumeScene()
    {
        var colliders = GameObject.FindObjectsOfType<HighlightPlus.HighlightTrigger>().Select(x => x.GetComponent<Collider>());
        foreach (var col in colliders) col.enabled = true;
        var dirs = GameObject.FindObjectsOfType<PlayableDirector>();
        foreach (var d in dirs)
        {
            if (d.state == PlayState.Paused) d.Resume();
        }
        ResumeMainAudio();
    } // method

    public void PauseMainAudio()
    {
        var con = FindObjectOfType<MainController>();
        if (con != null)
        {
            con.mainAudioSource.Pause();
        }
    }// method

    public void ResumeMainAudio()
    {
        var con = FindObjectOfType<MainController>();
        if (con != null)
        {
            con.mainAudioSource.UnPause();
        }
    }// method

    public void ChangeCursor()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }//method

    public void RestoreCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }// method

    public void TakeSnapShot()
    {
        string filePath = SFB.StandaloneFileBrowser.SaveFilePanel("Save snapshot", "", "snapshot.png", "png");
        if (string.IsNullOrEmpty(filePath)) return;
        UnityEngine.ScreenCapture.CaptureScreenshot(filePath);
    }
}
