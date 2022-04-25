using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(Button))]
public class MainMenuItemViewer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image iconImage;
    public Image toolTipImage;
    public Image titleImage;
    public VideoPlayer videoPlayer;
    public Image imageView;
    public GameObject hoverEffect;

    public MainMenuItemData data;

    private VideoClip _defaultVideoClip;
    private Sprite _defaultImage;

    void Awake()
    {
        iconImage.sprite = data.icon;
        toolTipImage.sprite = data.toolTip;
        _defaultVideoClip = videoPlayer.clip;
        _defaultImage = imageView.sprite;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (videoPlayer != null)
        {
            titleImage.sprite = data.title;
            titleImage.gameObject.SetActive(true);
            toolTipImage.gameObject.SetActive(true);
            SetCenterContent(data.videoClip, data.image);
            hoverEffect.SetActive(true);
        }
    }

    void SetCenterContent(VideoClip clip, Sprite sp)
    {
        imageView.gameObject.SetActive(false);
        videoPlayer.gameObject.SetActive(false);

        if (clip != null)
        {
            videoPlayer.clip = clip;
            videoPlayer.gameObject.SetActive(true);
            videoPlayer.Play();
            
        }
        else
        {
            imageView.sprite = sp;
            imageView.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (videoPlayer != null)
        {
            titleImage.sprite = null;
            titleImage.gameObject.SetActive(false);
            SetCenterContent(_defaultVideoClip, _defaultImage);
            hoverEffect.SetActive(false);
            toolTipImage.gameObject.SetActive(false);
        }
    }

}
