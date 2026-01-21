using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Hauptsteuerung für die Buch-UI. Öffnet/schließt mit B-Taste.
/// </summary>
public class BookUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject bookPanel; // Das Haupt-UI Panel
    [SerializeField] private Transform inventoryContainer; // Container für gesammelte Papers (links)
    [SerializeField] private Transform bookPageContainer; // Container für die 5 Buch-Slots (rechts)
    
    [Header("Prefabs")]
    [SerializeField] private GameObject paperItemPrefab; // Prefab für draggable Paper-Items
    
    [Header("Settings")]
    [SerializeField] private KeyCode toggleKey = KeyCode.B;
    
    private bool isOpen = false;
    
    private float previousTimeScale = 1f;

    void Start()
    {
        // UI zu Beginn verstecken
        if (bookPanel != null)
        {
            bookPanel.SetActive(false);
        }
    }

    void Update()
    {
        // B-Taste zum Öffnen/Schließen
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleBookUI();
        }
    }

    public void ToggleBookUI()
    {
        isOpen = !isOpen;
        
        if (bookPanel != null)
        {
            bookPanel.SetActive(isOpen);
        }

        // Cursor sichtbar machen wenn UI offen
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen;

        // Wenn UI geöffnet wird, Papers im Inventar aktualisieren
        if (isOpen)
        {
            previousTimeScale = Time.timeScale; // merken
            Time.timeScale = 0f;
            RefreshInventory();
        }
        else
        {
            Time.timeScale = previousTimeScale;
        }
    }

    /// <summary>
    /// Aktualisiert das Inventar mit allen gesammelten Papers
    /// </summary>
    private void RefreshInventory()
    {
        if (inventoryContainer == null || paperItemPrefab == null || PaperManager.instance == null)
            return;

        // Alte Items löschen
        foreach (Transform child in inventoryContainer)
        {
            Destroy(child.gameObject);
        }

        // Für jedes gesammelte Paper ein draggable Item erstellen
        for (int i = 1; i <= PaperManager.instance.maxPapers; i++)
        {
            if (PaperManager.instance.HasPaper(i))
            {
                GameObject paperItem = Instantiate(paperItemPrefab, inventoryContainer);
                
                // Paper-Item konfigurieren
                DraggablePaper draggable = paperItem.GetComponent<DraggablePaper>();
                if (draggable != null)
                {
                    draggable.paperID = i;
                }

                // Text setzen (TMPro Support)
                TMPro.TextMeshProUGUI tmpText = paperItem.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (tmpText != null)
                {
                    tmpText.text = $"{i}"; // Nur die Nummer anzeigen
                }
                else
                {
                    // Fallback für normales UI.Text
                    UnityEngine.UI.Text text = paperItem.GetComponentInChildren<UnityEngine.UI.Text>();
                    if (text != null)
                    {
                        text.text = $"{i}";
                    }
                }
            }
        }
    }

    public void CloseBookUI()
    {
        isOpen = false;
        if (bookPanel != null)
        {
            bookPanel.SetActive(false);
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = previousTimeScale;
    }

    /// <summary>
    /// Aktualisiert das Inventar (wird nach erfolgreichem Drop aufgerufen)
    /// </summary>
    public void UpdateInventory()
    {
        if (isOpen)
        {
            RefreshInventory();
        }
    }
}