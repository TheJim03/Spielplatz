using UnityEngine;

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

    public bool IsJuicy => juicinessEnabled;

    private void Awake()
    {
        // Singleton pattern
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
    }

    public void SetJuiciness(bool enabled)
    {
        juicinessEnabled = enabled;
    }
}