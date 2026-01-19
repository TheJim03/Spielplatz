using UnityEngine;

[DisallowMultipleComponent]
public class LookHighlight : MonoBehaviour
{
    [Header("Renderer, die gehighlightet werden sollen")]
    public Renderer[] renderers;

    [Header("Material für Highlight-Zustand")]
    public Material highlightMaterial;

    Material[][] originalMaterials;
    bool initialized;
    bool isHighlighted;

    void Awake()
    {
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>();

        originalMaterials = new Material[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            // sharedMaterials, damit wir nicht zur Laufzeit Material-Kopien erstellen
            originalMaterials[i] = renderers[i].sharedMaterials;
        }

        initialized = true;
    }

    public void SetHighlighted(bool on)
    {
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
                // Alle Slots mit dem Highlight-Material belegen
                int count = originalMaterials[i].Length;
                Material[] mats = new Material[count];
                for (int m = 0; m < count; m++)
                    mats[m] = highlightMaterial;

                r.sharedMaterials = mats;
            }
            else
            {
                // Original wiederherstellen
                r.sharedMaterials = originalMaterials[i];
            }
        }
    }
}
