using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(AudioSource))]
public class PossessionManager : MonoBehaviour
{
    [Header("VCams")]
    public CinemachineCamera vcamBall;
    public CinemachineCamera vcamBlock;
    public CinemachineCamera vcamMaus;
    public CinemachineCamera vcamAlt1;
    public CinemachineCamera vcamAlt2;
    public CinemachineCamera vcamAlt3;
    public CinemachineCamera vcamAlt4;
    public CinemachineCamera vcamAlt5;
    public CinemachineCamera vcamAlt6;
    public CinemachineCamera vcamAlt7;
    public CinemachineCamera vcamAlt8;
    public CinemachineCamera vcamAlt9;
    public CinemachineCamera vcamAlt10;

    [Header("Control Scripts")]
    public MonoBehaviour[] ballControllers;
    public MonoBehaviour[] blockControllers;
    public MonoBehaviour[] mausControllers;
    public MonoBehaviour[] alt1Controllers;
    public MonoBehaviour[] alt2Controllers;
    public MonoBehaviour[] alt3Controllers;
    public MonoBehaviour[] alt4Controllers;
    public MonoBehaviour[] alt5Controllers;
    public MonoBehaviour[] alt6Controllers;
    public MonoBehaviour[] alt7Controllers;
    public MonoBehaviour[] alt8Controllers;
    public MonoBehaviour[] alt9Controllers;
    public MonoBehaviour[] alt10Controllers;

    [Header("Scene Objects")]
    public GameObject ballObject;

    [Header("Look-to-Possess")]
    public Transform blockRoot;
    public Transform mausRoot;
    public Transform alt1Root;
    public Transform alt2Root;
    public Transform alt3Root;
    public Transform alt4Root;
    public Transform alt5Root;
    public Transform alt6Root;
    public Transform alt7Root;
    public Transform alt8Root;
    public Transform alt9Root;
    public Transform alt10Root;
    public LayerMask possessableMask = ~0;
    public float lookMaxDistance = 30f;

    [Header("Audio Feedback")]
    public AudioClip switchToBallClip;
    public AudioClip switchToBlockClip;
    public AudioClip switchToMausClip;
    public AudioClip switchToAlt1Clip;
    public AudioClip switchToAlt2Clip;
    public AudioClip switchToAlt3Clip;
    public AudioClip switchToAlt4Clip;
    public AudioClip switchToAlt5Clip;
    public AudioClip switchToAlt6Clip;
    public AudioClip switchToAlt7Clip;
    public AudioClip switchToAlt8Clip;
    public AudioClip switchToAlt9Clip;
    public AudioClip switchToAlt10Clip;
    [Range(0f, 1f)] public float switchVolume = 0.8f;

    [Header("Visual Feedback")]
    public ParticleSystem switchToBallEffect;
    public ParticleSystem switchToBlockEffect;
    public ParticleSystem switchToMausEffect;
    public ParticleSystem switchToAlt1Effect;
    public ParticleSystem switchToAlt2Effect;
    public ParticleSystem switchToAlt3Effect;
    public ParticleSystem switchToAlt4Effect;
    public ParticleSystem switchToAlt5Effect;
    public ParticleSystem switchToAlt6Effect;
    public ParticleSystem switchToAlt7Effect;
    public ParticleSystem switchToAlt8Effect;
    public ParticleSystem switchToAlt9Effect;
    public ParticleSystem switchToAlt10Effect;

    [Header("Feedback Toggles")]
    public bool enableAudioFeedback = true;
    public bool enableVisualFeedback = true;

    [Header("First-Person Rendering")]
    public string fpLayerName = "PlayerBodyFP";
    public float cullingPostBlendDelay = 0.05f;
    public float cullingBlendTimeout = 1.0f;

    private enum Controlled { Ball, Block, Maus, Alt1, Alt2, Alt3, Alt4, Alt5, Alt6, Alt7, Alt8, Alt9, Alt10 }
    private Controlled current = Controlled.Ball;

    private enum LookTarget { None, Block, Maus, Alt1, Alt2, Alt3, Alt4, Alt5, Alt6, Alt7, Alt8, Alt9, Alt10 }
    private LookTarget lookedTarget = LookTarget.None;

