using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Coin Settings")]
    public int coinValue = 1;

    [Header("Visual Settings")]
    public float rotationSpeed = 90f;
    public float floatHeight = 0.5f;
    public float floatSpeed = 2f;
    public bool enableFloating = false;

    [Header("Audio")]
    public AudioClip collectSound;
    [Range(0f, 1f)] public float soundVolume = 1f;

    [Header("Effects")]
    public GameObject collectEffect;

    [Header("Feedback Toggles")]
    public bool enableAudioFeedback = true;
    public bool enableVisualFeedback = true;
    public bool enableRotation = true;
    public bool enableDisappearAnimation = true;

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
        if (isCollected) return;

        bool juicy = JuicinessSettings.instance != null && JuicinessSettings.instance.IsJuicy;

        // 🔄 Rotation NUR wenn Juiciness AN
        if (enableRotation && juicy)
        {
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime, Space.Self);
        }

        // 🎈 Schweben NUR wenn Juiciness AN
        if (enableFloating && juicy)
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
        bool juicy = JuicinessSettings.instance != null && JuicinessSettings.instance.IsJuicy;

        // 1️⃣ Punkte hinzufügen (immer)
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddCoin(coinValue);
        }

        // 2️⃣ Sound NUR wenn Juiciness AN
        if (enableAudioFeedback && juicy && collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position, soundVolume);
        }

        // 3️⃣ Effekt NUR wenn Juiciness AN
        if (enableVisualFeedback && juicy && collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        // 4️⃣ Collider ausschalten
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 5️⃣ Ohne Juiciness: Sofort zerstören (keine Animation)
        if (!enableDisappearAnimation || !juicy)
        {
            Destroy(gameObject);
            yield break;
        }

        // 6️⃣ Juicy Animation: Schweben + Verblassen
        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + Vector3.up * floatHeight;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            transform.position = Vector3.Lerp(startPos, targetPos, progress);

            if (coinMaterial != null)
            {
                Color color = coinMaterial.color;
                color.a = 1f - progress;
                coinMaterial.color = color;
            }

            if (enableRotation)
            {
                transform.Rotate(0f, rotationSpeed * 3f * Time.deltaTime, 0f, Space.Self);
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}