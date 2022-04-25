using Doozy.Engine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class LandingViewController : MonoBehaviour
{
    const string LANDINGVIEW_OPEN_SAVED_KEY = "LANDINGVIEW_OPEN_SAVED_KEY";
    public UIView view;
    public VideoPlayer videoPlayer;
    public Toggle toggleOpen;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt(LANDINGVIEW_OPEN_SAVED_KEY, 1) == 1)
        {
            toggleOpen.isOn = true;
            view.Show();
            videoPlayer.Play();
        }
        else
        {
            toggleOpen.isOn = false;
            videoPlayer.Stop();
            view.Hide();
        }

        toggleOpen.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(ToggleOpen));
    }

    public void CloseLandingView()
    {
        videoPlayer.Stop();
        view.Hide();
    }

    public void OpenLandingView()
    {

        view.Show();
        videoPlayer.Play();
    }

    public void ToggleOpen(bool isOn)
    {
        PlayerPrefs.SetInt(LANDINGVIEW_OPEN_SAVED_KEY, isOn ? 1 : 0);
    }
}
