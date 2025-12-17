using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Ein Slot im Buch wo eine bestimmte Seite reingehört
/// </summary>
public class BookPageSlot : MonoBehaviour, IDropHandler
{
    [Header("Slot Settings")]
    public int requiredPaperID; // Welches Paper gehört in diesen Slot? (1-5)
    
    [Header("Visual Feedback")]
    [SerializeField] private Image slotImage;
    [SerializeField] private Color emptyColor = new Color(1f, 1f, 1f, 0.3f);
    [SerializeField] private Color filledColor = Color.white;
    [SerializeField] private GameObject placeholderText; // "Seite X fehlt"
    
    [Header("Audio")]
    [SerializeField] private AudioClip correctPlaceSound;
    [SerializeField] private AudioClip wrongPlaceSound;
    
    public bool isFilled { get; private set; } = false; // Public getter, private setter

    void Start()
    {
        UpdateVisuals();
        
        // Sicherstellen dass der Slot Raycasts empfangen kann
        Image img = GetComponent<Image>();
        if (img != null)
        {
            img.raycastTarget = true;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        DraggablePaper draggedPaper = eventData.pointerDrag?.GetComponent<DraggablePaper>();
        
        if (draggedPaper != null)
        {
            AcceptPaper(draggedPaper.paperID);
        }
    }

    /// <summary>
    /// Versucht ein Paper in diesem Slot zu platzieren
    /// </summary>
    /// <returns>True wenn das Paper akzeptiert wurde</returns>
    public bool AcceptPaper(int paperID)
    {
        // Bereits gefüllt?
        if (isFilled)
        {
            PlaySound(wrongPlaceSound);
            Debug.Log($"Slot {requiredPaperID} ist bereits gefüllt!");
            return false;
        }

        // Falsches Paper?
        if (paperID != requiredPaperID)
        {
            PlaySound(wrongPlaceSound);
            Debug.Log($"Falsches Paper! Slot braucht Paper {requiredPaperID}, aber bekam {paperID}");
            return false;
        }

        // ✅ Korrektes Paper!
        isFilled = true;
        PlaySound(correctPlaceSound);
        UpdateVisuals();
        
        Debug.Log($"✅ Paper {paperID} korrekt in Slot {requiredPaperID} platziert!");
        
        // Prüfen ob Buch vollständig
        CheckIfBookComplete();
        
        return true;
    }

    private void UpdateVisuals()
    {
        if (slotImage != null)
        {
            slotImage.color = isFilled ? filledColor : emptyColor;
        }

        if (placeholderText != null)
        {
            placeholderText.SetActive(!isFilled);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, 0.5f);
        }
    }

    private void CheckIfBookComplete()
    {
        // Alle Slots im Buch pr\u00fcfen
        BookPageSlot[] allSlots = Object.FindObjectsByType<BookPageSlot>(FindObjectsSortMode.None);
        
        bool allFilled = true;
        foreach (var slot in allSlots)
        {
            if (!slot.isFilled)
            {
                allFilled = false;
                break;
            }
        }

        if (allFilled)
        {
            Debug.Log("🎉 BUCH VOLLSTÄNDIG! Alle Seiten sind platziert!");
            // Hier später: Animation, Level Complete, etc.
        }
    }

    // Debug-Funktion
    [ContextMenu("Reset Slot")]
    public void ResetSlot()
    {
        isFilled = false;
        UpdateVisuals();
    }
}
