using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
public class Cafe : MonoBehaviour
{
    DialogueRunner dialogueRunner;
    public YarnProject[] yarnProjects;
    public bool allowExit;
    public GameObject playerObj;
    public void Awake()
    {
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        dialogueRunner.SetProject(yarnProjects[0]);
        playerObj = GameObject.Find("PlayerObj");
        allowExit = false;
    }

    private void OnEnable()
    {
        Player.OnExitButton += HandleExitButton;
    }

    private void OnDisable()
    {
        Player.OnExitButton -= HandleExitButton;
    }

    private void HandleExitButton()
    {
        if(allowExit)
        {
            if (ContinuousData.instance.CDtimeIndex < 3)
            {
                ContinuousData.instance.SceneChangeDetected("CampusGrounds", new Vector3(9.5f, 41.5f, 0f));
            }
            else
            {
                ContinuousData.instance.UpdateNextScene("CampusGrounds");
                ContinuousData.instance.SceneLoad(new Vector3(9.5f, 41.5f, 0f));
            }
           
        }
    }

    void Start()
    {
        dialogueRunner.AddCommandHandler("allowExit", AllowExit);
        if (ContinuousData.instance.CDdayIndex==0)
        {
            dialogueRunner.StartDialogue("SalemDay0Start");
        }
        if (ContinuousData.instance.CDdayIndex == 1)
        {
            dialogueRunner.StartDialogue("SalemDay1Start");
        }
         if (ContinuousData.instance.CDdayIndex == 2)
        {
            dialogueRunner.StartDialogue("Salem2Start");
        }
        if (ContinuousData.instance.CDdayIndex == 3)
        {
            dialogueRunner.StartDialogue("Salem3Start");
        }
        if (ContinuousData.instance.CDdayIndex == 4)
        {
            dialogueRunner.StartDialogue("Salem4");
        }
        if (ContinuousData.instance.CDdayIndex == 5)
        {
            dialogueRunner.StartDialogue("Salem5");
        }
        if (ContinuousData.instance.CDdayIndex == 6 && ContinuousData.instance.SalemRP>0 && ContinuousData.instance.EndingIndex == -1)
        {
            dialogueRunner.StartDialogue("Salem6Cafe");
        }
    }
     void Update()
    {

    }
    public void AllowExit()
    {
        dialogueRunner.Stop();
        ContinuousData.instance.SceneChangeDetected("CampusGrounds", new Vector3(9.5f,41.5f,0f));
    }
}
