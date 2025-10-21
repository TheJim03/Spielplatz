using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Coin Settings")]
    public int coinValue = 1;

    [Header("Visual Settings")]
    public float rotationSpeed = 90f; // Geschwindigkeit der Rotation um Y-Achse
    public float floatHeight = 0.5f; // Wie hoch beim Einsammeln
    public float floatSpeed = 2f; // Geschwindigkeit der Auf/Ab Bewegung (optional)
    public bool enableFloating = false; // Optional: Sanftes Auf/Ab schweben aktivieren

    [Header("Audio")]
    public AudioClip collectSound;
    [Range(0f, 1f)]
    public float soundVolume = 1f;

    [Header("Effects")]
    public GameObject collectEffect;

    private bool isCollected = false;
    private Material coinMaterial;
    private Vector3 startPosition;

    void Start()
    {
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
        if (!isCollected)
        {
            // Rotiere die Münze um die Y-Achse (dreht sich wie eine Münze)
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime, Space.Self);

            // Optional: Sanftes Auf und Ab schweben (deaktivierbar)
            if (enableFloating)
            {
                float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * 0.2f;
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger mit: " + other.gameObject.name + " (Tag: " + other.tag + ")");

        if (other.CompareTag("Player") && !isCollected)
        {
            Debug.Log("Münze wird eingesammelt!");
            isCollected = true;
            StartCoroutine(CollectAnimation());
        }
    }

    private IEnumerator CollectAnimation()
    {
        // 1. Zähler erhöhen
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddCoin(coinValue);
        }
        else
        {
            Debug.LogError("ScoreManager.instance ist NULL!");
        }

        // 2. Sound abspielen
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position, soundVolume);
        }

        // 3. Partikeleffekt
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        // 4. Animation: Nach oben schweben und verblassen
        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + Vector3.up * floatHeight;

        // Deaktiviere Collider
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            // Bewege nach oben
            transform.position = Vector3.Lerp(startPos, targetPos, progress);

            // Verblassen (Transparenz)
            if (coinMaterial != null)
            {
                Color color = coinMaterial.color;
                color.a = 1f - progress;
                coinMaterial.color = color;
            }

            // Weiter rotieren beim Einsammeln (um Y-Achse)
            transform.Rotate(0f, rotationSpeed * 3f * Time.deltaTime, 0f, Space.Self);

            yield return null;
        }

        // 5. Münze zerstören
        Destroy(gameObject);
    }
}