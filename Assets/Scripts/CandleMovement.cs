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

    bool isOn = false;

    // 🔒 global – Tutorial darf NIE wieder erscheinen
    static bool eHintConsumed = false;
    static bool shiftHintConsumed = false; // NEW

    bool hintActive = false;
    bool shiftHintActive = false;          // NEW

    void Start()
    {
        if (hintText) hintText.gameObject.SetActive(false);
        if (shiftHintText) shiftHintText.gameObject.SetActive(false); // NEW
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
    }

    void SetState(bool state)
    {
        if (flame) flame.SetActive(state);
        if (lightObject) lightObject.SetActive(state);
        if (particles) particles.SetActive(state);
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
