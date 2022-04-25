using Doozy.Engine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class PlayableVideoData : MonoBehaviour
{
    public VideoPlayer player;
    public RawImage renderView;
    public int size = 256;

    private RenderTexture _renderTexture = null;

    private void Start()
    {
        _renderTexture = new RenderTexture(size, size, 16);

        player.renderMode = VideoRenderMode.RenderTexture;
        player.targetTexture = _renderTexture;
        renderView.texture = _renderTexture;
    }
}