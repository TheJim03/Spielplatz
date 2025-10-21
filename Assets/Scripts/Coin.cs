using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Coin Settings")]
    public int coinValue = 1;

    [Header("Visual Settings")]
    public float rotationSpeed = 90f;  // Geschwindigkeit der Rotation
    public float floatHeight = 0.5f;   // Wie hoch beim Einsammeln
    public float floatSpeed = 2f;      // Geschwindigkeit des Schwebens
    public bool enableFloating = false; // Sanftes Auf/Ab-Schweben aktivieren

    [Header("Audio")]
    public AudioClip collectSound;
    [Range(0f, 1f)] public float soundVolume = 1f;

    [Header("Effects")]
    public GameObject collectEffect;

    [Header("Feedback Toggles")]
    public bool enableAudioFeedback = true;     // ✅ Sound beim Einsammeln
    public bool enableVisualFeedback = true;    // ✅ Partikel/VFX beim Einsammeln
    public bool enableRotation = true;          // ✅ Rotation im Idle & Collect aktivieren
    public bool enableDisappearAnimation = true; // ✅ Hochfliegen + Verblassen aktivieren

    private bool isCollected = false;
    private Material coinMaterial;
    private Vector3 startPosition;

    void Start()
    {
        // Renderer vorbereiten (Kopie des Materials)
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            coinMaterial = new Material(renderer.material);
            renderer.material = coinMaterial;
        }

        startPosition = transform.position;
    }

    void Update()
    {
        if (isCollected) return;

        // 🔄 Rotation nur wenn aktiviert
        if (enableRotation)
        {
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime, Space.Self);
        }

        // 🌊 Optionales Schweben
        if (enableFloating)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * 0.2f;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            isCollected = true;
            StartCoroutine(CollectAnimation());
        }
    }

    private IEnumerator CollectAnimation()
    {
        // 1️⃣ Punkte hinzufügen
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddCoin(coinValue);
        }
        else
        {
            Debug.LogError("ScoreManager.instance ist NULL!");
        }

        // 2️⃣ Sound (wenn erlaubt)
        if (enableAudioFeedback && collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position, soundVolume);
        }

        // 3️⃣ Effekt (wenn erlaubt)
        if (enableVisualFeedback && collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        // 4️⃣ Collider ausschalten, damit sie nicht mehr sammelbar ist
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 5️⃣ Falls Verschwinde-Animation deaktiviert → direkt zerstören
        if (!enableDisappearAnimation)
        {
            Destroy(gameObject);
            yield break;
        }

        // 6️⃣ Animation: Nach oben schweben und verblassen
        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + Vector3.up * floatHeight;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            // Nach oben bewegen
            transform.position = Vector3.Lerp(startPos, targetPos, progress);

            // Verblassen (nur wenn Material existiert)
            if (coinMaterial != null)
            {
                Color color = coinMaterial.color;
                color.a = 1f - progress;
                coinMaterial.color = color;
            }

            // 🔄 Weiterdrehen beim Einsammeln (nur wenn aktiviert)
            if (enableRotation)
            {
                transform.Rotate(0f, rotationSpeed * 3f * Time.deltaTime, 0f, Space.Self);
            }

            yield return null;
        }

        // 7️⃣ Münze zerstören
        Destroy(gameObject);
    }
}
