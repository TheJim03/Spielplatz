using UnityEngine;

public class LockRigidbodiesOnLayer : MonoBehaviour
{
    [Tooltip("Alle Rigidbodies auf diesem Layer werden unbeweglich gemacht.")]
    public string targetLayerName = "RoomStatic";

    [Tooltip("Statt nur isKinematic kannst du hier auch komplett freezen.")]
    public bool freezeCompletely = false;

    void Awake()
    {
        int targetLayer = LayerMask.NameToLayer(targetLayerName);
        if (targetLayer == -1)
        {
            Debug.LogError($"Layer '{targetLayerName}' existiert nicht. Bitte erst in den Project Settings anlegen.");
            return;
        }

        Rigidbody[] allBodies = FindObjectsOfType<Rigidbody>();

        foreach (var rb in allBodies)
        {
            if (rb.gameObject.layer != targetLayer)
                continue;

            if (freezeCompletely)
            {
                rb.isKinematic = false; // darf ruhig dynamisch bleiben
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
            else
            {
                // kinematisch = reagiert nicht mehr auf Kräfte / Schieben
                rb.isKinematic = true;
            }

            // Optional: zur Kontrolle
            // Debug.Log($"Locked Rigidbody auf {rb.name}");
        }
    }
}
