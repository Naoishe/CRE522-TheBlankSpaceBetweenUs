using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

/// <summary>
/// Library scene: return-to-campus trigger + auto-start Niko dialogue (same day rules as <see cref="NikoInteract"/>).
/// </summary>
[DefaultExecutionOrder(50)]
public class Library : MonoBehaviour
{
    public Collider2D toCampus;
    public Collider2D playerCollider;
    public GameObject playerObj;

    public YarnProject[] yarnProjects;
    public DialogueRunner dialogueRunner;

    public static Action ReturnToCampus;

    private bool _yarnProjectApplied;
    private bool _startedLibraryDialogue;
    private bool _libraryDialogueCoroutineScheduled;

    private bool _wasTouchingCampusExit;

    private GameObject _nikoPortrait;

    private void Awake()
    {
        ApplyLibraryYarnProjectToRunner();
    }

    private void OnEnable()
    {
        ContinuousData.AfterYarnSceneRefresh += OnAfterYarnSceneRefresh;
    }

    private void OnDisable()
    {
        ContinuousData.AfterYarnSceneRefresh -= OnAfterYarnSceneRefresh;
    }

    /// <summary>
    /// When <see cref="ContinuousData"/> exists, scene dialogue must start after duplicate-kit teardown
    /// (<see cref="ContinuousData.OnSceneLoadedRefreshYarnRoutine"/>); otherwise <see cref="StartDialogue"/>
    /// can run against a destroyed runner or before the DDOL UI is the single active system.
    /// </summary>
    private void OnAfterYarnSceneRefresh(Scene scene)
    {
        if (!isActiveAndEnabled)
            return;
        if (string.IsNullOrEmpty(scene.name) || scene.name != "Library")
            return;

        SyncPlayerColliderFromContinuousData();
        ApplyLibraryYarnProjectToRunner();
        ScheduleDialogueRoutineIfNeeded();
    }

    private void Start()
    {
        SyncPlayerColliderFromContinuousData();
        ApplyLibraryYarnProjectToRunner();
        StartCoroutine(ConfigureLibraryYarnAfterFrame());
    }

    /// <summary>
    /// Always schedules dialogue from <see cref="Start"/>; <see cref="AfterYarnSceneRefresh"/> may also fire
    /// but <see cref="_startedLibraryDialogue"/> de-duplicates.
    /// </summary>
    private IEnumerator ConfigureLibraryYarnAfterFrame()
    {
        yield return null;
        ApplyLibraryYarnProjectToRunner();
        yield return null;
        ApplyLibraryYarnProjectToRunner();
        ScheduleDialogueRoutineIfNeeded();
    }

    private void ScheduleDialogueRoutineIfNeeded()
    {
        if (_libraryDialogueCoroutineScheduled || _startedLibraryDialogue)
            return;
        if (!_yarnProjectApplied || yarnProjects == null || yarnProjects.Length == 0 || yarnProjects[0] == null)
            return;

        _libraryDialogueCoroutineScheduled = true;
        StartCoroutine(StartLibraryDialogueWhenReady());
    }

    /// <summary>
    /// Binds Library0 to the current preferred DialogueRunner. Does not start dialogue (that runs after scene-load teardown).
    /// </summary>
    private void ApplyLibraryYarnProjectToRunner()
    {
        if (yarnProjects == null || yarnProjects.Length == 0 || yarnProjects[0] == null)
        {
            Debug.LogError("Library: Assign the Library/Niko YarnProject on GameManager (Library0).", this);
            return;
        }

        ContinuousData.EnsureDialogueRunnerExistsInScene();
        if (ContinuousData.instance != null)
        {
            ContinuousData.instance.EnsureDialogueRunnerAndStorage();
            ContinuousData.instance.RegisterYarnCommandHandlersIfNeeded();
        }

        DialogueRunner resolved = ContinuousData.instance != null ? ContinuousData.instance.diaRunner : null;
        if (resolved == null || !resolved)
            resolved = ContinuousData.FindPreferredDialogueRunner();

        if (_yarnProjectApplied && (dialogueRunner == null || !dialogueRunner || dialogueRunner != resolved))
        {
            _yarnProjectApplied = false;
            _startedLibraryDialogue = false;
            _libraryDialogueCoroutineScheduled = false;
        }

        if (_yarnProjectApplied)
            return;

        if (resolved == null || !resolved)
        {
            Debug.LogError(
                "Library: No DialogueRunner found. Add the Dialogue System prefab or enter from a scene that already initializes Yarn.",
                this);
            return;
        }

        dialogueRunner = resolved;
        // SetProject throws if the runner is mid-dialogue from a previous scene; let StartLibraryDialogueWhenReady do the binding after Stop.
        if (resolved.IsDialogueRunning)
        {
            _yarnProjectApplied = false;
            return;
        }
        dialogueRunner.SetProject(yarnProjects[0]);
        _yarnProjectApplied = true;
    }

