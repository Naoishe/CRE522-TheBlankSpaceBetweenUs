using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class Gym : MonoBehaviour
{
    DialogueRunner dialogueRunner;
    public YarnProject[] yarnProjects;
    public bool allowExit;
    public GameObject playerObj;
    public GameObject FaustDiaImage;
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
        if (allowExit)
        {
            if (ContinuousData.instance.CDtimeIndex < 3)
            {
                ContinuousData.instance.SceneChangeDetected("CampusGrounds", new Vector3(-19.8f, 65f, 0f));
            }
            else
            {
                ContinuousData.instance.UpdateNextScene("CampusGrounds");
                ContinuousData.instance.SceneLoad(new Vector3(-19.8f, 65f, 0f));
            }
        }
    }

    void Start()
    {
        dialogueRunner.AddCommandHandler("allowExit", AllowExit);
        dialogueRunner.AddCommandHandler("startEventGame", StartEventGame);
        if (ContinuousData.instance.CDdayIndex == 0)
        {
            dialogueRunner.StartDialogue("Faust0");
        }
        if (ContinuousData.instance.CDdayIndex == 1)
        {
            dialogueRunner.StartDialogue("Faust1");
        }
        if (ContinuousData.instance.CDdayIndex == 2)
        {
            if(ContinuousData.instance.FaustRP>0 && ContinuousData.instance.playerClub=="Wrestling")
            {
                dialogueRunner.StartDialogue("Faust2Positive");
            }

            else
            {
                dialogueRunner.StartDialogue("Faust2Negative");
            }
        }
        if (ContinuousData.instance.CDdayIndex == 3)
        {
            dialogueRunner.StartDialogue("Faust3");
        }
        if (ContinuousData.instance.CDdayIndex == 4 && ContinuousData.instance.EndingIndex < 6)
        {
            DetermineFaustEnd();
        }
        else if (ContinuousData.instance.CDdayIndex == 4 && ContinuousData.instance.EndingIndex == 7)
        {
            dialogueRunner.StartDialogue("PlayerWinsEvent");
        }
        else if (ContinuousData.instance.CDdayIndex == 4 && ContinuousData.instance.EndingIndex == 8)
        {
            dialogueRunner.StartDialogue("PlayerLosesEvent");
        }
    }

    public void FixedUpdate()
    {
        FaustDiaImage.SetActive(ContinuousData.instance.FaustDiaImageState);
    }

    public void AllowExit()
    {
        dialogueRunner.Stop();
        allowExit = true;
        
    }

    public void LoadPlayerWinAgainstFaust()
    {
        dialogueRunner.StartDialogue("PlayerWins");
    }

    public void LoadPlayerLoseAgainstFaust()
    {
        dialogueRunner.StartDialogue("PlayerLoses");
    }

    public void LoadEventWinAgainstFaust()
    {
        dialogueRunner.StartDialogue("PlayerWinsEvent");
    }

    public void LoadEventLoseAgainstFaust()
    {
        dialogueRunner.StartDialogue("PlayerLosesEvent");
    }

    public void DetermineFaustEnd()
    {
        if (ContinuousData.instance.playerClub == "Wrestling")
        {
            dialogueRunner.StartDialogue("FaustEventStart");
        }
    }

    public void StartEventGame()
    {
               ContinuousData.instance.LoadScene("Wrestling");
    }

  


}
