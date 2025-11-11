using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(JuicinessManager))]
public class JuicyPostProcessing : MonoBehaviour, IJuicy
{
    public Volume postVolume;
    private JuicinessManager manager;

    void Awake()
    {
        manager = GetComponent<JuicinessManager>();
        manager.Register(this);
    }

    public void SetJuicy(bool on)
    {
        if (postVolume)
            postVolume.weight = on ? 1f : 0f;
    }
}