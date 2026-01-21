using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class CandleManager : MonoBehaviour
{
    public static CandleManager instance;

    [Header("Candle Tracking")]
    public int maxCandles = 2;  // Für Testing auf 2 gesetzt (später: 5)
    private int litCandles = 0;

    [Header("UI Display")]
    public TMP_Text candleCountText;

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

        // Sicherstellen dass wir nicht über Max gehen
        litCandles = Mathf.Clamp(litCandles, 0, maxCandles);

        UpdateCandleUI();
        CheckAllCandlesLit();
    }

    void UpdateCandleUI()
    {
        if (candleCountText != null)
        {
            candleCountText.text = $"Kerzen: {litCandles}/{maxCandles}";
            Debug.Log($"[CandleManager] UI aktualisiert: Kerzen: {litCandles}/{maxCandles}");
        }
        else
        {
            Debug.LogWarning("[CandleManager] candleCountText ist NULL! Bitte im Inspector zuweisen!");
        }
    }

    void CheckAllCandlesLit()
    {
        if (litCandles >= maxCandles && !eventTriggered)
        {
            eventTriggered = true;
            Debug.Log("[CandleManager] ⭐⭐⭐ ALLE KERZEN ANGEZÜNDET! ⭐⭐⭐ Event wird gefeuert!");
            onAllCandlesLit?.Invoke();
        }
        else
        {
            Debug.Log($"[CandleManager] Check: {litCandles}/{maxCandles} Kerzen an | Event getriggert: {eventTriggered}");
        }
    }

    /// <summary>
    /// Optional: Reset für Testing
    /// </summary>
    public void ResetCandles()
    {
        litCandles = 0;
        eventTriggered = false;
        UpdateCandleUI();
    }

    /// <summary>
    /// Debug Info
    /// </summary>
    public int GetLitCandleCount() => litCandles;
    public bool AreAllCandlesLit() => litCandles >= maxCandles;
}
