using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn.Unity;
using static Unity.Collections.AllocatorManager;

public class EndedClass : MonoBehaviour
{
    
    private bool endingclass;
    public YarnProject[] yarnProjects;
    public DialogueRunner dialogueRunner;

    private void Awake()
    {
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        dialogueRunner.SetProject(yarnProjects[0]);
    }

    private void Start()
    {
        
        endingclass = false;
    }

    private void Update()
    {


        if (endingclass)
        {
            SceneManager.LoadScene("CampusGrounds");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            endingclass = true;
        }
    }
}
