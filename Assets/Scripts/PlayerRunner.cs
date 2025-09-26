using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class PlayerRunner : MonoBehaviour
{
    private Rigidbody rb;
    private CapsuleCollider col;
    private Animator animator;   //  新增 Animator 引用

    [Header("Movement Settings")]
    public float dodgeSpeed = 5;
    public float startForwardSpeed = 5;
    public float maxForwardSpeed = 20;
    public float acceleration = 0.05f;
    public float jumpForce = 5;
    public float slideDuration = 0.5f;
    public float slideColliderHeight = 0.5f;

    private float forwardSpeed;
    private float originalColliderHeight;
    private Vector3 originalColliderCenter;
    private bool isSliding = false;

    private Coroutine jumpBoostCoroutine;
    private Coroutine speedBoostCoroutine;

    public enum MobileHorizMovement { Accelerometer, ScreenTouch }
    public MobileHorizMovement horizMovement = MobileHorizMovement.Accelerometer;

    [Header("Swipe Settings")]
    public float laneOffset = 2f;
    public float minSwipeDistance = 0.25f;
    private float minSwipeDistancePixels;
    private Vector2 touchStart;

    private int currentLane = 0;
    private bool isGrounded = true;

    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;
    public int MaxHealth => maxHealth;

    [Header("UI Settings")]
    public Transform heartContainer;  // Canvas 下的父物体
    public GameObject heartPrefab;    // 爱心图标 Prefab

    private List<GameObject> hearts = new List<GameObject>();

    public bool isJumpBoosted = false;
    public float originalJumpForce;

    [SerializeField] public Transform modelRoot; // 角色模型的父节点（不是整个Player，有Collider和Rigidbody的那个）
    [SerializeField] public float slideYOffset = -0.5f;
    [SerializeField] public float recoverSpeed = 5f;    // 恢复速度

    private float currentYOffset = 0f;

    [Header("Slide Settings")]
    public float slideResetDuration = 0.5f; // 调整为0.5~1秒比较自然

    private bool isDead = false;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        rb.constraints = RigidbodyConstraints.FreezeRotation;
        animator.applyRootMotion = false; // 不用 Root Motion
    }
    void Start()
    {
        

        minSwipeDistancePixels = minSwipeDistance * Screen.dpi;
        currentHealth = maxHealth;
        forwardSpeed = startForwardSpeed;

        originalColliderHeight = col.height;
        originalColliderCenter = col.center;

        currentHealth = maxHealth;
        UpdateHearts();
    }

    void Update()
    {
        if (isDead) return; // 死亡就不处理输入和动画状态

        if (forwardSpeed < maxForwardSpeed)
            forwardSpeed += acceleration * Time.deltaTime;

#if UNITY_STANDALONE || UNITY_WEBPLAYER
        if (Input.GetKeyDown(KeyCode.A)) ChangeLane(-1);
        if (Input.GetKeyDown(KeyCode.D)) ChangeLane(1);
        if (Input.GetKeyDown(KeyCode.Space)) Jump();
        if (Input.GetKeyDown(KeyCode.S)) Slide();
#elif UNITY_IOS || UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];
            SwipeInput(touch);
        }
