using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NikoInteract : InteractableObject
{
    public override void Interaction()
    {
        if(ContinuousData.instance.CDdayIndex==0)
        {
            dialogueRunner.StartDialogue("MeetingNiko");
        }
        else
        {
            Debug.Log("HERE");
        }
    }

    public override void EndSpecifics()
    {
        dialogueRunner.Stop();
    }
}
