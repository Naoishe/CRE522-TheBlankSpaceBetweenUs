using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class NikoInteract : InteractableObject
{
    public GameObject nikoDiaImage;
    public bool watchBool;
    public YarnProject[] yarnProjects;
    public void Start()
    {
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        dialogueRunner.SetProject(yarnProjects[0]);
        nikoDiaImage = GameObject.Find("NikoDiaImage");
        nikoDiaImage.SetActive(false);
    }
    public override void Interaction()
    {
        watchBool = true;
        if (ContinuousData.instance.CDdayIndex == 0)
        {
            dialogueRunner.StartDialogue("Niko0");
        }
        if (ContinuousData.instance.CDdayIndex == 1)
        {
            dialogueRunner.StartDialogue("Niko1");
        }
        if (ContinuousData.instance.CDdayIndex == 2)
        {
            dialogueRunner.StartDialogue("Niko2");
        }
        if (ContinuousData.instance.CDdayIndex == 3)
        {
            dialogueRunner.StartDialogue("Niko3");
        }
        if (ContinuousData.instance.CDdayIndex == 4)
        {
            dialogueRunner.StartDialogue("Niko4");
        }
        if (ContinuousData.instance.CDdayIndex == 5)
        {
            dialogueRunner.StartDialogue("Niko5");
        }
    }

    public override void UpdateExtra()
    {
        if (watchBool)
        {
            WatchImageBool();
        }
    }

    public override void EndSpecifics()
    {
        dialogueRunner.Stop();
        watchBool = false;
    }

    public void WatchImageBool()
    {
       nikoDiaImage.SetActive(ContinuousData.instance.NikoDiaImageState);
    }
}
