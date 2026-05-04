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
        if (ContinuousData.instance.CDdayIndex==0)
        {
            dialogueRunner.StartDialogue("MeetingNiko");
        }
        else
        {
            // Not day 0
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
