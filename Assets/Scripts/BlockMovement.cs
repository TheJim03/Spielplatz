using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BlockMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    Rigidbody rb; Transform cam;

    void Awake(){ rb = GetComponent<Rigidbody>(); cam = Camera.main.transform; }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 f = cam.forward; f.y = 0; f.Normalize();
        Vector3 r = cam.right;   r.y = 0; r.Normalize();

        Vector3 dir = (f * v + r * h).normalized;
        Vector3 vel = dir * moveSpeed + Vector3.up * rb.linearVelocity.y;
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, vel, 0.15f);
    }
}
