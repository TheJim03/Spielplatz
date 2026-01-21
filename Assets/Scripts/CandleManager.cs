using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class CandleManager : MonoBehaviour
{
    public static CandleManager instance;

    [Header("Candle Tracking")]
    public int maxCandles = 6;  // später 6 einstellen (oder im Inspector)
    private int litCandles = 0;

    [Header("UI Display")]
    public TMP_Text candleCountText;

    [Header("UI Message")]
    public TMP_Text messageText;                 // kurzer Hinweis-Text
    public float messageDuration = 2f;           // wie lange eingeblendet
    public string allCandlesLitMessage = "Etwas öffnet sich...";

    [Header("Event - All Candles Lit")]
    public UnityEvent onAllCandlesLit;

    private bool eventTriggered = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Debug.Log("[CandleManager] Singleton initialisiert!");
        }
        else
        {
            Debug.LogWarning("[CandleManager] Zweite Instanz gefunden und zerstört!");
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        Debug.Log($"[CandleManager] Start - UI Text vorhanden: {candleCountText != null}");

        // Am Anfang soll die Anzeige komplett weg sein
        if (candleCountText != null)
        {
            candleCountText.gameObject.SetActive(false);
        }

        // Hinweis-Text ebenfalls aus
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }

        UpdateCandleUI();
    }

    /// <summary>
    /// Wird von CandleMovement aufgerufen wenn Kerze an/aus geht
    /// </summary>
    public void NotifyCandleStateChanged(bool isOn)
    {
        if (isOn)
        {
            litCandles++;
            Debug.Log($"[CandleManager] Kerze ANGEZÜNDET! Aktuelle Anzahl: {litCandles}/{maxCandles} 🔥");
        }
        else
        {
            litCandles--;
            Debug.Log($"[CandleManager] Kerze GELÖSCHT! Aktuelle Anzahl: {litCandles}/{maxCandles} ❄️");
        }

        // Sicherstellen, dass wir nicht über Max/unter 0 gehen
        litCandles = Mathf.Clamp(litCandles, 0, maxCandles);

        UpdateCandleUI();
        CheckAllCandlesLit();
    }

    void UpdateCandleUI()
    {
        if (candleCountText == null)
        {
            Debug.LogWarning("[CandleManager] candleCountText ist NULL! Bitte im Inspector zuweisen!");
            return;
        }

        int remainingCandles = Mathf.Clamp(maxCandles - litCandles, 0, maxCandles);

        // Bedingungen für Sichtbarkeit:
        // - Noch keine Kerze an -> Anzeige versteckt
        // - Alle Kerzen an -> Anzeige versteckt
        if (litCandles == 0 || remainingCandles == 0)
        {
            if (candleCountText.gameObject.activeSelf)
            {
                Debug.Log("[CandleManager] UI versteckt (0 oder alle Kerzen an).");
            }

            candleCountText.gameObject.SetActive(false);
            return;
        }

        // Ab der ersten Kerze bis kurz vor "alle an" sichtbar
        candleCountText.gameObject.SetActive(true);

        // Text: WIE VIELE NOCH FEHLEN
        candleCountText.text = $"Es fehlen noch {remainingCandles} Kerze(n)";
        Debug.Log($"[CandleManager] UI aktualisiert: Es fehlen noch {remainingCandles} Kerze(n)");
    }

    void CheckAllCandlesLit()
    {
        if (litCandles >= maxCandles && !eventTriggered)
        {
            eventTriggered = true;
            Debug.Log("[CandleManager] ⭐⭐⭐ ALLE KERZEN ANGEZÜNDET! ⭐⭐⭐ Event wird gefeuert!");
            onAllCandlesLit?.Invoke();

            // Sicherstellen, dass die Anzeige verschwindet, falls sie noch aktiv ist
            if (candleCountText != null)
            {
                candleCountText.gameObject.SetActive(false);
            }

            // Hinweis-Text kurz einblenden
            if (messageText != null)
            {
                StartCoroutine(ShowAllCandlesMessage());
            }
        }
        else
        {
            Debug.Log($"[CandleManager] Check: {litCandles}/{maxCandles} Kerzen an | Event getriggert: {eventTriggered}");
        }
    }

    private IEnumerator ShowAllCandlesMessage()
    {
        messageText.text = allCandlesLitMessage;
        messageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(messageDuration);

        messageText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Optional: Reset für Testing
    /// </summary>
    public void ResetCandles()
    {
        litCandles = 0;
        eventTriggered = false;

        if (candleCountText != null)
        {
            candleCountText.gameObject.SetActive(false);
        }

        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }

        UpdateCandleUI();
    }

    public int GetLitCandleCount() => litCandles;
    public bool AreAllCandlesLit() => litCandles >= maxCandles;
}
