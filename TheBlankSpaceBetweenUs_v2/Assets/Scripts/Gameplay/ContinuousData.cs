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
    public static ContinuousData instance;
    public Scene currentScene;
    public Scene previousScene;
    public string playerName;

    public int CDtimeIndex;
    public int CDdayIndex;
    public string currentSceneName;
    public int currentSceneBuildIndex;


    public bool libraryVisited;

    public int interactionsHad;

    public string newVar;

    public InMemoryVariableStorage variableStorage;
    public Library libraryRef;

    public static Action ReturnYarnAsTrue;
    public static Action ReturnYarnAsFalse;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        CDtimeIndex = 0;
        CDdayIndex = 0;
        interactionsHad = 0;
        variableStorage = FindObjectOfType<InMemoryVariableStorage>();
        //nikoImagebool = false;
        libraryVisited = false;
    }

    public void Update()
    {
        if (currentSceneName == "Library")
        {
            libraryVisited = true;
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


}
