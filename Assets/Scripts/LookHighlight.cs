using UnityEngine;
using TMPro;

[DisallowMultipleComponent]
public class LookHighlight : MonoBehaviour
{
    [Header("Renderer, die gehighlightet werden sollen")]
    public Renderer[] renderers;

    [Header("Material für Highlight-Zustand")]
    public Material highlightMaterial;

    [Header("UI Text, der beim Hover angezeigt wird")]
    public TMP_Text hoverText;

    [TextArea]
    public string hoverMessage = "Press Right Click to change";

    // 🔒 Original-Materialien der Renderer
    Material[][] originalMaterials;

    bool initialized;
    bool isHighlighted;

    void Awake()
    {
        // Falls keine Renderer gesetzt wurden → automatisch alle Kinder nehmen
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>();

        // Speicher für Original-Materialien vorbereiten
        originalMaterials = new Material[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            // sharedMaterials, damit keine Runtime-Materialkopien entstehen
            originalMaterials[i] = renderers[i].sharedMaterials;
        }

        // Hover-Text standardmäßig ausblenden
        if (hoverText)
            hoverText.gameObject.SetActive(false);

        initialized = true;
    }

    /// <summary>
    /// Wird vom Look/Raycast-System aufgerufen,
    /// wenn das Objekt angeschaut oder verlassen wird
    /// </summary>
    public void SetHighlighted(bool on)
    {
        // Keine unnötigen Updates
        if (!initialized || isHighlighted == on) return;
        isHighlighted = on;

        if (highlightMaterial == null)
        {
            Debug.LogWarning("[LookHighlight] Kein HighlightMaterial gesetzt.", this);
            return;
        }

        for (int i = 0; i < renderers.Length; i++)
        {
            var r = renderers[i];
            if (r == null) continue;

            if (on)
            {
                // 👉 Alle Material-Slots mit Highlight-Material belegen
                int count = originalMaterials[i].Length;
                Material[] mats = new Material[count];

                for (int m = 0; m < count; m++)
                    mats[m] = highlightMaterial;

                r.sharedMaterials = mats;
            }
            else
            {
                // 👉 Original-Materialien wiederherstellen
                r.sharedMaterials = originalMaterials[i];
            }
        }

        // ---------- Hover-Text steuern ----------
        if (!hoverText) {return;}

        if (on)
        {
            hoverText.text = hoverMessage;
            hoverText.gameObject.SetActive(true);
        }
        else
        {
            hoverText.gameObject.SetActive(false);
        }
    }
}