#endif

        // 更新 Animator 参数
        if (animator != null)
        {
            // 假设 Animator 里有 Speed 参数
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetBool("IsSliding", isSliding);
        }

        if (modelRoot != null && animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("Slidinging"))
            {
                float t = stateInfo.normalizedTime; // 0 ~ 1 (动画进度)

                if (t < 0.5f)
                {
                    // 动画前半段：代码强制往下压
                    currentYOffset = Mathf.Lerp(currentYOffset, slideYOffset, Time.deltaTime * 10f);
                }
                else
                {
                    // 动画后半段：交给动画自己回位，不强制贴地
                    currentYOffset = Mathf.Lerp(currentYOffset, 0f, Time.deltaTime * recoverSpeed);
                }
            }
            else
            {
                // 非滑铲时：保持站立高度
                currentYOffset = Mathf.Lerp(currentYOffset, 0f, Time.deltaTime * recoverSpeed);
            }

            // 应用到模型本地坐标（只改 y）
            Vector3 lp = modelRoot.localPosition;
            lp.y = currentYOffset;
            modelRoot.localPosition = lp;
        }


    }

    void FixedUpdate()
    {
        if (isDead) return; // 死亡就不再移动
        // 前进
        Vector3 forwardMove = Vector3.forward * forwardSpeed * Time.fixedDeltaTime;

        // 横移
        Vector3 targetPos = new Vector3(currentLane * laneOffset, rb.position.y, rb.position.z);
        Vector3 moveDir = targetPos - rb.position;

        // 合并位移
        Vector3 totalMove = forwardMove + moveDir * dodgeSpeed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + totalMove);

        // 下滑时，空中强制向下加速度
        if (isSliding && !isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -10f, rb.linearVelocity.z);
        }
    }

    void ChangeLane(int direction)
    {
        currentLane = Mathf.Clamp(currentLane + direction, -1, 1);
    }

    void Jump()
    {
        if (isGrounded && !isSliding)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;

            if (animator != null)
                animator.SetTrigger("Jump"); //  播放跳跃动画
        }
    }

    void Slide()
    {
        if (isSliding) return;
        isSliding = true;

        StopCoroutine(nameof(SmoothResetCollider));

        // ��С��ײ��
        col.height = slideColliderHeight;
        col.center = new Vector3(originalColliderCenter.x, slideColliderHeight / 2f, originalColliderCenter.z);

        // ��������»���ֱ�Ӹ�һ���½��ٶ�
        if (!isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -10f, rb.linearVelocity.z);
        }

        //  播放下滑动画
        if (animator != null)
            animator.SetTrigger("Slide");

        StartCoroutine(EndSlideAfterDelay());
    }

    IEnumerator EndSlideAfterDelay()
    {
        yield return new WaitForSeconds(slideDuration);
        yield return StartCoroutine(SmoothResetCollider());
        isSliding = false;

        if (animator != null)
            animator.ResetTrigger("Slide"); //  防止卡住
    }

    IEnumerator SmoothResetCollider()
    {
        float elapsed = 0f;
        float duration = slideResetDuration;   // 用你在 Inspector 里设的时间

        float startHeight = col.height;
        Vector3 startCenter = col.center;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            col.height = Mathf.Lerp(startHeight, originalColliderHeight, t);
            col.center = Vector3.Lerp(startCenter, originalColliderCenter, t);
            yield return null;
        }

        col.height = originalColliderHeight;
        col.center = originalColliderCenter;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts[0].normal == Vector3.up)
            isGrounded = true;

        if (collision.gameObject.CompareTag("Obstacle"))
            TakeDamage(3);
    }

    void SwipeInput(Touch touch)
    {
        if (touch.phase == TouchPhase.Began)
        {
            touchStart = touch.position;
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            Vector2 touchEnd = touch.position;
            Vector2 delta = touchEnd - touchStart;

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                if (Mathf.Abs(delta.x) >= minSwipeDistancePixels)
                {
                    if (delta.x > 0) ChangeLane(1);
                    else ChangeLane(-1);
                }
            }
            else
            {
                if (delta.y > minSwipeDistancePixels && isGrounded)
                {
                    Jump();
                }
                else if (delta.y < -minSwipeDistancePixels)
                {
                    Slide();
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHearts(); // 更新UI
        Debug.Log("Player took damage! Current Health: " + currentHealth);

        if (currentHealth <= 0) Die();
    }


    public void Die()
    {
        if (isDead) return;   // 避免重复调用
        isDead = true;

        // 停止 buff 协程
        if (jumpBoostCoroutine != null) StopCoroutine(jumpBoostCoroutine);
        if (speedBoostCoroutine != null) StopCoroutine(speedBoostCoroutine);

        // 清除物理运动
        if (rb != null)
        {
            rb.linearVelocity  = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (animator != null)
            animator.SetTrigger("Die");

        Debug.Log("Player died!");

        // 用协程等待动画结束再调用 GameOver
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        // 等一帧让动画切换
        yield return null;

        float timeout = 5f;
        float timer = 0f;

        // 等待进入 Die 动画
        while (animator != null && !animator.GetCurrentAnimatorStateInfo(0).IsName("Die") && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // 等待动画播完
        if (animator != null && animator.GetCurrentAnimatorStateInfo(0).IsName("Die"))
        {
            while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f && timer < timeout)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }

        // 最后触发 GameOver
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null)
            gm.GameOver();
    }


    public bool IsSliding()
    {
        return isSliding;
    }

    public void ActivateJumpBoost(float multiplier, float duration)
    {
        // 如果已经在 Boost 中，先停止旧的协程
        if (jumpBoostCoroutine != null)
            StopCoroutine(jumpBoostCoroutine);

        // 启动新的协程
        jumpBoostCoroutine = StartCoroutine(JumpBoostRoutine(multiplier, duration));
    }

    private IEnumerator JumpBoostRoutine(float multiplier, float duration)
    {
        float originalJump = jumpForce;

        // 如果当前已经是 Boosted，保持倍率不叠加
        jumpForce = originalJump * multiplier;

        yield return new WaitForSeconds(duration);

        jumpForce = originalJump;
        jumpBoostCoroutine = null;
    }

    public void ActivateSpeedBoost(float multiplier, float duration)
    {
        // 如果已经在 Boost 中，先停止旧的协程
        if (speedBoostCoroutine != null)
            StopCoroutine(speedBoostCoroutine);

        speedBoostCoroutine = StartCoroutine(SpeedBoostRoutine(multiplier, duration));
    }

    private IEnumerator SpeedBoostRoutine(float multiplier, float duration)
    {
        float originalSpeed = forwardSpeed;

        // 如果当前已经 Boosted，不叠加倍率
        forwardSpeed = originalSpeed * multiplier;

        yield return new WaitForSeconds(duration);

        forwardSpeed = originalSpeed;
        speedBoostCoroutine = null;
    }

    void UpdateHearts()
    {
        // 清空旧图标
        foreach (var h in hearts)
            Destroy(h);
        hearts.Clear();

        float spacing = 20f; // 单位根据 Canvas 设置，像素为例
        Vector3 startPos = Vector3.zero;
        // 生成对应数量的爱心
        for (int i = 0; i < currentHealth; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartContainer);
            heart.transform.localPosition = startPos + new Vector3(i * spacing, -20, 0);
            hearts.Add(heart);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SpoiledFoodSpawn"))
            TakeDamage(1);
    }

    public void AddHealth(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHearts(); // 更新 UI
        Debug.Log("Player healed! Current Health: " + currentHealth);
    }

    
}
