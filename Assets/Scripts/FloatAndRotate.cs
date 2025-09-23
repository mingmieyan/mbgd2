using UnityEngine;

/// <summary>
/// 给任意 3D/2D 物体增加悬浮、缩放抖动、左右摇摆等效果
/// </summary>
public class FloatAndRotate : MonoBehaviour
{
    [Header("悬浮设置")]
    public float floatAmplitude = 0.5f;   // 上下浮动幅度
    public float floatFrequency = 1f;     // 上下浮动速度

    [Header("旋转设置")]
    public Vector3 rotationSpeed = new Vector3(0, 30f, 0); // 每秒自转角速度

    [Header("缩放抖动设置")]
    public float scaleAmplitude = 0.1f;   // 缩放抖动幅度
    public float scaleFrequency = 2f;     // 缩放频率

    private Vector3 startPos;
    private Vector3 startScale;

    void Start()
    {
        startPos = transform.localPosition;
        startScale = transform.localScale;
    }

    void Update()
    {
        // 上下浮动
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.localPosition = new Vector3(startPos.x, newY, startPos.z);

        // 自转
        transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);

        // 缩放抖动
        float scaleOffset = (Mathf.Sin(Time.time * scaleFrequency) + 1f) * 0.5f * scaleAmplitude;
        transform.localScale = startScale * (1f + scaleOffset);
    }
}