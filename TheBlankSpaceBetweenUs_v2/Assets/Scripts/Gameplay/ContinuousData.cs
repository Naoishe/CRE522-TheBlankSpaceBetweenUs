using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinuousData : MonoBehaviour
{
    public static ContinuousData instance;
    public Scene currentScene;
    public Scene previousScene;
    //public string playerName;

    public int CDtimeIndex;
    public int CDdayIndex;
    public string currentSceneName;
    public int currentSceneBuildIndex;

    public int interactionsHad;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        CDtimeIndex = 0;
        CDdayIndex = 0;
        interactionsHad = 0;
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

    

}
