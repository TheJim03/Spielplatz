using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Macht ein Paper-Item draggable in der UI
/// </summary>
public class DraggablePaper : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int paperID; // Welches Paper ist das? (1-5)

    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Vector2 originalPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        // CanvasGroup hinzufügen falls nicht vorhanden
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Canvas finden (für Drag-Skalierung)
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            // Fallback: Root Canvas suchen
            canvas = Object.FindFirstObjectByType<Canvas>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"\ud83d\udc46 Beginne Drag von Snippet {paperID}");
        
        // Original-Position und Parent speichern
        originalParent = transform.parent;
        originalPosition = rectTransform.anchoredPosition;

        // Während Drag: Halbtransparent & raycast ignorieren
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        // Zu Canvas-Root verschieben (damit es über allem ist)
        if (canvas != null)
        {
            transform.SetParent(canvas.transform);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Paper folgt der Maus
        if (canvas != null)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
        else
        {
            rectTransform.anchoredPosition += eventData.delta;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Wieder sichtbar & raycast aktivieren
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Raycast durchführen um alle UI-Elemente unter der Maus zu finden
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // Finde den ERSTEN BookPageSlot (nicht mehrfach prüfen!)
        BookPageSlot targetSlot = null;
        
        foreach (var result in results)
        {
            BookPageSlot slot = result.gameObject.GetComponentInParent<BookPageSlot>();
            if (slot != null)
            {
                targetSlot = slot;
                break; // SOFORT aufhören nach dem ersten gefundenen Slot!
            }
        }

        // Wenn ein Slot gefunden wurde, versuche Paper zu platzieren
        if (targetSlot != null && targetSlot.AcceptPaper(paperID))
        {
            // Paper wurde erfolgreich in Slot platziert
            Debug.Log($"✅ Snippet {paperID} erfolgreich in Slot platziert!");
            
            // Snippet visuell im Slot platzieren
            transform.SetParent(targetSlot.transform);
            rectTransform.anchoredPosition = Vector2.zero; // Zentriert im Slot
            
            // Snippet nicht mehr draggable machen
            canvasGroup.blocksRaycasts = false;
            this.enabled = false; // Drag-Script deaktivieren
            
            // Inventar aktualisieren (entfernt dieses Snippet aus der Liste)
            BookUI bookUI = Object.FindFirstObjectByType<BookUI>();
            if (bookUI != null)
            {
                bookUI.UpdateInventory();
            }
        }
        else
        {
            // Falls nicht erfolgreich gedroppt: Zurück zur Original-Position
            Debug.Log($"❌ Snippet {paperID} nicht korrekt platziert - zurück ins Inventar");
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = originalPosition;
        }
    }
}
