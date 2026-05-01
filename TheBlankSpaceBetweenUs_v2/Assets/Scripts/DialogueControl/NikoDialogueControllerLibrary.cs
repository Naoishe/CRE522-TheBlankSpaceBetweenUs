using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using System;
using UnityEngine.SceneManagement;

public class NikoDialogueControllerLibrary : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void Awake()
    {
        
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        
    }

    private void StartScript()
    {
        dialogueRunner.StartDialogue("NodeName");
    }
}

