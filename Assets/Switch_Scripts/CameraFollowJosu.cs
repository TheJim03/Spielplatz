using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollowJosu : MonoBehaviour
{
    [Header("Target & Offsets")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 6f, -8f);
    public float followLerp = 8f;

    [Header("FOV")]
    public float baseFOV = 60f;
    public float lerpFOV = 8f;

    float targetFOV;
    Camera cam;

    void Awake() { cam = GetComponent<Camera>(); }
    void Start() { targetFOV = baseFOV; if (cam) cam.fieldOfView = baseFOV; }

    void LateUpdate()
    {
        if (!target || !cam) return;

        Vector3 desiredPos = target.position + target.TransformDirection(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPos, followLerp * Time.deltaTime);

        Vector3 lookPoint = target.position + Vector3.up * 0.5f;
        var wantRot = Quaternion.LookRotation(lookPoint - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, wantRot, followLerp * Time.deltaTime);

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, lerpFOV * Time.deltaTime);
    }

    public void SetTargetAndStyle(Transform newTarget, float newFov, Vector3 newOffset)
    {
        target    = newTarget;
        targetFOV = newFov;
        offset    = newOffset;
    }
}