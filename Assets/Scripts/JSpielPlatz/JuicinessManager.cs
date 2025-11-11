using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JuicinessManager : MonoBehaviour
{
    public KeyCode toggleKey = KeyCode.J;
    public string prefsKey = "Juiciness_On";
    public Text uiText; // assign in inspector (JuiceText)
    public bool startOn = true;

    bool isJuicy = true;
    List<IJuicy> registered = new List<IJuicy>();

    void Awake()
    {
        // Lade gespeicherten Zustand (0/1) falls vorhanden
        if (PlayerPrefs.HasKey(prefsKey))
            isJuicy = PlayerPrefs.GetInt(prefsKey) == 1;
        else
            isJuicy = startOn;

        // Find all JuicyComponents in scene and register
        JuicyComponent[] comps = FindObjectsOfType<JuicyComponent>();
        foreach (var c in comps) Register(c);

        ApplyState();
        UpdateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleJuiciness();
        }
    }

    public void Register(IJuicy juicy)
    {
        if (!registered.Contains(juicy)) registered.Add(juicy);
    }

    public void ToggleJuiciness()
    {
        isJuicy = !isJuicy;
        PlayerPrefs.SetInt(prefsKey, isJuicy ? 1 : 0);
        PlayerPrefs.Save();
        ApplyState();
        UpdateUI();
    }

    void ApplyState()
    {
        foreach (var r in registered)
            r.SetJuicy(isJuicy);

        // Globale illumination tweak: simple example — lower Ambient intensity when not juicy
        if (!isJuicy)
        {
            RenderSettings.ambientIntensity = 0.4f;
        }
        else
        {
            RenderSettings.ambientIntensity = 1f;
        }
    }

    void UpdateUI()
    {
        if (uiText != null)
            uiText.text = "Juicyness: " + (isJuicy ? "ON" : "OFF") + "  (J zum umschalten)";
    }

    public bool IsJuicy() => isJuicy;
}