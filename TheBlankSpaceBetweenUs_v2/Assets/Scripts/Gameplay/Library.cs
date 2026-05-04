using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;
using UnityEngine.UI;
using Yarn.Unity;

public class Library : MonoBehaviour
{
    public Collider2D toCampus;
    public Collider2D playerCollider;
    public GameObject playerObj;
    public DialogueRunner dialogueRunner;
    public YarnProject[] yarnProjects;

    public static Action ReturnToCampus;


    private void Awake()
    {
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        dialogueRunner.SetProject(yarnProjects[ContinuousData.instance.CDdayIndex]);
    }
    void Start()
    {
        
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }


    void Update()
    {
        if (Physics2D.IsTouching(toCampus, playerCollider))
        {

            ContinuousData.instance.SceneChangeDetected("CampusGrounds", ContinuousData.instance.campusGrounds_LibrarySpawn);
        }

    }


    
}
