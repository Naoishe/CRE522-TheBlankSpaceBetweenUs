using System.Collections;
using UnityEngine;
using Yarn.Unity;

[DefaultExecutionOrder(10)]
public class CampusGrounds : MonoBehaviour
{
    /// <summary>Library interaction volume. Use the LibraryBuilding object&apos;s <see cref="PolygonCollider2D"/>; if unset, we find GameObject &quot;LibraryBuilding&quot; at runtime.</summary>
    public Collider2D toLibrary;
    public Collider2D toHome;
    public Collider2D toTheatre;
    public Collider2D toGym;
    public Collider2D toCafe;
    public Collider2D playerCollider;
    ///public GameObject noReturn;
    public AudioSource notificationSound;
    public string targetScene;

    public static System.Action SceneChanged;

    private GameObject homeLabel;
    private GameObject libraryLabel;
    private GameObject gymLabel;
    private GameObject theatreLabel;
    private GameObject cafeLabel;

    public DialogueRunner dialogueRunner;
    public YarnProject[] yarnProjects;

    [SerializeField] bool developerMode;

    private bool _yarnProjectApplied;

    // Previous-frame overlap so we only fire once per approach (FixedUpdate was restarting dialogue every frame).
    private bool _wasTouchingLibrary;
    private bool _wasTouchingHome;
    private bool _wasTouchingTheatre;
    private bool _wasTouchingCafe;
    private bool _wasTouchingGym;

    private void ConfigureCampusYarn()
    {
        if (yarnProjects == null || yarnProjects.Length == 0 || yarnProjects[0] == null)
        {
            Debug.LogError("CampusGrounds: Assign at least one YarnProject in the Inspector.", this);
            return;
        }

        ContinuousData.EnsureDialogueRunnerExistsInScene();
        DialogueRunner resolved = null;
        if (ContinuousData.instance != null)
        {
            ContinuousData.instance.RegisterYarnCommandHandlersIfNeeded();
            resolved = ContinuousData.instance.diaRunner;
        }
        if (resolved == null || !resolved)
            resolved = ContinuousData.FindPreferredDialogueRunner();

        if (_yarnProjectApplied && (dialogueRunner == null || !dialogueRunner || dialogueRunner != resolved))
            _yarnProjectApplied = false;

        if (_yarnProjectApplied)
            return;

        if (resolved == null || !resolved)
        {
            Debug.LogError(
                "CampusGrounds: No DialogueRunner found. Use the Dialogue System prefab or enter from a scene that already initializes Yarn.",
                this);
            return;
        }

        dialogueRunner = resolved;
        dialogueRunner.SetProject(yarnProjects[0]);
        _yarnProjectApplied = true;
    }

    private void ResolveLibraryBuildingCollider()
    {
        if (toLibrary != null && toLibrary)
            return;
        var building = GameObject.Find("LibraryBuilding");
        if (building == null)
            return;
        toLibrary = building.GetComponent<PolygonCollider2D>();
        if (toLibrary == null)
            toLibrary = building.GetComponent<Collider2D>();
    }

    /// <summary>Called from <see cref="CampusSceneDoorTrigger2D"/> when the library door should open Yarn instead of loading directly.</summary>
    public void OpenDoorDialogueFromTrigger(string yarnNode, bool playNotification)
    {
        if (string.IsNullOrWhiteSpace(yarnNode))
            return;
        TryBeginCampusDoorDialogue(yarnNode, playNotification);
    }

    private void Awake()
    {
        ResolveLibraryBuildingCollider();
        ConfigureCampusYarn();
        homeLabel = GameObject.Find("HomeLabel");
        libraryLabel = GameObject.Find("LibraryLabel");
    }

    private void Start()
    {
        DeveloperModeCheck();
        ConfigureCampusYarn();
        if (!_yarnProjectApplied)
            StartCoroutine(ConfigureCampusAfterFrame());
        else
            HookDialogueComplete();

        if (homeLabel != null)
            homeLabel.SetActive(false);
        if (libraryLabel != null)
            libraryLabel.SetActive(false);
    }

    private IEnumerator ConfigureCampusAfterFrame()
    {
        yield return null;
        ConfigureCampusYarn();
        HookDialogueComplete();
    }

    private void OnEnable()
    {
        HookDialogueComplete();
    }

    private void OnDisable()
    {
        if (dialogueRunner != null && dialogueRunner)
            dialogueRunner.onDialogueComplete.RemoveListener(OnCampusInteractionDialogueComplete);
    }

    private void HookDialogueComplete()
    {
        if (dialogueRunner == null || !dialogueRunner)
            return;
        dialogueRunner.onDialogueComplete.RemoveListener(OnCampusInteractionDialogueComplete);
        dialogueRunner.onDialogueComplete.AddListener(OnCampusInteractionDialogueComplete);
    }

    private void OnCampusInteractionDialogueComplete()
    {
        if (ContinuousData.instance != null)
            ContinuousData.instance.AllowPlayerToMove();
    }

