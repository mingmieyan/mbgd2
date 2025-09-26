using UnityEngine;

public class VideoOrientationController : MonoBehaviour
{
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (Screen.height > Screen.width)
        {
            // 竖屏 → 把视频竖过来
            rectTransform.localEulerAngles = new Vector3(0, 0, -90);
        }
        else
        {
            // 横屏 → 视频保持横着
            rectTransform.localEulerAngles = Vector3.zero;
        }
    }
}
