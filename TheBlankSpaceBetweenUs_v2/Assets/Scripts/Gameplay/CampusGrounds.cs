using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CampusGrounds : MonoBehaviour
{
    public Collider2D toLibrary;
    public Collider2D toHome;
    public Collider2D playerCollider;
    public GameObject noReturn;
    public AudioSource notificationSound;
    public string targetScene;

    public static Action SceneChanged;

    private GameObject homeLabel;
    private GameObject libraryLabel;

    [SerializeField] bool developerMode;
    private void Awake()
    {
        homeLabel = GameObject.Find("HomeLabel");
        libraryLabel = GameObject.Find("LibraryLabel");
    }

    private void Start()
    {
        DeveloperModeCheck();
        homeLabel.SetActive(false);
        libraryLabel.SetActive(false);

        //IF DEVELOPER MODE DISABLED:
        if (!developerMode)
        {
            if (ContinuousData.instance.libraryVisited)
            {
                
                ContinuousData.instance.player.transform.position = new Vector3(-34f, 45.5f, 0f);
            }
            else
            {
               
                ContinuousData.instance.player.transform.position = new Vector3(3.85f, 1f, 0f);
            }
        }
    }
        
    

    private void DeveloperModeCheck()
    {
        GameObject retrievedObject = GameObject.Find("GAMEPLAY_BLOCKER");
        if (retrievedObject != null)
        {
            developerMode = true;
        }
        else
        {
            developerMode = false;
        }
    }

    IEnumerator DelayObj(GameObject gameObj)
    {
        yield return new WaitForSeconds(5f);
        gameObj.SetActive(false);
        
    }

    private void Update()
    {
        

    }

    public void FixedUpdate()
    {
        MapLabelControls();
        CheckCollisions();

    }

    public void CheckCollisions()
    {
        if (Physics2D.IsTouching(toLibrary, playerCollider))
        {
            ContinuousData.instance.SceneChangeDetected("Library", ContinuousData.instance.library_EntranceSpawn);
        }
        if (Physics2D.IsTouching(toHome, playerCollider))
        {
            if (ContinuousData.instance.libraryVisited)
            {
                Debug.Log("GAME NOT CONTINUED FROM HERE");
                //CONTINUE POINT
                //ContinuousData.instance.SceneChangeDetected("PlayerHouse", ContinuousData.instance.playerHouse_EntranceSpawn);
            }
            else
            {
                //Error from trying to return home early 
                noReturn.SetActive(true);
                StartCoroutine(DelayObj(noReturn));
            }

        }
    }

    public void MapLabelControls()
    {
        GameObject homeTP = GameObject.Find("HLDetectionPoint");
        GameObject libraryTP = GameObject.Find("LLDetectionPoint");


        if (Vector3.Distance(ContinuousData.instance.player.transform.position, homeTP.transform.position) < 10f)
        {
            homeLabel.SetActive(true);
        }
        else
        {
            homeLabel.SetActive(false);
        }
        if (Vector3.Distance(ContinuousData.instance.player.transform.position, libraryTP.transform.position) < 10f)
        {
           libraryLabel.SetActive(true);
        }
        else
        {
            libraryLabel.SetActive(false);
        }

    }

   
}
