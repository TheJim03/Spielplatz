Purpose
-------
This file gives focused, actionable guidance for AI coding agents working on the Spielplatz Unity project. It highlights the architecture, patterns, developer workflows, and repo locations an agent needs to be productive without guessing project conventions.

Quick facts
-----------
- Project editor string: ProjectSettings/ProjectVersion.txt contains: `m_EditorVersion: 6000.2.7f2` (open this file to confirm the exact Unity editor used locally).
- Key packages (see `Packages/manifest.json`): `com.unity.inputsystem`, `com.unity.cinemachine`, `com.unity.ai.*`, `com.unity.test-framework`, `com.unity.render-pipelines.universal`, `com.unity.ugui`, TextMeshPro (TMP).

High-level architecture (what to know fast)
-----------------------------------------
- Gameplay split: possession-based control between a Ball and a Block. Central arbiter: `Assets/Scripts/PossessionScript.cs` (PossessionManager) toggles which controllers are active and switches Cinemachine cameras.
  - Pattern: two controller-groups are exposed as arrays (e.g., `ballControllers`, `blockControllers`) and enabled/disabled via `ApplyState`.
    - Example: `foreach (var s in ballControllers) if (s) s.enabled = toBall;`
- Movement: physics-driven movement via Rigidbody in `Assets/Scripts/BlockMovement.cs` and `Assets/Scripts/SphereMovement.cs` (also `BallMovement` variants). Use FixedUpdate for physics and manipulate `rb.linearVelocity` and `rb.linearDamping`.
  - Camera-relative movement is standard: compute cam.forward/right, zero Y, normalize, then build direction.
- UI & global state: `Assets/Scripts/ScoreManager.cs` uses a simple Singleton (`public static ScoreManager instance`) and TextMeshPro for on-screen coin counts.
- Audio/Visual feedback: AudioSource + ParticleSystem patterns are used in PossessionManager for switching feedback (play on switch, instantiate particles then Destroy after lifetime).

Notable repository conventions and patterns
----------------------------------------
- Inspector-first configuration: most tunables are public fields (Header attributes used). Keep changes to field visibility minimal — prefer adding [SerializeField] rather than changing public/private unless refactoring intentionally.
- RequireComponent usage: controller scripts often require components (see `[RequireComponent(typeof(Rigidbody))]`) — keep that when adding new controllers.
- Physics code lives in FixedUpdate; do not move physics to Update.
- Legacy Input API is used in scripts (Input.GetAxisRaw, Input.GetKey, Input.GetMouseButtonDown) even though the new Input System package exists in `Packages/manifest.json` and Input Action assets exist under `Assets/`. If you modify input handling, note this mixed state and update both the asset and code or convert consistently.
- Comments and some log strings are written in German. Preserve or translate thoughtfully when modifying user-visible text.

Integration points / external systems to watch
-------------------------------------------
- Cinemachine virtual cameras — look in `Assets/` for vcam prefabs and `PossessionScript.cs` references (vcamBall / vcamBlock).
- Input system: `Assets/InputSystem_Actions.inputactions` exists; but many movement scripts still call UnityEngine.Input. When adding input-driven features, prefer non-breaking, small changes and document conversions.
- TextMeshPro: used for HUD (`coinText` in ScoreManager). Include the TMPro namespace and package if editing UI.
- URP + Render pipeline: project includes Universal RP; shader/render changes should respect URP conventions.

Developer workflows
-------------------
- Opening/building: open the project in the Unity Editor matching the ProjectSettings/ProjectVersion.txt value. The Unity Editor compiles C# assemblies automatically — prefer using the Editor for compile/run iteration.
- IDE: `Spielplatz.sln` and `Assembly-CSharp.csproj` files exist. You can open the solution in Rider/Visual Studio for faster code navigation, but runtime testing must use Unity.
- Tests: `com.unity.test-framework` is present. If you add tests, place EditMode/PlayMode tests under a `Tests/` folder and follow Unity Test Framework conventions.
- Quick static check: to verify pure C# compile (non-Unity-specific compilation), open the .sln in the IDE and build; but prefer Unity Editor to catch assembly-definition and package issues.

Editing rules for AI agents (concrete, codebase-specific)
------------------------------------------------------
1. Preserve Inspector API: do not rename public fields or serialized fields used by scenes/prefabs without updating references in scenes/prefabs. If you must rename, list all affected scenes/prefabs and update them together.
2. Small, safe changes: prefer non-invasive changes (add new methods, small refactors) over large architectural rewrites. Example-safe edit: tune `moveSpeed` default, add [SerializeField] private backing field and preserve public accessors.
3. Physics: keep Rigidbody changes inside FixedUpdate and use `rb.linearVelocity` / `rb.AddForce` consistently with existing scripts.
4. Input conversion: if you convert a script to the new Input System, do it for the input surface only and leave the rest intact. Add usage notes and prefer feature-flagged conversion (comment + documented TODO) so human dev can verify scene bindings.
5. Audio/VFX lifecycle: when instantiating particle effects, follow the pattern in PossessionManager: set playOnAwake = false, Clear(), Play(), and Destroy after estimated lifetime.

Files & locations to inspect first
---------------------------------
- Gameplay & controllers: `Assets/Scripts/` (BallMovement/SphereMovement.cs, BlockMovement.cs, PossessionScript.cs, ScoreManager.cs)
- Scenes: `Assets/Scenes/` (open in Unity to see which prefabs/scripts are wired)
- Prefabs: `Assets/Prefabs/` (look for camera and controller prefabs)
- Input assets: `Assets/InputSystem_Actions.inputactions` and `Assets/InputSystem.inputsettings.asset`
- Project settings: `ProjectSettings/ProjectVersion.txt` and `Packages/manifest.json`

When unsure, do this (ordered)
-------------------------------
1. Open Unity Editor with project path `i:\Unity\UniProjekt\Spielplatz` and confirm editor version.
2. Inspect the Scene(s) in the Editor to see how `PossessionManager` and movement controllers are wired (vcams, controller arrays, ballObject, blockRoot).
3. If changing public/serialized fields, update scene/prefab references in the Editor before committing.

Follow-up
---------
If any of the above is unclear or you want me to: (a) convert one controller to the new Input System, (b) add a small unit/PlayMode test for ScoreManager, or (c) add a README describing scene wiring, tell me which and I'll implement the change and test locally.
