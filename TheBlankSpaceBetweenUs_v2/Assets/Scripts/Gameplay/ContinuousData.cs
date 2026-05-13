using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn.Unity;

[DefaultExecutionOrder(-100)]
public class ContinuousData : MonoBehaviour
{
    //Controling Variables
    [SerializeField] public bool newGame = true;
    public static ContinuousData instance;
    public int EndingIndex;

    //Currently Playing Scene Variables
    public int CDtimeIndex;
    public int CDdayIndex;
    public string currentSceneName;
    public int currentSceneBuildIndex;
    public Scene currentScene;
    public Scene previousScene;
    private string nextSceneString;

    //Gameplay Variables
    public bool libraryVisited;
    public bool allowInteracting;
    public float shortestDistance;

    //Spawn Point Vectors
    public Vector3 campusGrounds_BridgeSpawn = new(39.5f, 1f, 0f);
    public Vector3 campusGrounds_LibrarySpawn = new(-34, 46, 0);
    public Vector3 playerHouse_EntranceSpawn = new(-2.5f, -30, 0);
    public Vector3 playerHouse_MorningSpawn = new(3.5f, 1, 0);
    public Vector3 library_EntranceSpawn = new(2.5f, -12, 0);
    public Vector3 midday_Spawn = new(0, -9, 0);


    //PlayerVars
    public Collider2D playerCollider;
    public GameObject player;
    public string playerName;
    public Vector3 spawnPositionVector;
    public bool allowMovement;

    //Yarn
    public VariableStorageBehaviour yarnStorage;
    public Library libraryRef;
    public DialogueRunner diaRunner;
    public bool clubAttended;
    public string playerClub;

    //Events
    public static Action ReturnYarnAsTrue;
    public static Action ReturnYarnAsFalse;
    public static Action PreSceneChange;
    public static Action NewSceneLoaded;

    /// <summary>
    /// Fired at the end of <see cref="OnSceneLoadedRefreshYarnRoutine"/> (after duplicate dialogue UI teardown and runner rebind).
    /// Use this to start scene dialogue so <see cref="DialogueRunner"/> and EventSystem state are stable.
    /// </summary>
    public static Action<Scene> AfterYarnSceneRefresh;

    //ClubSavedVariables
    public int StrengthLevel;
    public int IntelligenceLevel;
    public int CharismaLevel;

    //Relationship Variables
    public int NikoRP;
    public int FaustRP;
    public int SalemRP;

    //DialogueImage Variables
    public bool NikoDiaImageState;
    public bool SalemDiaImageState;
    public bool FaustDiaImageState;

    //Audio Variables
    public AudioSource hurtSFX;
    public AudioSource talkingSFX;

    /// <summary>
    /// Instance ids we have finished binding <<command>> handlers on. Survives consolidate/reference churn better than a single int.
    /// </summary>
    private static readonly HashSet<int> _yarnRegisteredRunnerInstanceIds = new HashSet<int>();

    /// <summary>Blocks re-entrant <see cref="RegisterYarnCommandHandlersIfNeeded"/> before the id is added to the set above.</summary>
    private static int _yarnRegistrationInProgressInstanceId = int.MinValue;

    private static bool _dialogueKitRootMarkedDontDestroyOnLoad;

    private void Awake()
    {
        // Prefer the Yarn "Dialogue System" runner (has Line View + Canvas), not a bare runtime-created runner.
        diaRunner = FindPreferredDialogueRunner();
        yarnStorage = diaRunner != null ? diaRunner.VariableStorage : null;

        // Ensure player name and default runtime state are set
        if (playerName == null)
        {
            playerName = "Player";
        }
        instance = this;
        allowInteracting = true;
        DontDestroyOnLoad(gameObject);
        CDtimeIndex = 0;
        CDdayIndex = 0;

        // Find player object and set starting stats for a new game
        LocatePlayerObject();
        if (newGame)
        {
            StrengthLevel = 1;
            IntelligenceLevel = 1;
            CharismaLevel = 1;
            NikoRP = 0;
            FaustRP = 0;
            SalemRP = 0;
        }
        shortestDistance = 1000f;
        EndingIndex = -1;

        SceneManager.sceneLoaded += OnSceneLoadedRefreshYarn;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoadedRefreshYarn;
    }

