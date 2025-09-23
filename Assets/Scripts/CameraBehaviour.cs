using UnityEngine;
/// <summary>
/// Will adjust the camera to follow and face a target
/// </summary>
public class CameraBehaviour : MonoBehaviour
{
    [Tooltip("What object should the camera be lookingat")]
 public Transform target;
    [Tooltip("How offset will the camera be to thearget")]
 public Vector3 offset = new Vector3(0, 6, -10);

    [Tooltip("相机看向玩家的前方偏移 (x 左右, y 上下, z 前方)")]
    public Vector3 lookOffset = new Vector3(0, 2, 20);

    [Tooltip("平滑跟随速度")]
    public float smoothSpeed = 5f;
    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        if (target == null) return;

        // 计算目标位置
        Vector3 desiredPosition = target.position + offset;

        // 平滑插值，避免抖动
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // 计算相机注视点：玩家前方一点，而不是直接看玩家
        Vector3 lookAtPoint = target.position + lookOffset;

        transform.LookAt(lookAtPoint);
    }
}