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

    public static Action SceneChanged;

    private void Start()
    {
        if (ContinuousData.instance.previousScene.name == "Midday")
        {
            objective.SetActive(true);
            StartCoroutine(DelayObj());
        }
    }

    IEnumerator DelayObj()
    {
        yield return new WaitForSeconds(5f);
        objective.SetActive(false);
        
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
            targetScene = "PlayerHouse";
            SceneChanged?.Invoke();
        }
    }
}
