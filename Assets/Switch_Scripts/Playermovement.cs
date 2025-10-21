using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMover : MonoBehaviour
{
    [Header("Base Movement")]
    public float moveSpeed = 6f;
    public float acceleration = 25f;
    public float jumpForce = 5f;

    [Header("State (read-only)")]
    public bool grounded = true;
    public bool isControlled = false;   // <- wer steuern darf

    [Header("Juicy Toggle (J)")]
    public bool juicyMode = false;
    public float fovKickOnSprint = 8f;
    public float squashAmount = 0.15f;

    Rigidbody rb;
    Vector3 desiredVel;
    Vector3 originalScale;
    float squashCooldown;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        originalScale = transform.localScale;
    }

    public void SetControlled(bool enabled)
    {
        isControlled = enabled;

        // Physik eindeutig parken/aktivieren
        rb.linearVelocity        = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic     = !enabled;

        var col = GetComponent<Collider>();
        if (col) col.enabled = enabled;
    }

    void Update()
    {
        if (!isControlled) { desiredVel = Vector3.zero; return; }

        float h = 0f, v = 0f;
        if (Keyboard.current != null)
        {
            h = (Keyboard.current.dKey.isPressed ? 1f : 0f) +
                (Keyboard.current.aKey.isPressed ? -1f : 0f);
            v = (Keyboard.current.wKey.isPressed ? 1f : 0f) +
                (Keyboard.current.sKey.isPressed ? -1f : 0f);

            if (Keyboard.current.jKey.wasPressedThisFrame)
                juicyMode = !juicyMode;

            if (Keyboard.current.spaceKey.wasPressedThisFrame && grounded)
            {
                var v0 = rb.linearVelocity; v0.y = 0f; rb.linearVelocity = v0;
                rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
                grounded = false;
            }
        }

        // Richtung relativ zur Kamera
        var cam = Camera.main;
        Vector3 worldDir;
        if (cam)
        {
            Vector3 camF = cam.transform.forward; camF.y = 0; camF.Normalize();
            Vector3 camR = cam.transform.right;   camR.y = 0; camR.Normalize();
            worldDir = (camF * v + camR * h).normalized;
        }
        else worldDir = new Vector3(h, 0, v).normalized;

        float speed = moveSpeed;
        if (juicyMode && Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed)
            speed *= 1.35f;

        desiredVel = worldDir * speed;
    }

    void FixedUpdate()
    {
        if (!isControlled) return;

        Vector3 vel = rb.linearVelocity;
        Vector3 velXZ = new Vector3(vel.x, 0, vel.z);
        Vector3 newVelXZ = Vector3.MoveTowards(velXZ, desiredVel, acceleration * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector3(newVelXZ.x, vel.y, newVelXZ.z);

        if (squashCooldown <= 0f)
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, 10f * Time.fixedDeltaTime);
        else
            squashCooldown -= Time.fixedDeltaTime;
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.contactCount > 0 && c.GetContact(0).normal.y > 0.5f)
        {
            if (!grounded && juicyMode)
            {
                transform.localScale = new Vector3(
                    originalScale.x * (1f + squashAmount),
                    originalScale.y * (1f - squashAmount),
                    originalScale.z * (1f + squashAmount)
                );
                squashCooldown = 0.08f;
            }
            grounded = true;
        }
    }
}