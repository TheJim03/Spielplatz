using System.Collections;
using UnityEngine;

public class Paper : MonoBehaviour
{
    [Header("Paper Settings")]
    public int paperID = 1; // Welches Snippet ist das? (1-4)

    [Header("Audio")]
    public AudioClip collectSound;
    [Range(0f, 1f)] public float soundVolume = 1f;

    [Header("Pickup Animation")]
    public float pickupDuration = 0.5f;
    public float floatHeight = 1f;

    [Header("Interaction")]
    public float interactionDistance = 3f;

    private bool isCollected = false;
    private bool isPlayerLookingAt = false;
    private LookHighlight lookHighlight;

    private void Awake()
    {
        lookHighlight = GetComponent<LookHighlight>();
    }

    private void Update()
    {
        if (isCollected) return;

        // Raycast von Main Camera
        Camera cam = Camera.main;
        if (cam == null) return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        // Prüfen ob Spieler auf dieses Paper schaut
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.gameObject == gameObject)
            {
                if (!isPlayerLookingAt)
                {
                    isPlayerLookingAt = true;
                    // Highlight aktivieren
                    if (lookHighlight != null)
                        lookHighlight.SetHighlighted(true);
                }

                // E-Taste zum Einsammeln
                if (Input.GetKeyDown(KeyCode.E))
                {
                    isCollected = true;
                    StartCoroutine(CollectPaper());
                }
            }
            else
            {
                if (isPlayerLookingAt)
                {
                    isPlayerLookingAt = false;
                    // Highlight deaktivieren
                    if (lookHighlight != null)
                        lookHighlight.SetHighlighted(false);
                }
            }
        }
        else
        {
            if (isPlayerLookingAt)
            {
                isPlayerLookingAt = false;
                // Highlight deaktivieren
                if (lookHighlight != null)
                    lookHighlight.SetHighlighted(false);
            }
        }
    }

    // Optional: Visuelles Feedback wenn Spieler draufschaut
    private void OnDrawGizmos()
    {
        if (isPlayerLookingAt)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }

    private IEnumerator CollectPaper()
    {
        bool juicy = JuicinessSettings.instance != null && JuicinessSettings.instance.IsJuicy;

        // 1️⃣ Snippet zum Manager hinzufügen
        if (PaperManager.instance != null)
        {
            PaperManager.instance.CollectPaper(paperID);
        }

        // 2️⃣ Sound abspielen (immer, wenn Juicy AN)
        if (juicy && collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position, soundVolume);
        }

        // 3️⃣ Collider ausschalten
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 4️⃣ Aufnahme-Animation: Paper schwebt zum Spieler hoch
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + Vector3.up * floatHeight;

        while (elapsed < pickupDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / pickupDuration;

            // Sanftes Hochschweben
            transform.position = Vector3.Lerp(startPos, targetPos, progress);

            // Optional: Leicht rotieren während Aufnahme
            transform.Rotate(Vector3.up, 180f * Time.deltaTime, Space.World);

            yield return null;
        }

        // 5️⃣ Paper zerstören
        Destroy(gameObject);
    }
}
