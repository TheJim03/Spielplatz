using UnityEngine;

public class V2SlidingDoor : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float raiseHeight = 2f;
    [SerializeField] private float moveSpeed = 2f;

    [Header("Audio")]
    [SerializeField] private AudioClip openingSound;
    [Range(0f, 1f)]
    [SerializeField] private float soundVolume = 0.7f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool opening = false;
    private AudioSource audioSource;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + Vector3.up * raiseHeight;

        // AudioSource Setup
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && openingSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f; // 3D Sound
        }

        StartCoroutine(ConnectToCandleManager());
    }

    private System.Collections.IEnumerator ConnectToCandleManager()
    {
        yield return null;

        if (CandleManager.instance != null)
        {
            CandleManager.instance.onAllCandlesLit.AddListener(OnAllCandlesLit);
            Debug.Log("[SlidingDoor] Verbindung zum CandleManager hergestellt");
        }
        else
        {
            Debug.LogError("[SlidingDoor] CandleManager nicht gefunden!");
        }
    }

    private void Update()
    {
        if (!opening) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            opening = false;
        }
    }

    private void OnAllCandlesLit()
    {
        Debug.Log("[SlidingDoor] Alle Kerzen an → Tür fährt hoch.");
        opening = true;

        // Sound abspielen (nur wenn Juiciness aktiviert)
        bool juicy = JuicinessSettings.instance == null || JuicinessSettings.instance.IsJuicy;
        if (juicy && openingSound != null && audioSource != null)
        {
            audioSource.volume = soundVolume;
            audioSource.PlayOneShot(openingSound);
            Debug.Log("[SlidingDoor] Opening Sound abgespielt 🔊");
        }
    }
}
