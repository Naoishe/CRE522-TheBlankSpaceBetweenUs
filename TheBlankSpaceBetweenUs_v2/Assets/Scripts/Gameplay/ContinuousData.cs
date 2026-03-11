using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class ContinuousData : MonoBehaviour
{
    public static ContinuousData instance;
    public Scene currentScene;
    public Scene previousScene;
    public string playerName;

    public int CDtimeIndex;
    public int CDdayIndex;
    public string currentSceneName;
    public int currentSceneBuildIndex;
    public bool nikoImagebool;


    public bool libraryVisited;

    public int interactionsHad;

    public string newVar;

    public InMemoryVariableStorage variableStorage;
    
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        CDtimeIndex = 0;
        CDdayIndex = 0;
        interactionsHad = 0;
        variableStorage = FindObjectOfType<InMemoryVariableStorage>();
        nikoImagebool = false;
        libraryVisited = false;
}

    public void Update()
    {
        if (currentSceneName == "Library")
        {
            libraryVisited = true;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            FetchYarnVariable("$stringVar",newVar);

        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            UpdatePlayerName("Naoishe");
        }
    }

    private void OnEnable()
    {
        Day1Control.PreSceneChange += UpdatePrevScene;
    }
    private void OnDisable()
    {
        Day1Control.PreSceneChange -= UpdatePrevScene;
    }

    public void FixedUpdate()
    {
        currentScene= SceneManager.GetActiveScene();
        currentSceneName=currentScene.name;
        currentSceneBuildIndex = currentScene.buildIndex;
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

    public void FetchYarnVariable(string yarnVar, string unityVar)
    {
        variableStorage.TryGetValue(yarnVar, out unityVar);
        Debug.Log("String Fetched: " + unityVar);
    }


}
