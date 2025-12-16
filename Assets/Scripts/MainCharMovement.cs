using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class MinecraftRigidbodyController : MonoBehaviour
{
    [Header("References")]
    public Transform mainCamera;  // echte Main Camera (Cinemachine Brain)

    [Header("Move")]
    public float moveSpeed = 10f;
    public float acceleration = 35f;   // wie schnell er auf Speed kommt
    public float deceleration = 45f;   // wie schnell er stoppt
    public float airControl = 0.4f;    // falls du später springen willst (aktuell egal)

    [Header("Anti-Tip")]
    public float centerOfMassY = -0.5f;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        rb.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.centerOfMass = new Vector3(0f, centerOfMassY, 0f);

        if (!mainCamera)
        {
            if (Camera.main != null) mainCamera = Camera.main.transform;
            if (!mainCamera)
            {
                Debug.LogError("mainCamera fehlt.");
                enabled = false;
                return;
            }
        }
    }

    void FixedUpdate()
    {
        // kippen verhindern
        rb.angularVelocity = Vector3.zero;

        // Player folgt Kamera-Yaw (Minecraft)
        float yaw = mainCamera.eulerAngles.y;
        rb.MoveRotation(Quaternion.Euler(0f, yaw, 0f));

        // Input
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = (transform.forward * v + transform.right * h);
        if (inputDir.sqrMagnitude > 1f) inputDir.Normalize();

        // Zielgeschwindigkeit in XZ
        Vector3 desiredVelXZ = inputDir * moveSpeed;

        // aktuelle XZ-Geschwindigkeit
        Vector3 vel = rb.linearVelocity;
        Vector3 velXZ = new Vector3(vel.x, 0f, vel.z);

        // je nach Input beschleunigen oder abbremsen
        float rate = (inputDir.sqrMagnitude > 0.0001f) ? acceleration : deceleration;
        Vector3 newVelXZ = Vector3.MoveTowards(velXZ, desiredVelXZ, rate * Time.fixedDeltaTime);

        // anwenden (Y beibehalten für Gravity)
        rb.linearVelocity = new Vector3(newVelXZ.x, vel.y, newVelXZ.z);
    }
}
