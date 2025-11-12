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

    [Header("Juiciness (optional)")]
    [SerializeField] private AudioClip rollingSound;
    [SerializeField] private ParticleSystem movementParticles;

    private Rigidbody rb;
    private Transform cam;
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main.transform;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && rollingSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = rollingSound;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }
    }

    private bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        bool hit = Physics.Raycast(origin, Vector3.down, groundCheckDistance + 0.1f, groundLayer);

        if (JuicinessSettings.instance != null && JuicinessSettings.instance.IsJuicy)
        {
            Debug.DrawRay(origin, Vector3.down * (groundCheckDistance + 0.1f), hit ? Color.green : Color.red);
        }

        return hit;
    }

    void FixedUpdate()
    {
        rb.AddForce(Physics.gravity * (gravityMultiplier - 1f), ForceMode.Acceleration);

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 camForward = cam.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cam.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 moveDirection = (camForward * v + camRight * h).normalized;

        bool juicy = JuicinessSettings.instance != null && JuicinessSettings.instance.IsJuicy;

        if (moveDirection.magnitude > 0)
        {
            float currentSpeed = moveSpeed;
            Vector3 targetVelocity = moveDirection * currentSpeed + new Vector3(0, rb.linearVelocity.y, 0);

            if (juicy)
            {
                // 🎮 Juicy: Smooth Lerp für weiches Ausrollen
                rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, 0.15f);
                rb.linearDamping = dragWhenMoving;
            }
            else
            {
                // 🧊 Nicht juicy: Direkt snappen = statisch/stiff
                rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
                rb.linearDamping = 5f; // Höherer Drag = stoppt sofort
            }

            HandleMovementJuiciness(true);
        }
        else
        {
            if (!juicy)
            {
                // Sofort stoppen ohne Ausrollen
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
                rb.linearDamping = 10f;
            }
            else
            {
                rb.linearDamping = dragWhenNotMoving;
            }

            HandleMovementJuiciness(false);
        }
    }

    private void HandleMovementJuiciness(bool isMoving)
    {
        bool juicy = JuicinessSettings.instance != null && JuicinessSettings.instance.IsJuicy;

        if (!juicy)
        {
            if (audioSource != null && audioSource.isPlaying) audioSource.Stop();
            if (movementParticles != null && movementParticles.isPlaying) movementParticles.Stop();
            return;
        }

        if (isMoving)
        {
            if (audioSource != null && !audioSource.isPlaying && rollingSound != null)
            {
                audioSource.Play();
            }
            if (movementParticles != null && !movementParticles.isPlaying)
            {
                movementParticles.Play();
            }
        }
        else
        {
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            if (movementParticles != null && movementParticles.isPlaying)
            {
                movementParticles.Stop();
            }
        }
    }
}