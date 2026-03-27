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
    [SerializeField] public bool sceneTestMode;
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

    //Spawn Point Vectors
    public Vector3 campusGrounds_BridgeSpawn = new(39.5f, 1f, 0f);
    public Vector3 campusGrounds_LibrarySpawn = new(-34,46,0);
    public Vector3 playerHouse_EntranceSpawn = new(-2.5f,-30,0);
    public Vector3 playerHouse_MorningSpawn = new(3.5f,1,0);
    public Vector3 library_EntranceSpawn = new(2.5f,-12,0);
    public Vector3 midday_Spawn = new(0,-9,0);


    //PlayerVars
    public int interactionsHad;
    public Collider2D playerCollider;
    public GameObject player;
    public string playerName;
    public Vector3 spawnPositionVector;

    //Yarn
    public InMemoryVariableStorage variableStorage;
    public Library libraryRef;

    //Events
    public static Action ReturnYarnAsTrue;
    public static Action ReturnYarnAsFalse;
    public static Action PreSceneChange;
    public static Action NewSceneLoaded;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        CDtimeIndex = 0;
        CDdayIndex = 0;
        interactionsHad = 0;
        variableStorage = FindObjectOfType<InMemoryVariableStorage>();

        if (!sceneTestMode)
        {

        }
    }

    public void Update()
    {

    }

    private void OnEnable()
    {
        PreSceneChange += UpdatePrevScene;
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

    public void UpdateInteractionCount()
    {
        interactionsHad++;
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
        SetSpawnPosition(nextSpawnPoint);
    }

    public void SetSpawnPosition(Vector3 targetposition)
    {
        spawnPositionVector = targetposition;
        InitialisePlayer();
    }
    private void InitialisePlayer()
    {
        player.transform.position = spawnPositionVector;
    }
}
