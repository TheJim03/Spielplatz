using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine; // kannst du lassen, falls du Cinemachine installiert hast

public class PlayerTransformationManager : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera mainCam;                 // deine Hauptkamera (egal ob Cinemachine oder normal)
    public Transform cameraFollowOffset;   // optional: leerer Transform, bestimmt die Offset-Position
    public float followDistance = 6f;
    public float heightOffset = 2f;

    [Header("Gameplay")]
    public Transform currentControlled;
    public float transformRange = 10f;
    public KeyCode transformKey = KeyCode.C;
    public KeyCode revertKey = KeyCode.X;
    public Image crosshair;

    GameObject originalPlayer;
    bool isTransformed = false;
    CinemachineFreeLook freeLook; // erkannt automatisch, falls vorhanden

    void Start()
    {
        if (!mainCam) mainCam = Camera.main;
        freeLook = mainCam.GetComponent<CinemachineFreeLook>();
        if (!currentControlled)
        {
            var p = GameObject.FindWithTag("Player");
            if (p) currentControlled = p.transform;
        }
        if (currentControlled) originalPlayer = currentControlled.gameObject;

        UpdateCameraTarget(currentControlled);
    }

    void Update()
    {
        UpdateCrosshair();

        if (Input.GetKeyDown(transformKey)) TryTransform();
        if (Input.GetKeyDown(revertKey)) Revert();
    }

    void UpdateCrosshair()
    {
        if (!mainCam || !crosshair) return;

        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Color c = Color.white;
        if (Physics.Raycast(ray, out RaycastHit hit, transformRange))
        {
            if (hit.collider.GetComponent<TransformableObject>())
                c = hit.distance < transformRange * 0.5f ? Color.red : Color.yellow;
        }
        crosshair.color = c;
    }

    void TryTransform()
    {
        if (isTransformed) return;
        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, transformRange))
        {
            var target = hit.collider.GetComponent<TransformableObject>();
            if (target)
            {
                if (originalPlayer) originalPlayer.SetActive(false);
                currentControlled = target.transform;
                target.BecomeControlled(true);
                UpdateCameraTarget(currentControlled);
                isTransformed = true;
            }
        }
    }

    public void Revert()
    {
        if (!isTransformed) return;

        var tObj = currentControlled != null ? currentControlled.GetComponent<TransformableObject>() : null;
        if (tObj != null) tObj.BecomeControlled(false);

        if (originalPlayer) originalPlayer.SetActive(true);
        if (originalPlayer) currentControlled = originalPlayer.transform;

        UpdateCameraTarget(currentControlled);
        isTransformed = false;
    }

    void UpdateCameraTarget(Transform target)
    {
        if (!target || !mainCam) return;

        // Wenn die Kamera CinemachineFreeLook hat → Follow/LookAt setzen
        if (freeLook != null)
        {
            freeLook.Follow = target;
            freeLook.LookAt = target;
        }
        else
        {
            // normale Kamera: einfach hinter das Ziel positionieren
            Vector3 desiredPos = target.position - target.forward * followDistance + Vector3.up * heightOffset;
            mainCam.transform.position = desiredPos;
            mainCam.transform.LookAt(target);
        }
    }
}
