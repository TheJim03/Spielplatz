using UnityEngine;
using TMPro;

public class CandleMovement : MonoBehaviour
{
    [Header("Candle Objects")]
    public GameObject flame;
    public GameObject lightObject;
    public GameObject particles;

    [Header("UI Hint")]
    public TMP_Text hintText;              // Press E
    public TMP_Text shiftHintText;         // NEW: Press Shift

    [Header("Audio")]
    [SerializeField] private AudioClip lightSound;     // Sound beim Anzünden
    [SerializeField] private AudioClip extinguishSound; // Sound beim Ausmachen
    [Range(0f, 1f)]
    [SerializeField] private float soundVolume = 0.7f;

    bool isOn = false;
    private AudioSource audioSource;

    // 🔒 global – Tutorial darf NIE wieder erscheinen
    static bool eHintConsumed = false;
    static bool shiftHintConsumed = false; // NEW

    bool hintActive = false;
    bool shiftHintActive = false;          // NEW

    void Start()
    {
        if (hintText) hintText.gameObject.SetActive(false);
        if (shiftHintText) shiftHintText.gameObject.SetActive(false); // NEW

        // AudioSource Setup
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (lightSound != null || extinguishSound != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f; // 3D Sound
        }
    }

    void Update()
    {
        // Nur aktive Kerze
        if (!CompareTag("Candle"))
        {
            // 👉 Kerze verlassen → Texte ausblenden
            if (hintActive)
            {
                HideHint();
                hintActive = false;
                eHintConsumed = true;
            }

            if (shiftHintActive)
            {
                HideShiftHint();
                shiftHintActive = false;
            }

            return;
        }

        // 👉 Beim ERSTEN Besitz anzeigen
        if (!eHintConsumed && !hintActive)
        {
            ShowHint();
            hintActive = true;
        }

        if (!shiftHintConsumed && !shiftHintActive)
        {
            ShowShiftHint();        // NEW
            shiftHintActive = true;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (hintActive)
            {
                HideHint();
                hintActive = false;
                eHintConsumed = true;
            }

            Toggle();
        }
    }

    // 👉 Wird aufgerufen, wenn du dich zurückverwandelt hast
    void OnDisable()
    {
        if (shiftHintActive)
        {
            HideShiftHint();
            shiftHintActive = false;
            shiftHintConsumed = true; // NEW: nie wieder anzeigen
        }
    }

    void Toggle()
    {
        isOn = !isOn;
        SetState(isOn);

        // Manager benachrichtigen
        if (CandleManager.instance != null)
        {
            Debug.Log($"[CandleMovement] Toggle auf {(isOn ? "AN" : "AUS")} - Benachrichtige Manager");
            CandleManager.instance.NotifyCandleStateChanged(isOn);
        }
        else
        {
            Debug.LogError("[CandleMovement] CandleManager.instance ist NULL!");
        }
    }

    void SetState(bool state)
    {
        if (flame) flame.SetActive(state);
        if (lightObject) lightObject.SetActive(state);
        if (particles) particles.SetActive(state);

        // Sound abspielen (nur wenn Juiciness aktiviert)
        bool juicy = JuicinessSettings.instance == null || JuicinessSettings.instance.IsJuicy;
        if (juicy && audioSource != null)
        {
            AudioClip clipToPlay = state ? lightSound : extinguishSound;
            if (clipToPlay != null)
            {
                audioSource.volume = soundVolume;
                audioSource.PlayOneShot(clipToPlay);
                Debug.Log($"[CandleMovement] Sound abgespielt: {(state ? "ANZÜNDEN" : "AUSMACHEN")} 🔊");
            }
        }
    }

    void ShowHint()
    {
        if (!hintText) return;
        hintText.text = "Press E to light yourself";
        hintText.gameObject.SetActive(true);
    }

    void HideHint()
    {
        if (hintText)
            hintText.gameObject.SetActive(false);
    }

    // ---------- NEW: Shift Hint ----------

    void ShowShiftHint()
    {
        if (!shiftHintText) return;
        shiftHintText.text = "Press Shift to change back";
        shiftHintText.gameObject.SetActive(true);
    }

    void HideShiftHint()
    {
        if (shiftHintText)
            shiftHintText.gameObject.SetActive(false);
    }
}