    private void OnSceneLoadedRefreshYarn(Scene scene, LoadSceneMode mode)
    {
        if (instance != this)
            return;

        // Run synchronously so per-scene Awake-spawned UI never ships with two EventSystems / two Dialogue Systems.
        // The coroutine still re-runs these after a frame, but built players must not see input fight on the first frame.
        StopYarnDialogueSafe();
        DestroyDuplicateDialogueKitRootsInScene(scene);
        DedupeEventSystemsPreferPersistentDialogueUi();
        DisableRaycastOnTransparentSceneOverlays(scene);

        StartCoroutine(OnSceneLoadedRefreshYarnRoutine(scene));
    }

    private IEnumerator OnSceneLoadedRefreshYarnRoutine(Scene scene)
    {
        // Let Yarn Spinner tear down LinePresenter / CanvasGroup tweens before we destroy duplicate UI roots.
        yield return null;

        DestroyDuplicateDialogueKitRootsInScene(scene);
        DedupeEventSystemsPreferPersistentDialogueUi();
        DisableRaycastOnTransparentSceneOverlays(scene);
        if (diaRunner == null || !diaRunner)
        {
            diaRunner = null;
            yarnStorage = null;
        }
        EnsureDialogueRunnerAndStorage();
        RegisterYarnCommandHandlersIfNeeded();
        // Any LoadScene path may not call SetSpawnPosition; re-bind PlayerObj from the new scene.
        LocatePlayerObject();
        yield return null;
        DedupeEventSystemsPreferPersistentDialogueUi();
        DisableRaycastOnTransparentSceneOverlays(scene);

        AfterYarnSceneRefresh?.Invoke(scene);
    }

