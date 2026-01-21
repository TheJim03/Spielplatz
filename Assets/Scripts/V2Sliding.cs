using UnityEngine;

public class V2SlidingDoor : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float raiseHeight = 2f;
    [SerializeField] private float moveSpeed = 2f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool opening = false;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + Vector3.up * raiseHeight;

        StartCoroutine(ConnectToCandleManager());
    }

    private System.Collections.IEnumerator ConnectToCandleManager()
    {
        yield return null;

        if (CandleManager.instance != null)
        {
            CandleManager.instance.onAllCandlesLit.AddListener(OnAllCandlesLit);
            Debug.Log("[SlidingDoor] Verbindung zum CandleManager hergestellt");
        }
        else
        {
            Debug.LogError("[SlidingDoor] CandleManager nicht gefunden!");
        }
    }

    private void Update()
    {
        if (!opening) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            opening = false;
        }
    }

    private void OnAllCandlesLit()
    {
        Debug.Log("[SlidingDoor] Alle Kerzen an → Tür fährt hoch.");
        opening = true;
    }
}
