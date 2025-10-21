using UnityEngine;

public class CrosshairUI : MonoBehaviour
{
    [Header("Crosshair Settings")]
    public Texture2D crosshairTexture;
    public float size = 32f;
    public Color normalColor = Color.white;
    public Color targetColor = Color.red;

    [Header("Behavior Options")]
    public bool enableColorChange = true;   // ✅ Haken zum Ein-/Ausschalten der Farbänderung

    [Header("Dependencies")]
    public PossessionManager possessionManager;

    void OnGUI()
    {
        if (!crosshairTexture) return;

        bool lookingAtTarget = possessionManager && possessionManager.IsLookingAtTargetPublic();

        float x = (Screen.width - size) / 2f;
        float y = (Screen.height - size) / 2f;
        Color prevColor = GUI.color;

        // Nur färben, wenn erlaubt
        if (enableColorChange && lookingAtTarget)
            GUI.color = targetColor;
        else
            GUI.color = normalColor;

        GUI.DrawTexture(new Rect(x, y, size, size), crosshairTexture);
        GUI.color = prevColor;
    }
}
