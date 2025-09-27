using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
[RequireComponent(typeof(AudioSource))]
public class VideoAudioFix : MonoBehaviour
{
    void Awake()
    {
        var vp = GetComponent<VideoPlayer>();
        var audioSource = GetComponent<AudioSource>();

        vp.audioOutputMode = VideoAudioOutputMode.AudioSource;
        vp.EnableAudioTrack(0, true);
        vp.SetTargetAudioSource(0, audioSource);
    }
}

