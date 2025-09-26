using UnityEngine;

public class RootMotionController : MonoBehaviour
{
    public Animator animator;
    public float slideYOffset = -0.5f;    // 滑铲时往下偏移多少
    public float recoverSpeed = 5f;       // 恢复到原始高度的速度

    private float originalY;              // 原始高度
    private bool isSliding = false;

    private void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        originalY = transform.position.y;
    }

    void OnAnimatorMove()
    {
        // 保留 X/Z 由玩家控制，只取动画的 Y（再加工）
        Vector3 pos = transform.position;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Slidinging")) // Animator 状态名要和你的滑铲动画一致
        {
            if (!isSliding)
            {
                isSliding = true;
                originalY = transform.position.y;
            }

            // 滑铲过程 → 强制贴地
            pos.y = originalY + slideYOffset;
        }
        else if (isSliding)
        {
            // 动画结束，慢慢恢复
            isSliding = false;
        }

        // 如果滑铲结束，Y 平滑插值回原始高度
        if (!stateInfo.IsName("Slidinging") && Mathf.Abs(pos.y - originalY) > 0.01f)
        {
            pos.y = Mathf.Lerp(pos.y, originalY, Time.deltaTime * recoverSpeed);
        }

        transform.position = pos;
    }
}
