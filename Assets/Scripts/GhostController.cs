using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GhostController : MonoBehaviour
{
    [Header("Refs")]
    public Transform cameraPivot;

    [Header("Move")]
    public float moveSpeed = 4.5f;
    public float rotationSpeed = 540f;
    public float airControl = 0.85f;

    [Header("Hover")]
    public float hoverHeight = 1.2f;
    public float hoverSnap = 6f;
    public float bobAmplitude = 0.1f;
    public float bobFrequency = 2f;

    [Header("Float Jump")]
    public float floatBoostUp = 3.5f;
    public float floatDuration = 0.4f;
    public float gravity = 6f;

    [Header("Ground Check")]
    public float groundRayLength = 4f;
    public LayerMask groundMask = ~0;

    CharacterController ctrl;
    float verticalVel;
    float floatTimer;
    float bobT;
    bool isNearHoverPlane; // eigener Ground-Status

    void Awake()
    {
        ctrl = GetComponent<CharacterController>();
        if (!cameraPivot && Camera.main) cameraPivot = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // --- Input ---
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(h, 0f, v);
        input = Vector3.ClampMagnitude(input, 1f);

        // --- Kamerarahmen ---
        Vector3 f = Vector3.forward, r = Vector3.right;
        if (cameraPivot)
        {
            f = Vector3.ProjectOnPlane(cameraPivot.forward, Vector3.up).normalized;
            r = Vector3.ProjectOnPlane(cameraPivot.right,   Vector3.up).normalized;
        }
        Vector3 moveDir = (f * input.z + r * input.x);

        // --- Drehen ---
        if (moveDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // --- Sichere Bewegung (Test): SimpleMove für die Horizontale ---
        // SimpleMove übernimmt dt und leichte Grav – garantiert, dass WASD sofort "zieht".
        Vector3 horizontalVel = moveDir * moveSpeed * ((floatTimer > 0f) ? 1f : (isNearHoverPlane ? 1f : airControl));
        ctrl.SimpleMove(horizontalVel);

        // --- Hover-Zielhöhe bestimmen ---
        float groundY = transform.position.y;
        if (Physics.SphereCast(transform.position + Vector3.up * 0.2f, ctrl.radius * 0.9f, Vector3.down,
                               out RaycastHit hit, groundRayLength, groundMask, QueryTriggerInteraction.Ignore))
        {
            groundY = hit.point.y;
        }

        bobT += Time.deltaTime * bobFrequency * Mathf.PI * 2f;
        float bob = Mathf.Sin(bobT) * bobAmplitude;
        float targetY = groundY + hoverHeight + bob;

        // Sind wir nahe der Hover-Ebene?
        isNearHoverPlane = Mathf.Abs(transform.position.y - (groundY + hoverHeight)) < 0.07f;

        // --- Float Jump ---
        if (Input.GetButtonDown("Jump") && isNearHoverPlane)
        {
            floatTimer = floatDuration;
            verticalVel = floatBoostUp;
        }

        // Gravitation / Float
        if (floatTimer > 0f) { floatTimer -= Time.deltaTime; verticalVel -= gravity * 0.35f * Time.deltaTime; }
        else                 { verticalVel -= gravity * Time.deltaTime; }

        // Auf Zielhöhe einpendeln
        float heightError = targetY - transform.position.y;
        float heightCorrection = Mathf.Clamp(heightError * hoverSnap, -3f, 3f);
        float finalVertical = verticalVel + heightCorrection;

        // Vertikal separat bewegen (damit SimpleMove die Horizontale behält)
        Vector3 verticalMotion = Vector3.up * finalVertical * Time.deltaTime;
        ctrl.Move(verticalMotion);

        // Bodenkontakt → Vertikale dämpfen
        if ((ctrl.collisionFlags & CollisionFlags.Below) != 0)
            verticalVel = Mathf.Min(verticalVel, 0f);

        // ESC zum Freigeben (optional)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}