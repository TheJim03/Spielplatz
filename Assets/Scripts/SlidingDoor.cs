using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private float openHeight = 4f; // Wie hoch die Tür sich öffnet
    [SerializeField] private float moveSpeed = 2f; // Geschwindigkeit der Bewegung
    [SerializeField] private bool startOpen = false; // Startet die Tür offen?

    [Header("Audio")]
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AudioSource doorAudioSource; // Optional: Eigene AudioSource
    [Range(0f, 1f)]
    [SerializeField] private float soundVolume = 0.7f;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpen = false;
    private bool isMoving = false;

    void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + Vector3.up * openHeight;

        if (startOpen)
        {
            transform.position = openPosition;
            isOpen = true;
        }

        // Erstelle AudioSource falls nicht vorhanden
        if (doorAudioSource == null && (openSound != null || closeSound != null))
        {
            doorAudioSource = gameObject.AddComponent<AudioSource>();
            doorAudioSource.playOnAwake = false;
            doorAudioSource.spatialBlend = 1f; // 3D Sound
        }
    }

    void Update()
    {
        // Smooth Bewegung zur Zielposition
        Vector3 targetPos = isOpen ? openPosition : closedPosition;

        if (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }

    public void OpenDoor()
    {
        if (!isOpen)
        {
            Debug.Log("Tür öffnet sich!");
            isOpen = true;
            PlaySound(openSound);
        }
    }

    public void CloseDoor()
    {
        if (isOpen)
        {
            Debug.Log("Tür schließt sich!");
            isOpen = false;
            PlaySound(closeSound);
        }
    }

    public void ToggleDoor()
    {
        if (isOpen)
            CloseDoor();
        else
            OpenDoor();
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            if (doorAudioSource != null)
            {
                doorAudioSource.volume = soundVolume;
                doorAudioSource.PlayOneShot(clip);
            }
            else
            {
                AudioSource.PlayClipAtPoint(clip, transform.position, soundVolume);
            }
        }
    }

    // Visualisierung im Editor
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            closedPosition = transform.position;
            openPosition = closedPosition + Vector3.up * openHeight;
        }

        // Zeige geschlossene Position
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(closedPosition, transform.localScale);

        // Zeige offene Position
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(openPosition, transform.localScale);

        // Verbindungslinie
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(closedPosition, openPosition);
    }
}