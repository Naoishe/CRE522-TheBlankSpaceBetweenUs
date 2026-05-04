using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NikoInteract : InteractableObject
{
    public GameObject nikoDiaImage;
    public bool watchBool;
    public void Start()
    {
        nikoDiaImage = GameObject.Find("NikoDiaImage");
        nikoDiaImage.SetActive(false);
    }
    public override void Interaction()
    {
        watchBool = true;
        switch (ContinuousData.instance.CDdayIndex)
        {
            case 0:
                dialogueRunner.StartDialogue("MeetingNiko");
                break;
            case 1:
                dialogueRunner.StartDialogue("Niko1");
                break;
            case 2:
                dialogueRunner.StartDialogue("Niko2");
                break;
            case 3:
                dialogueRunner.StartDialogue("Niko3");
                break;
            case 4:
                dialogueRunner.StartDialogue("Niko4");
                break;
            case 5:
                dialogueRunner.StartDialogue("Niko5");
                break;
            default:
                dialogueRunner.StartDialogue("MeetingNiko");
                break;
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
