using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    Rigidbody rb;
    Vector3 inputVel;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Optional: fix rotations so sphere doesn't tumble:
        rb.freezeRotation = true;
    }

    void Update()
    {
        // Input (WASD / Arrow keys)
        float h = Input.GetAxisRaw("Horizontal"); // A/D, Left/Right
        float v = Input.GetAxisRaw("Vertical");   // W/S, Up/Down

        Vector3 camForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up).normalized;

        Vector3 desired = (camForward * v + camRight * h).normalized;
        inputVel = desired * moveSpeed;
    }

    void FixedUpdate()
    {
        if (inputVel.sqrMagnitude > 0.0001f)
        {
            Vector3 newPos = rb.position + inputVel * Time.fixedDeltaTime;
            rb.MovePosition(newPos);
        }
    }

    // Platzhalter für spätere Transformationen
    public void TransformInto(GameObject newForm)
    {
        // TODO: instantiate/activate newForm and hide player until revert
        gameObject.SetActive(false);
    }

    public void RevertToPlayer()
    {
        gameObject.SetActive(true);
    }
}