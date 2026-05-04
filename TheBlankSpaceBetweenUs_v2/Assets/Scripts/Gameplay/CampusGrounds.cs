using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class CampusGrounds : MonoBehaviour
{
    public Collider2D toLibrary;
    public Collider2D toHome;
    public Collider2D toTheatre;
    public Collider2D toGym;
    public Collider2D toCafe;
    public Collider2D playerCollider;
    public GameObject noReturn;
    public AudioSource notificationSound;
    public string targetScene;

    public static Action SceneChanged;

    private GameObject homeLabel;
    private GameObject libraryLabel;
    private GameObject gymLabel;
    private GameObject theatreLabel;
    private GameObject cafeLabel;

    public DialogueRunner dialogueRunner;
    public YarnProject[] yarnProjects;

    [SerializeField] bool developerMode;
    private void Awake()
    {
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        dialogueRunner.SetProject(yarnProjects[ContinuousData.instance.CDdayIndex]);
        homeLabel = GameObject.Find("HomeLabel");
        libraryLabel = GameObject.Find("LibraryLabel");
    }

    private void Start()
    {
        DeveloperModeCheck();
        homeLabel.SetActive(false);
        libraryLabel.SetActive(false);

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
            dialogueRunner.StartDialogue("EnterLibrary");
            ContinuousData.instance.SetMovementLock(false);
            //ContinuousData.instance.SceneChangeDetected("Library", ContinuousData.instance.library_EntranceSpawn);
        }
        if (Physics2D.IsTouching(toHome, playerCollider))
        {
            dialogueRunner.StartDialogue("EnterPlayerHouse");
            ContinuousData.instance.SetMovementLock(false);
            
        }
        if (Physics2D.IsTouching(toTheatre, playerCollider))
        {
            notificationSound.Play();
            dialogueRunner.StartDialogue("EnterTheatre");
            ContinuousData.instance.SetMovementLock(false);
        }
        if (Physics2D.IsTouching(toCafe, playerCollider))
        {
            notificationSound.Play();
            dialogueRunner.StartDialogue("EnterCafe");
            ContinuousData.instance.SetMovementLock(false);
        }
        if (Physics2D.IsTouching(toGym, playerCollider))
        {
            notificationSound.Play();
            dialogueRunner.StartDialogue("EnterGym");
            ContinuousData.instance.SetMovementLock(false);
        }
        
    }

    public void MapLabelControls()
    {
        GameObject homeTP = GameObject.Find("HLDetectionPoint");
        GameObject libraryTP = GameObject.Find("LLDetectionPoint");
        GameObject theatreTP = GameObject.Find("TLDetectionPoint");
        GameObject gymTP = GameObject.Find("GLDetectionPoint");
        GameObject cafeTP = GameObject.Find("CLDetectionPoint");


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
        if (Vector3.Distance(ContinuousData.instance.player.transform.position, theatreTP.transform.position) < 10f)
        {
            theatreLabel.SetActive(true);
        }
        else
        {
            theatreLabel.SetActive(false);
        }
        if (Vector3.Distance(ContinuousData.instance.player.transform.position, gymTP.transform.position) < 10f)
        {
            gymLabel.SetActive(true);
        }
        else
        {
            gymLabel.SetActive(false);
        }
        if (Vector3.Distance(ContinuousData.instance.player.transform.position, cafeTP.transform.position) < 10f)
        {
            cafeLabel.SetActive(true);
        }
        else
        {
            cafeLabel.SetActive(false);
        }

    }

   
    

   
}
