using UnityEngine;

public class VanishDoor : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private bool startOpen = false;
    [SerializeField] private bool disableColliderWhenOpen = true;

    [Header("Audio")]
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AudioSource doorAudioSource;
    [Range(0f, 1f)]
    [SerializeField] private float soundVolume = 0.7f;

    [Header("Visual Settings")]
    [SerializeField] private bool enableColorChange = false;
    [SerializeField] private Color closedColor = Color.red;
    [SerializeField] private Color openColor = Color.green;
    [SerializeField] private float colorChangeSpeed = 3f;

    [Header("Feedback Toggles")]
    [SerializeField] private bool enableFadeAnimation = true;
    [SerializeField] private bool enableAudio = true;

    private bool isOpen = false;
    private Renderer doorRenderer;
    private Material doorMaterial;
    private Collider doorCollider;
    private float currentAlpha = 1f;
    private float targetAlpha = 1f;

    void Start()
    {
        isOpen = startOpen;

        // Material-Setup für Transparenz
        doorRenderer = GetComponent<Renderer>();
        doorCollider = GetComponent<Collider>();

        if (doorRenderer != null)
        {
            doorMaterial = new Material(doorRenderer.material);
            doorRenderer.material = doorMaterial;

            // Stelle sicher, dass Material Transparenz unterstützt
            EnableTransparency(doorMaterial);

            // Initiale Farbe und Alpha
            Color initColor = isOpen ? openColor : closedColor;
            currentAlpha = isOpen ? 0f : 1f;
            targetAlpha = currentAlpha;
            doorMaterial.color = new Color(initColor.r, initColor.g, initColor.b, currentAlpha);
        }

        // Collider initial state
        if (doorCollider != null && disableColliderWhenOpen && isOpen)
        {
            doorCollider.enabled = false;
        }

        // AudioSource Setup
        if (enableAudio && doorAudioSource == null && (openSound != null || closeSound != null))
        {
            doorAudioSource = gameObject.AddComponent<AudioSource>();
            doorAudioSource.playOnAwake = false;
            doorAudioSource.spatialBlend = 1f;
        }
    }

    void Update()
    {
        if (doorMaterial == null) return;

        // Prüfe globale Juiciness-Einstellung
        bool juicy = JuicinessSettings.instance == null || JuicinessSettings.instance.IsJuicy;
        bool useAnimation = enableFadeAnimation && juicy;

        // Alpha-Animation
        if (useAnimation)
        {
            // 🎮 Smooth Fade
            currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, (1f / fadeDuration) * Time.deltaTime);
        }
        else
        {
            // 🧊 Instant Snap
            currentAlpha = targetAlpha;
        }

        // Farb-Animation (optional)
        Color baseColor = enableColorChange ? (isOpen ? openColor : closedColor) : doorMaterial.color;

        if (enableColorChange && juicy)
        {
            // Smooth Farbübergang
            Color targetColor = isOpen ? openColor : closedColor;
            baseColor = Color.Lerp(doorMaterial.color, targetColor, colorChangeSpeed * Time.deltaTime);
        }

        // Setze finale Farbe mit Alpha
        doorMaterial.color = new Color(baseColor.r, baseColor.g, baseColor.b, currentAlpha);

        // Collider deaktivieren wenn komplett transparent
        if (doorCollider != null && disableColliderWhenOpen)
        {
            doorCollider.enabled = currentAlpha > 0.01f;
        }
    }

    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            targetAlpha = 0f;

            bool juicy = JuicinessSettings.instance == null || JuicinessSettings.instance.IsJuicy;

            // Ohne Animation: sofort setzen
            if (!enableFadeAnimation || !juicy)
            {
                currentAlpha = 0f;
                if (doorCollider != null && disableColliderWhenOpen)
                    doorCollider.enabled = false;
            }

            if (enableAudio && juicy)
                PlaySound(openSound);

            Debug.Log("Tür verschwindet!");
        }
    }

    public void CloseDoor()
    {
        if (isOpen)
        {
            isOpen = false;
            targetAlpha = 1f;

            bool juicy = JuicinessSettings.instance == null || JuicinessSettings.instance.IsJuicy;

            // Ohne Animation: sofort setzen
            if (!enableFadeAnimation || !juicy)
            {
                currentAlpha = 1f;
                if (doorCollider != null)
                    doorCollider.enabled = true;
            }

            if (enableAudio && juicy)
                PlaySound(closeSound);

            Debug.Log("Tür erscheint wieder!");
        }
    }

    public void ToggleDoor()
    {
        if (isOpen) CloseDoor();
        else OpenDoor();
    }

    private void PlaySound(AudioClip clip)
    {
        if (!enableAudio || clip == null) return;

        if (doorAudioSource != null)
        {
            doorAudioSource.volume = soundVolume;
            doorAudioSource.PlayOneShot(clip);
        }
        else
        {
            AudioSource.PlayClipAtPoint(clip, transform.position, soundVolume);
        }
    }

    // Hilfsmethode: Material auf Transparent-Modus setzen
    private void EnableTransparency(Material mat)
    {
        // Standard Shader / URP Lit Shader Transparenz aktivieren
        mat.SetFloat("_Surface", 1); // 0 = Opaque, 1 = Transparent (URP)
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }

    private void OnDrawGizmos()
    {
        // Visualisierung im Editor
        Gizmos.color = isOpen ? new Color(0, 1, 0, 0.3f) : new Color(1, 0, 0, 0.8f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}