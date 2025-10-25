using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private float openHeight = 4f;   // Wie hoch die Tür sich öffnet
    [SerializeField] private float moveSpeed = 2f;    // Geschwindigkeit der Bewegung
    [SerializeField] private bool startOpen = false;  // Startet die Tür offen?

    [Header("Audio")]
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AudioSource doorAudioSource; // Optional: Eigene AudioSource
    [Range(0f, 1f)]
    [SerializeField] private float soundVolume = 0.7f;

    [Header("Feedback Toggles")]
    [SerializeField] private bool enableMovement = true; // ✅ Animation ein/aus
    [SerializeField] private bool enableAudio = true;    // ✅ Sound ein/aus

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpen = false;
    private bool isMoving = false;

    void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + Vector3.up * openHeight;

        // Startzustand
        isOpen = startOpen;
        if (!enableMovement)
        {
            // direkt an Zielposition snappen, wenn Animation aus
            transform.position = isOpen ? openPosition : closedPosition;
        }
        else if (startOpen)
        {
            transform.position = openPosition;
        }

        // AudioSource nur anlegen, wenn Audio aktiv und Clips vorhanden
        if (enableAudio && doorAudioSource == null && (openSound != null || closeSound != null))
        {
            doorAudioSource = gameObject.AddComponent<AudioSource>();
            doorAudioSource.playOnAwake = false;
            doorAudioSource.spatialBlend = 1f; // 3D Sound
        }
    }

    void Update()
    {
        Vector3 targetPos = isOpen ? openPosition : closedPosition;

        if (enableMovement)
        {
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
        else
        {
            // Ohne Animation: immer direkt am Ziel stehen
            if (transform.position != targetPos)
                transform.position = targetPos;

            isMoving = false;
        }
    }

    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            if (!enableMovement) transform.position = openPosition; // snap
            if (enableAudio) PlaySound(openSound);
            Debug.Log("Tür öffnet sich!");
        }
    }

    public void CloseDoor()
    {
        if (isOpen)
        {
            isOpen = false;
            if (!enableMovement) transform.position = closedPosition; // snap
            if (enableAudio) PlaySound(closeSound);
            Debug.Log("Tür schließt sich!");
        }
    }

    public void ToggleDoor()
    {
        if (isOpen) CloseDoor();
        else OpenDoor();
    }

    private void PlaySound(AudioClip clip)
    {
        if (!enableAudio || clip == null) return;

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

    // Visualisierung im Editor
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            closedPosition = transform.position;
            openPosition = closedPosition + Vector3.up * openHeight;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(closedPosition, transform.localScale);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(openPosition, transform.localScale);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(closedPosition, openPosition);
    }
}