    private AudioSource audioSource;
    private Camera mainCam;
    private CinemachineBrain brain;
    private bool initialized = false;
    private int playerBodyLayer = -1;
    private Coroutine cullingRoutine;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        mainCam = Camera.main;
        if (mainCam) brain = mainCam.GetComponent<CinemachineBrain>();

        if (audioSource)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }

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
        lookedTarget = (current == Controlled.Ball) ? RaycastToTarget() : LookTarget.None;

        if (Input.GetMouseButtonDown(1) && current == Controlled.Ball)
        {
            if (lookedTarget == LookTarget.Block) ApplyState(Controlled.Block, true);
            if (lookedTarget == LookTarget.Maus) ApplyState(Controlled.Maus, true);
            if (lookedTarget == LookTarget.Alt1) ApplyState(Controlled.Alt1, true);
            if (lookedTarget == LookTarget.Alt2) ApplyState(Controlled.Alt2, true);
            if (lookedTarget == LookTarget.Alt3) ApplyState(Controlled.Alt3, true);
            if (lookedTarget == LookTarget.Alt4) ApplyState(Controlled.Alt4, true);
            if (lookedTarget == LookTarget.Alt5) ApplyState(Controlled.Alt5, true);
            if (lookedTarget == LookTarget.Alt6) ApplyState(Controlled.Alt6, true);
            if (lookedTarget == LookTarget.Alt7) ApplyState(Controlled.Alt7, true);
            if (lookedTarget == LookTarget.Alt8) ApplyState(Controlled.Alt8, true);
            if (lookedTarget == LookTarget.Alt9) ApplyState(Controlled.Alt9, true);
            if (lookedTarget == LookTarget.Alt10) ApplyState(Controlled.Alt10, true);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && current != Controlled.Ball)
            ApplyState(Controlled.Ball, true);
    }

    void ApplyState(Controlled who, bool playAudio = true, bool playVfx = true)
    {
        current = who;

        // -------------------------------
        // Alle Kerzen auf "Untagged" setzen
        if (alt1Root) alt1Root.gameObject.tag = "Untagged";
        if (alt2Root) alt2Root.gameObject.tag = "Untagged";
        if (alt3Root) alt3Root.gameObject.tag = "Untagged";
        if (alt4Root) alt4Root.gameObject.tag = "Untagged";
        if (alt5Root) alt5Root.gameObject.tag = "Untagged";
        if (alt6Root) alt6Root.gameObject.tag = "Untagged";
        if (alt7Root) alt7Root.gameObject.tag = "Untagged";
        if (alt8Root) alt8Root.gameObject.tag = "Untagged";
        if (alt9Root) alt9Root.gameObject.tag = "Untagged";
        if (alt10Root) alt10Root.gameObject.tag = "Untagged";

        // Die aktuell kontrollierte Kerze auf "Candle" setzen
        if (who == Controlled.Alt1 && alt1Root) alt1Root.gameObject.tag = "Candle";
        if (who == Controlled.Alt2 && alt2Root) alt2Root.gameObject.tag = "Candle";
        if (who == Controlled.Alt3 && alt3Root) alt3Root.gameObject.tag = "Candle";
        if (who == Controlled.Alt4 && alt4Root) alt4Root.gameObject.tag = "Candle";
        if (who == Controlled.Alt5 && alt5Root) alt5Root.gameObject.tag = "Candle";
        if (who == Controlled.Alt6 && alt6Root) alt6Root.gameObject.tag = "Candle";
        if (who == Controlled.Alt7 && alt7Root) alt7Root.gameObject.tag = "Candle";
        if (who == Controlled.Alt8 && alt8Root) alt8Root.gameObject.tag = "Candle";
        if (who == Controlled.Alt9 && alt9Root) alt9Root.gameObject.tag = "Candle";
        if (who == Controlled.Alt10 && alt10Root) alt10Root.gameObject.tag = "Candle";
        // -------------------------------

        bool toBall = (who == Controlled.Ball);

        bool juicy = JuicinessSettings.instance != null && JuicinessSettings.instance.IsJuicy;

        if (vcamBall) vcamBall.Priority = (who == Controlled.Ball) ? 20 : 0;
        if (vcamBlock) vcamBlock.Priority = (who == Controlled.Block) ? 20 : 0;
        if (vcamMaus) vcamMaus.Priority = (who == Controlled.Maus) ? 20 : 0;
        if (vcamAlt1) vcamAlt1.Priority = (who == Controlled.Alt1) ? 20 : 0;
        if (vcamAlt2) vcamAlt2.Priority = (who == Controlled.Alt2) ? 20 : 0;
        if (vcamAlt3) vcamAlt3.Priority = (who == Controlled.Alt3) ? 20 : 0;
        if (vcamAlt4) vcamAlt4.Priority = (who == Controlled.Alt4) ? 20 : 0;
        if (vcamAlt5) vcamAlt5.Priority = (who == Controlled.Alt5) ? 20 : 0;
        if (vcamAlt6) vcamAlt6.Priority = (who == Controlled.Alt6) ? 20 : 0;
        if (vcamAlt7) vcamAlt7.Priority = (who == Controlled.Alt7) ? 20 : 0;
        if (vcamAlt8) vcamAlt8.Priority = (who == Controlled.Alt8) ? 20 : 0;
        if (vcamAlt9) vcamAlt9.Priority = (who == Controlled.Alt9) ? 20 : 0;
        if (vcamAlt10) vcamAlt10.Priority = (who == Controlled.Alt10) ? 20 : 0;

        if (ballObject) ballObject.SetActive(toBall);

        foreach (var s in ballControllers) if (s) s.enabled = (who == Controlled.Ball);
        foreach (var s in blockControllers) if (s) s.enabled = (who == Controlled.Block);
        foreach (var s in mausControllers) if (s) s.enabled = (who == Controlled.Maus);
        foreach (var s in alt1Controllers) if (s) s.enabled = (who == Controlled.Alt1);
        foreach (var s in alt2Controllers) if (s) s.enabled = (who == Controlled.Alt2);
        foreach (var s in alt3Controllers) if (s) s.enabled = (who == Controlled.Alt3);
        foreach (var s in alt4Controllers) if (s) s.enabled = (who == Controlled.Alt4);
        foreach (var s in alt5Controllers) if (s) s.enabled = (who == Controlled.Alt5);
        foreach (var s in alt6Controllers) if (s) s.enabled = (who == Controlled.Alt6);
        foreach (var s in alt7Controllers) if (s) s.enabled = (who == Controlled.Alt7);
        foreach (var s in alt8Controllers) if (s) s.enabled = (who == Controlled.Alt8);
        foreach (var s in alt9Controllers) if (s) s.enabled = (who == Controlled.Alt9);
        foreach (var s in alt10Controllers) if (s) s.enabled = (who == Controlled.Alt10);

        if (cullingRoutine != null) StopCoroutine(cullingRoutine);
        if (juicy) cullingRoutine = StartCoroutine(SwapFPCullingAfterBlend(toBall));
        else SwapFPCullingImmediate(toBall);

        if (enableAudioFeedback && juicy && playAudio && audioSource)
        {
            AudioClip clip = null;
            if (who == Controlled.Ball) clip = switchToBallClip;
            else if (who == Controlled.Block) clip = switchToBlockClip;
            else if (who == Controlled.Maus) clip = switchToMausClip;
            else if (who == Controlled.Alt1) clip = switchToAlt1Clip;
            else if (who == Controlled.Alt2) clip = switchToAlt2Clip;
            else if (who == Controlled.Alt3) clip = switchToAlt3Clip;
            else if (who == Controlled.Alt4) clip = switchToAlt4Clip;
            else if (who == Controlled.Alt5) clip = switchToAlt5Clip;
            else if (who == Controlled.Alt6) clip = switchToAlt6Clip;
            else if (who == Controlled.Alt7) clip = switchToAlt7Clip;
            else if (who == Controlled.Alt8) clip = switchToAlt8Clip;
            else if (who == Controlled.Alt9) clip = switchToAlt9Clip;
            else if (who == Controlled.Alt10) clip = switchToAlt10Clip;

            if (clip)
            {
                audioSource.pitch = Random.Range(0.97f, 1.03f);
                audioSource.PlayOneShot(clip, switchVolume);
            }
        }

        if (enableVisualFeedback && juicy && initialized && playVfx)
        {
            ParticleSystem prefab = null;
            Vector3 pos = transform.position;

            if (who == Controlled.Ball)
            {
                prefab = switchToBallEffect;
                pos = ballObject ? ballObject.transform.position : transform.position;
            }
            else if (who == Controlled.Block)
            {
                prefab = switchToBlockEffect;
                pos = blockRoot ? blockRoot.position : transform.position;
            }
            else if (who == Controlled.Maus)
            {
                prefab = switchToMausEffect;
                pos = mausRoot ? mausRoot.position : transform.position;
            }
            else if (who == Controlled.Alt1) { prefab = switchToAlt1Effect; pos = alt1Root ? alt1Root.position : transform.position; }
            else if (who == Controlled.Alt2) { prefab = switchToAlt2Effect; pos = alt2Root ? alt2Root.position : transform.position; }
            else if (who == Controlled.Alt3) { prefab = switchToAlt3Effect; pos = alt3Root ? alt3Root.position : transform.position; }
            else if (who == Controlled.Alt4) { prefab = switchToAlt4Effect; pos = alt4Root ? alt4Root.position : transform.position; }
            else if (who == Controlled.Alt5) { prefab = switchToAlt5Effect; pos = alt5Root ? alt5Root.position : transform.position; }
            else if (who == Controlled.Alt6) { prefab = switchToAlt6Effect; pos = alt6Root ? alt6Root.position : transform.position; }
            else if (who == Controlled.Alt7) { prefab = switchToAlt7Effect; pos = alt7Root ? alt7Root.position : transform.position; }
            else if (who == Controlled.Alt8) { prefab = switchToAlt8Effect; pos = alt8Root ? alt8Root.position : transform.position; }
            else if (who == Controlled.Alt9) { prefab = switchToAlt9Effect; pos = alt9Root ? alt9Root.position : transform.position; }
            else if (who == Controlled.Alt10) { prefab = switchToAlt10Effect; pos = alt10Root ? alt10Root.position : transform.position; }

            if (prefab)
            {
                var fx = Instantiate(prefab, pos, Quaternion.identity);
                var main = fx.main; main.playOnAwake = false; fx.Clear(true); fx.Play(true);
                Destroy(fx.gameObject, main.duration + main.startLifetime.constantMax + 0.5f);
            }
        }
    }

    IEnumerator SwapFPCullingAfterBlend(bool toBall)
    {
        if (!mainCam || playerBodyLayer < 0) yield break;

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

    void SwapFPCullingImmediate(bool toBall)
    {
        if (!mainCam || playerBodyLayer < 0) return;

        int bit = 1 << playerBodyLayer;
        if (toBall) mainCam.cullingMask |= bit;
        else mainCam.cullingMask &= ~bit;
    }

    static bool IsBrainBlending(CinemachineBrain b)
    {
        try { return b.ActiveBlend != null; }
        catch { }
        return false;
    }

    LookTarget RaycastToTarget()
    {
        if (!mainCam) return LookTarget.None;

        int mask = possessableMask;
        if (ballObject) mask &= ~(1 << ballObject.layer);
        if (playerBodyLayer >= 0) mask &= ~(1 << playerBodyLayer);

        Ray ray = new Ray(mainCam.transform.position, mainCam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, lookMaxDistance, mask, QueryTriggerInteraction.Ignore))
        {
            var tr = hit.collider.transform;
            if (blockRoot && tr.IsChildOf(blockRoot)) return LookTarget.Block;
            if (mausRoot && tr.IsChildOf(mausRoot)) return LookTarget.Maus;
            if (alt1Root && tr.IsChildOf(alt1Root)) return LookTarget.Alt1;
            if (alt2Root && tr.IsChildOf(alt2Root)) return LookTarget.Alt2;
            if (alt3Root && tr.IsChildOf(alt3Root)) return LookTarget.Alt3;
            if (alt4Root && tr.IsChildOf(alt4Root)) return LookTarget.Alt4;
            if (alt5Root && tr.IsChildOf(alt5Root)) return LookTarget.Alt5;
            if (alt6Root && tr.IsChildOf(alt6Root)) return LookTarget.Alt6;
            if (alt7Root && tr.IsChildOf(alt7Root)) return LookTarget.Alt7;
            if (alt8Root && tr.IsChildOf(alt8Root)) return LookTarget.Alt8;
            if (alt9Root && tr.IsChildOf(alt9Root)) return LookTarget.Alt9;
            if (alt10Root && tr.IsChildOf(alt10Root)) return LookTarget.Alt10;
        }
        return LookTarget.None;
    }

    public bool IsLookingAtTargetPublic() => (current == Controlled.Ball) && (lookedTarget != LookTarget.None);
}
