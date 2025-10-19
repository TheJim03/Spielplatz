using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;

    [Header("Camera Settings")]
    [SerializeField] private float distance = 10f;
    [SerializeField] private float height = 5f;
    [SerializeField] private float smoothSpeed = 10f;

    [Header("Mouse Look Settings")]
    [SerializeField] private float mouseSensitivity = 0.5f; // Reduziert von 2f auf 0.5f
    [SerializeField] private float minVerticalAngle = -20f;
    [SerializeField] private float maxVerticalAngle = 60f;

    private Vector2 lookInput;
    private float currentX = 0f;
    private float currentY = 20f;

    // Property für andere Scripts um Kamera-Richtung zu bekommen
    public float HorizontalAngle => currentX;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogError("Kein Target für die Kamera zugewiesen!");
            return;
        }

        currentX += lookInput.x * mouseSensitivity;
        currentY -= lookInput.y * mouseSensitivity;
        currentY = Mathf.Clamp(currentY, minVerticalAngle, maxVerticalAngle);

        Vector3 direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 desiredPosition = target.position + rotation * direction + Vector3.up * height;

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        transform.LookAt(target.position + Vector3.up * height);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}