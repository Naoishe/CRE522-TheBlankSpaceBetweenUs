using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Collections.AllocatorManager;

public class Day1Control : MonoBehaviour
{

    public static Action PreSceneChange;
    public static Action NewSceneLoaded;
    public static Action ObjUpdate1;
    public static Action ObjUpdate2;

    public Collider2D playerCollider;

    private GameObject player;

    private string nextSceneString;

    public static Day1Control instance;
    public string playerName;


    /// <summary>
    /// Need to create a method to reset variables upon 'replay'
    /// </summary>
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        
        NewSceneLoaded += InitialisePlayer;
        PHDay1.LeavingHouse += SceneChangeDetected;
        CampusGrounds.SceneChanged+= SceneChangeDetected;
        Library.ReturnToCampus += SceneChangeDetected;
    }

    private void OnDisable()
    {
        NewSceneLoaded -= InitialisePlayer;
        PHDay1.LeavingHouse -= SceneChangeDetected;
        CampusGrounds.SceneChanged -= SceneChangeDetected;
        Library.ReturnToCampus -= SceneChangeDetected;
    }

    private void Start()
    {
        player = GameObject.Find("PlayerObj");
        playerCollider = player.GetComponent<Collider2D>();
        
        
    }
    void Update()
    {
        
        player = GameObject.Find("PlayerObj");
        playerCollider = player.GetComponent<Collider2D>();

    }

public void SceneChangeDetected()
{
    if (ContinuousData.instance.currentSceneName == "PlayerHouse")
    {
    
        PreSceneChange?.Invoke();
        nextSceneString = "Midday";
        StartCoroutine(SceneLoad());
    }
    if (ContinuousData.instance.currentSceneName == "Library")
    {
            PreSceneChange?.Invoke();
            nextSceneString = "CampusGrounds";
            StartCoroutine(SceneLoad());
    }

    if (ContinuousData.instance.currentSceneName == "CampusGrounds")
    {
            CampusGroundsSceneChange();
    }

    if (ContinuousData.instance.currentSceneName == "Midday")
    {
        PreSceneChange?.Invoke();
        nextSceneString = "CampusGrounds";
        StartCoroutine(SceneLoad());
    }
}

    public void CampusGroundsSceneChange()
    {

        PreSceneChange?.Invoke();
        GameObject gameManager;
        gameManager = GameObject.Find("GameManager");
        nextSceneString = gameManager.GetComponent<CampusGrounds>().targetScene;
        StartCoroutine(SceneLoad());
    }

    public void ClassEnded()
    {
        nextSceneString = "CampusGrounds";
        BufferMethod();
    }

    private void BufferMethod()
    {
        PreSceneChange?.Invoke();
        StartCoroutine(SceneLoad());
    }

    IEnumerator SceneLoad()
    {
        //Debug.Log("Loading delay... - 1.5s");
        yield return new WaitForSeconds(0f);
        SceneManager.LoadScene(nextSceneString);
        NewSceneLoaded?.Invoke();
        

    }

    private void InitialisePlayer()
    {
        if (ContinuousData.instance.currentSceneName == "CampusGrounds" && ContinuousData.instance.previousScene.name=="Midday")
        {
            player.transform.position = new Vector3(40f,0.5f,0f);
        }
        if (ContinuousData.instance.currentSceneName == "CampusGrounds" && ContinuousData.instance.previousScene.name == "Library")
        {
            player.transform.position = new Vector3(-34f,43f, 0f);
        }
    }


}
