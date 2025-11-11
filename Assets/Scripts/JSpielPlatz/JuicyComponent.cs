using UnityEngine;

public interface IJuicy
{
    void SetJuicy(bool on);
}

[DisallowMultipleComponent]
public class JuicyComponent : MonoBehaviour, IJuicy
{
    // Welche Komponenten dieses GameObjects steuern wir?
    public AudioSource[] audioSources;
    public ParticleSystem[] particleSystems;
    public Animator[] animators;
    public Light[] extraLights;

    JuicinessManager manager;

    void Awake()
    {
        manager = FindObjectOfType<JuicinessManager>();
        if (manager != null) manager.Register(this);
        // Auto-fetch if arrays empty
        if (audioSources == null || audioSources.Length == 0) audioSources = GetComponentsInChildren<AudioSource>(true);
        if (particleSystems == null || particleSystems.Length == 0) particleSystems = GetComponentsInChildren<ParticleSystem>(true);
        if (animators == null || animators.Length == 0) animators = GetComponentsInChildren<Animator>(true);
        if (extraLights == null || extraLights.Length == 0) extraLights = GetComponentsInChildren<Light>(true);
    }

    public void SetJuicy(bool on)
    {
        // Sounds
        foreach (var a in audioSources)
        {
            if (a == null) continue;
            a.mute = !on;
        }

        // Particles
        foreach (var p in particleSystems)
        {
            if (p == null) continue;
            var em = p.emission;
            em.enabled = on;
            if (!on) p.Clear();
        }

        // Animations - if you want to disable special animations, we can set animator.enabled
        foreach (var an in animators)
        {
            if (an == null) continue;
            an.enabled = on;
        }

        // Lights
        foreach (var l in extraLights)
        {
            if (l == null) continue;
            l.enabled = on;
        }
    }
}