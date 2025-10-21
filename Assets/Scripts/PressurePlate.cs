using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    [Header("Plate Settings")]
    [SerializeField] private float pressDepth = 0.1f; // Wie tief die Platte gedrückt wird
    [SerializeField] private float pressSpeed = 5f; // Wie schnell sie sich bewegt
    [SerializeField] private bool stayPressed = false; // Bleibt gedrückt oder springt zurück?

    [Header("Visual")]
    [SerializeField] private Color normalColor = Color.gray;
    [SerializeField] private Color pressedColor = Color.red;

    [Header("Audio")]
    [SerializeField] private AudioClip pressSound;
    [SerializeField] private AudioClip releaseSound;
    [Range(0f, 1f)]
    [SerializeField] private float soundVolume = 0.5f;

    [Header("Connected Doors")]
    [SerializeField] private SlidingDoor[] connectedDoors; // Array für mehrere Türen

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

        // Material Setup
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
        // Smooth Bewegung zur Zielposition
        Vector3 targetPos = (isPressed || objectsOnPlate > 0) ? pressedPosition : originalPosition;
        transform.position = Vector3.Lerp(transform.position, targetPos, pressSpeed * Time.deltaTime);

        // Farbwechsel
        if (plateMaterial != null)
        {
            Color targetColor = (isPressed || objectsOnPlate > 0) ? pressedColor : normalColor;
            plateMaterial.color = Color.Lerp(plateMaterial.color, targetColor, pressSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            objectsOnPlate++;

            if (objectsOnPlate == 1) // Nur beim ersten Objekt
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

            if (objectsOnPlate <= 0 && !stayPressed)
            {
                Release();
            }
        }
    }

    private void Press()
    {
        Debug.Log("Druckplatte aktiviert!");
        isPressed = stayPressed; // Bei stayPressed bleibt sie gedrückt

        // Sound abspielen
        if (pressSound != null)
        {
            AudioSource.PlayClipAtPoint(pressSound, transform.position, soundVolume);
        }

        // Alle verbundenen Türen öffnen
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

        // Sound abspielen
        if (releaseSound != null)
        {
            AudioSource.PlayClipAtPoint(releaseSound, transform.position, soundVolume);
        }

        // Alle verbundenen Türen schließen
        foreach (SlidingDoor door in connectedDoors)
        {
            if (door != null)
            {
                door.CloseDoor();
            }
        }
    }
}