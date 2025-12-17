using UnityEngine;

public class CandleMovement : MonoBehaviour
{
    [Header("Candle Objects")]
    public GameObject flame;
    public GameObject lightObject;
    public GameObject particles;

    bool isOn = false;

    void OnEnable()
    {
        SetState(false); // Kerze startet aus
    }

    void Update()
    {
        // Nur reagieren, wenn der Spieler diese Kerze kontrolliert
        if (!CompareTag("Candle")) return;

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

    public void SetState(bool state)
    {
        if (flame) flame.SetActive(state);
        if (lightObject) lightObject.SetActive(state);
        if (particles) particles.SetActive(state);
    }
}