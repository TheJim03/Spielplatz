using UnityEngine;

public class TeleportPad : MonoBehaviour
{
    public Transform target;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            other.transform.position = target.position;
    }
}