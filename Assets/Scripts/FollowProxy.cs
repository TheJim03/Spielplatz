using UnityEngine;

public class FollowProxy : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 0.25f, 0);

    void LateUpdate()
    {
        if (!target) return;

        // Nur Position übernehmen …
        transform.position = target.position + offset;

        // … aber NICHT die Rotation! Fixiere sie (z.B. Welt-Identität).
        transform.rotation = Quaternion.identity;
    }
}
