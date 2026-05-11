using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

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
    public InMemoryVariableStorage yarnStorage;
    public Library libraryRef;
    public DialogueRunner diaRunner;
    public bool clubAttended;
    public string playerClub;

    //Events
    public static Action ReturnYarnAsTrue;
    public static Action ReturnYarnAsFalse;
    public static Action PreSceneChange;
    public static Action NewSceneLoaded;

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

    private void Awake()
    {
        // Initialize singleton and locate the dialogue runner and variable storage
        diaRunner = FindObjectOfType<DialogueRunner>();
        yarnStorage = diaRunner.VariableStorage as InMemoryVariableStorage;

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

    }

    public void Start()
    {
        // Register Yarn command handlers and cache current scene info
        currentScene = SceneManager.GetActiveScene();
        currentSceneName = currentScene.name;
        currentSceneBuildIndex = currentScene.buildIndex;
        diaRunner.AddCommandHandler<string>("joinClub", joinClub);
        diaRunner.AddCommandHandler("leaveClub", LeaveClub);
        diaRunner.AddCommandHandler<string, int>("incRelationship", IncRelationship);
        diaRunner.AddCommandHandler<string, int>("decRelationship", DecRelationship);
        diaRunner.AddCommandHandler("gatherVars", GatherVars);
        diaRunner.AddCommandHandler("allowPlayerToMove", AllowPlayerToMove);
        diaRunner.AddCommandHandler("freezePlayer", FreezePlayer);
        diaRunner.AddCommandHandler<string>("loadScene", LoadScene);
        diaRunner.AddCommandHandler("closeDialogue", CloseDialogue);
        diaRunner.AddCommandHandler<string>("pushObjectiveIndex", PushObjectiveIndex);
        diaRunner.AddCommandHandler<string, bool>("setBool", SetBool);
        diaRunner.AddCommandHandler<string, int>("setInt", SetInt);
        diaRunner.AddCommandHandler<string, string>("setString", SetString);
        diaRunner.AddCommandHandler<string, string>("setAnimTrigger", SetAnimTrigger);
        diaRunner.AddCommandHandler("feedPlayerNameToYarn", FeedPlayerNameToYarn);
        diaRunner.AddCommandHandler<string, int>("alterPlayerAttribute", AlterPlayerAttribute);
        diaRunner.AddCommandHandler<int>("cueEnding", CueEnding);

        // Set default movement and dialogue image states
        allowMovement = true;
        NikoDiaImageState = false;
        SalemDiaImageState = false;
        FaustDiaImageState = false;
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
        diaRunner = FindObjectOfType<DialogueRunner>();

    }
    private void OnDisable()
    {
        PreSceneChange -= UpdatePrevScene;
    }

    public void FixedUpdate()
    {
        // Update scene info and refresh Yarn variable-driven UI flags
        diaRunner = FindObjectOfType<DialogueRunner>();
        currentScene = SceneManager.GetActiveScene();
        currentSceneName = currentScene.name;
        currentSceneBuildIndex = currentScene.buildIndex;
        NikoDiaImageState = MonitorBool("$NikoDiaImage");
        SalemDiaImageState = MonitorBool("$SalemDiaImage");
        FaustDiaImageState = MonitorBool("$FaustDiaImage");
    }


    public void LocatePlayerObject()
    {
        // Find player GameObject and cache its collider
        player = GameObject.Find("PlayerObj");
        playerCollider = player.GetComponent<Collider2D>();

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
        // Begin scene change workflow and remember next spawn point
        PreSceneChange?.Invoke();
        nextSceneString = sceneToLoad;
        SceneLoad(nextSpawnPoint);

    }

    public void SceneLoad(Vector3 nextSpawnPoint)
    {
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

    public void SetMovementLock(bool boolLockState)
    {
        // Toggle player movement ability
        allowMovement = boolLockState;
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
        // Push key player variables into Yarn's variable storage
        diaRunner.VariableStorage.SetValue("$playerName", playerName);
        diaRunner.VariableStorage.SetValue("$playerClub", playerClub);
        diaRunner.VariableStorage.SetValue("$clubAttended", clubAttended);
        diaRunner.VariableStorage.SetValue("$salemRP", SalemRP);
        diaRunner.VariableStorage.SetValue("$nikoRP", NikoRP);
        diaRunner.VariableStorage.SetValue("$faustRP", FaustRP);

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
        // Read a boolean Yarn variable; return false if storage is unavailable
        if (yarnStorage == null)
        {
            Debug.LogError("Variable storage is not InMemoryVariableStorage");
            return false;
        }

        yarnStorage.TryGetValue(variableName, out bool value);
        return value;
    }


}
