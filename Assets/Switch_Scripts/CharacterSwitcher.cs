using UnityEngine;
using UnityEngine.InputSystem; // Neues Input-System

public class CharacterSwitcher : MonoBehaviour
{
    [Header("Playable Characters (Small, Big)")]
    public PlayerMover smallPlayer;
    public PlayerMover bigPlayer;

    [Header("Camera")]
    public CameraFollowJosu camFollow;

    [Header("Small Style")]
    public float smallFOV = 70f;
    public Vector3 smallOffset = new Vector3(0, 4.5f, -5.5f);

    [Header("Big Style")]
    public float bigFOV = 55f;
    public Vector3 bigOffset = new Vector3(0, 7.5f, -9.5f);

    [Header("Small Stats")]
    public float smallMove = 7.5f;
    public float smallAccel = 28f;
    public float smallJump = 7f;

    [Header("Big Stats")]
    public float bigMove = 5.0f;
    public float bigAccel = 22f;
    public float bigJump = 5.5f;

    [Header("Keys")]
    public KeyCode switchKey = KeyCode.Tab;
    public KeyCode resetKey  = KeyCode.R;

    bool controllingSmall = true;

    void Start()
    {
        // Sicherheitsabfragen
        if (!smallPlayer || !bigPlayer)
        {
            Debug.LogError("CharacterSwitcher: smallPlayer oder bigPlayer fehlt. Bitte im Inspector zuweisen!");
            enabled = false;
            return;
        }

        if (!camFollow)
        {
            camFollow = FindObjectOfType<CameraFollowJosu>();
            if (!camFollow)
            {
                Debug.LogError("CharacterSwitcher: Keine CameraFollowJosu gefunden oder zugewiesen!");
                enabled = false;
                return;
            }
        }

        // Startzustand: kleiner aktiv
        ApplyStats(smallPlayer, smallMove, smallAccel, smallJump);
        ApplyStats(bigPlayer, bigMove, bigAccel, bigJump);

        smallPlayer.SetControlled(true);
        bigPlayer.SetControlled(false);

        camFollow.SetTargetAndStyle(smallPlayer.transform, smallFOV, smallOffset);
        controllingSmall = true;
    }

    void Update()
    {
        // --- Eingabeprüfung für Switch ---
        bool switchPressed =
            Input.GetKeyDown(switchKey) ||                                // Altes Input-System
            (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame); // Neues Input-System

        if (switchPressed)
        {
            if (controllingSmall)
                ActivateBig(true);
            else
                ActivateSmall(true);

            controllingSmall = !controllingSmall;
        }

        // --- Eingabeprüfung für Reset ---
        bool resetPressed =
            Input.GetKeyDown(resetKey) ||                                // Altes Input-System
            (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame); // Neues Input-System

        if (resetPressed)
        {
            var p  = controllingSmall ? smallPlayer : bigPlayer;
            if (!p) return;
            var rb = p.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            p.transform.position += Vector3.up * 0.5f;
        }
    }

    // --- Aktivierung der Spieler ---
    void ActivateSmall(bool alsoCamera)
    {
        smallPlayer.SetControlled(true);
        bigPlayer.SetControlled(false);

        if (alsoCamera && camFollow)
            camFollow.SetTargetAndStyle(smallPlayer.transform, smallFOV, smallOffset);
    }

    void ActivateBig(bool alsoCamera)
    {
        bigPlayer.SetControlled(true);
        smallPlayer.SetControlled(false);

        if (alsoCamera && camFollow)
            camFollow.SetTargetAndStyle(bigPlayer.transform, bigFOV, bigOffset);
    }

    // --- Bewegungsparameter ---
    void ApplyStats(PlayerMover mover, float move, float accel, float jump)
    {
        if (!mover) return;
        mover.moveSpeed = move;
        mover.acceleration = accel;
        mover.jumpForce = jump;
    }
}