    /// <summary>
    /// Multiple <see cref="EventSystem"/> instances (persistent dialogue UI + scene) break UI raycasts and Yarn choices.
    /// </summary>
    private static void DedupeEventSystemsPreferPersistentDialogueUi()
    {
        var systems = UnityEngine.Object.FindObjectsOfType<EventSystem>(true);
        if (systems == null || systems.Length <= 1)
            return;

        EventSystem keep = null;
        foreach (var es in systems)
        {
            if (es == null || !es.gameObject.scene.IsValid())
                continue;
            if (es.gameObject.scene.name == "DontDestroyOnLoad")
            {
                keep = es;
                break;
            }
        }

        if (keep == null)
        {
            foreach (var es in systems)
            {
                if (es == null)
                    continue;
                for (Transform t = es.transform; t != null; t = t.parent)
                {
                    if (t.name.IndexOf("Dialogue", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        keep = es;
                        break;
                    }
                }
                if (keep != null)
                    break;
            }
        }

        if (keep == null)
            keep = systems[0];

        foreach (var es in systems)
        {
            if (es == null || es == keep)
                continue;
            UnityEngine.Object.Destroy(es.gameObject);
        }
    }

    /// <summary>
    /// After the first full kit is marked DontDestroyOnLoad, scene-level copies of the same prefab
    /// leave two Line Views / CanvasGroups; Yarn can still be fading the duplicate when it is destroyed.
    /// </summary>
    private void DestroyDuplicateDialogueKitRootsInScene(Scene scene)
    {
        if (!_dialogueKitRootMarkedDontDestroyOnLoad || !scene.IsValid())
            return;

        var runners = UnityEngine.Object.FindObjectsOfType<DialogueRunner>(true);
        foreach (var dr in runners)
        {
            if (dr == null || !dr)
                continue;
            if (DialogueRunnerUiScore(dr) < 10)
                continue;
            var root = dr.transform.root.gameObject;
            if (root == null || !root.scene.IsValid() || root.scene != scene)
                continue;

            try
            {
                dr.Stop();
            }
            catch
            {
                /* ignore */
            }

            Destroy(root);
        }
    }

    private void StopYarnDialogueSafe()
    {
        foreach (var dr in UnityEngine.Object.FindObjectsOfType<DialogueRunner>(true))
        {
            if (dr == null || !dr)
                continue;
            try
            {
                dr.Stop();
            }
            catch
            {
                /* Yarn may be mid-teardown */
            }
        }
    }

    private static void MarkDialogueSystemRootPersistent(DialogueRunner dr)
    {
        if (dr == null || !dr || _dialogueKitRootMarkedDontDestroyOnLoad)
            return;
        if (DialogueRunnerUiScore(dr) < 10)
            return;
        var root = dr.transform.root.gameObject;
        if (root == null)
            return;
        DontDestroyOnLoad(root);
        _dialogueKitRootMarkedDontDestroyOnLoad = true;
    }

    /// <summary>
    /// A scene-local transparent non-interactive <see cref="Image"/> with Raycast Target on
    /// can eat UI clicks and stop Yarn's Continue button from firing (often only at some resolutions/builds).
    /// Defuse those while preserving visuals.
    /// </summary>
    private static void DisableRaycastOnTransparentSceneOverlays(Scene scene)
    {
        if (!scene.IsValid())
            return;

        var images = UnityEngine.Object.FindObjectsOfType<Image>(true);
        if (images == null)
            return;

        foreach (var img in images)
        {
            if (img == null || !img)
                continue;
            if (!img.raycastTarget)
                continue;
            if (img.gameObject.scene != scene)
                continue;
            // Heuristic: fully transparent + no sprite + no selectable component => non-interactive overlay blocker.
            if (img.color.a > 0.001f || img.sprite != null)
                continue;
            if (img.GetComponent<Selectable>() != null)
                continue;

            img.raycastTarget = false;
        }
    }

    public void Start()
    {
        EnsureDialogueRunnerAndStorage();
        RegisterYarnCommandHandlersIfNeeded();

        if (diaRunner == null)
        {
            Debug.LogError("ContinuousData: No DialogueRunner available. Add Yarn Spinner’s Dialogue Runner to a bootstrap scene (or rely on auto-created runner after this error is fixed).");
            return;
        }

        // Cache current scene info
        currentScene = SceneManager.GetActiveScene();
        currentSceneName = currentScene.name;
        currentSceneBuildIndex = currentScene.buildIndex;

        // Set default movement and dialogue image states
        allowMovement = true;
        NikoDiaImageState = false;
        SalemDiaImageState = false;
        FaustDiaImageState = false;
    }

    /// <summary>
    /// Registers <<command>> handlers on the current <see cref="diaRunner"/>.
    /// Safe to call every frame: no-ops once this runner id is already bound (see also <see cref="FindPreferredDialogueRunner"/> tie-break).
    /// </summary>
    public void RegisterYarnCommandHandlersIfNeeded()
    {
        EnsureDialogueRunnerAndStorage();
        if (diaRunner == null || !diaRunner)
            return;

        int runnerId = diaRunner.GetInstanceID();
        if (_yarnRegisteredRunnerInstanceIds.Contains(runnerId))
            return;
        if (_yarnRegistrationInProgressInstanceId == runnerId)
            return;

        _yarnRegistrationInProgressInstanceId = runnerId;
        try
        {
            diaRunner.AddCommandHandler<string>("joinClub", joinClub);
            diaRunner.AddCommandHandler("leaveClub", LeaveClub);
            diaRunner.AddCommandHandler<string, int>("incRelationship", IncRelationship);
            diaRunner.AddCommandHandler<string, int>("decRelationship", DecRelationship);
            diaRunner.AddCommandHandler("gatherVars", GatherVars);
            diaRunner.AddCommandHandler("allowPlayerToMove", AllowPlayerToMove);
            diaRunner.AddCommandHandler("freezePlayer", FreezePlayer);
            diaRunner.AddCommandHandler<string>("loadScene", LoadScene);
            diaRunner.AddCommandHandler<string>("startPractice", StartPractice);
            diaRunner.AddCommandHandler("closeDialogue", CloseDialogue);
            diaRunner.AddCommandHandler<string>("pushObjectiveIndex", PushObjectiveIndex);
            diaRunner.AddCommandHandler<string, bool>("setBool", SetBool);
            diaRunner.AddCommandHandler<string, int>("setInt", SetInt);
            diaRunner.AddCommandHandler<string, string>("setString", SetString);
            diaRunner.AddCommandHandler<string, string>("setAnimTrigger", SetAnimTrigger);
            diaRunner.AddCommandHandler("feedPlayerNameToYarn", FeedPlayerNameToYarn);
            diaRunner.AddCommandHandler<string, int>("alterPlayerAttribute", AlterPlayerAttribute);
            diaRunner.AddCommandHandler<int>("cueEnding", CueEnding);

            _yarnRegisteredRunnerInstanceIds.Add(runnerId);
        }
        finally
        {
            if (_yarnRegistrationInProgressInstanceId == runnerId)
                _yarnRegistrationInProgressInstanceId = int.MinValue;
        }
    }

    public void Update()
    {
        // Evaluate and set story ending indices based on day and relationship state
        if (CDdayIndex == 6 && NikoRP > SalemRP && NikoRP > FaustRP && EndingIndex == -1)
        {
            EndingIndex = 5;
        }
        else
        {
            if (CDdayIndex == 6 && NikoRP <= 0 && playerClub != "Wrestling" && SalemRP <= 0)
            {
                EndingIndex = 5;
            }
            // Other endings are triggered elsewhere
        }

        // Set default graduation ending if no other ending selected
        if (CDdayIndex == 7 && EndingIndex == -1)
        {
            EndingIndex = 0;
        }

    }

    private void OnEnable()
    {
        // Subscribe to scene change event and refresh dialogue runner reference
        PreSceneChange += UpdatePrevScene;
        if (diaRunner == null)
            diaRunner = FindPreferredDialogueRunner();
        if (diaRunner != null)
            yarnStorage = diaRunner.VariableStorage;

    }
    private void OnDisable()
    {
        PreSceneChange -= UpdatePrevScene;
    }

    public void FixedUpdate()
    {
        // Update scene info and refresh Yarn variable-driven UI flags
        if (diaRunner == null || !diaRunner)
        {
            diaRunner = FindPreferredDialogueRunner();
            if (diaRunner != null && diaRunner)
                RegisterYarnCommandHandlersIfNeeded();
        }
        if (diaRunner != null && yarnStorage == null)
            yarnStorage = diaRunner.VariableStorage;
        if (player == null || !player)
            LocatePlayerObject();
        currentScene = SceneManager.GetActiveScene();
        currentSceneName = currentScene.name;
        currentSceneBuildIndex = currentScene.buildIndex;
        NikoDiaImageState = MonitorBool("$NikoDiaImage");
        SalemDiaImageState = MonitorBool("$SalemDiaImage");
        FaustDiaImageState = MonitorBool("$FaustDiaImage");
    }


    public void LocatePlayerObject()
    {
        // Find player GameObject and cache its collider (scene transitions destroy the old instance).
        var found = GameObject.Find("PlayerObj");
        if (found == null)
        {
            player = null;
            playerCollider = null;
            return;
        }
        player = found;
        playerCollider = player.GetComponent<Collider2D>();
    }

    /// <summary>
    /// Door volumes use trigger colliders; the player usually uses a non-trigger collider.
    /// <see cref="Physics2D.IsTouching"/> often never becomes true for that pair, so door prompts never run.
    /// </summary>
    public static bool CollidersOverlap2D(Collider2D a, Collider2D b)
    {
        if (a == null || !a || b == null || !b || !a.enabled || !b.enabled)
            return false;
        ColliderDistance2D d = Physics2D.Distance(a, b);
        return d.isOverlapped || d.distance <= 0f;
    }

    public void UpdatePrevScene()
    {
        // Store the current scene as the previous scene for transitions
        previousScene = currentScene;
    }

    public void UpdatePlayerName(string newName)
    {
        // Update stored player name
        playerName = newName;
    }

    public void UpdateSavedTime(int timeIndex, int dayIndex)
    {
        // Save current time and day indices from TimeManager
        CDtimeIndex = TimeManager.TimeFrameIndex;
        CDdayIndex = TimeManager.Day;
    }


    public void FetchYarnStringVariable(string yarnVar, string unityVar)
    {
        // Read a string variable from Yarn storage and log it
        yarnStorage.TryGetValue(yarnVar, out unityVar);
        Debug.Log("String Fetched: " + unityVar);
    }

    public void FetchYarnIntVariable(string yarnVar, int unityVar)
    {
        // Read an int variable from Yarn storage and log it
        yarnStorage.TryGetValue(yarnVar, out unityVar);
        Debug.Log("Int Fetched: " + unityVar);
    }

    public void SetYarnStringVariable(string yarnVar, string updatedString)
    {
        // Set a string variable in Yarn storage
        yarnStorage.SetValue(yarnVar, updatedString);

    }
    public void SceneChangeDetected(string sceneToLoad, Vector3 nextSpawnPoint)
    {
        StopYarnDialogueSafe();
        // Begin scene change workflow and remember next spawn point
        PreSceneChange?.Invoke();
        nextSceneString = sceneToLoad;
        SceneLoad(nextSpawnPoint);

    }

    public void SceneLoad(Vector3 nextSpawnPoint)
    {
        StopYarnDialogueSafe();
        AllowPlayerToMove();
        // Load the given scene, set spawn, and notify listeners
        SceneManager.LoadScene(nextSceneString);
        NewSceneLoaded?.Invoke();
        SetSpawnPosition(nextSpawnPoint);

        FeedPlayerNameToYarn();

    }

    public void SetSpawnPosition(Vector3 targetposition)
    {
        // Update the spawn vector and position the player
        spawnPositionVector = targetposition;
        LocatePlayerObject();
        InitialisePlayer();
    }
    private void InitialisePlayer()
    {
        // Move the player to the cached spawn position
        if (player == null || !player)
            return;
        player.transform.position = spawnPositionVector;
    }

    private void UpdateSpawnVector(Vector3 newSpawnVector)
    {
        // Update the internal spawn vector value
        spawnPositionVector = newSpawnVector;
    }

    public void UpdateNextScene(string nextScene)
    {
        // Set the name of the next scene without loading it
        nextSceneString = nextScene;
    }
    public void AlterPlayerAttribute(string attribute, int amount)
    {
        switch (attribute)
        {
            case "Strength":
                StrengthLevel += amount;
                break;
            case "Intelligence":
                IntelligenceLevel += amount;
                break;
            case "Charisma":
                CharismaLevel += amount;
                break;
            default:
                Debug.Log("ERROR: No Matching Attribute Found");
                break;
        }
    }

    public void SetMovementLock(bool movementLocked)
    {
        // When true, movement is locked (player frozen); when false, player can move.
        allowMovement = !movementLocked;
    }

    ///YARN COMMANDS

    public void joinClub(string clubName)
    {
        // Set the current club affiliation for the player
        playerClub = clubName;
    }


    public void LeaveClub()
    {
        // Clear the player's club affiliation
        playerClub = "None";
    }


    public void IncRelationship(string characterName, int amount)
    {
        if (characterName == "Niko")
        {
            NikoRP += amount;
        }
        else if (characterName == "Faust")
        {
            FaustRP += amount;
        }
        else if (characterName == "Salem")
        {
            SalemRP += amount;
        }
        else
        {
            Debug.Log("ERROR: No Matching Character Found");
        }
    }


    public void DecRelationship(string characterName, int amount)
    {
        if (characterName == "Niko")
        {
            NikoRP -= amount;
        }
        else if (characterName == "Faust")
        {
            FaustRP -= amount;
        }
        else if (characterName == "Salem")
        {
            SalemRP -= amount;
        }
        else
        {
            Debug.Log("ERROR: No Matching Character Found");
        }
    }


    public void GatherVars()
    {
        EnsureDialogueRunnerAndStorage();
        if (diaRunner == null || !diaRunner || diaRunner.VariableStorage == null)
            return;
        // Push key player variables into Yarn's variable storage
        diaRunner.VariableStorage.SetValue("$playerName", playerName);
        diaRunner.VariableStorage.SetValue("$playerClub", playerClub);
        diaRunner.VariableStorage.SetValue("$clubAttended", clubAttended);
        diaRunner.VariableStorage.SetValue("$salemRP", SalemRP);
        diaRunner.VariableStorage.SetValue("$nikoRP", NikoRP);
        diaRunner.VariableStorage.SetValue("$faustRP", FaustRP);

    }

    /// <summary>
    /// Yarn: attends club practice / minigame scene. Theatre has no practice scene in build yet.
    /// </summary>
    public void StartPractice(string clubName)
    {
        clubAttended = true;
        EnsureDialogueRunnerAndStorage();
        if (diaRunner != null && diaRunner.VariableStorage != null)
            diaRunner.VariableStorage.SetValue("$clubAttended", clubAttended);

        switch (clubName)
        {
            case "Wrestling":
                LoadScene("Gym");
                return;
            case "Debate":
                LoadScene("Library");
                return;
            case "Theatre":
            default:
                AllowPlayerToMove();
                if (diaRunner != null && diaRunner)
                {
                    try { diaRunner.Stop(); } catch { /* Yarn teardown */ }
                }
                break;
        }
    }

    public void AllowPlayerToMove()
    {
        // Allow the player to move
        allowMovement = true;
    }

    public void FreezePlayer()
    {
        // Prevent the player from moving
        allowMovement = false;
    }

    public void LoadScene(string sceneName)
    {
        StopYarnDialogueSafe();
        AllowPlayerToMove();
        // Immediately load the provided scene name
        nextSceneString = sceneName;
        SceneManager.LoadScene(nextSceneString);
        NewSceneLoaded?.Invoke();

    }

    public void CloseDialogue()
    {
        // Stop any running Yarn dialogue
        diaRunner.Stop();
    }

    public void PushObjectiveIndex(string objectiveTitleString)
    {
        Objective targetObjective = null;
        ObjectivesManager.instance.AssignObjectiveByTitle(objectiveTitleString, targetObjective);

        if (targetObjective != null)
        {
            ObjectivesManager.instance.IncObjectiveIndex(targetObjective);
        }
        else
        {
            Debug.Log("ERROR: No Matching Objective Found");
        }
    }

    public void SetBool(string boolName, bool boolState)
    {
        var field = GetType().GetField(boolName);

        if (field != null && field.FieldType == typeof(bool))
        {
            // Set a boolean field by name via reflection
            field.SetValue(this, boolState);
        }
        else
        {
            Debug.LogError($"Bool '{boolName}' not found!");
        }


    }

    public void SetInt(string intName, int intSet)
    {
        var field = GetType().GetField(intName);

        if (field != null && field.FieldType == typeof(int))
        {
            // Set an integer field by name via reflection
            field.SetValue(this, intSet);
        }
        else
        {
            Debug.LogError($"Int '{intName}' not found!");
        }
    }

    public void SetString(string stringName, string stringState)
    {
        // Set a string field by name via reflection
        var field = GetType().GetField(stringName);

        if (field != null && field.FieldType == typeof(string))
        {
            field.SetValue(this, stringState);
        }
        else
        {
            Debug.LogError($"String '{stringName}' not found!");
        }
    }

    public void SetAnimTrigger(string triggerName, string animatorName)
    {
        // Trigger an animation on the given animator GameObject
        Animator controllingAnimator = GameObject.Find(animatorName).GetComponent<Animator>();
        if (controllingAnimator != null)
        {
            controllingAnimator.SetTrigger(triggerName);
        }
        else
        {
            Debug.Log("ERROR: No Matching Animator Found");
        }
    }

    public void FeedPlayerNameToYarn()
    {

        // Write the stored player name into Yarn variable storage
        if (yarnStorage != null)
        {
            yarnStorage.SetValue("$playerName", playerName);
        }

    }

    public void PlayHurtSoundEffect()
    {
        // Play hurt SFX audio
        hurtSFX.Play();
    }

    public void PlayTalkSoundEffect()
    {
        // Play talking SFX audio
        talkingSFX.Play();
    }

    public void CueEnding(int endingNum)
    {
        // Set ending index and immediately transition to endings scene
        EndingIndex = endingNum;
        SceneChangeDetected("Endings", new Vector3(-2.5f, -14.7f, 0));
    }

    public bool MonitorBool(string variableName)
    {
        if (yarnStorage == null && diaRunner != null)
            yarnStorage = diaRunner.VariableStorage;
        if (yarnStorage == null)
            return false;

        yarnStorage.TryGetValue(variableName, out bool value);
        return value;
    }

    /// <summary>
    /// Picks the DialogueRunner that actually has Yarn line UI (Canvas / Line View). Bare runtime runners score lower.
    /// </summary>
    public static DialogueRunner FindPreferredDialogueRunner()
    {
        var runners = UnityEngine.Object.FindObjectsOfType<DialogueRunner>(true);
        if (runners == null || runners.Length == 0)
            return null;

        DialogueRunner best = null;
        int bestScore = int.MinValue;
        int bestInstanceId = int.MaxValue;
        foreach (var dr in runners)
        {
            if (dr == null || !dr)
                continue;
            int sc = DialogueRunnerUiScore(dr);
            int id = dr.GetInstanceID();
            // Stable when scores tie (otherwise FindObjectsOfType order can flip each frame and we re-register on the wrong runner).
            if (sc > bestScore || (sc == bestScore && id.CompareTo(bestInstanceId) < 0))
            {
                bestScore = sc;
                bestInstanceId = id;
                best = dr;
            }
        }
        return best;
    }

    private static int DialogueRunnerUiScore(DialogueRunner dr)
    {
        if (dr == null)
            return int.MinValue;
        int s = 0;
        string name = dr.gameObject.name;
        if (name.IndexOf("Dialogue System", StringComparison.OrdinalIgnoreCase) >= 0)
            s += 20;
        if (dr.GetComponentInChildren<Canvas>(true) != null)
            s += 10;
        if (name.StartsWith("DialogueRunner (runtime)", StringComparison.Ordinal))
            s -= 100;
        return s;
    }

    private static bool IsBareRuntimeDialogueRunner(DialogueRunner dr)
    {
        return dr != null && dr.gameObject.name.StartsWith("DialogueRunner (runtime)", StringComparison.Ordinal);
    }

    private void ConsolidateToPreferredDialogueRunner()
    {
        var preferred = FindPreferredDialogueRunner();
        if (preferred == null || preferred == diaRunner)
            return;

        DialogueRunner previous = diaRunner;
        if (previous != null && previous && IsBareRuntimeDialogueRunner(previous))
            Destroy(previous.gameObject);

        diaRunner = preferred;
        yarnStorage = diaRunner.VariableStorage;
        MarkDialogueSystemRootPersistent(diaRunner);
    }

    /// <summary>
    /// Ensures a DialogueRunner exists even if this scene has no ContinuousData yet,
    /// or before <see cref="Start"/> runs on ContinuousData.
    /// </summary>
    public static void EnsureDialogueRunnerExistsInScene()
    {
        var existing = FindPreferredDialogueRunner();
        if (existing != null)
        {
            if (instance != null)
            {
                instance.diaRunner = existing;
                if (existing.VariableStorage != null)
                    instance.yarnStorage = existing.VariableStorage;
                instance.RegisterYarnCommandHandlersIfNeeded();
            }
            MarkDialogueSystemRootPersistent(existing);
            return;
        }

        if (instance != null)
        {
            instance.EnsureDialogueRunnerAndStorage();
            instance.RegisterYarnCommandHandlersIfNeeded();
            return;
        }

        var go = new GameObject("DialogueRunner (runtime)");
        DontDestroyOnLoad(go);
        var dr = go.AddComponent<DialogueRunner>();
        var storage = go.AddComponent<InMemoryVariableStorage>();
        dr.VariableStorage = storage;
        Debug.LogWarning("No ContinuousData in this scene; created a minimal DialogueRunner (no on-screen lines). Add ContinuousDataObj and the Dialogue System Variant prefab from Assets/Prefabs.");
    }

    public void EnsureDialogueRunnerAndStorage()
    {
        if (diaRunner == null)
            diaRunner = FindPreferredDialogueRunner();

        if (diaRunner == null)
            diaRunner = FindObjectOfType<DialogueRunner>(true);

        if (diaRunner == null)
        {
            var go = new GameObject("DialogueRunner (runtime)");
            DontDestroyOnLoad(go);
            diaRunner = go.AddComponent<DialogueRunner>();
            var storage = go.AddComponent<InMemoryVariableStorage>();
            diaRunner.VariableStorage = storage;
            yarnStorage = storage;
            Debug.LogWarning("ContinuousData: Created a minimal DialogueRunner (no Line View). Add Assets/Prefabs/Dialogue System Variant.prefab to this scene for visible dialogue.");
        }
        else
        {
            if (yarnStorage == null)
                yarnStorage = diaRunner.VariableStorage;
        }

        ConsolidateToPreferredDialogueRunner();
        MarkDialogueSystemRootPersistent(diaRunner);
    }

}
