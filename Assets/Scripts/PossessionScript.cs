using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(AudioSource))]
public class PossessionManager : MonoBehaviour
{
    [Header("VCams")]
    public CinemachineCamera vcamBall;
    public CinemachineCamera vcamBlock;

    [Header("Control Scripts")]
    public MonoBehaviour[] ballControllers;
    public MonoBehaviour[] blockControllers;

    [Header("Scene Objects")]
    public GameObject ballObject;

    [Header("Look-to-Possess")]
    public Transform blockRoot;
    public LayerMask possessableMask = ~0;
    public float lookMaxDistance = 30f;

    [Header("Audio Feedback")]
    public AudioClip switchToBallClip;
    public AudioClip switchToBlockClip;
    [Range(0f, 1f)] public float switchVolume = 0.8f;

    [Header("Visual Feedback")]
    public ParticleSystem switchToBallEffect;
    public ParticleSystem switchToBlockEffect;

    [Header("Feedback Toggles")]
    public bool enableAudioFeedback = true;
    public bool enableVisualFeedback = true;

    // ── FP-Rendering (Layer-Basiert) ────────────────────────────────────────────────
    [Header("First-Person Rendering")]
    [Tooltip("Name des Layers, auf dem die sichtbaren Block-Meshes liegen (Child: PlayerBodyFP).")]
    public string fpLayerName = "PlayerBodyFP";
    [Tooltip("Zusätzliche Wartezeit nach Blend-Ende, bevor der Körper ausgeblendet/eingeblendet wird.")]
    public float cullingPostBlendDelay = 0.05f;
    [Tooltip("Maximale Wartezeit auf Blend-Ende, falls die API nichts meldet.")]
    public float cullingBlendTimeout = 1.0f;

    private enum Controlled { Ball, Block }
    private Controlled current = Controlled.Ball;

    private AudioSource audioSource;
    private Camera mainCam;
    private CinemachineBrain brain;
    private bool isLookingAtBlock;
    private bool initialized = false;
    private int playerBodyLayer = -1;
    private Coroutine cullingRoutine;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        mainCam = Camera.main;
        if (mainCam) brain = mainCam.GetComponent<CinemachineBrain>();

        if (audioSource) { audioSource.playOnAwake = false; audioSource.loop = false; }

        playerBodyLayer = LayerMask.NameToLayer(fpLayerName);
        if (playerBodyLayer < 0)
            Debug.LogWarning($"[PossessionManager] FP-Layer \"{fpLayerName}\" existiert nicht.");
    }

    void Start()
    {
        ApplyState(Controlled.Ball, playAudio: false);
        initialized = true;
    }

    void Update()
    {
        isLookingAtBlock = (current == Controlled.Ball) && RaycastToBlock();

        if (Input.GetMouseButtonDown(1) && isLookingAtBlock && current != Controlled.Block)
            ApplyState(Controlled.Block, playAudio: true);

        if (Input.GetKeyDown(KeyCode.LeftShift) && current != Controlled.Ball)
            ApplyState(Controlled.Ball, playAudio: true);
    }

    void ApplyState(Controlled who, bool playAudio = true, bool playVfx = true)
    {
        current = who;
        bool toBall = (who == Controlled.Ball);

        // Prüfe globale Juiciness-Einstellung
        bool juicy = JuicinessSettings.instance != null && JuicinessSettings.instance.IsJuicy;

        // Kameras schalten
        if (vcamBall) vcamBall.Priority = toBall ? 20 : 0;
        if (vcamBlock) vcamBlock.Priority = toBall ? 0 : 20;

        // Kugel rendern
        if (ballObject) ballObject.SetActive(toBall);

        // Steuerung schalten
        foreach (var s in ballControllers) if (s) s.enabled = toBall;
        foreach (var s in blockControllers) if (s) s.enabled = !toBall;

        // FP-Culling: nur wenn Juiciness AN → mit Blend-Wait, sonst sofort
        if (cullingRoutine != null) StopCoroutine(cullingRoutine);
        if (juicy)
        {
            cullingRoutine = StartCoroutine(SwapFPCullingAfterBlend(toBall));
        }
        else
        {
            // 🧊 Ohne Juiciness: Sofort snappen (kein Blend-Wait)
            SwapFPCullingImmediate(toBall);
        }

        // 🔊 Audio NUR wenn Juiciness AN
        if (enableAudioFeedback && juicy && playAudio && audioSource)
        {
            var clip = toBall ? switchToBallClip : switchToBlockClip;
            if (clip)
            {
                audioSource.pitch = Random.Range(0.97f, 1.03f);
                audioSource.PlayOneShot(clip, switchVolume);
            }
        }

        // 🎆 VFX NUR wenn Juiciness AN
        if (enableVisualFeedback && juicy && initialized && playVfx)
        {
            var prefab = toBall ? switchToBallEffect : switchToBlockEffect;
            if (prefab)
            {
                Vector3 pos = toBall
                    ? (ballObject ? ballObject.transform.position : transform.position)
                    : (blockRoot ? blockRoot.position : transform.position);

                var fx = Instantiate(prefab, pos, Quaternion.identity);
                var main = fx.main; main.playOnAwake = false; fx.Clear(true); fx.Play(true);
                Destroy(fx.gameObject, main.duration + main.startLifetime.constantMax + 0.5f);
            }
        }
    }

    IEnumerator SwapFPCullingAfterBlend(bool toBall)
    {
        if (!mainCam || playerBodyLayer < 0) yield break;

        // Warten bis Blend Ende (oder Timeout)
        float t = 0f;
        while (brain && IsBrainBlending(brain) && t < cullingBlendTimeout)
        {
            t += Time.deltaTime;
            yield return null;
        }
        if (cullingPostBlendDelay > 0f) yield return new WaitForSeconds(cullingPostBlendDelay);

        int bit = 1 << playerBodyLayer;
        if (toBall) mainCam.cullingMask |= bit;
        else mainCam.cullingMask &= ~bit;
    }

    // 🧊 Neue Methode: Sofortiges Culling ohne Blend-Wartezeit
    void SwapFPCullingImmediate(bool toBall)
    {
        if (!mainCam || playerBodyLayer < 0) return;

        int bit = 1 << playerBodyLayer;
        if (toBall) mainCam.cullingMask |= bit;
        else mainCam.cullingMask &= ~bit;
    }

    static bool IsBrainBlending(CinemachineBrain b)
    {
        try { return b.ActiveBlend != null; } catch { }
        return false;
    }

    bool RaycastToBlock()
    {
        if (!mainCam || !blockRoot) return false;

        int mask = possessableMask;
        if (ballObject) mask &= ~(1 << ballObject.layer);
        if (playerBodyLayer >= 0) mask &= ~(1 << playerBodyLayer);

        Ray ray = new Ray(mainCam.transform.position, mainCam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, lookMaxDistance, mask, QueryTriggerInteraction.Ignore))
            return hit.collider && hit.collider.transform.IsChildOf(blockRoot);

        return false;
    }

    public bool IsLookingAtTargetPublic() => (current == Controlled.Ball) && isLookingAtBlock;
}