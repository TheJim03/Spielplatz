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

    void OnGUI()
    {
        if (!crosshairTexture) return;

        bool lookingAtTarget = possessionManager && possessionManager.IsLookingAtTargetPublic();

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
}
