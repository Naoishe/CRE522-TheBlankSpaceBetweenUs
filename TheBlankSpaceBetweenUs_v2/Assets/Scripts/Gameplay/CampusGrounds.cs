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
    public string targetScene;
    public GameObject objective;
    public GameObject noReturn;

    public AudioSource notificationSound;

    public static Action SceneChanged;

    private void Start()
    {
        /*if (ContinuousData.instance.previousScene.name == "Midday")
        {
            objective.SetActive(true);
            notificationSound.Play();
            StartCoroutine(DelayObj());
        }*/
        objective.SetActive(true);
        notificationSound.Play();
        StartCoroutine(DelayObj(objective));
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
            if(ContinuousData.instance.libraryVisted > 0)
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
    }
}
