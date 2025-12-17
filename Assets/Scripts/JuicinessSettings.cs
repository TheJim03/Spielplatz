using UnityEngine;
using TMPro;

/// <summary>
/// Zentraler Manager für globale Juiciness-Einstellungen.
/// Immer aktiv, kein Toggle mehr per Input.
/// </summary>
public class JuicinessSettings : MonoBehaviour
{
    public static JuicinessSettings instance;

    [Header("Global Juiciness Settings")]
    [SerializeField] private bool juicinessEnabled = true; // immer AN

    [Header("UI-Anzeige")]
    [SerializeField] private TextMeshProUGUI juicinessText;

    [Header("Visuelle Verbesserungen (einfach)")]
    [SerializeField] private Light mainLight;
    [SerializeField] private Camera mainCamera;

    public bool IsJuicy => juicinessEnabled;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Sicherstellen, dass Juiciness wirklich AN ist
        juicinessEnabled = true;

        if (mainLight == null)
            mainLight = FindFirstObjectByType<Light>();

        if (mainCamera == null)
            mainCamera = Camera.main;

        UpdateVisuals();
        UpdateJuicinessText();
    }

    private void UpdateJuicinessText()
    {
        if (juicinessText != null)
        {
            juicinessText.text = "Juiciness: ON";
            juicinessText.color = Color.green;
        }
    }

    private void UpdateVisuals()
    {
        if (mainLight != null)
        {
            mainLight.intensity = 1.5f;
            mainLight.color = new Color(1f, 0.95f, 0.8f);
        }

        if (mainCamera != null)
        {
            mainCamera.backgroundColor = new Color(0.95f, 0.95f, 1f);
        }
    }
}
