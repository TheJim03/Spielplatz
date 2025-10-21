using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(AudioSource))]
public class PossessionManager : MonoBehaviour
{
    [Header("VCams")]
    public CinemachineCamera vcamBall;
    public CinemachineCamera vcamBlock;

    [Header("Control Scripts")]
    public MonoBehaviour[] ballControllers;    // z.B. BallMovement
    public MonoBehaviour[] blockControllers;   // z.B. BlockMovement

    [Header("Scene Objects")]
    public GameObject ballObject;              // Kugel wird versteckt, wenn Block aktiv

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

    private enum Controlled { Ball, Block }
    private Controlled current = Controlled.Ball; // Startzustand (wird in Start initialisiert)

    private AudioSource audioSource;
    private Camera mainCam;
    private bool isLookingAtBlock;
    private bool initialized = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        mainCam = Camera.main;
        if (audioSource)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }
    }

    void Start()
    {
        // Initial anwenden – aber OHNE Sound
        ApplyState(Controlled.Ball, playAudio: false);
        initialized = true;
    }

    void Update()
    {
        // Blicktest nur relevant, wenn wir die Kugel besitzen
        isLookingAtBlock = (current == Controlled.Ball) && RaycastToBlock();

        // Rechtsklick: nur wechseln, wenn Block im Blick UND wir nicht schon im Block sind
        if (Input.GetMouseButtonDown(1) && isLookingAtBlock && current != Controlled.Block)
            ApplyState(Controlled.Block, playAudio: true);

        // Shift: nur wechseln, wenn wir nicht schon in der Kugel sind
        if (Input.GetKeyDown(KeyCode.LeftShift) && current != Controlled.Ball)
            ApplyState(Controlled.Ball, playAudio: true);
    }

    // Zentraler State-Wechsel
    void ApplyState(Controlled who, bool playAudio = true, bool playVfx = true)
    {
        current = who;
        bool toBall = (who == Controlled.Ball);

        // Kameras
        if (vcamBall)  vcamBall.Priority  = toBall ? 20 : 0;
        if (vcamBlock) vcamBlock.Priority = toBall ? 0  : 20;

        // Kugel ein-/ausblenden
        if (ballObject) ballObject.SetActive(toBall);

        // Steuerung an/aus
        foreach (var s in ballControllers)  if (s) s.enabled = toBall;
        foreach (var s in blockControllers) if (s) s.enabled = !toBall;

        // 🎧 Audio nur bei tatsächlichem Wechsel und wenn erlaubt
        if (enableAudioFeedback && playAudio && audioSource)
        {
            var clip = toBall ? switchToBallClip : switchToBlockClip;
            if (clip)
            {
                audioSource.pitch = Random.Range(0.97f, 1.03f); // kleines Pitch-Variation = „juicy“
                audioSource.PlayOneShot(clip, switchVolume);
            }
        }

        // ✨ Partikel-Effekt beim Wechsel (wenn erlaubt)
        if (enableVisualFeedback && initialized && playVfx)
        {
            var prefab = toBall ? switchToBallEffect : switchToBlockEffect;
            if (prefab)
            {
                Vector3 pos = toBall
                    ? (ballObject ? ballObject.transform.position : transform.position)
                    : (blockRoot ? blockRoot.position : transform.position);

                var fx = Instantiate(prefab, pos, Quaternion.identity);

                var main = fx.main;
                main.playOnAwake = false;
                fx.Clear(true);
                fx.Play(true);

                float life = main.duration + main.startLifetime.constantMax + 0.5f;
                Destroy(fx.gameObject, life);
            }
        }
    }

    bool RaycastToBlock()
    {
        if (!mainCam || !blockRoot) return false;

        int mask = possessableMask;
        if (ballObject) mask &= ~(1 << ballObject.layer);

        Ray ray = new Ray(mainCam.transform.position, mainCam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, lookMaxDistance, mask, QueryTriggerInteraction.Ignore))
            return hit.collider && hit.collider.transform.IsChildOf(blockRoot);

        return false;
    }

    // Für dein Crosshair
    public bool IsLookingAtTargetPublic() => (current == Controlled.Ball) && isLookingAtBlock;
}