    private IEnumerator StartLibraryDialogueWhenReady()
    {
        // OnSceneLoadedRefreshYarnRoutine: yield, DestroyDuplicateDialogueKitRootsInScene (strips scene-local Dialogue System).
        // Wait extra frames then re-resolve the DDOL runner so we never call StartDialogue on a destroyed instance.
        yield return null;
        yield return null;
        yield return null;
        yield return null;

        if (_startedLibraryDialogue)
            yield break;

        if (yarnProjects == null || yarnProjects.Length == 0 || yarnProjects[0] == null)
            yield break;

        for (int attempt = 0; attempt < 8; attempt++)
        {
            ContinuousData.EnsureDialogueRunnerExistsInScene();
            if (ContinuousData.instance != null)
            {
                ContinuousData.instance.EnsureDialogueRunnerAndStorage();
                ContinuousData.instance.RegisterYarnCommandHandlersIfNeeded();
            }

            DialogueRunner dr = ContinuousData.instance != null ? ContinuousData.instance.diaRunner : null;
            if (dr == null || !dr)
                dr = ContinuousData.FindPreferredDialogueRunner();

            if (dr != null && dr)
            {
                dialogueRunner = dr;

                if (_startedLibraryDialogue)
                    yield break;

                if (dr.IsDialogueRunning)
                {
                    dr.Stop();
                    // Wait until Yarn flips IsDialogueRunning off before calling SetProject (it throws otherwise).
                    for (int wait = 0; wait < 6 && dr.IsDialogueRunning; wait++)
                        yield return null;
                }

                if (dr.YarnProject != yarnProjects[0])
                {
                    if (dr.IsDialogueRunning)
                        yield break;
                    dr.SetProject(yarnProjects[0]);
                }

                ContinuousData.instance?.GatherVars();

                string node = GetNikoNodeForCurrentDay();
                bool nodeExists = dr.YarnProject != null
                    && dr.YarnProject.NodeNames != null
                    && System.Array.IndexOf(dr.YarnProject.NodeNames, node) >= 0;
                int presenterCount = CountDialoguePresenters(dr);

                if (presenterCount == 0)
                    Debug.LogWarning("Library: DialogueRunner has 0 DialoguePresenters; lines will run silently. Ensure the Dialogue System prefab is loaded (start from PlayerHouse, not Library).", this);

                if (!nodeExists)
                {
                    string available = (dr.YarnProject != null && dr.YarnProject.NodeNames != null)
                        ? string.Join(", ", dr.YarnProject.NodeNames)
                        : "<none>";
                    Debug.LogError(
                        $"Library: Yarn project '{(dr.YarnProject != null ? dr.YarnProject.name : "<null>")}' has no node '{node}'. Available: [{available}]. " +
                        $"Reimport Library0.yarnproject (right-click → Reimport) so Niko0.yarn is picked up.",
                        this);
                    yield break;
                }

                _startedLibraryDialogue = true;
                try
                {
                    dr.StartDialogue(node);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Library: StartDialogue threw {ex.GetType().Name}: {ex.Message}", this);
                }
                yield break;
            }

            yield return null;
        }

        Debug.LogWarning("Library: Failed to start Niko dialogue after retries. Check DialogueRunner / Yarn project.", this);
    }

    /// <summary>
    /// In Yarn Spinner 3 the presenter list is private; we infer from <see cref="DialoguePresenterBase"/> components on the runner hierarchy.
    /// </summary>
    private static int CountDialoguePresenters(DialogueRunner dr)
    {
        if (dr == null)
            return 0;
        int n = 0;
        var presenters = dr.GetComponentsInChildren<DialoguePresenterBase>(true);
        if (presenters != null)
            n = presenters.Length;
        return n;
    }

    private static string GetNikoNodeForCurrentDay()
    {
        int day = ContinuousData.instance != null ? ContinuousData.instance.CDdayIndex : 0;
        if (day == 1) return "Niko1";
        if (day == 2) return "Niko2";
        if (day == 3) return "Niko3";
        if (day == 4) return "Niko4";
        if (day == 5) return "Niko5";
        return "Niko0";
    }

    private void Update()
    {
        SyncPlayerColliderFromContinuousData();

        if (_nikoPortrait == null)
            _nikoPortrait = GameObject.Find("NikoDiaImage");
        if (_nikoPortrait != null && ContinuousData.instance != null)
            _nikoPortrait.SetActive(ContinuousData.instance.NikoDiaImageState);

        if (toCampus == null || !toCampus || playerCollider == null || !playerCollider)
            return;
        if (ContinuousData.instance == null)
            return;

        bool touching = ContinuousData.CollidersOverlap2D(toCampus, playerCollider);
        if (touching && !_wasTouchingCampusExit)
            ContinuousData.instance.SceneChangeDetected("CampusGrounds", ContinuousData.instance.campusGrounds_LibrarySpawn);
        _wasTouchingCampusExit = touching;
    }

    private void SyncPlayerColliderFromContinuousData()
    {
        var cd = ContinuousData.instance;
        if (cd == null)
            return;
        if (cd.player == null || !cd.player || cd.playerCollider == null || !cd.playerCollider)
            cd.LocatePlayerObject();
        if (playerCollider == null || !playerCollider)
            playerCollider = cd.playerCollider;
    }
}
