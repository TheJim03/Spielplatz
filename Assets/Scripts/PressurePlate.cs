using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    [Header("Plate Settings")]
    [SerializeField] private float pressDepth = 0.1f;
    [SerializeField] private float pressSpeed = 5f;
    [SerializeField] private bool stayPressed = false;

    [Header("Visual")]
    [SerializeField] private Color normalColor = Color.gray;
    [SerializeField] private Color pressedColor = Color.red;

    [Header("Audio")]
    [SerializeField] private AudioClip pressSound;
    [SerializeField] private AudioClip releaseSound;
    [Range(0f, 1f)]
    [SerializeField] private float soundVolume = 0.5f;

    [Header("Connected Doors")]
    [SerializeField] private SlidingDoor[] connectedDoors;

    [Header("Feedback Toggles")]
    [SerializeField] private bool enableMovementFeedback = true;
    [SerializeField] private bool enableVisualFeedback = true;
    [SerializeField] private bool enableAudioFeedback = true;

    private Vector3 originalPosition;
    private Vector3 pressedPosition;
    private bool isPressed = false;
    private int objectsOnPlate = 0;
    private Renderer plateRenderer;
    private Material plateMaterial;

    void Start()
    {
        originalPosition = transform.position;
        pressedPosition = originalPosition - Vector3.up * pressDepth;

        plateRenderer = GetComponent<Renderer>();
        if (plateRenderer != null)
        {
            plateMaterial = new Material(plateRenderer.material);
            plateRenderer.material = plateMaterial;
            plateMaterial.color = normalColor;
        }
    }

    void Update()
    {
        bool plateShouldBeDown = (isPressed || objectsOnPlate > 0);
        bool juicy = JuicinessSettings.instance != null && JuicinessSettings.instance.IsJuicy;

        // 🎮 Bewegung nur wenn Juiciness AN
        if (enableMovementFeedback && juicy)
        {
            Vector3 targetPos = plateShouldBeDown ? pressedPosition : originalPosition;
            transform.position = Vector3.Lerp(transform.position, targetPos, pressSpeed * Time.deltaTime);
        }
        else if (enableMovementFeedback && !juicy)
        {
            // 🧊 Ohne Juiciness: Direkt snappen (keine Animation)
            transform.position = plateShouldBeDown ? pressedPosition : originalPosition;
        }

        // 🎨 Farbwechsel nur wenn Juiciness AN
        if (enableVisualFeedback && juicy && plateMaterial != null)
        {
            Color targetColor = plateShouldBeDown ? pressedColor : normalColor;
            plateMaterial.color = Color.Lerp(plateMaterial.color, targetColor, pressSpeed * Time.deltaTime);
        }
        else if (plateMaterial != null)
        {
            // 🧊 Ohne Juiciness: Immer Normalfarbe
            plateMaterial.color = normalColor;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            objectsOnPlate++;

            if (objectsOnPlate == 1)
            {
                Press();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            objectsOnPlate--;

            if (objectsOnPlate <= 0)
            {
                if (!stayPressed)
                    Release();
                else
                    isPressed = true;
            }
        }
    }

    private void Press()
    {
        Debug.Log("Druckplatte aktiviert!");
        isPressed = true;

        bool juicy = JuicinessSettings.instance != null && JuicinessSettings.instance.IsJuicy;

        // 🔊 Sound nur wenn Juiciness AN
        if (enableAudioFeedback && juicy && pressSound != null)
        {
            AudioSource.PlayClipAtPoint(pressSound, transform.position, soundVolume);
        }

        foreach (SlidingDoor door in connectedDoors)
        {
            if (door != null)
            {
                door.OpenDoor();
            }
        }
    }

    private void Release()
    {
        Debug.Log("Druckplatte deaktiviert!");
        isPressed = false;

        bool juicy = JuicinessSettings.instance != null && JuicinessSettings.instance.IsJuicy;

        // 🔊 Sound nur wenn Juiciness AN
        if (enableAudioFeedback && juicy && releaseSound != null)
        {
            AudioSource.PlayClipAtPoint(releaseSound, transform.position, soundVolume);
        }

        foreach (SlidingDoor door in connectedDoors)
        {
            if (door != null)
            {
                door.CloseDoor();
            }
        }
    }
}