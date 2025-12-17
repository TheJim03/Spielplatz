using UnityEngine;

public class CandleForm : MonoBehaviour
{
    [Header("Candle Objects")]
    public GameObject flame;
    public GameObject lightObject;
    public GameObject particles;

    bool isOn = false;

    void OnEnable()
    {
        SetState(false); // Kerze startet an
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Toggle();
        }
    }

    void Toggle()
    {
        isOn = !isOn;
        SetState(isOn);
    }

    void SetState(bool state)
    {
        if (flame) flame.SetActive(state);
        if (lightObject) lightObject.SetActive(state);
        if (particles) particles.SetActive(state);
    }
}