using UnityEngine;

public class DashMechanic : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 20f;      // Speed of the dash
    public float dashDuration = 0.1f;  // Duration of the dash

    [Header("Ground Check")]
    [Tooltip("Set this to the layer used by ground objects.")]
    public LayerMask groundLayer;      // Layer for 'Ground' objects

    private Vector2 dashDirection;     // Direction of the dash
    private bool isDashing = false;    // Is the player currently dashing?
    private float dashEndTime = 0;     // When the dash should end
    private Rigidbody2D rb;            // Player's Rigidbody2D

    // This flag determines if we can initiate another dash
    private bool canDash = true;       

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Capture horizontal input
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        // Update dash direction based on input, default to facing direction if no input
        if (horizontalInput != 0)
        {
            dashDirection = new Vector2(horizontalInput, 0f);
        }

        // Trigger dash on key press if not already dashing and we still have a dash available
        if (Input.GetKeyDown(CompleteAchievementsRunManager.Instance.dashKey) && !isDashing && canDash)
        {
            BeginDash();
            CompleteAchievementsRunManager.Instance.OnDashUsed();
        }

        // If currently dashing and time has expired, end the dash
        if (isDashing && Time.time >= dashEndTime)
        {
            EndDash();
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = dashDirection.normalized * dashSpeed;
        }
    }

    void BeginDash()
    {
        isDashing = true;
        canDash = false;  // We used our dash, so we can't dash again until we reset on ground
        dashEndTime = Time.time + dashDuration;

        // Apply the initial dash velocity
        rb.linearVelocity = dashDirection.normalized * dashSpeed;
    }

    void EndDash()
    {
        isDashing = false;
        // We do NOT reset canDash here; that's done when we collide with ground
    }

    // Whenever we collide with something, check if it's on the Ground layer
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Method 1: If you have an actual Ground layer in the inspector
        //           (using a LayerMask for collision.gameObject.layer)
        if (IsInLayerMask(collision.gameObject.layer, groundLayer))
        {
            // We've hit the ground, so reset dash availability
            canDash = true;
        }

        // Alternatively, if you just manually compare with a "Ground" layer index:
        // if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        // {
        //     canDash = true;
        // }
    }

        // Whenever we collide with something, check if it's on the Ground layer
    void OnTriggerEnter2D(Collider2D collision)
    {

        if (IsInLayerMask(collision.gameObject.layer, groundLayer))
        {
            // We've hit the ground, so reset dash
            canDash = true;
        }
    }

    // Helper function to check if a layer is in a LayerMask
    bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return ((layerMask.value & (1 << layer)) != 0);
    }
}
