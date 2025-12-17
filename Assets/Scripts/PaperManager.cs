using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PaperManager : MonoBehaviour
{
    public static PaperManager instance;

    [Header("Paper Collection Settings")]
    public int maxPapers = 4; // Maximal 4 Paper-Snippets
    private HashSet<int> collectedPapers = new HashSet<int>(); // Welche Papers wurden gesammelt?

    [Header("UI (optional)")]
    public TextMeshProUGUI paperText; // Zeigt z.B. "Papers: 3/5"

    void Awake()
    {
        // Singleton-Setup
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdatePaperUI();
    }

    /// <summary>
    /// Fügt ein Paper-Fragment zur Sammlung hinzu
    /// </summary>
    public void CollectPaper(int paperID)
    {
        if (collectedPapers.Contains(paperID))
        {
            Debug.LogWarning($"Paper {paperID} wurde bereits eingesammelt!");
            return;
        }

        collectedPapers.Add(paperID);
        Debug.Log($"Snippet {paperID} eingesammelt! Gesamt: {collectedPapers.Count}/{maxPapers}");

        UpdatePaperUI();
        
        // NICHT hier prüfen ob Buch komplett - nur beim Platzieren!
    }

    /// <summary>
    /// Prüft ob alle Paper-Fragmente gesammelt wurden
    /// </summary>
    public bool IsBookComplete()
    {
        return collectedPapers.Count >= maxPapers;
    }

    /// <summary>
    /// Wird aufgerufen wenn alle Papers gesammelt wurden
    /// </summary>
    private void OnBookComplete()
    {
        Debug.Log("🎉 ALLE SNIPPETS GESAMMELT! Das Buch ist vollständig!");
        // Hier später: Buch-UI öffnen, Level abschließen, etc.
    }

    /// <summary>
    /// Prüft ob ein bestimmtes Paper bereits gesammelt wurde
    /// </summary>
    public bool HasPaper(int paperID)
    {
        return collectedPapers.Contains(paperID);
    }

    /// <summary>
    /// Gibt die Anzahl der gesammelten Papers zurück
    /// </summary>
    public int GetCollectedCount()
    {
        return collectedPapers.Count;
    }

    /// <summary>
    /// Aktualisiert die UI-Anzeige
    /// </summary>
    void UpdatePaperUI()
    {
        if (paperText != null)
        {
            paperText.text = $"Seiten: {collectedPapers.Count}/{maxPapers}";
        }
    }

    // Debug-Funktion: Alle Papers zurücksetzen
    [ContextMenu("Reset All Papers")]
    public void ResetPapers()
    {
        collectedPapers.Clear();
        Debug.Log("Alle Papers zurückgesetzt!");
        UpdatePaperUI();
    }
}
