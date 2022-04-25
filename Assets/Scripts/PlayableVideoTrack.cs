using UnityEngine.Video;
using UnityEngine.Timeline;

[TrackClipType(typeof(PlayableVideoAsset), false)]
[TrackBindingType(typeof(PlayableVideoData))]
public class PlayableVideoTrack : TrackAsset
{

}