using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoEndToScene : MonoBehaviour
{
    [Tooltip("VideoPlayer 组件")]
    public VideoPlayer videoPlayer;

    [Tooltip("播放完后切换到的场景名")]
    public string nextSceneName = "Level1";

    void Start()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer != null)
        {
            // 注册事件
            videoPlayer.loopPointReached += OnVideoEnd;
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        // 切换场景
        SceneManager.LoadScene(nextSceneName);
    }
}
