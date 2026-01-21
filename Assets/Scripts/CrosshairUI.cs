using UnityEngine;

public class CrosshairUI : MonoBehaviour
{
    [Header("Crosshair Settings")]
    public Texture2D crosshairTexture;
    public float size = 32f;
    public Color normalColor = Color.white;
    public Color targetColor = Color.red;

    [Header("Behavior Options")]
    public bool enableColorChange = true;   // lokaler Schalter

    [Header("Dependencies")]
    public PossessionManager possessionManager;

    [Header("Interaction Detection")]
    public float raycastDistance = 3f;
    public LayerMask interactableLayer = ~0;

    void OnGUI()
    {
        if (!crosshairTexture) return;

        bool lookingAtTarget = IsLookingAtInteractable();

        // globaler Juicy-Schalter (J) – fehlt er, default = true
        bool juicyOn = JuicinessSettings.instance == null || JuicinessSettings.instance.IsJuicy;

        float x = (Screen.width - size) / 2f;
        float y = (Screen.height - size) / 2f;

        var prev = GUI.color;
        if (enableColorChange && juicyOn && lookingAtTarget)
            GUI.color = targetColor;
        else
            GUI.color = normalColor;

        GUI.DrawTexture(new Rect(x, y, size, size), crosshairTexture);
        GUI.color = prev;
    }

    private bool IsLookingAtInteractable()
    {
        // 1. Prüfe PossessionManager (Kerzen etc.)
        if (possessionManager && possessionManager.IsLookingAtTargetPublic())
            return true;

        // 2. Prüfe ob wir auf ein Paper schauen
        Camera cam = Camera.main;
        if (cam == null) return false;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance, interactableLayer))
        {
            // Paper oder andere Objekte mit LookHighlight
            if (hit.collider.GetComponent<Paper>() != null)
                return true;

            if (hit.collider.GetComponent<LookHighlight>() != null)
                return true;
        }

        return false;
    }
}
