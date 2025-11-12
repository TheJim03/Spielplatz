using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(CinemachineCamera))]
public class CameraSideStepAvoidance : MonoBehaviour
{
    public Transform target;                      // dein Follow-Target (z.B. FollowProxy)
    public LayerMask obstacleMask;                // Wände/Level (nicht Player)
    public float sampleAngle = 25f;               // wie weit seitlich testen (Grad)
    public int   sampleSteps = 4;                 // wie fein wir testen
    public float biasSpeed = 120f;                // °/s – wie schnell wir rüberschieben
    public float clearCheckPadding = 0.1f;        // Puffer für Treffer
    public float losCheckRadius = 0.2f;           // Kugelstrahl für LOS

    CinemachineCamera vcam;
    CinemachineOrbitalFollow orbital;

    void Awake()
    {
        vcam = GetComponent<CinemachineCamera>();
        orbital = GetComponent<CinemachineOrbitalFollow>();
    }

    void LateUpdate()
    {
        if (target == null || orbital == null || vcam == null) return;

        // aktuelle gewünschte Kamera-Position vom vcam holen
        var brainCam = vcam.State.GetFinalPosition();    // geplante Cam-Pos
        var targetPos = target.position;

        // Sichtlinie verdeckt?
        bool blocked = Physics.SphereCast(
            targetPos, losCheckRadius,
            (brainCam - targetPos).normalized,
            out RaycastHit hit, Vector3.Distance(targetPos, brainCam),
            obstacleMask, QueryTriggerInteraction.Ignore);

        if (!blocked)
            return; // kein Bias nötig – Spieler-Eingabe lässt die Achse normal

        // Wir suchen links/rechts um das Ziel herum den freieren Winkel
        float bestAngleOffset = 0f;
        float bestScore = -Mathf.Infinity;

        float baseYaw = orbital.HorizontalAxis.Value; // aktueller Orbit-Winkel (in Grad)

        for (int side = -1; side <= 1; side += 2) // -1 = links, +1 = rechts
        {
            for (int i = 1; i <= sampleSteps; i++)
            {
                float off = side * (sampleAngle * i / sampleSteps);
                float testYaw = baseYaw + off;

                // Position mit gleichem Radius um Ziel schätzen:
                float radius = orbital.Radius; // aktueller Abstand aus dem OrbitalFollow
                Quaternion rot = Quaternion.Euler(0f, testYaw, 0f);
                Vector3 dir = rot * Vector3.back;           // „hinter“ das Target
                Vector3 testPos = targetPos + dir * radius; // Kandidaten-Position

                // Sicht prüfen
                bool occluded = Physics.SphereCast(
                    targetPos, losCheckRadius,
                    (testPos - targetPos).normalized,
                    out RaycastHit h2, radius - clearCheckPadding,
                    obstacleMask, QueryTriggerInteraction.Ignore);

                // Score: frei = gut, je früher frei, desto besser
                float score = occluded ? -i : (sampleSteps - i + 1);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestAngleOffset = off;
                }
            }
        }

        // Wenn wir etwas Besseres gefunden haben: Horizontal-Achse in diese Richtung ziehen
        float targetYaw = baseYaw + bestAngleOffset;
        float newYaw = Mathf.MoveTowardsAngle(baseYaw, targetYaw, biasSpeed * Time.deltaTime);
        orbital.HorizontalAxis.Value = newYaw;
    }
}