    private void DeveloperModeCheck()
    {
        GameObject retrievedObject = GameObject.Find("GAMEPLAY_BLOCKER");
        developerMode = retrievedObject != null;
    }

    public void FixedUpdate()
    {
        EnsurePlayerRefsFromContinuousData();
        MapLabelControls();
        CheckCollisions();
    }

    private void EnsurePlayerRefsFromContinuousData()
    {
        var cd = ContinuousData.instance;
        if (cd == null)
            return;
        if (cd.player == null || !cd.player || cd.playerCollider == null || !cd.playerCollider)
            cd.LocatePlayerObject();
        if (playerCollider == null || !playerCollider)
            playerCollider = cd.playerCollider;
    }

    public void CheckCollisions()
    {
        if (playerCollider == null || !playerCollider)
            return;

        TryLoadLibraryEdge();
        TryDoorEdge(toHome, ref _wasTouchingHome, "EnterPlayerHouse", playNotification: false);
        TryDoorEdge(toTheatre, ref _wasTouchingTheatre, "EnterTheatre", playNotification: true);
        TryDoorEdge(toCafe, ref _wasTouchingCafe, "EnterCafe", playNotification: true);
        TryDoorEdge(toGym, ref _wasTouchingGym, "EnterGym", playNotification: true);
    }

    /// <summary>
    /// Fallback for cases where the scene-level CampusSceneDoorTrigger2D is missing/unhooked.
    /// Keeps library access working from CampusGrounds by loading on first overlap edge.
    /// </summary>
    private void TryLoadLibraryEdge()
    {
        if (toLibrary == null || !toLibrary)
            return;
        bool touching = ContinuousData.CollidersOverlap2D(toLibrary, playerCollider);
        if (touching && !_wasTouchingLibrary)
        {
            var cd = ContinuousData.instance;
            if (cd != null)
            {
                cd.AllowPlayerToMove();
                cd.LoadScene("Library");
            }
        }
        _wasTouchingLibrary = touching;
    }

    private void TryDoorEdge(Collider2D zone, ref bool wasTouching, string yarnNode, bool playNotification)
    {
        if (zone == null || !zone)
            return;
        bool touching = ContinuousData.CollidersOverlap2D(zone, playerCollider);
        if (touching && !wasTouching)
            TryBeginCampusDoorDialogue(yarnNode, playNotification);
        wasTouching = touching;
    }

    private void TryBeginCampusDoorDialogue(string yarnNode, bool playNotification)
    {
        ConfigureCampusYarn();
        if (dialogueRunner == null || !dialogueRunner)
            return;
        if (dialogueRunner.IsDialogueRunning)
            return;

        var cd = ContinuousData.instance;
        if (cd != null)
            cd.GatherVars();

        if (playNotification && notificationSound != null)
            notificationSound.Play();

        dialogueRunner.StartDialogue(yarnNode);
        StartCoroutine(ApplyFreezeAfterDoorDialogueStarts());
    }

    private IEnumerator ApplyFreezeAfterDoorDialogueStarts()
    {
        // IsDialogueRunning may not flip until the next frame after StartDialogue.
        yield return null;
        var cd = ContinuousData.instance;
        if (cd == null)
            yield break;
        if (dialogueRunner == null || !dialogueRunner)
        {
            cd.AllowPlayerToMove();
            yield break;
        }
        if (dialogueRunner.IsDialogueRunning)
            cd.FreezePlayer();
        else
            cd.AllowPlayerToMove();
    }

    public void MapLabelControls()
    {
        var cd = ContinuousData.instance;
        if (cd == null || cd.player == null || !cd.player)
            return;

        GameObject homeTP = GameObject.Find("HLDetectionPoint");
        GameObject libraryTP = GameObject.Find("LLDetectionPoint");
        GameObject theatreTP = GameObject.Find("TLDetectionPoint");
        GameObject gymTP = GameObject.Find("GLDetectionPoint");
        GameObject cafeTP = GameObject.Find("CLDetectionPoint");

        var p = cd.player.transform.position;
        if (homeLabel != null && homeTP != null)
        {
            homeLabel.SetActive(Vector3.Distance(p, homeTP.transform.position) < 10f);
        }
        if (libraryLabel != null && libraryTP != null)
        {
            libraryLabel.SetActive(Vector3.Distance(p, libraryTP.transform.position) < 10f);
        }
        if (theatreLabel != null && theatreTP != null)
        {
            theatreLabel.SetActive(Vector3.Distance(p, theatreTP.transform.position) < 10f);
        }
        if (gymLabel != null && gymTP != null)
        {
            gymLabel.SetActive(Vector3.Distance(p, gymTP.transform.position) < 10f);
        }
        if (cafeLabel != null && cafeTP != null)
        {
            cafeLabel.SetActive(Vector3.Distance(p, cafeTP.transform.position) < 10f);
        }
    }
}
