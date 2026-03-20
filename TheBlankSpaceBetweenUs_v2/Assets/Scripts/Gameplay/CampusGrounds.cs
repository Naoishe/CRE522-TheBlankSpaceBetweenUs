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
    public GameObject objective;
    public GameObject objective2;
    public GameObject noReturn;
    public AudioSource notificationSound;
    public string targetScene;

    public static Action SceneChanged;

    private GameObject homeLabel;
    private GameObject libraryLabel;
    private GameObject player;
    private void Awake()
    {
        player = GameObject.Find("PlayerObj");
        homeLabel = GameObject.Find("HomeLabel");
        libraryLabel = GameObject.Find("LibraryLabel");
    }

    private void Start()
    {
        homeLabel.SetActive(false);
        libraryLabel.SetActive(false);
        objective.SetActive(false);
        objective2.SetActive(false);
        if (ContinuousData.instance.libraryVisited)
        {
            objective2.SetActive(true);
            notificationSound.Play();
            StartCoroutine(DelayObj(objective2));
            player.transform.position = new Vector3(-34f,45.5f,0f);
        }
        else
        {
            objective.SetActive(true);
            notificationSound.Play();
            StartCoroutine(DelayObj(objective));
            player.transform.position = new Vector3(3.85f,1f,0f);
        }
    }

    IEnumerator DelayObj(GameObject gameObj)
    {
        yield return new WaitForSeconds(5f);
        gameObj.SetActive(false);
        
    }

    private void Update()
    {
        if (Physics2D.IsTouching(toLibrary, playerCollider))
        {
            targetScene = "Library";
            SceneChanged?.Invoke();
        }
        if (Physics2D.IsTouching(toHome, playerCollider))
        {
            if (ContinuousData.instance.libraryVisited)
            {
                targetScene = "HolderScene";
                SceneChanged?.Invoke();

            }
            else
            {
                noReturn.SetActive(true);
                StartCoroutine(DelayObj(noReturn));
            }
           
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            if (ContinuousData.instance.libraryVisited)
            {
                targetScene = "HolderScene";
                SceneChanged?.Invoke();
            }
            else
            {
                noReturn.SetActive(true);
                StartCoroutine(DelayObj(noReturn));

            }

        }
        

        if (Input.GetKeyDown(KeyCode.L))
        {
            targetScene = "Library";
            SceneChanged?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            float gap = Vector3.Distance(player.transform.position, libraryLabel.transform.position);
            Debug.Log("Gap: " + gap);
        }

        
    }

    public void FixedUpdate()
    {
        MapLabelControls();
    }

    public void MapLabelControls()
    {
        GameObject homeTP = GameObject.Find("HLDetectionPoint");
        GameObject libraryTP = GameObject.Find("LLDetectionPoint");


        if (Vector3.Distance(player.transform.position, homeTP.transform.position) < 10f)
        {
            homeLabel.SetActive(true);
        }
        else
        {
            homeLabel.SetActive(false);
        }
        if (Vector3.Distance(player.transform.position, libraryTP.transform.position) < 10f)
        {
           libraryLabel.SetActive(true);
        }
        else
        {
            libraryLabel.SetActive(false);
        }

    }

   
}
