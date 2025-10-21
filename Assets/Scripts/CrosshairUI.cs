using UnityEngine;

public class CrosshairUI : MonoBehaviour
{
    [Header("Crosshair Settings")]
    public Texture2D crosshairTexture;
    public float size = 32f;
    public Color normalColor = Color.white;
    public Color targetColor = Color.red;

    [Header("Dependencies")]
    public PossessionManager possessionManager;

    void OnGUI()
    {
        if (!crosshairTexture) return;

        bool lookingAtTarget = possessionManager && possessionManager.IsLookingAtTargetPublic();

        float x = (Screen.width - size) / 2f;
        float y = (Screen.height - size) / 2f;
        Color prevColor = GUI.color;

        GUI.color = lookingAtTarget ? targetColor : normalColor;
        GUI.DrawTexture(new Rect(x, y, size, size), crosshairTexture);

        GUI.color = prevColor;
    }
}
