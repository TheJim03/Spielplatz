using UnityEngine;
using TMPro;

/// <summary>
/// Zentraler Manager für globale Juiciness-Einstellungen.
/// Kann per Tastendruck (default: J) ein-/ausgeschaltet werden.
/// </summary>
public class JuicinessSettings : MonoBehaviour
{
    public static JuicinessSettings instance;

    [Header("Global Juiciness Settings")]
    [SerializeField] private bool juicinessEnabled = true;

    [Header("Input")]
    [SerializeField] private KeyCode toggleKey = KeyCode.J;

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
        if (mainLight == null)
            mainLight = FindFirstObjectByType<Light>();

        if (mainCamera == null)
            mainCamera = Camera.main;

        UpdateVisuals();
        UpdateJuicinessText();
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleJuiciness();
        }
    }

    public void ToggleJuiciness()
    {
        juicinessEnabled = !juicinessEnabled;
        Debug.Log($"Juiciness ist jetzt: {(juicinessEnabled ? "AN" : "AUS")}");
        UpdateJuicinessText();
        UpdateVisuals();
    }

    private void UpdateJuicinessText()
    {
        if (juicinessText != null)
        {
            juicinessText.text = "Juiciness: " + (juicinessEnabled ? "ON" : "OFF");
            juicinessText.color = juicinessEnabled ? Color.green : Color.red;
        }
    }

    private void UpdateVisuals()
    {
        // einfache visuelle “Juiciness”-Änderungen
        if (mainLight != null)
        {
            mainLight.intensity = juicinessEnabled ? 1.5f : 1f;
            mainLight.color = juicinessEnabled ? new Color(1f, 0.95f, 0.8f) : Color.white;
        }

        if (mainCamera != null)
        {
            mainCamera.backgroundColor = juicinessEnabled ? new Color(0.95f, 0.95f, 1f) : Color.gray;
        }
    }
}
