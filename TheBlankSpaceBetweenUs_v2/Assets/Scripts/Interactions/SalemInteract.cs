using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class SalemInteract : InteractableObject
{
    public GameObject salemDiaImage;
    public bool watchBool;
    public void Start()
    {
        salemDiaImage = GameObject.Find("SalemDiaImage");
        salemDiaImage.SetActive(false);
    }
    public override void Interaction()
    {
        watchBool = true;
        if (ContinuousData.instance.CDdayIndex == 0)
        {
            dialogueRunner.StartDialogue("MeetingSalem");
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
        salemDiaImage.SetActive(ContinuousData.instance.SalemDiaImageState);
    }
}
