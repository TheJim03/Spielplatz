using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float dragWhenNotMoving = 0f;
    public float dragWhenMoving = 0f;
    public float gravityMultiplier = 2f;

    [Header("Sprint Settings")]
    public float sprintMultiplier = 2f;
    public float sprintDuration = 5f;
    public float sprintCooldown = 20f;

    [Header("Slow Settings")]
    public float slowMultiplier = 0.5f;
    public float slowDuration = 5f;
    public float slowCooldown = 20f;
    [Header("Audio")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.5f;
    private Rigidbody rb;
    private Transform cam;

    // Sprinting
    /*
    private float sprintTimeLeft = 0f;
    private float sprintCooldownTimer = 0f;
    private bool sprintTimerRunning = false;
    private bool isSprinting = false;
    */

    // Slowing
    /*
    private float slowTimeLeft = 0f;
    private float slowCooldownTimer = 0f;
    private bool slowTimerRunning = false;
    private bool isSlowing = false;
    */

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main.transform;

        // sprintTimeLeft = sprintDuration;
        // slowTimeLeft = slowDuration;
    }
    
    private bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        bool hit = Physics.Raycast(origin, Vector3.down, groundCheckDistance + 0.1f, groundLayer);
        Debug.DrawRay(origin, Vector3.down * (groundCheckDistance + 0.1f), hit ? Color.green : Color.red);
        return hit;
    }
    

    void FixedUpdate()
    {
        // Gravity
        rb.AddForce(Physics.gravity * (gravityMultiplier - 1f), ForceMode.Acceleration);

        // Sprint Logic (disabled)
        /*
        if (sprintCooldownTimer > 0f)
        {
            sprintCooldownTimer -= Time.fixedDeltaTime;
        }
        else
        {
            if (sprintTimerRunning)
            {
                sprintTimeLeft -= Time.fixedDeltaTime;

                if (Input.GetKey(KeyCode.LeftControl) && sprintTimeLeft > 0f)
                    isSprinting = true;
                else
                    isSprinting = false;

                if (sprintTimeLeft <= 0f)
                {
                    isSprinting = false;
                    sprintTimerRunning = false;
                    sprintCooldownTimer = sprintCooldown;
                    sprintTimeLeft = 0f;
                }
            }

            if (!sprintTimerRunning && sprintCooldownTimer <= 0f && sprintTimeLeft <= 0f)
                sprintTimeLeft = sprintDuration;
        }
        */

        // Slow Logic (disabled)
        /*
        if (slowCooldownTimer > 0f)
        {
            slowCooldownTimer -= Time.fixedDeltaTime;
        }
        else
        {
            if (slowTimerRunning)
            {
                slowTimeLeft -= Time.fixedDeltaTime;

                if (Input.GetKey(KeyCode.LeftShift) && slowTimeLeft > 0f)
                    isSlowing = true;
                else
                    isSlowing = false;

                if (slowTimeLeft <= 0f)
                {
                    isSlowing = false;
                    slowTimerRunning = false;
                    slowCooldownTimer = slowCooldown;
                    slowTimeLeft = 0f;
                }
            }

            if (!slowTimerRunning && slowCooldownTimer <= 0f && slowTimeLeft <= 0f)
                slowTimeLeft = slowDuration;
        }
        */

        // Movement
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 camForward = cam.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cam.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 moveDirection = (camForward * v + camRight * h).normalized;

        if (moveDirection.magnitude > 0)
        {
            float currentSpeed = moveSpeed;
            // if (isSprinting) currentSpeed *= sprintMultiplier;
            // if (isSlowing) currentSpeed *= slowMultiplier;

            Vector3 targetVelocity = moveDirection * currentSpeed + new Vector3(0, rb.linearVelocity.y, 0);
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, 0.15f);
            rb.linearDamping = dragWhenMoving;
        }
        else
        {
            rb.linearDamping = dragWhenNotMoving;
        }
    }
}
