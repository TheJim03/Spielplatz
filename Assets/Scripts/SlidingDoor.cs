using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private float openHeight = 4f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool startOpen = false;

    [Header("Audio")]
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AudioSource doorAudioSource;
    [Range(0f, 1f)]
    [SerializeField] private float soundVolume = 0.7f;

    [Header("Visual Settings")]
    [SerializeField] private bool enableColorChange = true;
    [SerializeField] private Color closedColor = Color.red;
    [SerializeField] private Color openColor = Color.green;
    [SerializeField] private float colorChangeSpeed = 3f;

    [Header("Feedback Toggles")]
    [SerializeField] private bool enableMovement = true;
    [SerializeField] private bool enableAudio = true;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpen = false;
    private bool isMoving = false;
    private Renderer doorRenderer;
    private Material doorMaterial;

    void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + Vector3.up * openHeight;

        isOpen = startOpen;

        // Material-Setup für Farbwechsel
        doorRenderer = GetComponent<Renderer>();
        if (doorRenderer != null)
        {
            doorMaterial = new Material(doorRenderer.material);
            doorRenderer.material = doorMaterial;
            doorMaterial.color = isOpen ? openColor : closedColor;
        }

        // Prüfe Juiciness für Startposition
        bool juicy = JuicinessSettings.instance == null || JuicinessSettings.instance.IsJuicy;
        bool useAnimation = enableMovement && juicy;

        if (!useAnimation)
        {
            transform.position = isOpen ? openPosition : closedPosition;
        }
        else if (startOpen)
        {
            transform.position = openPosition;
        }

        if (enableAudio && doorAudioSource == null && (openSound != null || closeSound != null))
        {
            doorAudioSource = gameObject.AddComponent<AudioSource>();
            doorAudioSource.playOnAwake = false;
            doorAudioSource.spatialBlend = 1f;
        }
    }

    void Update()
    {
        Vector3 targetPos = isOpen ? openPosition : closedPosition;

        // Prüfe globale Juiciness-Einstellung
        bool juicy = JuicinessSettings.instance == null || JuicinessSettings.instance.IsJuicy;
        bool useAnimation = enableMovement && juicy;

        // Positions-Animation
        if (useAnimation)
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
            // Ohne Animation: direkt snappen
            if (transform.position != targetPos)
                transform.position = targetPos;

            isMoving = false;
        }

        // Farb-Animation (nur wenn Juiciness AN)
        if (enableColorChange && doorMaterial != null)
        {
            Color targetColor = isOpen ? openColor : closedColor;

            if (juicy)
            {
                // 🎨 Smooth Farbübergang
                doorMaterial.color = Color.Lerp(doorMaterial.color, targetColor, colorChangeSpeed * Time.deltaTime);
            }
            else
            {
                // 🧊 Sofort snappen
                doorMaterial.color = targetColor;
            }
        }
    }

    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;

            bool juicy = JuicinessSettings.instance == null || JuicinessSettings.instance.IsJuicy;
            if (!enableMovement || !juicy)
                transform.position = openPosition;

            if (enableAudio && juicy)
                PlaySound(openSound);

            Debug.Log("Tür öffnet sich!");
        }
    }

    public void CloseDoor()
    {
        if (isOpen)
        {
            isOpen = false;

            bool juicy = JuicinessSettings.instance == null || JuicinessSettings.instance.IsJuicy;
            if (!enableMovement || !juicy)
                transform.position = closedPosition;

            if (enableAudio && juicy)
                PlaySound(closeSound);

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