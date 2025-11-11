using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TransformableObject : MonoBehaviour
{
    Rigidbody rb;
    bool isControlled = false;

    public float moveSpeed = 3f;
    public float rotationSpeed = 80f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
    }

    public void BecomeControlled(bool state)
    {
        isControlled = state;
        rb.isKinematic = !state;
    }

    void Update()
    {
        if (!isControlled) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v);
        move = Camera.main.transform.TransformDirection(move);
        move.y = 0;

        rb.MovePosition(rb.position + move * moveSpeed * Time.deltaTime);

        // Optional rotation based on input
        if (move.sqrMagnitude > 0.001f)
        {
            Quaternion rot = Quaternion.LookRotation(move);
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, rot, rotationSpeed * Time.deltaTime));
        }
    }
}