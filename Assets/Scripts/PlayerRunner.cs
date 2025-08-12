using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerRunner : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Movement Settings")]
    [Tooltip("Horizontal dodge speed")]
    public float dodgeSpeed = 5;
    [Tooltip("Initial forward speed")]
    public float startForwardSpeed = 5;
    [Tooltip("Maximum forward speed")]
    public float maxForwardSpeed = 20;
    [Tooltip("Forward acceleration per second")]
    public float acceleration = 0.05f;
    [Tooltip("Jump force")]
    public float jumpForce = 5;

    private float forwardSpeed;

    public enum MobileHorizMovement
    {
        Accelerometer,
        ScreenTouch
    }
    [Tooltip("Mobile horizontal movement type")]
    public MobileHorizMovement horizMovement = MobileHorizMovement.Accelerometer;

    [Header("Swipe Settings")]
    public float laneOffset = 2f; // Distance between lanes
    public float minSwipeDistance = 0.25f;
    private float minSwipeDistancePixels;
    private Vector2 touchStart;

    private int currentLane = 0; // Middle = 0, Left = -1, Right = 1
    private bool isGrounded = true;

    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation; // Prevent rolling
        minSwipeDistancePixels = minSwipeDistance * Screen.dpi;
        currentHealth = maxHealth;

        forwardSpeed = startForwardSpeed; // start speed
    }

    void Update()
    {
        // Forward acceleration
        if (forwardSpeed < maxForwardSpeed)
        {
            forwardSpeed += acceleration * Time.deltaTime;
        }

#if UNITY_STANDALONE || UNITY_WEBPLAYER
        // Horizontal movement
        if (Input.GetKeyDown(KeyCode.A)) ChangeLane(-1);
        if (Input.GetKeyDown(KeyCode.D)) ChangeLane(1);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space)) Jump();
#elif UNITY_IOS || UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];
            SwipeInput(touch);
        }
#endif
    }

    void FixedUpdate()
    {
        // Forward movement + smooth lane switch
        Vector3 forwardMove = Vector3.forward * forwardSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + forwardMove);

        Vector3 targetPos = new Vector3(currentLane * laneOffset, rb.position.y, rb.position.z);
        Vector3 moveDir = targetPos - rb.position;
        rb.MovePosition(rb.position + moveDir * dodgeSpeed * Time.fixedDeltaTime);
    }

    void ChangeLane(int direction)
    {
        int targetLane = Mathf.Clamp(currentLane + direction, -1, 1);
        currentLane = targetLane;
    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Restore jump state
        if (collision.contacts[0].normal == Vector3.up)
        {
            isGrounded = true;
        }

        // Hit obstacle
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            TakeDamage(1);
        }
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
                // Left/Right swipe
                if (Mathf.Abs(delta.x) >= minSwipeDistancePixels)
                {
                    if (delta.x > 0) ChangeLane(1);
                    else ChangeLane(-1);
                }
            }
            else
            {
                // Up swipe = jump
                if (delta.y > minSwipeDistancePixels && isGrounded)
                {
                    Jump();
                }
            }
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player took damage! Current Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player died!");
        // TODO: Add game over logic here
        gameObject.SetActive(false);
    }
}
