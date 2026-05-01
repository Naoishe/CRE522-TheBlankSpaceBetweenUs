using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class ContinuousData : MonoBehaviour
{
    //Controling Variables
    [SerializeField] public bool newGame=true;
    public static ContinuousData instance;

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
    public Vector3 campusGrounds_LibrarySpawn = new(-34,46,0);
    public Vector3 playerHouse_EntranceSpawn = new(-2.5f,-30,0);
    public Vector3 playerHouse_MorningSpawn = new(3.5f,1,0);
    public Vector3 library_EntranceSpawn = new(2.5f,-12,0);
    public Vector3 midday_Spawn = new(0,-9,0);


    //PlayerVars
    public Collider2D playerCollider;
    public GameObject player;
    public string playerName;
    public Vector3 spawnPositionVector;
    public bool allowMovement;

    //Yarn
    public InMemoryVariableStorage variableStorage;
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

    private void Awake()
    {
        instance = this;
        allowInteracting = true;
        DontDestroyOnLoad(gameObject);
        CDtimeIndex = 0;
        CDdayIndex = 0;
        variableStorage = FindObjectOfType<InMemoryVariableStorage>();
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
        allowMovement = true;

    }

    public void Update()
    {

    }

    private void OnEnable()
    {
        PreSceneChange += UpdatePrevScene;
        diaRunner = FindObjectOfType<DialogueRunner>();

    }
    private void OnDisable()
    {
        PreSceneChange -= UpdatePrevScene;
    }

    public void FixedUpdate()
    {
        currentScene = SceneManager.GetActiveScene();
        currentSceneName = currentScene.name;
        currentSceneBuildIndex = currentScene.buildIndex;
    }

    public void LocatePlayerObject()
    {
        player = GameObject.Find("PlayerObj");
        playerCollider = player.GetComponent<Collider2D>();

    }

    public void UpdatePrevScene()
    {
        previousScene = currentScene;
    }

    public void UpdateSavedTime(int timeIndex, int dayIndex)
    {
        CDtimeIndex = TimeManager.TimeFrameIndex;
        CDdayIndex = TimeManager.Day;
    }


    public void UpdatePlayerName(string name)
    {
        playerName = name;
        variableStorage.SetValue("$playerName", playerName);

    }

    public void FetchYarnStringVariable(string yarnVar, string unityVar)
    {
        variableStorage.TryGetValue(yarnVar, out unityVar);
        Debug.Log("String Fetched: " + unityVar);
    }
    public void FetchYarnBoolVariable(string yarnVar, bool unityVar)
    {
        variableStorage.TryGetValue(yarnVar, out unityVar);
        Debug.Log("Bool Fetched: " + unityVar);
        if (unityVar)
        {
            ReturnYarnAsTrue?.Invoke();
        }
        else
        {
            ReturnYarnAsFalse?.Invoke();
        }
    }
    public void FetchYarnIntVariable(string yarnVar, int unityVar)
    {
        variableStorage.TryGetValue(yarnVar, out unityVar);
        Debug.Log("Int Fetched: " + unityVar);
    }

    public void SetYarnStringVariable(string yarnVar, string updatedString)
    {
        variableStorage.SetValue(yarnVar, updatedString);

    }
    public void SceneChangeDetected(string sceneToLoad, Vector3 nextSpawnPoint)
    {
        PreSceneChange?.Invoke();
        nextSceneString = sceneToLoad;
        SceneLoad(nextSpawnPoint);

    }

    public void SceneLoad(Vector3 nextSpawnPoint)
    {
        SceneManager.LoadScene(nextSceneString);
        NewSceneLoaded?.Invoke();
        SetSpawnPosition(nextSpawnPoint);
        diaRunner = FindObjectOfType<DialogueRunner>();
    }

    public void SetSpawnPosition(Vector3 targetposition)
    {
        spawnPositionVector = targetposition;
        LocatePlayerObject();
        InitialisePlayer();
    }
    private void InitialisePlayer()
    {
        player.transform.position = spawnPositionVector;
    }

    [YarnCommand("incPlayerAttribute")]
    public void IncPlayerAttribute(string attribute, int amount)
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
        allowMovement = boolLockState;
    }

    ///YARN COMMANDS

    [YarnCommand( "startPractice")]
    public void StartPractice(string practiceScene)
    {
        SceneManager.LoadScene(practiceScene);
    }

    [YarnCommand( "endPractice")]
    public void EndPractice()
    {
        SceneManager.LoadScene("CampusGrounds");
    }

    [YarnCommand("joinClub")]
    public void joinClub(string clubName)
    {
        playerClub = clubName;
    }

    [YarnCommand("leaveClub")]
    public void LeaveClub()
    {
        playerClub = "None";
    }

    [YarnCommand("incRelationship")]
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

    [YarnCommand("decRelationship")]
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

    [YarnCommand("gatherVars")]
    public void GatherVars()
    {
        diaRunner.VariableStorage.SetValue("$playerName", playerName);
        diaRunner.VariableStorage.SetValue("$playerClub", playerClub);
        diaRunner.VariableStorage.SetValue("$clubAttended", clubAttended);
        diaRunner.VariableStorage.SetValue("$salemRP", SalemRP);
        diaRunner.VariableStorage.SetValue("$nikoRP", NikoRP);
        diaRunner.VariableStorage.SetValue("$faustRP", FaustRP);

    }

    [YarnCommand("allowPlayerToMove")]
    public void AllowPlayerToMove()
    {
        allowMovement = true;
    }

    [YarnCommand("freezePlayer")]
    public void FreezePlayer()
    {
        allowMovement = false;
    }

    [YarnCommand("loadScene")]
    public void LoadScene(string sceneName)
    {
        nextSceneString = sceneName;
        SceneManager.LoadScene(nextSceneString);
        NewSceneLoaded?.Invoke();
        diaRunner = FindObjectOfType<DialogueRunner>();
    }

    [YarnCommand("closeDialogue")]
    public void CloseDialogue()
    {
        diaRunner.Stop();
    }
}
