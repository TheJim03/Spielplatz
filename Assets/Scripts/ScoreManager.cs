using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // WICHTIG: Für die Textanzeige (optional)

public class ScoreManager : MonoBehaviour
{
    // Singleton-Pattern: Macht dieses Script von überall leicht erreichbar
    public static ScoreManager instance;

    public int totalCoins = 0;

    // Optional: UI-Text zur Anzeige
    public TextMeshProUGUI coinText;

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
        // Optional: UI zu Beginn aktualisieren
        UpdateCoinText();
    }

    // Diese Funktion wird von der Münze aufgerufen
    public void AddCoin(int amount)
    {
        totalCoins += amount;
        Debug.Log("Münze eingesammelt! Gesamt: " + totalCoins);

        // Optional: UI aktualisieren
        UpdateCoinText();
    }

    // Optional: UI-Update-Funktion
    void UpdateCoinText()
    {
        if (coinText != null)
        {
            coinText.text = "Münzen: " + totalCoins;
        }
    }
}